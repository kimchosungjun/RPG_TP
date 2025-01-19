using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace sapra.InfiniteLands{  
    public interface IRenderVegetation : IDisposable{
        public IHoldVegetation GetAsset();
        public void OnOriginShift(CommandBuffer bf, ComputeShader compute, int kernel, Vector3 offset);
        public void Render(IControlMatrices matrixControl, MaterialPropertyBlock propertyBlock, bool EnableCulling);
        
        public void HandleColliders(Vector3 playerPosition);
        public void DrawGizmos();
    }
}