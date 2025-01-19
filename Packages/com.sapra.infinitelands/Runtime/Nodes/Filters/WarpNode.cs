using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

using System;
using Unity.Mathematics;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Warp", type = "Filter", docs = "https://ensapra.com/packages/infinite_lands/nodes/warp.html")]
    public class WarpNode : HeightNodeBase, IGeneratePoints
    {
        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator HeightMap;
        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator Warp;
        [Input(typeof(IGive<HeightData>), true)] public HeightDataGenerator Mask;
        protected override InfiniteLandsNode[] Dependancies => new HeightDataGenerator[]{HeightMap, Warp};

        int finalArrayLength = 0;
        int altCount;
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
            altCount = 0;
            int resH = HeightMap.PrepareNode(manager, ref finalArrayLength, resolution, ratio, this.guid+requestGuid);
            int resWX = Warp.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid)
;
            int resWY = Warp.PrepareNode(manager, ref altCount, resolution, ratio, GenerationSettings.GetSeedGuid(requestGuid, 1));

            int max = Math.Max(resH, Math.Max(resWX, resWY));
            if(Mask != null)
                max = Math.Max(Mask.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid)
,max);
            return max;
        }
        protected override Vector2 GetMinMaxValue()
        {
            return HeightMap.minMaxValue;
        }

        public JobHandle GiveMePoints(GenerationSettings settings, out NativeArray<float3> points, NativeArray<float3> parentPoints, JobHandle dependancy)
        {
            GenerationSettings previousSettings = settings.derivedFrom;
            GenerationSettings variant = GenerationSettings.NewSeedSettings(altCount, 1, previousSettings);

            HeightData xData = Warp.RequestHeight(previousSettings);
            HeightData yData = Warp.RequestHeight(variant);

            JobHandle onceFinished = JobHandle.CombineDependencies(xData.jobHandle, yData.jobHandle, dependancy);
            IndexAndResolution warpXIndex = xData.indexData;
            IndexAndResolution warpYIndex = yData.indexData;
            
            points = settings.manager.GetReturnableArray<float3>(settings.terrain.ID, settings.pointsLength);

            JobHandle warpPoints;
            if (Mask != null)
            {
                HeightData maskJob = Mask.RequestHeight(previousSettings);
                JobHandle afterBoth = JobHandle.CombineDependencies(onceFinished, maskJob.jobHandle);
                warpPoints = WarpPointsMaskedJob.ScheduleParallel(parentPoints, points, 
                    previousSettings.globalMap, variant.globalMap, warpXIndex, warpYIndex, maskJob.indexData,
                    settings.pointsLength, settings.meshSettings.Resolution, afterBoth);
            }
            else{

                warpPoints = WarpPointsJob.ScheduleParallel(parentPoints, points, 
                    previousSettings.globalMap, variant.globalMap, warpXIndex, warpYIndex,
                    settings.pointsLength, settings.meshSettings.Resolution, onceFinished);
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