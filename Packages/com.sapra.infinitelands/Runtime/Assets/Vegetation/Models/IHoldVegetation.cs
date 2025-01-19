using System;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.InfiniteLands{  
    public interface IHoldVegetation
    {
        public string name{get;}
        public enum DensityHeightMode{Independent, Bounded, Absolute}
        public enum AlignmentMode{Up, Terrain, Ground}

        public bool SkipRendering();
        public Vector2 GetMinimumMaximumScale();
        public float GetVerticalPosition();
        public AlignmentMode GetAlignmentMode();
        public DensityHeightMode GetDensityMode();
        public float GetPositionRandomness();
        public float GetTextureRandomness();
        public float ExtraVerticalBound();

        public float GetViewDistance();
        public float GetDistanceBetweenItems();
        public IRenderVegetation GetRenderer(IPaintTerrain painter, IGenerate<TextureResult> TextureMaker,PointStore store, VegetationSettings settings, 
            List<Camera> cameras, RenderTexture[] depthtextures, Transform colliderParent);
        public ICompact GetCompactor(VegetationSettings settings, int ChunksPerBuffer, List<GraphicsBuffer.IndirectDrawIndexedArgs> arguments);
    }
}