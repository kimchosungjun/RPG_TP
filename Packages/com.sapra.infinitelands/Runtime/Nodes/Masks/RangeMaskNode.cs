using Unity.Jobs;
using UnityEngine;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Range Mask", type = "Mask", docs ="https://ensapra.com/packages/infinite_lands/nodes/rangemask.html")]
    public class RangeMaskNode : HeightNodeBase
    {
        protected override Vector2 GetMinMaxValue()
        {
            return new Vector2(0, 1);
        }

        //HeightPass
        public Vector2 MinMaxHeight = new Vector2(-1000, 1000);
        public float BlendFactor = 20;

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
            return RangeSelectorJob.ScheduleParallel(settings.globalMap, previousJob.indexData, target, 
                MinMaxHeight, BlendFactor,
                settings.pointsLength, previousJob.jobHandle);
        }
    }
}