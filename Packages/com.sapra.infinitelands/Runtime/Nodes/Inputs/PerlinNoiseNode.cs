using Unity.Jobs;
using UnityEngine;

using System;
using Unity.Mathematics;
using Unity.Collections;
using static sapra.InfiniteLands.Noise;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Perlin Noise", type = "Input", docs = "https://ensapra.com/packages/infinite_lands/nodes/perlin_noise.html")]
    public class PerlinNoiseNode : HeightNodeBase
    {
        public enum PerlinType{PerlinValue, Perlin}
        public PerlinType NoiseType = PerlinType.Perlin;
        public Vector2 MinMaxHeight = new Vector2(0, 1);
        [Min(0.001f)] public float TileSize = 1;

        [Min(1)] public int Octaves = 1;
        public Vector3 Rotation;

        [ShowIfCustom(nameof(octavesEnabled))][Range(1,10)]public int Lacunarity = 2;
        [ShowIfCustom(nameof(octavesEnabled))][Range(0f, 1f)] public float Persistence = .5f;

        public bool RidgeMode;

        private bool octavesEnabled => Octaves > 1;
        protected override Vector2 GetMinMaxValue(){
            return MinMaxHeight;
        }
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[0];

        protected override HeightGenerationResult UpdateIndices(HeightGenerationResult indicesReady, GenerationSettings settings)
        {
            indicesReady.OutputResolution = settings.meshSettings.Resolution;
            return indicesReady;
        }


        NoiseSettings getsettings()
        {
            return new NoiseSettings()
            {
                scale = TileSize,
                octaves = Octaves,
                minmaxValue = MinMaxHeight,
                lacunarity = Lacunarity,
                persistence = Persistence,
                rotation = Rotation,
                ridgeMode = RidgeMode
            };
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            NoiseSettings noiseSettings = getsettings();           
            int indexOffset = GetRandomIndex();
            float maxOctaves = Mathf.Pow(int.MaxValue, 1f/Lacunarity);
            noiseSettings.octaves = Mathf.Max(1,Mathf.Min(noiseSettings.octaves, Mathf.FloorToInt(maxOctaves)));
            NativeArray<float3x4> vectorized = settings.points.Reinterpret<float3x4>(sizeof(float)*3);
            NativeArray<float4> heightMap = settings.globalMap.Reinterpret<float4>(sizeof(float));

            switch (NoiseType)
            {
                case PerlinType.PerlinValue:
                    return NoiseJob<Perlin2D<Value>>.ScheduleParallel(vectorized,                        
                        heightMap,
                        noiseSettings, settings.terrain.Position, target, settings.pointsLength,
                        settings.meshSettings.Seed + indexOffset,
                        settings.dependancy);
                case PerlinType.Perlin:
                    return NoiseJob<Perlin2D<Perlin>>.ScheduleParallel(vectorized,                        
                        heightMap, noiseSettings, settings.terrain.Position,
                        target, settings.pointsLength,
                        settings.meshSettings.Seed + indexOffset,
                        settings.dependancy);
                default:
                    return NoiseJob<Perlin2D<Perlin>>.ScheduleParallel(vectorized, 
                        heightMap,
                        noiseSettings, settings.terrain.Position, target, settings.pointsLength,
                        settings.meshSettings.Seed + indexOffset,
                        settings.dependancy);
            }
        }
    }
}