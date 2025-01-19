using Unity.Jobs;
using UnityEngine;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Gate", type = "Mask", docs ="https://ensapra.com/packages/infinite_lands/nodes/gate.html")]
    public class GateMaskNode : HeightNodeBase
    {
        protected override Vector2 GetMinMaxValue()
        {
            return new Vector2(0, 1);
        }
        //Curve Mode
        [BoundedCurveAttribute] public AnimationCurve FilterCurve =
            new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 0, 0) });

        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator Input;
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[]{Input};
        int variation;
        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            return Input.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid);
        }
        protected override void OnValidate() {
            base.OnValidate();
            variation++;
        }
        public SampledAnimationCurve CreateCurve() => new SampledAnimationCurve(FilterCurve);

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            SampledAnimationCurve curve = settings.manager.GetValue(this.guid+variation, CreateCurve);
            HeightData previousJob = Input.RequestHeight(settings);
            return CurveMaskJob.ScheduleParallel(settings.globalMap, target, previousJob.indexData,
                curve, Input.minMaxValue, settings.pointsLength,
                previousJob.jobHandle);
        }
    }
}