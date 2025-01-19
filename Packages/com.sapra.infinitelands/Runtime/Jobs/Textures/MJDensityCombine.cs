using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using static Unity.Mathematics.math;
using Unity.Collections.LowLevel.Unsafe;

namespace sapra.InfiniteLands
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    internal struct MJDensityCombine : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> globalArray;

        [ReadOnly] NativeArray<IndexAndResolution> targetIndices;

        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> targetArray;

        int target;
        int resolution;
        int arrayLenght;
        int textureLength;

        public void Execute(int i)
        {
            float currentDensity = 0;
            for (int x = 0; x < targetIndices.Length; x++)
            {
                int index = MapTools.RemapIndex(i, resolution, targetIndices[x].Resolution);
                currentDensity += globalArray[targetIndices[x].Index * arrayLenght + index];
            }
            targetArray[target * textureLength + i] = saturate(currentDensity);
        }

        public static JobHandle ScheduleParallel(NativeArray<float> globalArray, NativeArray<float> targetArray,
            NativeArray<IndexAndResolution> targetIndices, int target, int textureLength, int arrayLenght,
            int resolution, JobHandle dependency)
        {
            return new MJDensityCombine()
            {
                globalArray = globalArray,
                targetArray = targetArray,
                targetIndices = targetIndices,
                target = target,
                arrayLenght = arrayLenght,
                resolution = resolution,
                textureLength = textureLength,
            }.ScheduleParallel(textureLength, resolution, dependency);
        }
    }
}