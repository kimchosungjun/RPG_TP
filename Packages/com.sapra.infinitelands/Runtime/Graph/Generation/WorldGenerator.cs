using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;

namespace sapra.InfiniteLands
{
    public class WorldGenerator 
    {        
        IEnumerable<InfiniteLandsNode> nodes;
        IEnumerable<AssetWithType> assets;

        HeightOutputNode output;
        Dictionary<string, IndexAndResolution> RequestsSettings;

        Dictionary<IAsset, ILoadAsset[]> AssetLoaders;
        public WorldGenerator(IGraph generator){
            nodes = generator.GetNodes();
            output = generator.GetOutputNode();
            assets = generator.GetAssets();
            AssetLoaders = generator.GetAssetLoaders();
            RequestsSettings = new Dictionary<string, IndexAndResolution>();
        }

        public ChunkData GenerateWorld(TerrainConfig terrain, MeshSettings meshSettings, DataManager manager, IndexManager indexManager)
        {
            indexManager.Reset();
            float ratio = meshSettings.ratio;
            string guid = GenerationSettings.GetGUID(output.guid, meshSettings.Resolution, meshSettings.MeshScale);
            
            IndexAndResolution config = PrepareRequest(indexManager, guid,  meshSettings.Resolution, ratio);
            int ArraySize = config.Index; 
            
            MeshSettings finalSettings = meshSettings;
            finalSettings.Resolution = config.Resolution;
            finalSettings.MeshScale = config.Resolution/ratio;

            GenerationSettings settings = GenerationSettings.NewSettings(ArraySize, ratio, finalSettings, terrain, manager,indexManager, output, guid);
            WorldFinalData biomeOutput = output.PerformTerrain(settings, meshSettings);

            List<AssetData> assetData = new List<AssetData>();

            if(meshSettings.CustomTextureResolution && meshSettings.TextureResolution != meshSettings.Resolution){
                float ratio2 = meshSettings.TextureResolution/meshSettings.MeshScale;
                string guid2 = GenerationSettings.GetGUID(output.guid, meshSettings.TextureResolution, meshSettings.MeshScale);

                IndexAndResolution config2 = PrepareRequest(indexManager, guid2,  meshSettings.TextureResolution, ratio2);
                MeshSettings finalSettings2 = meshSettings;
                finalSettings2.Resolution = config2.Resolution;
                finalSettings2.MeshScale = config2.Resolution/ratio2;

                MeshSettings newOg = meshSettings;
                newOg.Resolution = meshSettings.TextureResolution;
                GenerationSettings tst = GenerationSettings.NewSettings(ArraySize, ratio2, finalSettings2, terrain, manager, indexManager,output, guid2);
                foreach(AssetWithType assetWithType in assets){
                    AssetData processAsset = GenerateAssetTextures(assetWithType, tst, newOg, manager);
                    assetData.Add(processAsset);
                }
            }else{
                foreach(AssetWithType assetWithType in assets){
                    AssetData processAsset = GenerateAssetTextures(assetWithType, settings, meshSettings, manager);
                    assetData.Add(processAsset);
                }
            }

            List<JobHandle> assetJobs = assetData.Select(a => a.job).ToList();
            assetJobs.Add(biomeOutput.jobHandle);
            
            NativeArray<JobHandle> jobs = new NativeArray<JobHandle>(assetJobs.ToArray(), Allocator.Temp);
            JobHandle everythingGenerated = JobHandle.CombineDependencies(jobs);
            jobs.Dispose();

            //Generate Coordinate Data
            int length = MapTools.LengthFromResolution(meshSettings.Resolution);
            NativeArray<CoordinateData> coordinateDatas = manager.GetReturnableArray<CoordinateData>(string.Format("{0}-coordinates", terrain.ID), length);
            JobHandle calculateCoordinates = CoordinateDataJob.ScheduleParallel(biomeOutput.FinalPositions,
                //vegTextures.map, sets.Length, texTextures.map, textures.Length,
                coordinateDatas, terrain.Position, meshSettings.Resolution, everythingGenerated);

            AssetsData assetsData = new AssetsData(assetData, manager, string.Format("{0}-assets", terrain.ID));
            CoordinateDataHolder coordinate = new CoordinateDataHolder(coordinateDatas, manager,string.Format("{0}-coordinates", terrain.ID));
            
            //Store the data into the struct
            ChunkData data = new ChunkData(terrain, meshSettings);
            data.assetsData = assetsData;
            data.worldFinalData = biomeOutput;
            data.handle = calculateCoordinates;
            data.coordinateData = coordinate;
            data.manager = manager;
            return data;
        }

        private AssetData GenerateAssetTextures(AssetWithType assetPack, GenerationSettings settings, MeshSettings originalSetings, DataManager manager){
            var assets = assetPack.assets;
            int count = assets.Count();
            var indices = new NativeArray<IndexAndResolution>[count];
            var jobs = new NativeArray<JobHandle>(count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            int index = 0;
            foreach(var asset in assets){
                ILoadAsset[] loaders = AssetLoaders[asset];
                List<NoiseOutput> outputs = GenerationHelper.ProcessAssets(loaders, asset, settings);  

                NativeArray<JobHandle> internalJobs = new NativeArray<JobHandle>(outputs.Select(a => a.jobHandle).ToArray(), Allocator.Temp);
                jobs[index] = JobHandle.CombineDependencies(internalJobs);
                internalJobs.Dispose();

                NativeArray<IndexAndResolution> internalIndices = new NativeArray<IndexAndResolution>(outputs.Select(a => a.globalIndex).ToArray(), Allocator.Persistent);
                indices[index] = internalIndices;      
                index++;    
            }

            JobHandle afterVegetationCreated = JobHandle.CombineDependencies(jobs);
            jobs.Dispose();

            var length = count;
            var textureLength = (originalSetings.Resolution + 1) * (originalSetings.Resolution + 1);//MapResize.LengthFromResolution(originalSetings.Resolution);
            NativeArray<float> finalTargetMap = manager.GetReturnableArray<float>(string.Format("{0}-assets", settings.terrain.ID),length*textureLength);

            NativeArray<JobHandle> CombineJobs = new NativeArray<JobHandle>(indices.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            for (int x = 0; x < indices.Length; x++)
            {
                CombineJobs[x] = MJDensityCombine.ScheduleParallel(settings.globalMap, finalTargetMap, indices[x], x,
                    textureLength, settings.pointsLength, originalSetings.Resolution, afterVegetationCreated);
            }

            JobHandle afterCombined = JobHandle.CombineDependencies(CombineJobs);
            CombineJobs.Dispose();

            for (int i = 0; i < indices.Length; i++)
            {
                indices[i].Dispose(afterCombined);
            }

            AssetData data = new AssetData(){
                job = afterCombined,
                map = finalTargetMap,
                assets = assets,
                assetType = assetPack.originalType
            };
            
            return data;
        }
        
        public IndexAndResolution PrepareRequest(IndexManager manager, string guid, int resolution, float ratio){
            IndexAndResolution result;
            if(!RequestsSettings.TryGetValue(guid, out result)){
                int ArraySize = 0; 
                int TargetResolution = GenerationHelper.PrepareNodes(nodes, manager, ref ArraySize, resolution, ratio,  guid);
                result = new IndexAndResolution(ArraySize, TargetResolution); 
                RequestsSettings.TryAdd(guid,result);        
            }
            return result;
        }
    }
}