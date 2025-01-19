using UnityEngine;
using Unity.Jobs;

using UnityEngine.Serialization;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Curve", type = "Operation", docs ="https://ensapra.com/packages/infinite_lands/nodes/curve.html")]
    public class CurveNode : HeightNodeBase
    {
        [BoundedCurveAttribute]
        [FormerlySerializedAs("FilterCurve")] public AnimationCurve Function =
            new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 0, 0) });

        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator Input;
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[]{Input};

        int variation = 0;
        protected override Vector2 GetMinMaxValue()
        {
            return Input.minMaxValue;
        }

        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            return Input.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid);
        }
        protected override void OnValidate() {
            base.OnValidate();
            variation++;
        }
        public SampledAnimationCurve CreateCurve() => new SampledAnimationCurve(Function);


        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {        
            SampledAnimationCurve curve = settings.manager.GetValue(this.guid+variation, CreateCurve);
            HeightData previousJob = Input.RequestHeight(settings);
            return RemapCurveJob.ScheduleParallel(settings.globalMap, previousJob.indexData,
                target, curve, Input.minMaxValue,
                settings.pointsLength,previousJob.jobHandle);
        }
    }
}