using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using static Unity.Mathematics.math;
using System.Threading;
using Unity.Mathematics;

namespace sapra.InfiniteLands
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct ApplyHeightJob : IJobFor
    {
        [WriteOnly] NativeArray<Vertex> vertices;

        [NativeDisableContainerSafetyRestriction]
        [ReadOnly] NativeArray<float3> normalMap;
        [ReadOnly] NativeArray<float> globalMap;
        IndexAndResolution heightIndex;
        IndexAndResolution normalIndex;

        [NativeDisableContainerSafetyRestriction]
        NativeArray<int> MinMaxHeight;

        public int arrayLength;
        public int Resolution;
        public float MeshScale;

        public unsafe void Execute(int i)
        {
            int index = MapTools.RemapIndex(i, Resolution, heightIndex.Resolution);
            int norindex = MapTools.RemapIndex(i, Resolution, normalIndex.Resolution);
            
            float3 normal = normalMap[norindex];
            float noise = globalMap[index + heightIndex.Index*arrayLength];
            
            int2 xy = MapTools.IndexToVector(i, Resolution);

            float3 position = 0;
            position.z = (float)xy.y / Resolution - 0.5f;
            position.x = (float)xy.x / Resolution - 0.5f;
            position *= MeshScale;
            position.y = noise;

            this.vertices[i] = new Vertex(){
                position = position,
                normal = normal,
                texCoord0 = 0.5f+position.xz/MeshScale
            };

            int sY = (int)floor(noise);
            int bY = (int)ceil(noise);

            int valMin = MinMaxHeight[0];
            int valMax = MinMaxHeight[1];

            InterlockedMin(ref ((int*)MinMaxHeight.GetUnsafePtr())[0], sY);
            InterlockedMax(ref ((int*)MinMaxHeight.GetUnsafePtr())[1], bY);
        }


        public static JobHandle ScheduleParallel(NativeArray<Vertex> vertices, NativeArray<float> globalMap,
            NativeArray<int> MinMaxHeight, IndexAndResolution height, int resolution, float MeshScale,
            NativeArray<float3> normalMap, IndexAndResolution normalData, 
            JobHandle dependency
        ) => new ApplyHeightJob()
        {
            vertices = vertices,
            globalMap = globalMap,
            heightIndex = height,
            MeshScale = MeshScale,
            MinMaxHeight = MinMaxHeight,
            arrayLength = vertices.Length,
            Resolution = resolution,
            normalIndex = normalData,
            normalMap = normalMap,
        }.ScheduleParallel(vertices.Length, resolution, dependency);


        public static bool InterlockedMax(ref int target, int newValue)
        {
            int snapshot;
            bool stillMore;
            do
            {
                snapshot = target;
                stillMore = newValue > snapshot;
            } while (stillMore && Interlocked.CompareExchange(ref target, newValue, snapshot) != snapshot);

            return stillMore;
        }

        public static bool InterlockedMin(ref int target, int newValue)
        {
            int snapshot;
            bool stillMore;
            do
            {
                snapshot = target;
                stillMore = newValue < snapshot;
            } while (stillMore && Interlocked.CompareExchange(ref target, newValue, snapshot) != snapshot);

            return stillMore;
        }
    }
}