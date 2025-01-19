using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace sapra.InfiniteLands
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    internal struct MTJGeneral : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<Color32> normalColor;

        [ReadOnly] NativeArray<float> globalArray;
        float2 minMaxValue;
        IndexAndResolution origin;
        int verticesLength;
        int resolution;
        public void Execute(int i)
        {
            int index = MapTools.RemapIndex(i, resolution, origin.Resolution);
            float current = globalArray[origin.Index*verticesLength + index];
            float colorValue = 0;
            if (minMaxValue.y - minMaxValue.x != 0)
            {
                colorValue = (current - minMaxValue.x) / (minMaxValue.y - minMaxValue.x);
            }
            normalColor[i] = JobExtensions.toColor(saturate(colorValue));
        }

        public static JobHandle ScheduleParallel(NativeArray<Color32> textureArray,
            NativeArray<float> globalArray, float2 minMaxValue, IndexAndResolution origin, int verticesLength,
            int resolution, JobHandle dependency)
        {
            return new MTJGeneral()
            {
                normalColor = textureArray,
                globalArray = globalArray,
                minMaxValue = minMaxValue,
                verticesLength = verticesLength,
                origin = origin,
                resolution = resolution,
            }.ScheduleParallel(textureArray.Length, resolution, dependency);
        }
    }
}