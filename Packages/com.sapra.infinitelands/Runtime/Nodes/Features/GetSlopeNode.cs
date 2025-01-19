using Unity.Jobs;
using UnityEngine;

using Unity.Collections;
using Unity.Mathematics;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Get Slope", type = "Features", docs ="https://ensapra.com/packages/infinite_lands/nodes/getslope.html")]
    public class GetSlopeNode : HeightNodeBase
    {
        protected override Vector2 GetMinMaxValue()
        {
            return new Vector2(0, 1);
        }

        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator Input;
        public GenerationModeNode FeatureMode = GenerationModeNode.Default;
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[]{Input};
        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            return Input.PrepareNode(manager,ref currentCount, MapTools.IncreaseResolution(resolution,1), ratio, requestGuid, true);
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {           
            Matrix4x4 targetMatrix = settings.GetVectorMatrix(FeatureMode);
            HeightData normals = Input.RequestNormal(settings, out NativeArray<float3> normalMap, out _);
            return GetSlope.ScheduleParallel(normalMap, normals.indexData,
                settings.globalMap, targetMatrix,
                target,
                settings.pointsLength,
                normals.jobHandle);
        }
    }
}