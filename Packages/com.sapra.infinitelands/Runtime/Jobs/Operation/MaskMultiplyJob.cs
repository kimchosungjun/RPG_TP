using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace sapra.InfiniteLands{
    public interface MaskMultiplyMode{
        public float GetValue(float2 minMax, float value, float mask);
    }
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct MaskMultiplyJob<T> : IJobFor where T: MaskMultiplyMode
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> heightMap;

        float2 CurrentMinMax;

        int verticesLenght;
        IndexAndResolution mask;
        IndexAndResolution target;
        IndexAndResolution current;

        public void Execute(int i)
        {
            int index = MapTools.RemapIndex(i, target.Resolution, current.Resolution);
            int indexmask = MapTools.RemapIndex(i, target.Resolution, mask.Resolution);

            float value = heightMap[current.Index * verticesLenght + index];
            float maskValue = heightMap[mask.Index * verticesLenght + indexmask];

            heightMap[target.Index * verticesLenght + i] = default(T).GetValue(CurrentMinMax, value,maskValue);
        }


        public static JobHandle ScheduleParallel(NativeArray<float> globalMap, 
            IndexAndResolution mask, IndexAndResolution current,
            IndexAndResolution target, float2 currentMinMax,
            int length, JobHandle dependency) => new MaskMultiplyJob<T>()
        {
            heightMap = globalMap,
            mask = mask,
            target = target,
            current = current,
            verticesLenght = length,
            CurrentMinMax = currentMinMax,
        }.ScheduleParallel(target.Length, target.Resolution, dependency);
    }
}