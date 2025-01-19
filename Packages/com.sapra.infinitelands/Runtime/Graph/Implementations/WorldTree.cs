using System.Collections.Generic;
using UnityEngine;

namespace sapra.InfiniteLands
{
    [CreateAssetMenu(fileName = "World", menuName = "InfiniteLands/Graph/World")]
    public class WorldTree : TerrainGenerator
    {
        public List<BiomeTree> PreviouslyConnected = new List<BiomeTree>();
        public override void InitializingAsset()
        {
            foreach(BiomeTree biome in PreviouslyConnected){
                if(biome == null)
                    continue;
                biome.OnValuesChanged -= OnValuesChanged;
            }
            PreviouslyConnected.Clear();
            foreach(InfiniteLandsNode node in nodes){
                if(node is BiomeNode biomeNode){
                    biomeNode.biome.OnValuesChanged += OnValuesChanged;
                    PreviouslyConnected.Add(biomeNode.biome);
                }
            }
        }
    }
}