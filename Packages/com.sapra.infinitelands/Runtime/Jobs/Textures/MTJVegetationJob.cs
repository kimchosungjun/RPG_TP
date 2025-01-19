using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Unity.Collections.LowLevel.Unsafe;

namespace sapra.InfiniteLands
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct MTJVegetationJobFlat : IJobFor
    {
        [ReadOnly] NativeArray<float> globalArray;

        [NativeDisableContainerSafetyRestriction]
        NativeArray<Color32> normalColor;

        int ArrayLength;
        int target;
        int MaxArrays;

        public void Execute(int i)
        {
            float basicR = globalArray[target * ArrayLength + i];
            float basicG = 0;
            float basicB = 0;
            float basicA = 0;
            int p1 = (target + 1) * ArrayLength + i;
            if (p1 < MaxArrays*ArrayLength)
            {
                basicG = globalArray[p1];
                int p2 = (target + 2) * ArrayLength + i;
                if (p2 < MaxArrays*ArrayLength)
                {
                    basicB = globalArray[p2];
                    int p3 = (target + 3) * ArrayLength + i;
                    if (p3 < MaxArrays*ArrayLength)
                    {
                        basicA = globalArray[p3];
                    }
                }
            }

            float4 basics = float4(basicR, basicG, basicB, basicA);
            normalColor[i] = JobExtensions.toColor32(saturate(basics));
        }

        public static JobHandle ScheduleParallel(NativeArray<float> globalArray,
            NativeArray<Color32> normalColor, int target, int MaxArrays,
            int ArrayLength, int resolution, JobHandle dependency)
        {
            return new MTJVegetationJobFlat()
            {
                normalColor = normalColor,
                globalArray = globalArray,
                target = target,
                ArrayLength = ArrayLength,
                MaxArrays = MaxArrays,
            }.ScheduleParallel(normalColor.Length, resolution, dependency);
        }
    }
}