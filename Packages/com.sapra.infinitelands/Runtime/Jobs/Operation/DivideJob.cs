using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace sapra.InfiniteLands
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct DivideJob : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> heightMap;

        int verticesLenght;

        IndexAndResolution dividend;
        IndexAndResolution divisor;

        IndexAndResolution target;

        public void Execute(int i)
        {
            int indexDividend = MapTools.RemapIndex(i, target.Resolution, dividend.Resolution);
            int indexDivisor = MapTools.RemapIndex(i, target.Resolution, divisor.Resolution);

            float valueDividend = heightMap[dividend.Index * verticesLenght + indexDividend];
            float valueDivisor = heightMap[divisor.Index * verticesLenght + indexDivisor];
                        heightMap[target.Index * verticesLenght + i] = valueDivisor != 0 ? valueDividend/valueDivisor : 0;
        }

        public static JobHandle ScheduleParallel(NativeArray<float> globalMap, 
            IndexAndResolution dividend, IndexAndResolution divisor, IndexAndResolution target, 
            int length, JobHandle dependency) => new DivideJob()
        {
            heightMap = globalMap,
            target = target,
            dividend = dividend,
            divisor = divisor,
            verticesLenght = length,
        }.ScheduleParallel(target.Length, target.Resolution, dependency);
    }
}