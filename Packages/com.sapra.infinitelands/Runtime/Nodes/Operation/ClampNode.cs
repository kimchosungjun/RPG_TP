using UnityEngine;
using Unity.Jobs;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Clamp", type = "Operation", docs ="https://ensapra.com/packages/infinite_lands/nodes/clamp.html")]
    public class ClampNode : HeightNodeBase
    {
        public Vector2 ClampMinMax = new Vector2(0, 1);

        protected override Vector2 GetMinMaxValue()
        {
            return new Vector2(Mathf.Max(Input.minMaxValue.x, ClampMinMax.x),
                Mathf.Min(Input.minMaxValue.y, ClampMinMax.y));
        }

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
            return ClampJob.ScheduleParallel(settings.globalMap, previousJob.indexData, target,
                ClampMinMax,
                settings.pointsLength, settings.meshSettings.Resolution, previousJob.jobHandle);
        }
    }
}