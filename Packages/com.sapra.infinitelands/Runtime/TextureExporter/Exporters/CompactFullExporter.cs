using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace sapra.InfiniteLands{
    public class CompactFullExporter : IExportTextures
    {
        private IBurstTexturePool texturePool;
        private IBurstTexturePool heightPool;

        public void SetExporterResolution(int resolution)
        {
            texturePool = new BurstTexturePool(resolution);
            heightPool = new BurstTexturePool(resolution);
        }

        public CompactFullExporter(int resolution, int meshRes)
        {
            texturePool = new BurstTexturePool(resolution);
            heightPool = new BurstTexturePool(meshRes);
        }
        public CompactFullExporter(){}
        private Dictionary<Type, string[]> AssetTypeName = new Dictionary<Type, string[]>();

        public string description => "Export the textures compacted, keeping the height map at full range";

        public ExportedMultiResult GenerateHeightTexture(WorldFinalData finalData)
        {
            BurstTexture burstTexture = heightPool.GetTexture("NormalAndHeight", FilterMode.Bilinear, TextureFormat.RGBAFloat);
            NativeArray<Vertex4> reinterpreted = finalData.FinalPositions.Reinterpret<Vertex4>(Vertex.size);
            JobHandle finalTextureJob;

            finalTextureJob = MTJHeightJob.ScheduleParallel(reinterpreted,
                burstTexture.GetRawData<Color>(), heightPool.GetTextureResolution(), finalData.jobHandle);
            
            return new ExportedMultiResult(finalTextureJob, new BurstTexture[]{burstTexture}, default);
        }
        
        private string[] VectorizeNames(string[] originals)
        {
            int nameCount = Mathf.CeilToInt(originals.Length / 4f);
            string[] newNames = new string[nameCount];
            for (int i = 0; i < newNames.Length; i++)
            {
                string a = i * 4 < originals.Length ? originals[i * 4] : "";
                string b = i * 4 + 1 < originals.Length ? originals[i * 4 + 1] : "";
                string c = i * 4 + 2 < originals.Length ? originals[i * 4 + 2] : "";
                string d = i * 4 + 3 < originals.Length ? originals[i * 4 + 3] : "";

                newNames[i] = a + " - " + b + " - " + c + " - " + d;
            }

            return newNames;
        }

        public ExportedMultiResult GenerateDensityTextures(AssetData data)
        {
            if(data.assetType == null){
                return ExportedMultiResult.Default;
            }

            if(!AssetTypeName.TryGetValue(data.assetType, out string[] names)){
                names = VectorizeNames(data.assets.Select(a => a.name).ToArray());
                AssetTypeName.Add(data.assetType, names);
            }

            BurstTexture[] masks = texturePool.GetTexture(names, FilterMode.Bilinear);
            //Generate density textures
            NativeArray<JobHandle> TextureCreationJob = new NativeArray<JobHandle>(names.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            int count = data.assets.Count();
            for (int i = 0; i < names.Length; i++)
            {
                NativeArray<Color32> rawTexture = masks[i].GetRawData<Color32>();
                TextureCreationJob[i] = MTJVegetationJobFlat.ScheduleParallel(data.map, rawTexture,
                    i * 4, count, texturePool.GetTextureLength(), texturePool.GetTextureResolution(), data.job);
            }

            JobHandle textureCreated = JobHandle.CombineDependencies(TextureCreationJob);
            TextureCreationJob.Dispose();
            return new ExportedMultiResult(textureCreated, masks, data);
        }

        public void ReturnExportedData(ExportedMultiResult data) => texturePool.Return(data.textures);
        public void DestroyTextures(Action<UnityEngine.Object> Destroy) => texturePool.DestroyBurstTextures(Destroy);
        public int GetTextureResolution() => texturePool.GetTextureResolution();
        public void ResetExporter() => AssetTypeName.Clear();
    }
}