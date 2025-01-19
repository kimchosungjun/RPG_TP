using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace sapra.InfiniteLands{
    public class CompactNormalizedExporter : IExportTextures
    {
        public string description => "Export the textures compacted,  normalizing the height map";
        public IBurstTexturePool texturePool;
        public void SetExporterResolution(int resolution)
        {
            texturePool = new BurstTexturePool(resolution);
        }
        private Dictionary<Type, string[]> AssetTypeName = new Dictionary<Type, string[]>();

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
        public ExportedMultiResult GenerateHeightTexture(WorldFinalData finalData)
        {
            BurstTexture burstTexture = texturePool.GetTexture("NormalAndHeight",  FilterMode.Bilinear, TextureFormat.RGBAFloat);
            NativeArray<Vertex4> reinterpreted = finalData.FinalPositions.Reinterpret<Vertex4>(Vertex.size);
            JobHandle finalTextureJob;

            finalTextureJob = MTJHeightNormalizedJob.ScheduleParallel(reinterpreted,
                burstTexture.GetRawData<Color>(), finalData.MinMaxHeight, texturePool.GetTextureResolution(), finalData.jobHandle);
            
            return new ExportedMultiResult(finalTextureJob, new BurstTexture[]{burstTexture}, default);

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
            //Generate density textures
            BurstTexture[] masks = texturePool.GetTexture(names, FilterMode.Bilinear);
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