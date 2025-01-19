using UnityEngine;
using Unity.Jobs;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("New Range", type = "Operation", docs ="https://ensapra.com/packages/infinite_lands/nodes/newrange.html")]
    public class NewRangeNode : HeightNodeBase
    {
        //New Range
        public float MinimumValue = 0;
        public float MaximumValue = 1;

        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator Input;
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[]{Input};

        protected override Vector2 GetMinMaxValue()
        {
            return new Vector2(MinimumValue, MaximumValue);
        }

        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            return Input.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid)
;
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            HeightData previousJob = Input.RequestHeight(settings);
            return RemapHeightJob.ScheduleParallel(settings.globalMap, previousJob.indexData,
                target, minMaxValue, Input.minMaxValue,
                settings.pointsLength, previousJob.jobHandle);
        }
    }
}