using System;
using System.Collections.Generic;
using sapra.InfiniteLands.NaughtyAttributes;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace sapra.InfiniteLands
{
    [ExecuteInEditMode]
    public class PointStore : ChunkProcessor<ChunkData>, IGenerate<CoordinateDataHolder>
    {
        [Header("Base configuration")] 
        private Dictionary<Vector3Int, ChunkData> _chunksGenerated = new();

        private Vector2 LocalGridOffset;
        private int MaxLODValue;

        public Action<CoordinateDataHolder> onProcessDone { get; set; }
        public Action<CoordinateDataHolder> onProcessRemoved { get; set; }
        public Action<IGraph, MeshSettings> onReload { get; set ; }

        public IGraph graph{get; private set;}
        public MeshSettings settings{get; private set;}

        private IControlMatrices matrixControl;
        private ISetupChunkHirearchy chunkHirearchy;

        public override void Dispose()
        {
            foreach(KeyValuePair<Vector3Int, ChunkData> values in _chunksGenerated){
                values.Value.coordinateData?.RemoveProcessor(this);
            }
        }

        public override void Initialize(IGraph generator, MeshSettings settings)
        {
            graph = generator;

            var visualizeTerrain = GetComponent<IVisualizeTerrain>();
            InitializePointStore(settings, GetComponent<IControlMatrices>(), GetComponentInChildren<ChunkControl>());
            ConfigureOffset(visualizeTerrain.localGridOffset, settings.MeshScale);
            onReload?.Invoke(generator, settings);
        }
        public void InitializePointStore(MeshSettings settings, IControlMatrices maxtriControl, ISetupChunkHirearchy chunkHirearchy){
            _chunksGenerated = new();
            this.chunkHirearchy = chunkHirearchy;
            this.settings = settings;
            this.matrixControl = maxtriControl;
        }
        public void ConfigureOffset(Vector2 centerOfOrigin, float originSize){
            LocalGridOffset = MapTools.GetOffsetInGrid(centerOfOrigin, originSize)+originSize/2.0f;
        }
        protected override void OnProcessRemoved(ChunkData chunk)=> RemoveChunk(chunk);
        protected override void OnProcessAdded(ChunkData chunk) => AddChunk(chunk);
        public void RemoveChunk(ChunkData chunk)
        {
            Vector3Int finalCord = chunk.terrainConfig.ID;
            chunk.coordinateData?.RemoveProcessor(this);   
            _chunksGenerated.Remove(finalCord);
            if(chunk.coordinateData != null)
                onProcessRemoved?.Invoke(chunk.coordinateData);
        }

        public void AddChunk(ChunkData chunk){
            Vector3Int finalCord = chunk.terrainConfig.ID;
            MaxLODValue = Mathf.Max(finalCord.z, MaxLODValue);

            chunk.coordinateData?.AddProcessor(this);        

            if (!_chunksGenerated.TryAdd(finalCord, chunk)){
                Debug.LogWarning("Can't add new chunk " + finalCord.ToString());
            }
            else if(chunk.coordinateData != null)
                onProcessDone?.Invoke(chunk.coordinateData);
        }

        #region APIs
        public CoordinateData GetCoordinateDataAtGridPosition(Vector2 position, out bool foundData, bool interpolated)
        {
            ChunkData result = GetChunkDataAtGridPosition(position);
            if(result != null){
                foundData = true;
                if(interpolated)
                    return CoordinateDataResultInterpolated(result.coordinateData.points, result.meshSettings.MeshScale, position, result.terrainConfig.Position);
                else
                    return CoordinateDataResult(result.coordinateData.points, result.meshSettings.MeshScale, position, result.terrainConfig.Position);
            }
            foundData = false;
            return CoordinateData.Default;
        }

        public Vector3 ToLocalSpacePoint(Vector3 position) => matrixControl.worldToLocalMatrix.MultiplyPoint(position);
        public Vector3 ToWorldSpacePoint(Vector3 position) => matrixControl.localToWorldMatrix.MultiplyPoint(position);

        public Vector3 ToLocalSpaceVector(Vector3 vector) => matrixControl.worldToLocalMatrix.MultiplyVector(vector);
        public Vector3 ToWorldSpaceVector(Vector3 vector) => matrixControl.localToWorldMatrix.MultiplyVector(vector);

        public CoordinateData GetCoordinateDataAtPosition(Vector3 position, out bool foundData, bool interpolated = false, bool inWorldSpace = true)
        {
            Vector3 flattened = matrixControl.worldToLocalMatrix.MultiplyPoint(position);
            CoordinateData result = GetCoordinateDataAtGridPosition(new Vector2(flattened.x, flattened.z), out foundData, interpolated);

            if(inWorldSpace)
                return result.ApplyMatrix(matrixControl.localToWorldMatrix);
            return result;
        }
        private CoordinateData CoordinateDataResult(NativeArray<CoordinateData> points, float correctMeshScale, Vector2 simplePos, Vector3 position){
            Vector2 leftCorner = new Vector2(position.x, position.z)-new Vector2(correctMeshScale,correctMeshScale)/2f;
            Vector2 flatUV = (simplePos-leftCorner) / correctMeshScale;
            Vector2Int index = Vector2Int.RoundToInt(flatUV * settings.Resolution);
            return SampleData(points, index);
        }

        private CoordinateData CoordinateDataResultInterpolated(NativeArray<CoordinateData> points, float correctMeshScale, Vector2 simplePos, Vector3 position){
            Vector2 leftCorner = new Vector2(position.x, position.z)-new Vector2(correctMeshScale,correctMeshScale)/2f;
            Vector2 flatUV = (simplePos-leftCorner)*settings.Resolution / correctMeshScale;
            
            Vector2Int indexA = Vector2Int.FloorToInt(flatUV);
            Vector2Int indexB = indexA+new Vector2Int(0, 1);
            Vector2Int indexC = indexA+new Vector2Int(1, 1);
            Vector2Int indexD = indexA+new Vector2Int(1, 0);

            Vector2 t = flatUV-indexA;

            var dataA = SampleData(points, indexA);
            var dataB = SampleData(points, indexB);
            var dataC = SampleData(points, indexC);
            var dataD = SampleData(points, indexD);

            CoordinateData XBottom = CoordinateData.Lerp(dataA, dataD, t.x);
            CoordinateData XTop = CoordinateData.Lerp(dataB, dataC, t.x);

            return CoordinateData.Lerp(XBottom, XTop, t.y);
        }

        private CoordinateData SampleData(NativeArray<CoordinateData> points, Vector2Int index){
            index.x = Mathf.Clamp(index.x, 0, settings.Resolution);
            index.y = Mathf.Clamp(index.y, 0, settings.Resolution);

            if(index.x + index.y * (settings.Resolution + 1) <  points.Length)
                return points[index.x + index.y * (settings.Resolution + 1)];
            else 
                return CoordinateData.Default;
        }

        public ChunkData GetChunkDataAtPosition(Vector3 position){
            Vector3 flattened = matrixControl.worldToLocalMatrix.MultiplyPoint(position);
            return GetChunkDataAtGridPosition(new Vector2(flattened.x, flattened.z));
        }

        public ChunkData GetChunkDataAtGridPosition(Vector2 localFlatPosition){
            for(int i = MaxLODValue; i >= 0; i--){
                Vector3Int id = chunkHirearchy.TransformPositionToID(localFlatPosition, i, LocalGridOffset, settings.MeshScale);
                if(_chunksGenerated.TryGetValue(id, out ChunkData result))
                    return result;
            }
            return null;
        }
        
        public bool IsPointInChunk(Vector3 position, TerrainConfig terrainData){
            Vector3 flattened = matrixControl.worldToLocalMatrix.MultiplyPoint(position);
            return IsPointInChunkAtGrid(new Vector2(flattened.x, flattened.z), terrainData);
        }
        public bool IsPointInChunkAtGrid(Vector2 flatPosition, TerrainConfig terrainData){
            Vector3Int pointInGrid = chunkHirearchy.TransformPositionToID(flatPosition, terrainData.ID.z, LocalGridOffset, settings.MeshScale);
            return terrainData.ID.Equals(pointInGrid);
        }
        #endregion
    }
}