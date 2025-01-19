using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace sapra.InfiniteLands{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct CurveMaskJob : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> heightMap;

        float2 CurrentMinMax;

        [NativeDisableContainerSafetyRestriction]
        [ReadOnly] SampledAnimationCurve animationCurve;
        
        IndexAndResolution target;
        IndexAndResolution current;
        int verticesLenght;

        public void Execute(int i)
        {
            int index = MapTools.RemapIndex(i, target.Resolution, current.Resolution);

            float value = heightMap[current.Index * verticesLenght + index];
            float normalized = JobExtensions.invLerp(CurrentMinMax.x, CurrentMinMax.y, value);
            heightMap[target.Index * verticesLenght + i] = animationCurve.EvaluateLerp(normalized);
        }

        public static JobHandle ScheduleParallel(NativeArray<float> globalMap, 
            IndexAndResolution target, IndexAndResolution current,
            SampledAnimationCurve curve, float2 currentMinMax,
            int length, JobHandle dependency) => new CurveMaskJob()
        {
            heightMap = globalMap,
            target = target,
            current = current,
            animationCurve = curve,
            verticesLenght = length,
            CurrentMinMax = currentMinMax,
        }.ScheduleParallel(target.Length, target.Resolution, dependency);
    }
}