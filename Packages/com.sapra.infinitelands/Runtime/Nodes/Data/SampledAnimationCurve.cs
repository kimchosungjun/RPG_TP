using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using System;
using Unity.Jobs;

namespace sapra.InfiniteLands
{
    public struct SampledAnimationCurve : IDisposableJob
    {
        public static SampledAnimationCurve Default =>
            new SampledAnimationCurve(new AnimationCurve());

        [NativeDisableParallelForRestriction]
        private NativeArray<float> FunctionModifier;


        /// <param name="samples">Must be 2 or higher</param>
        private readonly static int samples = 255;

        public SampledAnimationCurve(AnimationCurve ac)
        {
            FunctionModifier = new NativeArray<float>(samples, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            float timeFrom = 0f;
            float timeTo = 1f;
            float timeStep = (timeTo - timeFrom) / (samples - 1);

            for (int i = 0; i < samples; i++)
            {
                FunctionModifier[i] = Mathf.Clamp01(ac.Evaluate(timeFrom + (i * timeStep)));
            }
            
        }

        public void Dispose(JobHandle dependancy)
        {
            if (FunctionModifier.IsCreated)
                FunctionModifier.Dispose(dependancy);
        }

        public bool IsCreated => FunctionModifier.IsCreated;

        public float4 EvaluateLerp(float4 time)
        {
            var value = getValue(FunctionModifier, time, samples);
            return value;
        }

        public float EvaluateLerp(float time)
        {
            float value = getValue(FunctionModifier, time, samples);
            return value;
        }

        static float4 getValue(NativeArray<float> func, float4 value, int samples)
        {
            int len = samples - 1;
            float4 floatIndex = math.saturate(value) * len;
            int4 floorIndex = (int4)floatIndex;
            int4 ceilIndex = math.clamp(floorIndex + 1, 0, len);

            float4 lowerValue = 0f;
            lowerValue.x = func[floorIndex.x];
            lowerValue.y = func[floorIndex.y];
            lowerValue.z = func[floorIndex.z];
            lowerValue.w = func[floorIndex.w];

            float4 higherValue = 0f;
            higherValue.x = func[ceilIndex.x];
            higherValue.y = func[ceilIndex.y];
            higherValue.z = func[ceilIndex.z];
            higherValue.w = func[ceilIndex.w];
            return math.lerp(lowerValue, higherValue, math.frac(floatIndex));
        }

        static float getValue(NativeArray<float> func, float value,int samples)
        {
            int len = samples - 1;
            float floatIndex = math.saturate(value) * len;
            int floorIndex = (int)floatIndex;
            int ceilIndex = math.clamp(floorIndex + 1, 0, len);

            float lowerValue = func[floorIndex];
            float higherValue = func[ceilIndex];

            return math.lerp(lowerValue, higherValue, math.frac(floatIndex));
        }
    }
}