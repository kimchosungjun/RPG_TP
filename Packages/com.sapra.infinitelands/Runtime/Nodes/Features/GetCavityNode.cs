using Unity.Jobs;
using UnityEngine;

using Unity.Collections;
using Unity.Mathematics;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Get Cavity", type = "Features", docs ="https://ensapra.com/packages/infinite_lands/nodes/getcavity.html")]
    public class GetCavityNode : HeightNodeBase
    {
        [Min(0.01f)] public float CavitySize = 10;

        protected override Vector2 GetMinMaxValue()
        {
            return new Vector2(0, 1);
        }

        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator Input;
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[]{Input};
        public GenerationModeNode FeatureMode = GenerationModeNode.Default;
        protected override int IncreaseNodeIndices(int currentIndex)
        {
            return currentIndex+2;
        }

        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            float Size = ratio*CavitySize;
            resolution = MapTools.IncreaseResolution(resolution, Mathf.CeilToInt(Size)+1);
            return Input.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid, true);
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            float Size = Mathf.Min(settings.ratio*CavitySize, MapTools.MaxIncreaseSize);
            int EffectSize = Mathf.Max(1, Mathf.FloorToInt(Size));
            float ExtraSize = Mathf.Clamp01(Size-EffectSize);

            HeightData normals = Input.RequestNormal(settings, out NativeArray<float3> normalMap, out HeightData previousJob);
            IndexAndResolution original = previousJob.indexData;
            
            IndexAndResolution channelTarget = new IndexAndResolution(target.Index+1, original.Resolution-2);
                        Matrix4x4 targetMatrix = settings.GetVectorMatrix(FeatureMode);

            JobHandle separateMaps = CalculateChannels.ScheduleParallel(normalMap, normals.indexData,
                                settings.globalMap,targetMatrix, channelTarget,
                                settings.pointsLength,
                                normals.jobHandle);
            
            JobHandle calculateCavities = GetCavityJob.ScheduleParallel(settings.globalMap, target, channelTarget, 
                EffectSize, ExtraSize, settings.pointsLength, separateMaps);

            return calculateCavities;
        }
    }
}