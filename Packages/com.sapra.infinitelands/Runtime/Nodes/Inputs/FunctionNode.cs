using UnityEngine;


using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Function", type = "Input", docs = "https://ensapra.com/packages/infinite_lands/nodes/function.html")]
    public class FunctionNode : HeightNodeBase
    {
        public enum FunctionType
        {
            Sine,
            Square,
            Triangle,
            SawTooth
        }

        public FunctionType functionType;
        public Vector2 FromTo = new Vector2(0, 1);
        public float YRotation = 0;
        [Min(0.01f)] public float Period = 1;

        protected override Vector2 GetMinMaxValue()
        {
            return new Vector2(FromTo.x, FromTo.y);
        }
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[0];

        protected override HeightGenerationResult UpdateIndices(HeightGenerationResult indicesReady, GenerationSettings settings)
        {
            indicesReady.OutputResolution = settings.meshSettings.Resolution;
            return indicesReady;
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            NativeArray<float3x4> vectorized = settings.points.Reinterpret<float3x4>(sizeof(float)*3);
            NativeArray<float4> heightMap = settings.globalMap.Reinterpret<float4>(sizeof(float));
            switch (functionType)
            {
                case FunctionType.Square:
                    return FunctionJob<FSquare>.ScheduleParallel(vectorized,
                        heightMap, new Vector2(FromTo.x + .01f, FromTo.y - 0.01f),
                        settings.terrain.Position,
                        Period, YRotation, target, settings.pointsLength,
                        settings.dependancy);
                case FunctionType.Triangle:
                    return FunctionJob<FTriangle>.ScheduleParallel(vectorized,
                        heightMap, FromTo, settings.terrain.Position,
                        Period, YRotation, target, settings.pointsLength,
                        settings.dependancy);
                case FunctionType.SawTooth:
                    return FunctionJob<FSawTooth>.ScheduleParallel(vectorized,
                        heightMap, FromTo, settings.terrain.Position,
                        Period, YRotation, target, settings.pointsLength,
                        settings.dependancy);
                default:
                    return FunctionJob<FSine>.ScheduleParallel(vectorized,
                        heightMap, FromTo, settings.terrain.Position,
                        Period, YRotation, target, settings.pointsLength,
                        settings.dependancy);
        
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (FromTo.x >= FromTo.y)
            {
                FromTo.x = FromTo.y - 0.1f;
            }
        }

    }
}