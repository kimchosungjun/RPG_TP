using UnityEngine;
using UnityEngine.Serialization;

namespace AkiDevCat.AVL.Rendering
{
    [System.Serializable]
    public struct LightData
    {
        public uint Type;
        public Vector3 Origin;
        public Vector3 Right;
        public Vector3 Up;
        public Vector3 Forward;
        public Vector3 BoundingOrigin;
        public float  BoundingRadius;
        public Color Color;
        public float Range;
        public Vector2 Rect;
        public Vector4 PrimaryAngle;
        public Vector4 SecondaryAngle;
        public float SecondaryEnergyModifier;
        public float Scattering;
        public float CullingFading;
        public int MaskID;
    }
}