using Unity.Jobs;
using UnityEngine;
using System;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Apply Mask", type = "Operation", docs = "https://ensapra.com/packages/infinite_lands/nodes/applymask.html")]
    public class AppplyMask : HeightNodeBase
    {
        public enum ToValue{Minimum, Maximum, Zero}
        
        protected override Vector2 GetMinMaxValue() {
            if(ValueAtZero.Equals(ToValue.Zero))
                return new Vector2(Mathf.Min(0, Input.minMaxValue.x),
                    Mathf.Min(0, Input.minMaxValue.y));
            else
            return Input.minMaxValue;
        }

        public ToValue ValueAtZero = ToValue.Minimum;
        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator Input;
        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator Mask;
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[]{Input,Mask};

        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            int resI = Input.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid);
            int resM = Mask.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid);
            return Math.Max(resI, resM);
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            HeightData previousJobMask = Mask.RequestHeight(settings);
            HeightData previousJobHeight = Input.RequestHeight(settings);
            
            switch (ValueAtZero){
                case ToValue.Maximum:
                    return MaskMultiplyJob<Maximum>.ScheduleParallel(settings.globalMap,
                        previousJobMask.indexData, previousJobHeight.indexData, target, Input.minMaxValue,
                        settings.pointsLength,
                        JobHandle.CombineDependencies(previousJobHeight.jobHandle, previousJobMask.jobHandle));
                case ToValue.Zero:
                    return MaskMultiplyJob<Zero>.ScheduleParallel(settings.globalMap,
                        previousJobMask.indexData, previousJobHeight.indexData, target, Input.minMaxValue,
                        settings.pointsLength,
                        JobHandle.CombineDependencies(previousJobHeight.jobHandle, previousJobMask.jobHandle));
                default:
                    return MaskMultiplyJob<Minimum>.ScheduleParallel(settings.globalMap,
                        previousJobMask.indexData, previousJobHeight.indexData, target, Input.minMaxValue,
                        settings.pointsLength,
                        JobHandle.CombineDependencies(previousJobHeight.jobHandle, previousJobMask.jobHandle));
            }
        }

        private struct Minimum : MaskMultiplyMode
        {
            public float GetValue(float2 minMax, float value, float mask)
            {
                return lerp(minMax.x, value, saturate(mask));
            }
        }

        private struct Maximum : MaskMultiplyMode
        {
            public float GetValue(float2 minMax, float value, float mask)
            {
                return lerp(minMax.y, value, saturate(mask));
            }
        }

        private struct Zero : MaskMultiplyMode
        {
            public float GetValue(float2 minMax, float value, float mask)
            {
                return value*saturate(mask);
            }
        }
    }
}