using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Height Output", type = "Output", canCreate = false)]
    public class HeightOutputNode : InfiniteLandsNode, IGeneratePoints, IOutput
    {
        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator HeightMap;
        public HeightDataGenerator map => HeightMap;
        public Vector2 MinMax => HeightMap.minMaxValue;

        protected override InfiniteLandsNode[] Dependancies => new InfiniteLandsNode[]{HeightMap};

        public int PrepareNode(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            return HeightMap.PrepareNode(manager, ref currentCount, MapTools.IncreaseResolution(resolution,1), ratio, requestGuid, true); //We increase one because of the normal map
        }
        
        public WorldFinalData PerformTerrain(GenerationSettings settings, MeshSettings meshSettings)
        {
            settings.PointMaker = this;
            string requestor = string.Format("{0}-worldFinalData", settings.terrain.ID);

            HeightData normalData = HeightMap.RequestNormal(settings, out NativeArray<float3> normalMap, out HeightData outputTerrain);
            var data = new[] { (int)map.minMaxValue.y, (int)map.minMaxValue.x };
            NativeArray<int> MinMaxHeight = settings.manager.GetReturnableArray(requestor, data);
            
            int length = MapTools.LengthFromResolution(meshSettings.Resolution);
            NativeArray<Vertex> finalPositions = settings.manager.GetReturnableArray<Vertex>(requestor, length);
            JobHandle applyHeight = ApplyHeightJob.ScheduleParallel(finalPositions, settings.globalMap, 
                MinMaxHeight, outputTerrain.indexData, meshSettings.Resolution, meshSettings.MeshScale,
                normalMap, normalData.indexData, normalData.jobHandle);


            return new WorldFinalData(settings.manager, requestor, MinMaxHeight, finalPositions, MinMax, applyHeight);
        }

        public JobHandle GiveMePoints(GenerationSettings settings, out NativeArray<float3> points, NativeArray<float3> parent, JobHandle dependancy)
        {
            points = settings.manager.GetReturnableArray<float3>(settings.terrain.ID, settings.pointsLength);
            JobHandle increaseMap = SimpleGridMap.ScheduleParallel(points,
                    settings.pointsLength, settings.meshSettings.Resolution, settings.meshSettings.MeshScale, default);
            return increaseMap;
        }

    }
}