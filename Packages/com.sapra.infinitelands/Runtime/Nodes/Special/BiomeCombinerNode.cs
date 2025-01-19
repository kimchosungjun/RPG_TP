using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using System.Linq;
using System;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Biome Combiner", typeof(WorldTree), type = "Biome", singleInstance = true, docs = "https://ensapra.com/packages/infinite_lands/nodes/biome.html")]
    public class BiomeCombinerNode : HeightNodeBase, ILoadAsset
    {
        public enum Operation
        {
            Add,
            MaxWeight,
            MaxHeight
        }

        public Operation operation;
        [Input(typeof(IGive<BiomeData>))] public List<InfiniteLandsNode> Biomes = new List<InfiniteLandsNode>();

        public Dictionary<IAsset, int> indexAndOffset;
        public BiomeTree[] biomes => Biomes.Cast<BiomeNode>().Select(a => a.biome).ToArray();

        private bool isMaxHeight => operation == Operation.MaxHeight;
        private bool isMaxWeight => operation == Operation.MaxWeight;

        protected override InfiniteLandsNode[] Dependancies => Biomes.Cast<BiomeNode>().ToArray();

        public IEnumerable<IAsset> GetAssets() => biomes.SelectMany(a => a.GetAssets()).SelectMany(a => a.assets).Distinct();

        [ShowIfCustom("isMaxHeight")] [Min(0.01f)] public float HeightBlending;
        [ShowIfCustom("isMaxWeight")] [Range(0.001f,1)] public float WeightBlending;

        [Range(0,0.5f)] public float TextureSmoothing;
        [Range(0,0.5f)] public float VegetationSmoothing;

        protected override Vector2 GetMinMaxValue()
        {
            Vector2[] dataGivers = Biomes.Select(a => (a as BiomeNode).minMaxValue).ToArray();
            Vector2 MinMaxValue = new Vector2(float.MaxValue, float.MinValue);
            foreach (Vector2 minMax in dataGivers)
            {
                MinMaxValue.x = math.min(MinMaxValue.x, minMax.x);
                MinMaxValue.y = math.max(MinMaxValue.y, minMax.y);
            }

            return MinMaxValue;
        }

        
        public void SetAsset(IAsset asset) => Debug.LogError("Can't set an asset inside a biome");

        public NativeArray<IndexAndResolution> GenerateTargetArray(IndexAndResolution target)
        {
            var array = new NativeArray<IndexAndResolution>(Biomes.Count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < Biomes.Count; i++)
            {
                IndexAndResolution textureTarget = new IndexAndResolution(target.Index + i + 1, target.Resolution);
                array[i] = textureTarget; 
            }
            return array;
        }
        public NativeArray<IndexAndResolution> GenerateIndexArray(GenerationSettings settings)
        {
            var array = new NativeArray<IndexAndResolution>(Biomes.Count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);        
            for (int i = 0; i < Biomes.Count; i++)
            {
                BiomeNode node = Biomes[i] as BiomeNode;
                array[i] = node.RequestHeightIndex(settings);
            }
            return array;
        }
        public NativeArray<IndexAndResolution> GenerateMaskArray(GenerationSettings settings)
        {
            var array = new NativeArray<IndexAndResolution>(Biomes.Count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < Biomes.Count; i++)
            {
                BiomeNode node = Biomes[i] as BiomeNode;
                array[i] = node.RequestDensityIndex(settings);
            }
            return array;
        }


        
        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            var targetMaskIndices = settings.manager.GetValue(settings.BranchGUID+this.guid+"-target", () => GenerateTargetArray(target));
            var indicesArray = settings.manager.GetValue(settings.BranchGUID+this.guid+"-indices", () => GenerateIndexArray(settings));
            var maskIndices = settings.manager.GetValue(settings.BranchGUID+this.guid+"-masks", () => GenerateMaskArray(settings));

            int biomesCount = Biomes.Count;
            BiomeData[] processedData = new BiomeData[biomesCount];
            NativeArray<JobHandle> combinedJobs = new NativeArray<JobHandle>(biomesCount * 2, Allocator.Temp,
                NativeArrayOptions.UninitializedMemory);

            for (int i = 0; i < biomesCount; i++)
            {
                BiomeNode node = Biomes[i] as BiomeNode;
                processedData[i] = node.RequestHeight(settings);

                combinedJobs[i * 2] = processedData[i].HeightMap.jobHandle;
                combinedJobs[i * 2 + 1] = processedData[i].DensityMap.jobHandle;
            }

            JobHandle afterAllBiomes = JobHandle.CombineDependencies(combinedJobs);
            combinedJobs.Dispose();

            switch (operation)
            {
                case Operation.MaxHeight:
                    return MaxHeightBiomeJob.ScheduleParallel(settings.globalMap, indicesArray, maskIndices,
                        targetMaskIndices, minMaxValue, HeightBlending, target, biomesCount,
                        settings.pointsLength, afterAllBiomes);
                case Operation.MaxWeight:
                    return MaxWeightBiomeJob.ScheduleParallel(settings.globalMap, indicesArray, maskIndices,
                        targetMaskIndices, WeightBlending, target, biomesCount,
                        settings.pointsLength, afterAllBiomes);
                default:
                    return AddBiomeJob.ScheduleParallel(settings.globalMap, indicesArray, maskIndices,
                        targetMaskIndices, minMaxValue, target, biomesCount,
                        settings.pointsLength, afterAllBiomes);
            }
        }


        public struct ArrayWithMaskAndJob{
            public ArrayWithMask maskArray;
            public JobHandle job;
        }

        public bool AssetExists(IAsset asset){
            return GetAssets().Any(a => a.Equals(asset));
        }

        public NoiseOutput ProcessAsset(IAsset asset, GenerationSettings settings)
        {
            var targetMaskIndices = settings.manager.GetValue(settings.BranchGUID+this.guid+"-target", () => GenerateTargetArray(RequestIndex(settings)));
            List<ArrayWithMaskAndJob> currentIndices = new List<ArrayWithMaskAndJob>();
            for (int i = 0; i < Biomes.Count; i++)
            {
                BiomeNode node = Biomes[i] as BiomeNode;
                if(node.biome.GetAssetLoaders().TryGetValue(asset, out ILoadAsset[] loaders)){
                    List<NoiseOutput> allOutputs = GenerationHelper.ProcessAssets(loaders, asset, settings);
                    currentIndices.AddRange(allOutputs.Select( a => new ArrayWithMaskAndJob(){
                        job = a.jobHandle,
                        maskArray = new ArrayWithMask(){
                            Density = a.globalIndex,
                            Mask = targetMaskIndices[i],
                        }
                    }));
                }
            }

            HeightData current = RequestHeight(settings);

            IndexAndResolution assetTarget = current.indexData;
            assetTarget.Index = indexAndOffset[asset];
            
            NativeArray<JobHandle> jobHandles = new NativeArray<JobHandle>(currentIndices.Select(a => a.job).ToArray(), Allocator.Temp);
            NativeArray<ArrayWithMask> jobIndices = new NativeArray<ArrayWithMask>(currentIndices.Select(a => a.maskArray).ToArray(), Allocator.Persistent);
            
            JobHandle afterTextureGeneration = JobHandle.CombineDependencies(current.jobHandle, JobHandle.CombineDependencies(jobHandles));
            jobHandles.Dispose();
            
            JobHandle jobHandle;
            Type assetType = asset.GetType();
            if(typeof(IHoldTextures).IsAssignableFrom(assetType)){
                jobHandle = MasksBiomeCombine.ScheduleParallel(settings.globalMap, TextureSmoothing,jobIndices,
                    jobIndices.Length, assetTarget, settings.pointsLength, afterTextureGeneration);
            }
            
            else if(typeof(IHoldVegetation).IsAssignableFrom(assetType)){
                jobHandle = MasksBiomeCombine.ScheduleParallel(settings.globalMap, VegetationSmoothing, jobIndices,
                    jobIndices.Length, assetTarget, settings.pointsLength, afterTextureGeneration);
            }
            else{
                Debug.LogWarning("Processing Unhandled Asset: "+ assetType);
                jobHandle = MasksBiomeCombineStep.ScheduleParallel(settings.globalMap, jobIndices,
                    jobIndices.Length, assetTarget, settings.pointsLength, afterTextureGeneration);
            }

            NoiseOutput textureOutput = new NoiseOutput(){
                globalIndex = assetTarget,
                jobHandle = jobHandle,
            };
            jobIndices.Dispose(jobHandle);          
            return textureOutput;
        }

        protected override int IncreaseNodeIndices(int currentIndex)
        {
            currentIndex += Biomes.Count;
            indexAndOffset = new Dictionary<IAsset, int>();

            IAsset[] AllTheAssets = GetAssets().ToArray();
            for(int i = 1; i <= AllTheAssets.Length; i++){
                indexAndOffset.Add(AllTheAssets[i-1], currentIndex+i);
            }            
            currentIndex += AllTheAssets.Length;
            return currentIndex;
        }
        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            int currentMax = resolution;
            foreach (BiomeNode biome in Biomes)
            {
                int newValue = biome.PrepareBiome(manager,ref currentCount, resolution, ratio, requestGuid);
                currentMax = Math.Max(currentMax, newValue);
            }        
            return currentMax;
        }

    }
}