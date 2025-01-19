using UnityEngine;
using System;

namespace sapra.InfiniteLands
{
    public interface IVisualizeTerrain : IGenerate<ChunkData>
    {
        public void ChangeGenerator(IGraph generator);
        public void ForceGeneration(bool instanUpdate);
        public void RequestMesh(TerrainConfig config);
        public void UnrequestMesh(TerrainConfig config, ChunkData existingData);
        public void DisableChunk(IRenderChunk chunk);
        public IRenderChunk GenerateChunk(Vector3Int ID);
        public bool DrawGizmos{get;}
        public Vector2 localGridOffset{get;}
    }
}