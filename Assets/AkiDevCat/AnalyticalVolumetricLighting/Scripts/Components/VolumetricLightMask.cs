using System;
using UnityEngine;

namespace AkiDevCat.AVL.Components
{
    public class VolumetricLightMask : MonoBehaviour
    {
        [SerializeField] public MaskShape shape;
        
        public Matrix4x4 worldToMaskMatrix => transform.worldToLocalMatrix;
        
        public float BoundingRadius => shape switch 
        {
            MaskShape.OBB => transform.lossyScale.magnitude / 2.0f,
            MaskShape.Ellipsoid => Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z) / 2.0f,
            _ => throw new NotImplementedException()
        };
        
        private void OnEnable()
        {
            GlobalMaskManager.AddActiveMask(this);
        }

        private void OnDisable()
        {
            GlobalMaskManager.RemoveActiveMask(this);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = worldToMaskMatrix.inverse;
            
            switch (shape)
            {
                case MaskShape.OBB:
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                    break;
                case MaskShape.Ellipsoid:
                    Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
                    break;
            }
        }
    }
}