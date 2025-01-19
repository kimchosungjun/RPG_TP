using UnityEngine;

namespace sapra.InfiniteLands{
    public interface IRenderChunk 
    {
        public GameObject gameObject{get;}
        public void Reuse(TerrainConfig config, MeshSettings meshSettings);        
        public bool VisibilityCheck(Vector3 playerPosition, Plane[] cameraPlanes, float GenerationDistance);
    }
}