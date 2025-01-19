using Unity.Jobs;
using UnityEngine;
using System;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Interpolate", type = "Operation", docs = "https://ensapra.com/packages/infinite_lands/nodes/interpolate.html")]
    public class InterpolateNode : HeightNodeBase
    {
        protected override Vector2 GetMinMaxValue()
        {
            return new Vector2(Mathf.Min(InputAt0.minMaxValue.x, InputAt1.minMaxValue.x),
                Mathf.Max(InputAt0.minMaxValue.y, InputAt1.minMaxValue.y));
        }

        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator InputAt0;
        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator InputAt1;

        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator Mask;
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[]{InputAt0,InputAt1,Mask};

        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            int res0 = InputAt0.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid)
;
            int res1 = InputAt1.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid)
;
            int max = Math.Max(res0, res1);
            if(Mask != null)
                max = Math.Max(max, Mask.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid)
);
            return max;
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            HeightData previousJobMask = Mask.RequestHeight(settings);
            HeightData previousJobHeight0 = InputAt0.RequestHeight(settings);
            HeightData previousJobHeight1 = InputAt1.RequestHeight(settings);

            return InterpolateJob.ScheduleParallel(settings.globalMap,
                previousJobMask.indexData, previousJobHeight0.indexData, previousJobHeight1.indexData, target, Mask.minMaxValue,
                settings.pointsLength, 
                JobHandle.CombineDependencies(previousJobHeight1.jobHandle, previousJobMask.jobHandle,
                    previousJobHeight0.jobHandle));
        }
    }
}