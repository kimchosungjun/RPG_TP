using UnityEngine;

using Unity.Mathematics;
using Unity.Jobs;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Constant", type = "Input", docs = "https://ensapra.com/packages/infinite_lands/nodes/constant.html")]
    public class ConstantNode : HeightNodeBase
    {
        public float Value;
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[0];

        protected override Vector2 GetMinMaxValue()
        {
            return new Vector2(Value - 0.001f, Value);
        }
        protected override HeightGenerationResult UpdateIndices(HeightGenerationResult indicesReady, GenerationSettings settings)
        {
            indicesReady.OutputResolution = settings.meshSettings.Resolution;
            return indicesReady;
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            return ConstantJob.ScheduleParallel(settings.globalMap.Reinterpret<float4>(sizeof(float)), Value,
                    target, settings.pointsLength, settings.dependancy);
        }
    }
}