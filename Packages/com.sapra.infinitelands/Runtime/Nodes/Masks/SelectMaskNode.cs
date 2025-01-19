using Unity.Jobs;
using UnityEngine;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Select Mask", type = "Mask", docs ="https://ensapra.com/packages/infinite_lands/nodes/normalmask.html")]
    public class SelectMaskNode : HeightNodeBase
    {
        protected override Vector2 GetMinMaxValue()
        {
            return new Vector2(0, 1);
        }

        [MinMaxCustom(0,1)] public Vector2 Range = new Vector2(0, 1);
        [Range(0, 1)] public float BlendFactor = .1f;

        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator Input;
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[]{Input};
        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            return Input.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid)
;
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            HeightData previousJob = Input.RequestHeight(settings);
            if(Input.minMaxValue == minMaxValue){
                return RangeSelectorJob.ScheduleParallel(settings.globalMap, previousJob.indexData, target,
                    Range, BlendFactor, settings.pointsLength,
                    previousJob.jobHandle);
            }
            else{
                return NormalizeRangeSelectorJob.ScheduleParallel(settings.globalMap, previousJob.indexData, target,
                    Range, BlendFactor, Input.minMaxValue, settings.pointsLength, 
                    previousJob.jobHandle);
            }
        }
    }
}