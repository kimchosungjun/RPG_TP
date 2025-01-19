using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

using System;
using Unity.Mathematics;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Directional Warp", type = "Filter", docs = "https://ensapra.com/packages/infinite_lands/nodes/warp.html")]
    public class DirectionalWarpNode : HeightNodeBase, IGeneratePoints
    {
        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator HeightMap;
        [Input(typeof(IGive<HeightData>), true)] public HeightDataGenerator Mask;
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[]{HeightMap};

        int finalArrayLength = 0;
        public float Strength;
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
            finalArrayLength = 0;
            int resNormalMap = HeightMap.PrepareNode(manager, ref finalArrayLength, MapTools.IncreaseResolution(resolution,1), ratio, requestGuid);
            int resH = HeightMap.PrepareNode(manager, ref finalArrayLength, resolution, ratio, this.guid+requestGuid);
            int max = Math.Max(resH, resNormalMap);
            if(Mask != null)
                max = Math.Max(Mask.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid) ,max);
            return max;
        }
        protected override Vector2 GetMinMaxValue()
        {
            return HeightMap.minMaxValue;
        }

        public JobHandle GiveMePoints(GenerationSettings settings, out NativeArray<float3> points, NativeArray<float3> parentPoints, JobHandle dependancy)
        {
            GenerationSettings previousSettings = settings.derivedFrom;

            HeightData normalMapData = HeightMap.RequestNormal(previousSettings, out NativeArray<float3> normalMap, out _);
            IndexAndResolution normalIndex = normalMapData.indexData;
            float targetStrength = Strength/settings.ratio;
            points = settings.manager.GetReturnableArray<float3>(settings.terrain.ID, settings.pointsLength);

            JobHandle warpPoints;
            if (Mask != null)
            {
                HeightData maskJob = Mask.RequestHeight(previousSettings);
                JobHandle afterBoth = JobHandle.CombineDependencies(normalMapData.jobHandle, maskJob.jobHandle);
                warpPoints = WarpPointsNormalMaskedJob.ScheduleParallel(parentPoints, points, 
                    previousSettings.globalMap, normalMap, normalIndex, targetStrength, maskJob.indexData,
                    settings.pointsLength, settings.meshSettings.Resolution, afterBoth);
            }
            else{

                warpPoints = WarpPointsNormalMapJob.ScheduleParallel(parentPoints, points, 
                    normalMap, normalIndex, targetStrength,
                    settings.pointsLength, settings.meshSettings.Resolution, normalMapData.jobHandle);
            }
            return warpPoints;
        }

        public override JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings)
        {
            GenerationSettings newSettings = GenerationSettings.NewDerivedSettings(finalArrayLength, this, this.guid+settings.BranchGUID, settings);
            
            HeightData current = HeightMap.RequestHeight(newSettings);

            return WarpTrespasJob.ScheduleParallel(settings.globalMap, newSettings.globalMap, 
                target, current.indexData, 
                settings.pointsLength, newSettings.pointsLength, current.jobHandle);
        }
    }
}