using System;
using System.Linq;

using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace sapra.InfiniteLands{
    public class ExpandedExporter : IExportTextures
    {
        public string description => "Export the textures individually as a greyscale";

        public IBurstTexturePool texturePool;
        public void SetExporterResolution(int resolution)
        {
            texturePool = new BurstTexturePool(resolution);
        }

        public ExportedMultiResult GenerateHeightTexture(WorldFinalData finalData)
        {
            BurstTexture normalMap = texturePool.GetTexture("Normal Map", FilterMode.Bilinear, TextureFormat.RGBAFloat);
            BurstTexture heightMap = texturePool.GetTexture("Height Map", FilterMode.Bilinear, TextureFormat.RGBAFloat);

            NativeArray<Vertex4> reinterpreted = finalData.FinalPositions.Reinterpret<Vertex4>(Vertex.size);
            JobHandle finalTextureJob;

            finalTextureJob = MTJHeightSeparated.ScheduleParallel(reinterpreted,
                normalMap.GetRawData<Color>(), heightMap.GetRawData<Color>(), finalData.MinMaxHeight, texturePool.GetTextureResolution(), finalData.jobHandle);
            
            return new ExportedMultiResult(finalTextureJob, new BurstTexture[]{heightMap, normalMap}, default);
        }

        public ExportedMultiResult GenerateDensityTextures(AssetData data)
        {
            var names = data.assets.Select(a => a.name).ToArray();

            //Generate density textures
            BurstTexture[] masks = texturePool.GetTexture(names, FilterMode.Bilinear);
            NativeArray<JobHandle> TextureCreationJob = new NativeArray<JobHandle>(names.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < names.Length; i++)
            {
                NativeArray<Color32> rawTexture = masks[i].GetRawData<Color32>();
                TextureCreationJob[i] = MTJTextureSingleChannel.ScheduleParallel(data.map, rawTexture,
                    i, texturePool.GetTextureLength(), texturePool.GetTextureResolution(), data.job);
            }

            JobHandle textureCreated = JobHandle.CombineDependencies(TextureCreationJob);
            TextureCreationJob.Dispose();
            return new ExportedMultiResult(textureCreated, masks, data);
        }

        public void ReturnExportedData(ExportedMultiResult data) => texturePool.Return(data.textures);
        public void DestroyTextures(Action<UnityEngine.Object> Destroy) => texturePool.DestroyBurstTextures(Destroy);
        public int GetTextureResolution() => texturePool.GetTextureResolution();
        public void ResetExporter(){}

    }
}