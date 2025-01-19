using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using System.Linq;

using System;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Combine", type = "Operation", docs = "https://ensapra.com/packages/infinite_lands/nodes/combine.html")]
    public class CombineNode : HeightNodeBase
    {
        protected override Vector2 GetMinMaxValue()
        {
            IEnumerable<Vector2> dataGivers = HeightMaps.Select(a => a as HeightDataGenerator).Select(a => a.minMaxValue);
            switch (operation)
            {
                case Operation.Max:
                {
                    Vector2 MinMaxValue = new Vector2(float.MinValue, float.MinValue);
                    foreach (Vector2 minMax in dataGivers)
                    {
                        MinMaxValue.x = math.max(MinMaxValue.x, minMax.x);
                        MinMaxValue.y = math.max(MinMaxValue.y, minMax.y);
                    }

                    return MinMaxValue;
                }
                case Operation.Min:
                {
                    Vector2 MinMaxValue = new Vector2(float.MaxValue, float.MaxValue);
                    foreach (Vector2 minMax in dataGivers)
                    {
                        MinMaxValue.x = math.min(MinMaxValue.x, minMax.x);
                        MinMaxValue.y = math.min(MinMaxValue.y, minMax.y);
                    }

                    return MinMaxValue;
                }
                case Operation.HeightBased:
                {
                    Vector2 MinMaxValue = new Vector2(float.MinValue, float.MinValue);
                    foreach (Vector2 minMax in dataGivers)
                    {
                        MinMaxValue.x = math.max(MinMaxValue.x, minMax.x);
                        MinMaxValue.y = math.max(MinMaxValue.y, minMax.y);
                    }

                    return MinMaxValue;
                }
                case Operation.NormalizedMultiply:
                    return new Vector2(0, 1);
                default:
                {
                    Vector2 MinMaxValue = Vector2.zero;
                    foreach (Vector2 minMax in dataGivers)
                    {
                        MinMaxValue.x += minMax.x;
                        MinMaxValue.y += minMax.y;
                    }

                    return MinMaxValue;
                }
            }
        }

        public enum Operation
        {
            Add,
            Max,
            Min,
            HeightBased,
            NormalizedMultiply
        }

        public Operation operation;
        [Input(typeof(IGive<HeightData>))] public List<InfiniteLandsNode> HeightMaps = new List<InfiniteLandsNode>();

        protected override InfiniteLandsNode[] Dependancies => HeightMaps.ToArray();
        protected override bool ExtraValidationSteps() => HeightMaps.Count > 0;

        private bool isHeight => operation == Operation.HeightBased;
        [ShowIfCustom("isHeight")] public float BlendFactor = 10;
        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            int currentMax = resolution;
            foreach (InfiniteLandsNode height in HeightMaps)
            {
                HeightDataGenerator giveHeightData = height as HeightDataGenerator;
                currentMax = Math.Max(currentMax,giveHeightData.PrepareNode(manager,ref currentCount, resolution, ratio, requestGuid));
            }        
            return currentMax;
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            IEnumerable<HeightDataGenerator> dataGivers = HeightMaps.Select(a => a as HeightDataGenerator);
            int length = dataGivers.Count();
            HeightData[] processedData = new HeightData[length];

            NativeArray<JobHandle> combinedJobs = new NativeArray<JobHandle>(length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<IndexAndResolution> indicesArray = new NativeArray<IndexAndResolution>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            
            int index = 0;
            foreach(var element in dataGivers){
                processedData[index] = element.RequestHeight(settings);
                indicesArray[index] = processedData[index].indexData;
                combinedJobs[index] = processedData[index].jobHandle;
                index++;
            }
            JobHandle onceChild = JobHandle.CombineDependencies(combinedJobs);
            combinedJobs.Dispose();

            JobHandle combineJob;
            switch (operation)
            {
                case Operation.Max:
                    combineJob = MaxJob.ScheduleParallel(settings.globalMap, indicesArray, target,
                        length, settings.pointsLength, 
                        onceChild);
                    break;
                case Operation.Min:
                    combineJob = MinJob.ScheduleParallel(settings.globalMap, indicesArray, target,
                        length, settings.pointsLength,
                        onceChild);
                    break;
                case Operation.HeightBased:
                    combineJob = HeightBlend.ScheduleParallel(settings.globalMap, indicesArray, minMaxValue, BlendFactor,
                        target, length, settings.pointsLength, onceChild);
                    break;
                case Operation.NormalizedMultiply:
                    NativeArray<float2> MinMaxHeights = new NativeArray<float2>(dataGivers.Select(a => (float2)a.minMaxValue).ToArray(), Allocator.Persistent);
                    combineJob = NormalizedMultiplyJob.ScheduleParallel(settings.globalMap, indicesArray, MinMaxHeights, target,
                        length, settings.pointsLength, 
                        onceChild);
                    MinMaxHeights.Dispose(combineJob);
                    break;
                default:
                    combineJob = AddJob.ScheduleParallel(settings.globalMap, indicesArray, target,
                        length, settings.pointsLength, 
                        onceChild);
                    break;
            }

            indicesArray.Dispose(combineJob);
            return combineJob;
        }
    }
}