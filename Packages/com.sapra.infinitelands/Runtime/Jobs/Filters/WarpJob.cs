using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace sapra.InfiniteLands{    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct WarpTrespasJob : IJobFor
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> targetGlobalMap;

        [NativeDisableContainerSafetyRestriction] [ReadOnly]
        NativeArray<float> originalGlobalMap;

        IndexAndResolution current;
        IndexAndResolution target;
        int arrayLength;
        int originalArrayLength;
        public void Execute(int i)
        {            
            int index = MapTools.RemapIndex(i, target.Resolution, current.Resolution);
            targetGlobalMap[target.Index*arrayLength+i] = originalGlobalMap[current.Index*originalArrayLength+index];
        }

        public static JobHandle ScheduleParallel(NativeArray<float> targetGlobalMap, NativeArray<float> originalGlobalMap,
            IndexAndResolution target, IndexAndResolution current,
            int length, int originalArrayLength, JobHandle dependency) => new WarpTrespasJob()
        {
            targetGlobalMap = targetGlobalMap,
            originalGlobalMap = originalGlobalMap,
            current = current,
            target = target,
            arrayLength = length,
            originalArrayLength = originalArrayLength
        }.ScheduleParallel(target.Length, target.Resolution, dependency);
    }


    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct WarpPointsJob : IJobFor
    {
        [ReadOnly] NativeArray<float3> originalPoints;
        [WriteOnly] NativeArray<float3> targetPoints;

        [NativeDisableContainerSafetyRestriction] [ReadOnly]
        NativeArray<float> globalMapX;

        [NativeDisableContainerSafetyRestriction] [ReadOnly]
        NativeArray<float> globalMapY;
        IndexAndResolution warpX;
        IndexAndResolution warpY;

        int arrayLength;
        int resolution;
        public void Execute(int i)
        {
            int warpXIndex = MapTools.RemapIndex(i, resolution, warpX.Resolution);
            float warpValueX = globalMapX[warpXIndex+ arrayLength*warpX.Index];
            warpValueX = warpValueX * 2f - 1f;

            int warpYIndex = MapTools.RemapIndex(i, resolution, warpY.Resolution);
            float warpValueY = globalMapY[warpYIndex+arrayLength*warpY.Index];
            warpValueY = warpValueY * 2f - 1f;
            
            float3 finalWarp = float3(warpValueX, 0, warpValueY);
            targetPoints[i] = originalPoints[i]+finalWarp;
        }

        public static JobHandle ScheduleParallel(NativeArray<float3> originalPoints, NativeArray<float3> targetPoints,
            NativeArray<float> globalMapX, NativeArray<float> globalMapY, IndexAndResolution warpX,IndexAndResolution warpY,
            int length, int resolution, JobHandle dependency) => new WarpPointsJob()
        {
            originalPoints = originalPoints,
            globalMapX = globalMapX,
            globalMapY = globalMapY,
            targetPoints = targetPoints,
            warpX = warpX,
            warpY = warpY,
            arrayLength = length,
            resolution = resolution,
        }.ScheduleParallel(length, resolution, dependency);
    }


    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct WarpPointsMaskedJob : IJobFor
    {
        [ReadOnly] NativeArray<float3> originalPoints;
        [WriteOnly] NativeArray<float3> targetPoints;

        [NativeDisableContainerSafetyRestriction] [ReadOnly]
        NativeArray<float> globalMapX;
        [NativeDisableContainerSafetyRestriction] [ReadOnly]
        NativeArray<float> globalMapY;

        IndexAndResolution warpX;
        IndexAndResolution warpY;
        IndexAndResolution mask;

        int arrayLength;
        int resolution;
        public void Execute(int i)
        {
            int warpXIndex = MapTools.RemapIndex(i, resolution, warpX.Resolution);
            float warpValueX = globalMapX[warpXIndex+ arrayLength*warpX.Index];
            warpValueX = warpValueX * 2f - 1f;

            int warpYIndex = MapTools.RemapIndex(i, resolution, warpY.Resolution);
            float warpValueY = globalMapY[warpYIndex+arrayLength*warpY.Index];
            warpValueY = warpValueY * 2f - 1f;
            
            int maskIndex = MapTools.RemapIndex(i, resolution, mask.Resolution);
            float maskValue = globalMapX[maskIndex+arrayLength*mask.Index];
            warpValueX *= maskValue;
            warpValueY *= maskValue;
            

            float3 finalWarp = float3(warpValueX, 0, warpValueY);
            targetPoints[i] = originalPoints[i] + finalWarp;
        }

        public static JobHandle ScheduleParallel(NativeArray<float3> originalPoints, NativeArray<float3> targetPoints,
            NativeArray<float> globalMapX, NativeArray<float> globalMapY, IndexAndResolution warpX,IndexAndResolution warpY, IndexAndResolution mask,
            int length, int resolution, JobHandle dependency) => new WarpPointsMaskedJob()
        {
            originalPoints = originalPoints,
            globalMapX = globalMapX,
            globalMapY = globalMapY,
            targetPoints = targetPoints,
            warpX = warpX,
            warpY = warpY,
            arrayLength = length,
            mask = mask,
            resolution = resolution,
        }.ScheduleParallel(length, resolution, dependency);
    }

    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct WarpPointsNormalMapJob : IJobFor
    {
        [ReadOnly] NativeArray<float3> originalPoints;
        [WriteOnly] NativeArray<float3> targetPoints;

        [NativeDisableContainerSafetyRestriction] [ReadOnly]
        NativeArray<float3> normalMap;
        IndexAndResolution normalIndex;

        int arrayLength;
        int resolution;
        float strength;
        public void Execute(int i)
        {
            int warpIndex = MapTools.RemapIndex(i, resolution, normalIndex.Resolution);
            float3 warpValueX = normalMap[warpIndex+ arrayLength*normalIndex.Index];
    
            float3 finalWarp = float3(warpValueX.x, 0, warpValueX.z)*strength;
            targetPoints[i] = originalPoints[i]+finalWarp;
        }

        public static JobHandle ScheduleParallel(NativeArray<float3> originalPoints, NativeArray<float3> targetPoints,
            NativeArray<float3> normalMap, IndexAndResolution normalIndex, float strength,
            int length, int resolution, JobHandle dependency) => new WarpPointsNormalMapJob()
        {
            strength = strength,
            originalPoints = originalPoints,
            normalMap = normalMap,
            targetPoints = targetPoints,
            normalIndex = normalIndex,
            arrayLength = length,
            resolution = resolution,
        }.ScheduleParallel(length, resolution, dependency);
    }
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct WarpPointsNormalMaskedJob : IJobFor
    {
        [ReadOnly] NativeArray<float3> originalPoints;
        [WriteOnly] NativeArray<float3> targetPoints;

        [NativeDisableContainerSafetyRestriction] [ReadOnly]
        NativeArray<float> globalMapX;
        [NativeDisableContainerSafetyRestriction] [ReadOnly]
        NativeArray<float3> normalMap;
        IndexAndResolution normalIndex;
        IndexAndResolution mask;

        int arrayLength;
        int resolution;
        float strength;

        public void Execute(int i)
        {
            int warpIndex = MapTools.RemapIndex(i, resolution, normalIndex.Resolution);
            float3 warpValueX = normalMap[warpIndex+ arrayLength*normalIndex.Index];
    
            
            int maskIndex = MapTools.RemapIndex(i, resolution, mask.Resolution);
            float maskValue = globalMapX[maskIndex+arrayLength*mask.Index];
            warpValueX *= maskValue;
            

            float3 finalWarp = float3(warpValueX.x, 0, warpValueX.z)*strength;
            targetPoints[i] = originalPoints[i] + finalWarp;
        }

        public static JobHandle ScheduleParallel(NativeArray<float3> originalPoints, NativeArray<float3> targetPoints,
            NativeArray<float> globalMapX, NativeArray<float3> normalMap, IndexAndResolution normalIndex, float strength, IndexAndResolution mask,
            int length, int resolution, JobHandle dependency) => new WarpPointsNormalMaskedJob()
        {
            strength = strength,
            originalPoints = originalPoints,
            globalMapX = globalMapX,
            normalMap = normalMap,
            targetPoints = targetPoints,
            normalIndex = normalIndex,
            arrayLength = length,
            mask = mask,
            resolution = resolution,
        }.ScheduleParallel(length, resolution, dependency);
    }
}