using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;

namespace sapra.InfiniteLands
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    internal struct MasksBiomeCombine : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> globalArray;

        [ReadOnly] NativeArray<ArrayWithMask> ArrayWithMasks;

        IndexAndResolution target;
        int arrayLenght;
        int masksCount;

        float TextureSmoothing;

        public void Execute(int i)
        {
            float finalWeight = 0;
            for (int x = 0; x < masksCount; x++)
            {
                ArrayWithMask mask = ArrayWithMasks[x];
                int maskIndex = MapTools.RemapIndex(i, target.Resolution, mask.Mask.Resolution);
                int densityIndex = MapTools.RemapIndex(i, target.Resolution, mask.Density.Resolution);
                
                float maskValue = globalArray[mask.Mask.Index * arrayLenght + maskIndex];
                float densityValue = globalArray[mask.Density.Index * arrayLenght + densityIndex];

                maskValue = math.smoothstep(0.5f-TextureSmoothing, 0.5f+TextureSmoothing, maskValue);
                maskValue = math.saturate(maskValue);
                
                finalWeight += maskValue*densityValue;
            }

            globalArray[target.Index * arrayLenght + i] = finalWeight;
        }

        public static JobHandle ScheduleParallel(NativeArray<float> globalArray, float TextureSmoothing,
            NativeArray<ArrayWithMask> ArrayWithMasks,
            int masksCount, IndexAndResolution target, int arrayLenght, JobHandle dependency)
        {
            return new MasksBiomeCombine()
            {
                TextureSmoothing = TextureSmoothing,
                globalArray = globalArray,
                ArrayWithMasks = ArrayWithMasks,
                masksCount = masksCount,
                target = target,
                arrayLenght = arrayLenght,
            }.ScheduleParallel(target.Length, target.Resolution, dependency);
        }
    }

    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    internal struct MasksBiomeCombineStep : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> globalArray;

        [ReadOnly] NativeArray<ArrayWithMask> ArrayWithMasks;

        IndexAndResolution target;
        int arrayLenght;
        int masksCount;

        public void Execute(int i)
        {
            float finalWeight = 0;
            for (int x = 0; x < masksCount; x++)
            {
                ArrayWithMask mask = ArrayWithMasks[x];

                int maskIndex = MapTools.RemapIndex(i, target.Resolution, mask.Mask.Resolution);
                int densityIndex = MapTools.RemapIndex(i, target.Resolution, mask.Density.Resolution);
                
                float smoothed = math.smoothstep(.5f, .8f, globalArray[mask.Mask.Index * arrayLenght + maskIndex]);
                finalWeight += smoothed * globalArray[mask.Density.Index * arrayLenght + densityIndex];
            }

            globalArray[target.Index * arrayLenght + i] = finalWeight;
        }

        public static JobHandle ScheduleParallel(NativeArray<float> globalArray,
            NativeArray<ArrayWithMask> ArrayWithMasks,
            int masksCount, IndexAndResolution target, int arrayLenght, JobHandle dependency)
        {
            return new MasksBiomeCombineStep()
            {
                globalArray = globalArray,
                ArrayWithMasks = ArrayWithMasks,
                masksCount = masksCount,
                target = target,
                arrayLenght = arrayLenght,
            }.ScheduleParallel(target.Length, target.Resolution, dependency);
        }
    }
}