using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace sapra.InfiniteLands
{    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    public struct GenerateNormalMap : IJobFor
    {
        [ReadOnly] NativeArray<float3> points;
        [WriteOnly] NativeArray<float3> normalMap;

        int verticesResolution;
        
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float> globalMap;

        IndexAndResolution normalIndex;
        IndexAndResolution current;

        int arrayLenght;
        float MeshScale;
        public void Execute(int i)
        {
            int2 vector = MapTools.IndexToVector(i, normalIndex.Resolution);
            
            float3 normal = calculateNormalAtIndex(vector.x, vector.y);
            normalMap[i] = normal;
        }
        
        float3 calculateNormalAtIndex(int x, int y){
            float3 normal1 = calculatePlaneNormalFromIndices(int2(x, y), int2(x - 1, y), int2(x - 1, y + 1));
            float3 normal2 = calculatePlaneNormalFromIndices(int2(x, y), int2(x - 1, y + 1), int2(x, y + 1));
            float3 normal3 = calculatePlaneNormalFromIndices(int2(x, y), int2(x, y + 1), int2(x + 1, y));
            float3 normal4 = calculatePlaneNormalFromIndices(int2(x, y), int2(x + 1, y), int2(x + 1, y - 1));
            float3 normal5 = calculatePlaneNormalFromIndices(int2(x, y), int2(x + 1, y - 1), int2(x, y - 1));
            float3 normal6 = calculatePlaneNormalFromIndices(int2(x, y), int2(x, y - 1), int2(x - 1, y));

            float3 result = normalize(normal1 + normal2 + normal3 + normal4 + normal5 + normal6);

            return result;
        }

        float3 calculatePlaneNormalFromIndices(int2 A, int2 B, int2 C)
        {
            var PA = position(A.x, A.y);
            var PB = position(B.x, B.y);
            var PC = position(C.x, C.y);

            float3 re1 = normalize(PB - PA);
            if(length(re1).Equals(float.NaN) || length(re1) < 1e-1)
                re1 = float3(0,0,1);

            float3 re2 = normalize(PC - PA);
             if(length(re2).Equals(float.NaN) || length(re2) < 1e-1)
                re2 = float3(1,0,0);

            float3 result = cross(re1, re2);
            if(result.y < 0)
                result = -result;
            
            return normalize(result);
        }


        public float3 position(int x, int y)
        {
            int index = MapTools.GetFlatIndex(int2(x,y), normalIndex.Resolution, current.Resolution);
            int vertexIndex = MapTools.GetFlatIndex(int2(x,y), normalIndex.Resolution, verticesResolution);
            float3 ps = points[vertexIndex];
            float2 scaledPosition = MeshScale*float2(x,y)/(verticesResolution); //fake fix for strong warping, should it be used?


            float3 position = float3(ps.x, globalMap[index + current.Index * arrayLenght],ps.z);
            return position;
        }

        public static JobHandle ScheduleParallel(NativeArray<float3> vertices, NativeArray<float3> normalMap,
            NativeArray<float> globalMap, IndexAndResolution normalIndex, IndexAndResolution original, 
            int arrayLenght, int verticesResolution, float meshScale,
            JobHandle dependency) => new GenerateNormalMap()
        {
            points = vertices,
            verticesResolution = verticesResolution,
            arrayLenght = arrayLenght,
            globalMap = globalMap,
            normalIndex = normalIndex,
            current = original,
            MeshScale = meshScale,
            normalMap = normalMap,
        }.ScheduleParallel(normalIndex.Length, normalIndex.Resolution, dependency);
    }
}