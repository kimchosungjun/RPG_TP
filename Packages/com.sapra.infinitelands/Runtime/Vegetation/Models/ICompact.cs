using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace sapra.InfiniteLands{
    public interface ICompact : IDisposable
    {
        public ComputeBuffer PerInstanceData{get;}
        public VegetationChunk[] ChunksForPosition{get;}

        public void DrawBounds();
        public int SetChunk(VegetationChunk chunk);
        public void DisableChunk(int chunkIndex);

        public void InitialCompact(CommandBuffer bf, Matrix4x4 localToWorldMatrix);
        public void VisibilityCheck(CommandBuffer bf, bool EnableCulling);
        
        public void OriginShift(CommandBuffer bf,ComputeShader shader, int kernel);
        public void CountAndCompact(CommandBuffer bf);
        public void DrawItems(MaterialPropertyBlock propertyBlock, Camera cam);
        public void SetGenerationData(CommandBuffer bf,ComputeShader shader, int kernel, int ChunkIndex);
    }
}