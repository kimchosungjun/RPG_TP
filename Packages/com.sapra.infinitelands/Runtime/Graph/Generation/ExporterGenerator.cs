using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace sapra.InfiniteLands
{
    public class ExporterGenerator 
    {
        IGraph generator;
        public ExporterGenerator(IGraph generator){
            this.generator = generator;
        }
        public List<Texture2D> GenerateAndExportWorld(int EditorResolution, float MeshScale, int Seed, Vector2 WorldOffset, IExportTextures exporter)
        {
            TerrainConfig config = new TerrainConfig
            {
                Position = WorldOffset,
                TerrainNormal = Vector3.up,
            };
            
            MeshSettings meshSettings = new MeshSettings
            {
                Resolution = EditorResolution,
                MeshScale = MeshScale,
                Seed = Seed,
                meshType = MeshSettings.MeshType.Normal
            };

            var manager = new DataManager();
            var indexManager = new IndexManager();


            //DeepRestart();
            generator.ValidationCheck();
            generator.Initialize();
            
            WorldGenerator worldGenerator = new WorldGenerator(generator);
            ChunkData generationData = worldGenerator.GenerateWorld(config, meshSettings, manager, indexManager);
            generationData.Complete();

            List<Texture2D> texturesToExport = new List<Texture2D>();
            var heightResult = exporter.GenerateHeightTexture(generationData.worldFinalData);
            heightResult.job.Complete();
            texturesToExport.AddRange(heightResult.textures.Select(a => a.ApplyTexture()));
            foreach(AssetData data in generationData.assetsData.assets){
                var result = exporter.GenerateDensityTextures(data);
                result.job.Complete();
                texturesToExport.AddRange(result.textures.Select(a => a.ApplyTexture()));
            }

            generationData.Return();
            manager.Dispose(default);
            return texturesToExport;
        }
    }
}