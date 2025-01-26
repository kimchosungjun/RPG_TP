using UnityEngine;

namespace AkiDevCat.AVL.Rendering
{
    [System.Serializable]
    public struct LightClusterData
    {
        // Up, Right, Down, Left frustum planes
        public Matrix4x4 FrustumPlanes4;
        public float FarPlaneDepth;
        public uint BufferOffset;
        public uint LightCount;
        public uint MaskBufferOffset;
        public uint MaskCount;
    }
}