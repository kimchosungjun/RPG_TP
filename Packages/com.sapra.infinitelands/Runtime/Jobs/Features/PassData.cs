using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace sapra.InfiniteLands
{    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct PassDataJob : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> globalMap;
        IndexAndResolution target;
        IndexAndResolution original;
        int arrayLenght;

        public void Execute(int i)
        {
            int index = MapTools.RemapIndex(i, target.Resolution, original.Resolution);
            globalMap[arrayLenght * target.Index + i] = globalMap[arrayLenght * original.Index + index];
        }

        public static JobHandle ScheduleParallel(NativeArray<float> globalMap,
            IndexAndResolution target, IndexAndResolution Original,
            int arrayLenght, JobHandle dependency) => new PassDataJob()
        {
            target = target,
            original = Original,
            arrayLenght = arrayLenght,
            globalMap = globalMap,
        }.ScheduleParallel(target.Length, target.Resolution, dependency);
    }
}