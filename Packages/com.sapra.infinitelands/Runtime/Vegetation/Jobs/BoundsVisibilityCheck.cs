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
    public struct BoundsVisibilityCheck : IJobFor
    {        
        [ReadOnly] NativeArray<Plane> frustrumPlanes;
        [ReadOnly] NativeArray<float> chunkSizes;
        [ReadOnly] NativeArray<Bounds> bounds;
        [WriteOnly] NativeArray<bool> isVisible;

        float3 cameraPosition;
        public void Execute(int i)
        {
            Bounds boundingBox = bounds[i];
            Vector3 closestPoint = boundingBox.ClosestPoint(cameraPosition);
            float distance = Vector3.Distance(cameraPosition, closestPoint);
            bool InView = TestPlanesAABB(frustrumPlanes, boundingBox);
            isVisible[i] = InView || distance < chunkSizes[i] / 2;
        }

        public bool TestPlanesAABB(NativeArray<Plane> planes, Bounds bounds)
        {
            for (int i = 0; i < planes.Length; i++)
            {
            Plane plane = planes[i];
            float3 normal_sign = math.sign(plane.normal);
            float3 test_point = (float3)(bounds.center) + (bounds.extents * normal_sign);

                float dot = math.dot(test_point, plane.normal);
                if (dot + plane.distance < 0)
                    return false;
            }

            return true;
        }
        public static JobHandle ScheduleParallel(NativeArray<Plane> frustrumPlanes, 
            NativeArray<float> chunkSizes, NativeArray<Bounds> bounds,  NativeArray<bool> isVisible, 
            float3 cameraPosition, int length, JobHandle dependency) => new BoundsVisibilityCheck()
        {
            frustrumPlanes = frustrumPlanes,
            chunkSizes = chunkSizes,
            bounds = bounds,
            isVisible = isVisible,
            cameraPosition = cameraPosition,
        }.ScheduleParallel(length, 8, dependency);
    }
}