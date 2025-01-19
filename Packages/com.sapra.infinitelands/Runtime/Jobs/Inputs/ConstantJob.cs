using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;

namespace sapra.InfiniteLands
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    internal struct ConstantJob : IJobFor
    {
        float Value;

        [NativeDisableContainerSafetyRestriction] [WriteOnly]
        NativeArray<float4> globalMap;

        IndexAndResolution target;
        
        int verticesLength;
        public void Execute(int i)
        {
            globalMap[i + target.Index*verticesLength] = Value;
        }

        public static JobHandle ScheduleParallel(NativeArray<float4> globalMap, 
            float Value, IndexAndResolution target,
            int arrayLength, JobHandle dependency) => new ConstantJob()
        {
            globalMap = globalMap,
            target = target,
            verticesLength = arrayLength/4,
            Value = Value,
        }.ScheduleParallel(target.Length/4, target.Resolution, dependency);
    }
}