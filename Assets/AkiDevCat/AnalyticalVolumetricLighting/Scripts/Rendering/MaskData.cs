using UnityEngine;

namespace AkiDevCat.AVL.Rendering
{
    [System.Serializable]
    public struct MaskData
    {
        public uint Type;
        public Matrix4x4 WorldToMask;
        public Vector3 Origin;
        public float BoundingRadius;
    }
}