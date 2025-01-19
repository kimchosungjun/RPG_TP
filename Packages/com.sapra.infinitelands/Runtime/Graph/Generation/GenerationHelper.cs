using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace sapra.InfiniteLands
{
    public static class GenerationHelper{

        public static IEnumerable<ILoadAsset> FindAssetsLike(IAsset asset, IEnumerable<InfiniteLandsNode> nodes){
            return nodes.Where(a => a.isValid).OfType<ILoadAsset>().Where(a => a.AssetExists(asset));

        }

        public static List<NoiseOutput> ProcessAssets(ILoadAsset[] assetLoaders, IAsset asset, GenerationSettings settings)
        {
            List<NoiseOutput> textureOutputs = new List<NoiseOutput>();
            foreach(ILoadAsset loader in assetLoaders){
                NoiseOutput output = loader.ProcessAsset(asset, settings);
                textureOutputs.Add(output);
            }

            return textureOutputs;
        }

        public static int PrepareNodes(IEnumerable<InfiniteLandsNode> nodes, IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid){
            int MaxResolution = resolution;
            foreach(InfiniteLandsNode node in nodes){
                if (node is IOutput output){
                    int newResolution = output.PrepareNode(manager,ref currentCount, resolution, ratio, requestGuid);
                    MaxResolution = Math.Max(MaxResolution, newResolution);
                }
            }
            return MaxResolution;
        }
    }
}