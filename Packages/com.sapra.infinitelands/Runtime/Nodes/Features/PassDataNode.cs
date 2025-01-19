using Unity.Jobs;
using UnityEngine;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Only pass Data", type = "Features")]
    public class PassData : HeightNodeBase
    {
        protected override Vector2 GetMinMaxValue()
        {
            return new Vector2(0, 1);
        }

        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator Input;
        protected override InfiniteLandsNode[] Dependancies => new InfiniteLandsNode[]{Input};

        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            return Input.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid);
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            HeightData previousJob = Input.RequestHeight(settings);

            JobHandle calculateCavities = PassDataJob.ScheduleParallel(
                settings.globalMap,
                target, previousJob.indexData,
                settings.pointsLength, previousJob.jobHandle);

            return calculateCavities;
        }
    }
}