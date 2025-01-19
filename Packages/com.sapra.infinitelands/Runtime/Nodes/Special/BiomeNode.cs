using UnityEngine;
using System;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Biome Output", typeof(WorldTree), type = "Biome", docs = "https://ensapra.com/packages/infinite_lands/nodes/biome.html")]
    public class BiomeNode : InfiniteLandsNode, IGive<BiomeData>
    {
        public string portName => "Biome Data";

        public BiomeTree biome;
        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator DensityLayer;

        public HeightDataGenerator density => DensityLayer;
        public Vector2 minMaxValue => biome.MinMaxHeight;

        protected override InfiniteLandsNode[] Dependancies => new InfiniteLandsNode[]{density, biome?.output};
        protected override bool ExtraValidationSteps()
        {
            if(biome != null){
                biome.ValidationCheck();
                return biome.ValidOutput;
            }
            return false;
        }

        public BiomeData RequestHeight(GenerationSettings settings)
        {
            return new BiomeData()
            {
                HeightMap = biome.output.HeightMap.RequestHeight(settings),
                DensityMap = DensityLayer.RequestHeight(settings),
            };
        }

        public IndexAndResolution RequestHeightIndex(GenerationSettings settings)
        {
            return biome.output.HeightMap.RequestIndex(settings);
        }

        public IndexAndResolution RequestDensityIndex(GenerationSettings settings)
        {
            return DensityLayer.RequestIndex(settings);
        }
        
        public int PrepareBiome(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid){
            int biomRes = GenerationHelper.PrepareNodes(biome.GetNodes(),manager,ref currentCount, resolution, ratio, requestGuid);
            int denRes = DensityLayer.PrepareNode(manager,ref currentCount, resolution, ratio, requestGuid);
            return Math.Max(biomRes, denRes);
        }
    }
}