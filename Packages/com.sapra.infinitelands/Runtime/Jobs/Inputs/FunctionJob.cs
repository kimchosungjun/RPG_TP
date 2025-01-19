using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using static Unity.Mathematics.math;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;

namespace sapra.InfiniteLands
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously = true)]
    internal struct FunctionJob<T> : IJobFor where T : struct, IFunctionJob
    {
        [ReadOnly] NativeArray<float3x4> vertices;

        [NativeDisableContainerSafetyRestriction] [WriteOnly]
        NativeArray<float4> globalMap;
        
        IndexAndResolution target;
        int verticesLength;
        
        float2 FromTo;
        float3 offset;
        float frequency;
        float YRotation;

        public void Execute(int i)
        {
            float4x3 pt = transpose(vertices[i]);
            pt.c0 += offset.x;
            pt.c2 += offset.z;

            float4x3 position = Matrix.TransformVectors(pt);
            globalMap[i + target.Index*verticesLength] = lerp(FromTo.x, FromTo.y, default(T).GetValue(position, frequency, YRotation));
        }

        public float3x4 Matrix
        {
            get
            {
                float3x3 m = mul(
                    float3x3(1f), Unity.Mathematics.float3x3.EulerZXY(0, math.radians(YRotation + 45), 0)
                );
                return float3x4(m.c0, m.c1, m.c2, 0);
            }
        }


        public static JobHandle ScheduleParallel(NativeArray<float3x4> vertices, NativeArray<float4> globalMap,
            float2 FromTo, float3 offset, float period, float YRotation,
            IndexAndResolution target, int verticesLength, JobHandle dependency) => new FunctionJob<T>()
        {
            globalMap = globalMap,
            target = target,
            FromTo = FromTo,
            vertices = vertices,
            offset = offset,
            YRotation = YRotation,
            frequency = 1f / period,
            verticesLength = verticesLength/4,
        }.ScheduleParallel(target.Length/4, target.Resolution, dependency);
    }


    public interface IFunctionJob
    {
        public abstract float4 GetValue(float4x3 position, float frequency, float YRotation);
    }

    public struct FSquare : IFunctionJob
    {
        public float4 GetValue(float4x3 position, float frequency, float YRotation)
        {
            float4 xValue = position.c0;
            float4 normalized = xValue * frequency;
            return 2 * floor(normalized) - floor(2 * normalized) + 1;
        }
    }

    public struct FTriangle : IFunctionJob
    {
        public float4 GetValue(float4x3 position, float frequency, float YRotation)
        {
            float4 xValue = position.c0;
            float4 normalized = xValue * frequency + .25f;
            return 2 * abs(normalized - floor(normalized + .5f));
        }
    }

    public struct FSine : IFunctionJob
    {
        public float4 GetValue(float4x3 position, float frequency, float YRotation)
        {
            float4 xValue = position.c0;
            float4 normalized = xValue * frequency;
            return (sin(normalized * 2 * PI) + 1F) / 2f;
        }
    }

    public struct FSawTooth : IFunctionJob
    {
        public float4 GetValue(float4x3 position, float frequency, float YRotation)
        {
            float4 xValue = position.c0;
            float4 normalized = xValue * frequency;
            return normalized - floor(normalized);
        }
    }
}