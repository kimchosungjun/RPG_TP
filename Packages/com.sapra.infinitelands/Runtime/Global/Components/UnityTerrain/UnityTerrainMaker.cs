using UnityEngine;
using System.Linq;
using Unity.Collections;
using System;
using UnityEngine.Pool;
using System.Collections.Generic;

namespace sapra.InfiniteLands.UnityTerrain{
    [ExecuteAlways]
    public class UnityTerrainMaker : ChunkProcessor<ChunkData>, IGenerate<TerrainWithHeightResult>
    {
        public Action<TerrainWithHeightResult> onProcessDone { get; set; }
        public Action<TerrainWithHeightResult> onProcessRemoved { get; set; }
        public Action<IGraph, MeshSettings> onReload { get; set; }

        private Dictionary<Vector3Int, TerrainWithHeightResult> CompletedRequests = new();

        public IGraph graph{get; private set;}

        public MeshSettings settings{get; private set;}

        public ObjectPool<TerrainData> terrainDataPool;

        public int TerrainResolution = 257;
        public override void Dispose()
        {
            terrainDataPool?.Dispose();
        }

        public override void Initialize(IGraph generator, MeshSettings settings)
        {
            graph = generator;
            this.settings = settings;
            if(terrainDataPool == null)
                terrainDataPool = new ObjectPool<TerrainData>(CreateTerrain, actionOnDestroy: AdaptiveDestroy);
        }

        private TerrainData CreateTerrain(){
            return new();
        }
        protected override void OnProcessAdded(ChunkData chunk)
        {
            //Create terrain
            TerrainData newTerrain = terrainDataPool.Get();
            newTerrain.heightmapResolution = TerrainResolution;
            newTerrain.size = new Vector3(chunk.meshSettings.MeshScale, chunk.worldFinalData.MinMaxHeight.y-chunk.worldFinalData.MinMaxHeight.x, chunk.meshSettings.MeshScale);

            chunk.worldFinalData.AddProcessor(this);
            float[,] heights = new float[TerrainResolution,TerrainResolution];
            for(int x = 0; x < TerrainResolution; x++){
                for(int y = 0; y < TerrainResolution; y++){
                    float height = Remap(chunk.meshSettings.Resolution+1, TerrainResolution, y, x, chunk.worldFinalData.FinalPositions, 0);
                    heights[x,y] = Mathf.InverseLerp(chunk.worldFinalData.MinMaxHeight.x, chunk.worldFinalData.MinMaxHeight.y, height);
                }
            }
            newTerrain.SetHeights(0,0, heights);
            chunk.worldFinalData.RemoveProcessor(this);
            TerrainWithHeightResult result = new TerrainWithHeightResult(){
                ID = chunk.terrainConfig.ID,
                terrainData = newTerrain,
                data = chunk,
            };
            CompletedRequests.TryAdd(chunk.terrainConfig.ID, result);
            onProcessDone.Invoke(result);
        }

        private float Remap(int currentResolution, int targetResolution, int xTarget, int yTarget, NativeArray<Vertex> map, int offset){
            float factor = (float)(currentResolution)/(float)(targetResolution);
            float ogX = xTarget*factor;
            float ogY = yTarget*factor;

            if(xTarget == 0)
                ogX = 0;
            else if(xTarget == targetResolution-1)
                ogX = currentResolution-1;
            if(yTarget == 0)
                ogY = 0;
            else if(yTarget == targetResolution-1)
                ogY = currentResolution-1;

            int btmX = Mathf.Min(Mathf.FloorToInt(ogX), currentResolution);
            int btmY = Mathf.Min(Mathf.FloorToInt(ogY), currentResolution);
            int topX = Mathf.Min(Mathf.CeilToInt(ogX), currentResolution);
            int topY = Mathf.Min(Mathf.CeilToInt(ogY), currentResolution);
            
            int maxLength = MapTools.LengthFromResolution(currentResolution-1)-1;
            int indBtmX = Mathf.Min(maxLength, btmX+btmY*(currentResolution));
            int indBtmY = Mathf.Min(maxLength, btmX+topY*(currentResolution));
            int indTopX = Mathf.Min(maxLength, topX+btmY*(currentResolution));
            int indTopY = Mathf.Min(maxLength, topX+topY*(currentResolution));

            float leftDown = map[indBtmX].position.y;
            float leftUp = map[indBtmY].position.y;
            float rightDown = map[indTopX].position.y;
            float righTUp = map[indTopY].position.y;

            float fcX = ogX-btmX;
            float fcy = ogY-btmY;
            
            float interBotm = Mathf.Lerp(leftDown, rightDown, fcX);
            float interUp = Mathf.Lerp(leftUp, righTUp, fcX);
            return Mathf.Lerp(interBotm, interUp, fcy);
        }
        protected override void OnProcessRemoved(ChunkData chunk)
        {
            if(CompletedRequests.TryGetValue(chunk.terrainConfig.ID, out var result)){
                terrainDataPool.Release(result.terrainData);
                CompletedRequests.Remove(chunk.terrainConfig.ID);
            }
        }
    }
}