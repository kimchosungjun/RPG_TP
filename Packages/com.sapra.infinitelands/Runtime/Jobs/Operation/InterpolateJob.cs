using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace sapra.InfiniteLands
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct InterpolateJob : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> heightMap;

        float2 MaskMinMax;

        int verticesLenght;

        IndexAndResolution height0;
        IndexAndResolution height1;
        IndexAndResolution mask;

        IndexAndResolution target;

        public void Execute(int i)
        {
            int index0 = MapTools.RemapIndex(i, target.Resolution, height0.Resolution);
            int inde1 = MapTools.RemapIndex(i, target.Resolution, height1.Resolution);
            int indexMask = MapTools.RemapIndex(i, target.Resolution, mask.Resolution);

            float value0 = heightMap[height0.Index * verticesLenght + index0];
            float value1 = heightMap[height1.Index * verticesLenght + inde1];
            float maskValue = JobExtensions.invLerp(MaskMinMax.x, MaskMinMax.y, heightMap[mask.Index * verticesLenght + indexMask]);
            heightMap[target.Index * verticesLenght + i] = lerp(value0, value1, maskValue);
        }

        public static JobHandle ScheduleParallel(NativeArray<float> globalMap, 
            IndexAndResolution mask, IndexAndResolution height0, IndexAndResolution height1, IndexAndResolution target, 
            float2 MaskMinMax,
            int length, JobHandle dependency) => new InterpolateJob()
        {
            heightMap = globalMap,
            mask = mask,
            target = target,
            height0 = height0,
            height1 = height1,
            verticesLenght = length,
            MaskMinMax = MaskMinMax,
        }.ScheduleParallel(target.Length, target.Resolution, dependency);
    }
}