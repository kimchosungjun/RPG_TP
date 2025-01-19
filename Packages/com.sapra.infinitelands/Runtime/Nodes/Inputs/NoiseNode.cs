using Unity.Jobs;
using UnityEngine;

using System;
using Unity.Mathematics;
using Unity.Collections;
using static sapra.InfiniteLands.Noise;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Noise [DEPRECATED]", type = "DEPRECATED", docs = "https://ensapra.com/packages/infinite_lands/nodes/noise.html")]
    public class NoiseNode : HeightNodeBase
    {
    
        public NoiseSettings.NoiseBurstType NoiseType = NoiseSettings.NoiseBurstType.Simplex;
        public Vector2 MinMaxHeight = new Vector2(0, 1);
        [Min(0.001f)] public float TileSize = 1;

        [Min(1)] public int Octaves = 1;
        public Vector3 Rotation;

        [ShowIfCustom(nameof(octavesEnabled))][Range(1,10)]public int Lacunarity = 2;
        [ShowIfCustom(nameof(octavesEnabled))][Range(0f, 1f)] public float Persistence = .5f;

        [Range(0.001f, 1)] [ShowIfCustom(nameof(isVoronoi))]
        public float SmoothVoronoi = .001f;
        public bool RidgeMode;

        private bool isVoronoi => NoiseType == NoiseSettings.NoiseBurstType.VoronoiF1
                                  || NoiseType == NoiseSettings.NoiseBurstType.VoronoiF2
                                  || NoiseType == NoiseSettings.NoiseBurstType.VoronoiF1F2
                                  || NoiseType == NoiseSettings.NoiseBurstType.VoronoiIndex;

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
                SmoothVoronoi = SmoothVoronoi,
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
                case NoiseSettings.NoiseBurstType.PerlinValue:
                    return NoiseJob<Perlin2D<Value>>.ScheduleParallel(vectorized,                        
                        heightMap,
                        noiseSettings, settings.terrain.Position, target, settings.pointsLength,
                        settings.meshSettings.Seed + indexOffset,
                        settings.dependancy);
                case NoiseSettings.NoiseBurstType.Perlin:
                    return NoiseJob<Perlin2D<Perlin>>.ScheduleParallel(vectorized,                        
                        heightMap, noiseSettings, settings.terrain.Position,
                        target, settings.pointsLength,
                        settings.meshSettings.Seed + indexOffset,
                        settings.dependancy);
                case NoiseSettings.NoiseBurstType.SimplexValue:
                    return NoiseJob<Simplex2D<Value>>.ScheduleParallel(vectorized, 
                        heightMap, noiseSettings, settings.terrain.Position,
                        target, settings.pointsLength,
                        settings.meshSettings.Seed + indexOffset,
                        settings.dependancy);
                case NoiseSettings.NoiseBurstType.Simplex:
                    return NoiseJob<Simplex2D<Simplex>>.ScheduleParallel(vectorized,
                        heightMap, noiseSettings, settings.terrain.Position,
                        target, settings.pointsLength,
                        settings.meshSettings.Seed + indexOffset,
                        settings.dependancy);
                case NoiseSettings.NoiseBurstType.VoronoiF1:
                    return NoiseJob<Voronoi2D<F1>>.ScheduleParallel(vectorized, 
                    heightMap,
                        noiseSettings, settings.terrain.Position, target, settings.pointsLength,
                        settings.meshSettings.Seed + indexOffset,
                        settings.dependancy);
                case NoiseSettings.NoiseBurstType.VoronoiF2:
                    return NoiseJob<Voronoi2D<F2>>.ScheduleParallel(vectorized, 
                        heightMap,
                        noiseSettings, settings.terrain.Position, target, settings.pointsLength,
                        settings.meshSettings.Seed + indexOffset,
                        settings.dependancy);
                case NoiseSettings.NoiseBurstType.VoronoiF1F2:
                    return NoiseJob<F2MinusF1>.ScheduleParallel(vectorized, 
                        heightMap,
                        noiseSettings, settings.terrain.Position, target, settings.pointsLength,
                        settings.meshSettings.Seed + indexOffset,
                        settings.dependancy);
                case NoiseSettings.NoiseBurstType.VoronoiIndex:
                    return NoiseJob<VoronoiIndex>.ScheduleParallel(vectorized, 
                        heightMap,
                        noiseSettings, settings.terrain.Position, target, settings.pointsLength,
                        settings.meshSettings.Seed + indexOffset,
                        settings.dependancy);
                default:
                    return NoiseJob<Perlin2D<Value>>.ScheduleParallel(vectorized, 
                        heightMap,
                        noiseSettings, settings.terrain.Position, target, settings.pointsLength,
                        settings.meshSettings.Seed + indexOffset,
                        settings.dependancy);
            }
        }
    }
}