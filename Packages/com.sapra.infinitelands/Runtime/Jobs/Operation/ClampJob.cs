using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using static Unity.Mathematics.math;

namespace sapra.InfiniteLands
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct ClampJob : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> heightMap;

        IndexAndResolution target;
        IndexAndResolution current;
        int verticesLenght;
        float2 minMaxValue;

        public void Execute(int i)
        {
            int index = MapTools.RemapIndex(i, target.Resolution, current.Resolution);
            float value = heightMap[current.Index * verticesLenght + index];
            heightMap[target.Index * verticesLenght + i] = clamp(value, minMaxValue.x, minMaxValue.y);
        }

        public static JobHandle ScheduleParallel(NativeArray<float> globalMap, IndexAndResolution current, IndexAndResolution target,
            float2 minMaxValue, int length, int resolution,
            JobHandle dependency) => new ClampJob()
        {
            heightMap = globalMap,
            current = current,
            target = target,
            verticesLenght = length,
            minMaxValue = minMaxValue,
        }.ScheduleParallel(target.Length, target.Resolution, dependency);
    }
}