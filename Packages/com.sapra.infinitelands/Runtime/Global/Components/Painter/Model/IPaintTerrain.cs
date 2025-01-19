using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace sapra.InfiniteLands{
    public interface IPaintTerrain
    {
        public void AssignMaterials(CommandBuffer bf, ComputeShader compute, int kernelIndex);
        public void AssignMaterials(Vector3Int id, Material material);
        public void AssignMaterials(Material material);

        public TextureResult GetTextureAt(Vector2 position);
    }
}