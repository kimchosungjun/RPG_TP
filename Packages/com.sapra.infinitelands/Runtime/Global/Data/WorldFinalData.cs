using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace sapra.InfiniteLands
{
    public class WorldFinalData : ProcessableData
    {
        public WorldFinalData(DataManager manager, string requestor,
            NativeArray<int> chunkMinMax, NativeArray<Vertex> finalPositions, Vector2 minMaxHeight, JobHandle job) : base(manager, requestor){
            MinMaxHeight = minMaxHeight;
            ChunkMinMax = chunkMinMax;
            FinalPositions = finalPositions;
            jobHandle = job;
        }

        public NativeArray<int> ChunkMinMax;
        public NativeArray<Vertex> FinalPositions;
        public Vector2 MinMaxHeight;
        public JobHandle jobHandle;
    }
}