using sapra.InfiniteLands;
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
    public struct RemapHeightJob : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> heightMap;

        IndexAndResolution current;
        IndexAndResolution target;

        float2 CurrentMinMax;
        float2 TargetMinMax;
        int verticesLenght;

        public void Execute(int i)
        {
            int index = MapTools.RemapIndex(i, target.Resolution, current.Resolution);
            float value = heightMap[current.Index * verticesLenght + index];
            float normalized = JobExtensions.invLerp(CurrentMinMax.x, CurrentMinMax.y, value);
            heightMap[target.Index * verticesLenght + i] = lerp(TargetMinMax.x, TargetMinMax.y, normalized);
        }

        public static JobHandle ScheduleParallel(NativeArray<float> globalMap, 
            IndexAndResolution current,IndexAndResolution target,
            float2 targetMinMax, float2 currentMinMax,
            int verticsLengt, JobHandle dependency) => new RemapHeightJob()
        {
            heightMap = globalMap,
            current = current,
            target = target,
            verticesLenght = verticsLengt,
            TargetMinMax = targetMinMax,
            CurrentMinMax = currentMinMax,
        }.ScheduleParallel(target.Length, target.Resolution, dependency);
    }

    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct RemapCurveJob : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> heightMap;

        [NativeDisableContainerSafetyRestriction]
        [ReadOnly] SampledAnimationCurve curve;

        float2 CurrentMinMax;

        IndexAndResolution current;
        IndexAndResolution target;

        int verticesLenght;

        public void Execute(int i)
        {
            int index = MapTools.RemapIndex(i, target.Resolution, current.Resolution);
            float value = heightMap[current.Index * verticesLenght + index];
            float normalized = JobExtensions.invLerp(CurrentMinMax.x, CurrentMinMax.y, value);
            heightMap[target.Index * verticesLenght + i] = 
                lerp(CurrentMinMax.x, CurrentMinMax.y, curve.EvaluateLerp(normalized));
        }

        public static JobHandle ScheduleParallel(NativeArray<float> globalMap, 
            IndexAndResolution current,IndexAndResolution target,
            SampledAnimationCurve curve, float2 currentMinMax,
            int verticsLengt, JobHandle dependency) => new RemapCurveJob()
        {
            heightMap = globalMap,
            current = current,
            target = target,
            verticesLenght = verticsLengt,
            curve = curve,
            CurrentMinMax = currentMinMax,
        }.ScheduleParallel(target.Length, target.Resolution, dependency);
    }
}