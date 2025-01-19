using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace sapra.InfiniteLands
{
    public struct AssetData{
        public JobHandle job;
        public NativeArray<float> map;
        public IEnumerable<IAsset> assets;
        public Type assetType;
    }
    public class ChunkData
    {
        public TerrainConfig terrainConfig{get; private set;}
        public MeshSettings meshSettings{get; private set;}
        public ChunkData(TerrainConfig terrainConfig, MeshSettings meshSettings){
            this.terrainConfig = terrainConfig;
            this.meshSettings = meshSettings;
        }
        //World Data
        public WorldFinalData worldFinalData;
        public CoordinateDataHolder coordinateData;
        public AssetsData assetsData;

        public DataManager manager;
        public JobHandle handle;

        public bool InstantGeneration;
        public void Return()
        {
            if(manager == null)
                return;
            manager.Return(terrainConfig.ID.ToString());
            assetsData.RemoveProcessor(this);
            worldFinalData.RemoveProcessor(this);
            coordinateData.RemoveProcessor(this);
        }

        public void Complete(){
            assetsData.AddProcessor(this);
            worldFinalData.AddProcessor(this);
            coordinateData.AddProcessor(this);

            handle.Complete();
            terrainConfig = GenerateConfiguration();
        }

        public TerrainConfig GenerateConfiguration(){
            try{
                float MinValue = worldFinalData.ChunkMinMax[0];
                float MaxValue = worldFinalData.ChunkMinMax[1];
                
                return new TerrainConfig(terrainConfig.ID, new Vector2(MinValue, MaxValue), terrainConfig.TerrainNormal, meshSettings.MeshScale, terrainConfig.Position);
            }catch{
                return default;
            }
        }
        public bool IsCompleted => handle.IsCompleted;
    }
}