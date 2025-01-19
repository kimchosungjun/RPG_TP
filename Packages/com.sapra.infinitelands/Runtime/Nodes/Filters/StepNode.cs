using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Step", type = "Filter", docs = "https://ensapra.com/packages/infinite_lands/nodes/step.html")]
    public class StepNode : HeightNodeBase
    {
        public enum StepMode
        {
            ByDistance,
            ByCount
        }

        public StepMode mode;
        public bool byDistance => mode == StepMode.ByDistance;

        [ShowIfCustom(nameof(byDistance))] [Min(0.001f)]
        public float Distance = 20;

        public bool byCount => mode == StepMode.ByCount;

        [ShowIfCustom("byCount")] [Min(1)]
        public int Steps = 1;

        [Range(0.001f, 1)] public float Stepness = 0;
        [Range(0, 1)] public float Flatness = 1;

        [Range(0, 1)] public float LevelVariance;


        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator HeightMap;
        [Input(typeof(IGive<HeightData>), true)] public HeightDataGenerator Mask;
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[]{HeightMap};
        protected override bool ExtraValidationSteps()
        {
            if(Mask != null){
                Mask.ValidationCheck();
                return Mask.isValid;
            }
            return true;
        }

        protected override int PrepareSubNodes(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            int resH = HeightMap.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid)
;
            if(Mask != null)
                resH = Math.Max(Mask.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid), resH);
            return resH;
        }

        protected override Vector2 GetMinMaxValue()
        {
            if(HeightMap != null)
                return HeightMap.minMaxValue;
            else
                return new Vector2(0,1);
        }

        public NativeArray<float> GenerateFloats()
        {
            int stepCount;
            switch (mode)
            {
                case StepMode.ByDistance:
                    stepCount = Mathf.CeilToInt((HeightMap.minMaxValue.y - HeightMap.minMaxValue.x) / Distance);
                    break;
                default:
                    stepCount = Steps;
                    break;
            }

            stepCount += 1;

            var levelHeight = new NativeArray<float>(stepCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            float accomulative = 0;
            levelHeight[0] = 0;


            for (int i = 1; i < stepCount; i++)
            {
                float rand = randValue(i + 123 + stepCount * 12312 + Mathf.RoundToInt(accomulative * 13125));
                accomulative += rand * 2;
                levelHeight[i] = accomulative;
            }

            for (int i = 1; i < stepCount; i++)
            {
                levelHeight[i] = Mathf.Lerp(HeightMap.minMaxValue.x, HeightMap.minMaxValue.y,
                    Mathf.Lerp((float)i / (stepCount - 1), levelHeight[i] / accomulative, LevelVariance));
            }
            return levelHeight;
        }

        float randValue(int z) // iq version
        {
            const uint k = 1103515245U; // GLIB C
            uint x = (uint)z;
            x = ((x >> 8) ^ x) * k;
            x = ((x >> 8) ^ x) * k;
            x = ((x >> 8) ^ x) * k;
            return (float)x / 0xffffffffU;
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            NativeArray<float> levelHeight = settings.manager.GetValue(this.guid, GenerateFloats);
            HeightData previousJob = HeightMap.RequestHeight(settings);
            if (Mask != null)
            {
                HeightData previousMaskJob = Mask.RequestHeight(settings);
                JobHandle afterBoth = JobHandle.CombineDependencies(previousJob.jobHandle, previousMaskJob.jobHandle);
                return StepJobMasked.ScheduleParallel(settings.globalMap, previousJob.indexData, target, previousMaskJob.indexData,
                    levelHeight.Length, Stepness, Flatness, HeightMap.minMaxValue, levelHeight,
                    settings.pointsLength,afterBoth);
            }
            else
                return StepJob.ScheduleParallel(settings.globalMap, previousJob.indexData, target,
                    levelHeight.Length, Stepness, Flatness, HeightMap.minMaxValue, levelHeight,
                    settings.pointsLength, previousJob.jobHandle);

        }
    }
}