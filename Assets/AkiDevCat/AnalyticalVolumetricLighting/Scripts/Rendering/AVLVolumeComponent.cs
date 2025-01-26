using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace AkiDevCat.AVL.Rendering
{
    [Serializable, VolumeComponentMenu("AkiDevCat/Analytic Volumetric Lighting")]
    public class AVLVolumeComponent : VolumeComponent
    {
        [Header("Air Constants")]
        public ClampedFloatParameter airDensity = new(0.0f, 0.0f, 1.0f);

        public ClampedFloatParameter fogDensityGlobal = new(0.0f, 0.0f, 1.0f);
        
        public ClampedFloatParameter fogDensityPerLightModifier = new(0.5f, 0.0f, 1.0f);
        
        public ClampedFloatParameter fogScattering = new(0.0f, 0.0f, 1.0f);

        public ColorParameter fogColor = new(new Color(0.01f, 0.03f, 0.05f));

        public bool IsEnabled => active && airDensity.value > 0.0f;
        
        protected override void OnEnable()
        {
            displayName = "Analytic Volumetric Lighting";
            
            base.OnEnable();
        }
    }
}