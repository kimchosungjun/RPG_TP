using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;

namespace sapra.InfiniteLands{
    [ExecuteAlways]
    public class TerrainPainter : ChunkProcessor<ChunkData>, IPaintTerrain, IGenerate<TextureResult>
    {
        private class TextureProcess{
            public AssetsData assetData;
            public WorldFinalData finalData;
            public TextureResult request;
        }

        private TextureArrayPool textureArrayPool;
        private TerrainTextures textureArrays;

        public Material terrainMaterial;
        private PointStore pointStore;
        private Dictionary<Vector3Int, TextureResult> ReloadableRequests = new();
        private List<TextureProcess> TexturesToProcess = new();
        private List<Material> Orphans = new List<Material>();
        private UnityEngine.Pool.ObjectPool<Material> _materialPool;

        private IExportTextures exporter;

        public IGraph graph{get; private set;}
        public MeshSettings settings{get; private set;}
        public Vector2 localGridOffset{get; private set;}

        private IAsset DesiredAsset;
        public IAsset GetDesired => DesiredAsset;
        public IEnumerable<AssetWithType> assets;

        public bool ProceduralTexturing => textureArrays != null && DesiredProcedural;
        private bool DesiredProcedural = true;

        public Action<TextureResult> onProcessDone { get; set; }
        public Action<TextureResult> onProcessRemoved { get; set; }
        public Action<IGraph, MeshSettings> onReload{get;set;}

        private HashSet<Vector3Int> TextureToRemove = new();

        void OnValidate()
        {
            ReassignMaterials();
        }

        public override void Initialize(IGraph generator, MeshSettings settings)
        {   
            pointStore = GetComponent<PointStore>();
            this.graph = generator;
            this.settings = settings;

            if(generator == null)
                return;
                
            #if UNITY_EDITOR
            TextureReloader.OnSaveAnyAsset -= ReassignMaterials;
            TextureReloader.OnSaveAnyAsset += ReassignMaterials;
            #endif

            if(textureArrayPool != null){
                foreach(TextureResult request in ReloadableRequests.Values){
                    textureArrayPool.Release(request.TextureMasksArray);
                }
                textureArrayPool.Dispose();
                textureArrayPool = null;
            }
            
            ReloadableRequests.Clear();
            Orphans.Clear();

            if(textureArrays != null){
                textureArrays.Release();
                textureArrays = null;
            }

            if(terrainMaterial == null){
                Debug.LogWarningFormat("No material has been set in {0}. Creating a temporal one", nameof(terrainMaterial));
                terrainMaterial = new Material(Shader.Find("Infinite Lands/Terrain"));
            }
            
            if(_materialPool == null)
                _materialPool = new UnityEngine.Pool.ObjectPool<Material>(CreateMaterial, actionOnDestroy: AdaptiveDestroy);

            if(exporter == null || exporter.GetTextureResolution() != settings.TextureResolution){
                exporter?.DestroyTextures(AdaptiveDestroy);
                exporter = new CompactFullExporter(settings.TextureResolution,settings.Resolution);
            }
            exporter.ResetExporter();
            
            UpdateTextureArray(generator);
        
            onReload?.Invoke(generator, settings);
        }
        
        public override void Dispose()
        {
            if(textureArrays != null){
                textureArrays.Release();
                textureArrays = null;
            }

            foreach(var result in TexturesToProcess){
                result.request.handle.Complete();
                var request = result.request;
                foreach(ExportedMultiResult pack in request.packs){
                    exporter.ReturnExportedData(pack);
                }
                exporter.ReturnExportedData(request.NormalAndHeightTexture);
                result.finalData.RemoveProcessor(this);  
                result.assetData.RemoveProcessor(this);            
            }
            
            foreach(var request in ReloadableRequests){
                var result = request.Value;
                foreach(ExportedMultiResult pack in result.packs){
                    exporter.ReturnExportedData(pack);
                }
                exporter.ReturnExportedData(result.NormalAndHeightTexture);
                textureArrayPool?.Release(result.TextureMasksArray);
                _materialPool.Release(result.groundMaterial);     
            }
            TexturesToProcess.Clear();
            TextureToRemove.Clear();
            ReloadableRequests.Clear();


            if(exporter != null)
                exporter.DestroyTextures(AdaptiveDestroy);
                
            if(_materialPool != null){
                if(_materialPool.CountActive > 0)
                    Debug.LogError("Not all materials have been released");
                _materialPool.Dispose();
            }
            
            if(textureArrayPool != null)
                textureArrayPool.Dispose();

            #if UNITY_EDITOR
            TextureReloader.OnSaveAnyAsset -= ReassignMaterials;
            #endif

        }

        protected override void OnProcessAdded(ChunkData chunk){
            if(!terrainMaterial)
                return;
            var NormalAndHeightTexture = exporter.GenerateHeightTexture(chunk.worldFinalData);
            AssetsData assetData = chunk.assetsData;
            WorldFinalData finalData = chunk.worldFinalData;

            assetData.AddProcessor(this);
            finalData.AddProcessor(this);

            List<ExportedMultiResult> results = new();
            List<JobHandle> jobs = new(){NormalAndHeightTexture.job};
            foreach(AssetData data in assetData.assets){
                ExportedMultiResult result = exporter.GenerateDensityTextures(data);
                results.Add(result);
                jobs.Add(result.job);
            }
            
            NativeArray<JobHandle> jobs_native = new NativeArray<JobHandle>(jobs.ToArray(), Allocator.Temp);
            JobHandle combined = JobHandle.CombineDependencies(jobs_native);
            jobs_native.Dispose();
           
            TextureResult request = new TextureResult(results){
                NormalAndHeightTexture = NormalAndHeightTexture,
                handle = combined,
                settings = chunk.meshSettings,
                terrainConfig = chunk.terrainConfig,
            };

            TexturesToProcess.Add(new TextureProcess(){request = request, assetData = assetData, finalData = finalData});
            if(chunk.InstantGeneration)
                UpdateRequests(true);
        }

        protected override void OnProcessRemoved(ChunkData chunk){
            var found = TexturesToProcess.Any(a => a.request.terrainConfig.ID.Equals(chunk.terrainConfig.ID));
            if(found){
                TextureToRemove.Add(chunk.terrainConfig.ID);
            }

            if(ReloadableRequests.TryGetValue(chunk.terrainConfig.ID, out TextureResult result)){
                foreach(ExportedMultiResult pack in result.packs){
                    exporter.ReturnExportedData(pack);
                }
                exporter.ReturnExportedData(result.NormalAndHeightTexture);
                _materialPool.Release(result.groundMaterial);
                textureArrayPool?.Release(result.TextureMasksArray);
                onProcessRemoved?.Invoke(result);
            }
            ReloadableRequests.Remove(chunk.terrainConfig.ID);
        }

        void Update(){
            UpdateRequests(false);
        }

        void UpdateRequests(bool instantApply){
            if(TexturesToProcess.Count <= 0)
                return;

            for(int i =  TexturesToProcess.Count-1; i >= 0; i--){
                TextureProcess process = TexturesToProcess[i];
                TextureResult result = process.request;
                if(result.handle.IsCompleted || instantApply){
                    Vector3Int ID = result.terrainConfig.ID;

                    if(TextureToRemove.Contains(ID)){
                        foreach(ExportedMultiResult pack in result.packs){
                            exporter.ReturnExportedData(pack);
                        }
                        exporter.ReturnExportedData(result.NormalAndHeightTexture);
                        TextureToRemove.Remove(ID);
                    }   
                    else{
                        Material mat = _materialPool.Get();
                        result.CompleteRequest(ref textureArrayPool, mat, AdaptiveDestroy);
                        process.finalData.RemoveProcessor(this);
                        process.assetData.RemoveProcessor(this);
                        ReloadableRequests.TryAdd(ID, result);
                        AssignMaterials(ID, mat);

                        onProcessDone?.Invoke(result);
                    }

                    TexturesToProcess.RemoveAt(i);
                }
            }
        }


        private void ReassignMaterials(){
            foreach(KeyValuePair<Vector3Int, TextureResult> reassignable in ReloadableRequests){
                TextureResult reques = reassignable.Value;
                foreach(Material material in reques.materials){
                    if (textureArrays != null){
                        textureArrays.ApplyTextureArrays(material);
                    }
                    reques.ReloadMaterial(material, DesiredAsset,ProceduralTexturing);
                }
            }   

            foreach(Material material in Orphans){
                if (textureArrays != null){
                    textureArrays.ApplyTextureArrays(material);
                }
            }
        }

        private Material CreateMaterial(){
            return new(terrainMaterial);
        }
        
        public void ChangeToTexture(IAsset asset, bool procedural){
            DesiredAsset = asset;
            DesiredProcedural = procedural;
            ReassignMaterials();
        }

        public void AssignMaterials(CommandBuffer bf, ComputeShader compute, int kernelIndex){
            if(textureArrays != null)
                textureArrays.ApplyTextureArrays(bf, compute, kernelIndex);
        }

        public void AssignMaterials(Vector3Int id, Material material){
            if(material == null)
                return;

            if (textureArrays != null){
                textureArrays.ApplyTextureArrays(material);
            }
            
            if(!ReloadableRequests.TryGetValue(id, out TextureResult request)){
                return;
            }

            request.ReloadMaterial(material, DesiredAsset, ProceduralTexturing);
            request.AddMaterial(material);
        }
        public void AssignMaterials(Material material){
            if(material == null)
                return;

            if (textureArrays != null){
                textureArrays.ApplyTextureArrays(material);
            }

            Orphans.Add(material);
        }

        private void UpdateTextureArray(IGraph graph){
            assets = graph.GetAssets();
            IEnumerable<IHoldTextures> textures = assets.
                Where(a => typeof(IHoldTextures).IsAssignableFrom(a.originalType)).
                SelectMany(a => a.assets).
                OfType<IHoldTextures>().ToArray();

            if(textures.Count() > 0)
                textureArrays = new TerrainTextures(textures, AdaptiveDestroy);
        }

        public TextureResult GetTextureAt(Vector2 position){
            ChunkData result = pointStore.GetChunkDataAtGridPosition(position);
            if(result != null){
                return GetTerrainTextures(result.terrainConfig.ID);
            }
            return null;
        }
        
        private TextureResult GetTerrainTextures(Vector3Int id){
            if(ReloadableRequests.TryGetValue(id, out TextureResult result)){
                return result;
            }
            return null;
        }
    }
}