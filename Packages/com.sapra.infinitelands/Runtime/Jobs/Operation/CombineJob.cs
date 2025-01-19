using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace sapra.InfiniteLands
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct AddJob : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> globalArray;

        [ReadOnly] NativeArray<IndexAndResolution> indices;
        IndexAndResolution target;

        int arrayLenght;
        int arrayCount;

        //Moved out into diferent jobs!
        public void Execute(int i)
        {
            float sum = 0;
            for (int x = 0; x < arrayCount; x++)
            {
                int index = indices[x].Index;
                int locator = MapTools.RemapIndex(i, target.Resolution, indices[x].Resolution);
                sum += globalArray[index * arrayLenght + locator];
            }

            globalArray[target.Index * arrayLenght + i] = sum;
        }

        public static JobHandle ScheduleParallel(NativeArray<float> input, NativeArray<IndexAndResolution> indices,IndexAndResolution target,
            int arrayCount, int arrayLength, JobHandle dependency) => new AddJob()
        {
            globalArray = input,
            target = target,
            indices = indices,
            arrayCount = arrayCount,
            arrayLenght = arrayLength,
        }.ScheduleParallel(target.Length, target.Resolution, dependency);
    }

    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct MaxJob : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> globalArray;

        [ReadOnly] NativeArray<IndexAndResolution> indices;
        IndexAndResolution target;

        int arrayLenght;
        int arrayCount;

        //Moved out into diferent jobs!
        public void Execute(int i)
        {
            float sum = float.MinValue;
            for (int x = 0; x < arrayCount; x++)
            {
                int index = indices[x].Index;
                int locator = MapTools.RemapIndex(i, target.Resolution, indices[x].Resolution);
                sum = max(globalArray[index * arrayLenght + locator], sum);
            }

            globalArray[target.Index * arrayLenght + i] = sum;
        }

        public static JobHandle ScheduleParallel(NativeArray<float> input, NativeArray<IndexAndResolution> indices,IndexAndResolution target,
            int arrayCount, int arrayLength, JobHandle dependency) => new MaxJob()
        {
            globalArray = input,
            target = target,
            indices = indices,
            arrayCount = arrayCount,
            arrayLenght = arrayLength,
        }.ScheduleParallel(target.Length, target.Resolution, dependency);
    }

    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct MinJob : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> globalArray;

        [ReadOnly] NativeArray<IndexAndResolution> indices;
        IndexAndResolution target;

        int arrayLenght;
        int arrayCount;

        //Moved out into diferent jobs!
        public void Execute(int i)
        {
            float sum = float.MaxValue;
            for (int x = 0; x < arrayCount; x++)
            {
                int index = indices[x].Index;
                int locator = MapTools.RemapIndex(i, target.Resolution, indices[x].Resolution);
                sum = min(globalArray[index * arrayLenght + locator], sum);
            }

            globalArray[target.Index * arrayLenght + i] = sum;
        }

        public static JobHandle ScheduleParallel(NativeArray<float> input, NativeArray<IndexAndResolution> indices,IndexAndResolution target,
            int arrayCount, int arrayLength, JobHandle dependency) => new MinJob()
        {
            globalArray = input,
            target = target,
            indices = indices,
            arrayCount = arrayCount,
            arrayLenght = arrayLength,
        }.ScheduleParallel(target.Length, target.Resolution, dependency);
    }

    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct HeightBlend : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> globalArray;

        [ReadOnly] NativeArray<IndexAndResolution> indices;
        IndexAndResolution target;
        public float BlendFactor;

        int arrayLenght;
        int arrayCount;
        float2 MinMax;

        //Moved out into diferent jobs!
        public void Execute(int i)
        {
            float maxValue = 0;
            NativeArray<float> remapedWeights = new NativeArray<float>(arrayCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for (int x = 0; x < arrayCount; x++)
            {
                int index = indices[x].Index;
                int locator = MapTools.RemapIndex(i, target.Resolution, indices[x].Resolution);
                remapedWeights[x] = JobExtensions.invLerp(MinMax.x, MinMax.y, globalArray[index * arrayLenght + locator]);
                maxValue = max(maxValue, remapedWeights[x]);
            }

            maxValue -= BlendFactor / (MinMax.y - MinMax.x);

            float totalSum = 0;
            for (int x = 0; x < arrayCount; x++)
            {
                remapedWeights[x] = max(remapedWeights[x] - maxValue, 0);
                totalSum += remapedWeights[x];
            }

            float sum = 0;
            for (int x = 0; x < arrayCount; x++)
            {
                int index = indices[x].Index;
                sum += globalArray[index * arrayLenght + i] * remapedWeights[x] / totalSum;
            }

            globalArray[target.Index * arrayLenght + i] = sum;
        }

        public static JobHandle ScheduleParallel(NativeArray<float> input, NativeArray<IndexAndResolution> indices,float2 minMax, float BlendFactor,
            IndexAndResolution target, int arrayCount, int arrayLength, JobHandle dependency) => new HeightBlend()
        {
            globalArray = input,
            target = target,
            indices = indices,
            arrayCount = arrayCount,
            arrayLenght = arrayLength,
            MinMax = minMax,
            BlendFactor = BlendFactor,
        }.ScheduleParallel(target.Length, target.Resolution, dependency);
    }

    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct NormalizedMultiplyJob : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> globalArray;

        [ReadOnly] NativeArray<float2> MinMaxHeights;

        [ReadOnly] NativeArray<IndexAndResolution> indices;
        IndexAndResolution target;

        int arrayLenght;
        int arrayCount;

        //Moved out into diferent jobs!
        public void Execute(int i)
        {
            float mult = 1;
            for (int x = 0; x < arrayCount; x++)
            {
                int index = indices[x].Index;
                int locator = MapTools.RemapIndex(i, target.Resolution, indices[x].Resolution);
                mult *= JobExtensions.invLerp(MinMaxHeights[x].x, MinMaxHeights[x].y,globalArray[index * arrayLenght + locator]);
            }

            globalArray[target.Index * arrayLenght + i] = mult;
        }

        public static JobHandle ScheduleParallel(NativeArray<float> input, NativeArray<IndexAndResolution> indices, NativeArray<float2> MinMaxHeights, 
            IndexAndResolution target,
            int arrayCount, int arrayLength, JobHandle dependency) => new NormalizedMultiplyJob()
        {
            MinMaxHeights = MinMaxHeights,
            globalArray = input,
            target = target,
            indices = indices,
            arrayCount = arrayCount,
            arrayLenght = arrayLength,
        }.ScheduleParallel(target.Length, target.Resolution, dependency);
    }
}