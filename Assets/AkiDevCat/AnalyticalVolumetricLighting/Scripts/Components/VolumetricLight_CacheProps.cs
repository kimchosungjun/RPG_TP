using System.Collections;
using UnityEngine;

/*
 * This might be quite hard to see & read but this is the simplest & fastest way to cache variables
 */

namespace AkiDevCat.AVL.Components
{
    public partial class VolumetricLight
    {
        public Transform TransformCached { get; private set; }
        
        public Vector3 TransformRightCached { get; private set; }
        
        public Vector3 TransformUpCached { get; private set; }
        
        public Vector3 TransformForwardCached { get; private set; }
        
        public Color LightVisibleColorCached { get; private set; }
        
        public Vector3 BoundingOriginCached { get; private set; }
        
        public float BoundingRadiusCached { get; private set; }
        
        public Vector4 PrimaryAngleRadCached { get; private set; }
        
        public Vector4 SecondaryAngleRadCached { get; private set; }
        
        public float LightScatteringSqrCached { get; private set; }

        public int LastUpdatedFrame { get; private set; } = -1;

        private bool _transformHasBeenCached = false;

        public void UpdateLightCache()
        {
            var frame = Time.frameCount;
            
            if (LastUpdatedFrame == frame)
            {
                return;
            }

            LastUpdatedFrame = frame;

            UpdateLightCacheForce();
        }
        
        public void UpdateLightCacheForce()
        {
            if (CanCopyLightComponent)
            {
                CopyFromLightComponentSettings();
            }
            
            if (!_transformHasBeenCached)
            {
                TransformCached = transform;
            }

            if (_lightColorInvalidated || _lightIntensityInvalidated || _lightColorTemperatureInvalidated || _useColorTemperatureInvalidated)
            {
                LightVisibleColorCached = LightVisibleColor;
            }

            if (TransformCached.hasChanged)
            {
                TransformRightCached = TransformCached.right;
                TransformUpCached = TransformCached.up;
                TransformForwardCached = TransformCached.forward;
            }

            if (TransformCached.hasChanged || _lightRangeInvalidated || _lightPrimaryAngleInvalidated || _lightSecondaryAngleInvalidated)
            {
                BoundingOriginCached = BoundingOrigin;
                BoundingRadiusCached = BoundingRadius;
            }

            if (_lightPrimaryAngleInvalidated)
            {
                Vector4 result;
                result.x = lightPrimaryAngle * Mathf.Deg2Rad;
                result.y = Mathf.Sin(result.x * 0.5f);
                result.z = Mathf.Cos(result.x * 0.5f);
                result.w = Mathf.Tan(result.x);
                PrimaryAngleRadCached = result;
            }

            if (_lightSecondaryAngleInvalidated)
            {
                Vector4 result;
                result.x = lightSecondaryAngle * Mathf.Deg2Rad;
                result.y = Mathf.Sin(result.x * 0.5f);
                result.z = Mathf.Cos(result.x * 0.5f);
                result.w = Mathf.Tan(result.x);
                SecondaryAngleRadCached = result;
            }

            if (_lightScatteringInvalidated)
            {
                LightScatteringSqrCached = lightScattering * lightScattering;
            }

            transform.hasChanged = false;
            
            _baseLightComponentInvalidated = false;
            _copyLightComponentInvalidated = false;
            _lightShapeInvalidated = false;
            _lightColorInvalidated = false;
            _useColorTemperatureInvalidated = false;
            _lightColorTemperatureInvalidated = false;
            _lightIntensityInvalidated = false;
            _lightScatteringInvalidated = false;
            _lightRangeInvalidated = false;
            _lightRectInvalidated = false;
            _lightPrimaryAngleInvalidated = false;
            _lightSecondaryAngleInvalidated = false;
            _lightMaskInvalidated = false;
            _distanceCullingEnabledInvalidated = false;
            _softCullingDistanceInvalidated = false;
            _hardCullingDistanceInvalidated = false;
        }

        #region Field Overrides
        
        public Light baseLightComponent
        {
            get => _baseLightComponent;
            set
            {
                if (_baseLightComponentInvalidated)
                {
                    _baseLightComponent = value;
                    return;
                }
                
                _baseLightComponentInvalidated = true;
                _baseLightComponent = value;
            }
        }
        [System.NonSerialized] private bool _baseLightComponentInvalidated = true;
        
        public bool copyLightComponent
        {
            get => _copyLightComponent;
            set
            {
                if (_copyLightComponentInvalidated)
                {
                    _copyLightComponent = value;
                    return;
                }
                
                _copyLightComponentInvalidated = true;
                _copyLightComponent = value;
            }
        }
        [System.NonSerialized] private bool _copyLightComponentInvalidated = true;
        
        public LightShape lightShape
        {
            get => _lightShape;
            set
            {
                if (_lightShapeInvalidated)
                {
                    _lightShape = value;
                    return;
                }
                
                _lightShapeInvalidated = true;
                _lightShape = value;
            }
        }
        [System.NonSerialized] private bool _lightShapeInvalidated = true;
        
        public Color lightColor
        {
            get => _lightColor;
            set
            {
                if (_lightColorInvalidated)
                {
                    _lightColor = value;
                    return;
                }
                
                _lightColorInvalidated = true;
                _lightColor = value;
            }
        }
        [System.NonSerialized] private bool _lightColorInvalidated = true;
        
        public bool useColorTemperature
        {
            get => _useColorTemperature;
            set
            {
                if (_useColorTemperatureInvalidated)
                {
                    _useColorTemperature = value;
                    return;
                }
                
                _useColorTemperatureInvalidated = true;
                _useColorTemperature = value;
            }
        }
        [System.NonSerialized] private bool _useColorTemperatureInvalidated = true;
        
        public float lightColorTemperature
        {
            get => _lightColorTemperature;
            set
            {
                if (_lightColorTemperatureInvalidated)
                {
                    _lightColorTemperature = value;
                    return;
                }
                
                _lightColorTemperatureInvalidated = true;
                _lightColorTemperature = value;
            }
        }
        [System.NonSerialized] private bool _lightColorTemperatureInvalidated = true;
        
        public float lightIntensity
        {
            get => _lightIntensity;
            set
            {
                if (_lightIntensityInvalidated)
                {
                    _lightIntensity = value;
                    return;
                }
                
                _lightIntensityInvalidated = true;
                _lightIntensity = value;
            }
        }
        [System.NonSerialized] private bool _lightIntensityInvalidated = true;
        
        public float lightScattering
        {
            get => _lightScattering;
            set
            {
                if (_lightScatteringInvalidated)
                {
                    _lightScattering = value;
                    return;
                }
                
                _lightScatteringInvalidated = true;
                _lightScattering = value;
            }
        }
        [System.NonSerialized] private bool _lightScatteringInvalidated = true;
        
        public float lightRange
        {
            get => _lightRange;
            set
            {
                if (_lightRangeInvalidated)
                {
                    _lightRange = value;
                    return;
                }
                
                _lightRangeInvalidated = true;
                _lightRange = value;
            }
        }
        [System.NonSerialized] private bool _lightRangeInvalidated = true;
        
        public Vector2 lightRect
        {
            get => _lightRect;
            set
            {
                if (_lightRectInvalidated)
                {
                    _lightRect = value;
                    return;
                }
                
                _lightRectInvalidated = true;
                _lightRect = value;
            }
        }
        [System.NonSerialized] private bool _lightRectInvalidated = true;

        public bool mainLightAlignment
        {
            get => _mainLightAlignment;
            set => _mainLightAlignment = value;
        }
        
        public float lightPrimaryAngle
        {
            get => _lightPrimaryAngle;
            set
            {
                if (_lightPrimaryAngleInvalidated)
                {
                    _lightPrimaryAngle = value;
                    return;
                }
                
                _lightPrimaryAngleInvalidated = true;
                _lightPrimaryAngle = value;
            }
        }
        [System.NonSerialized] private bool _lightPrimaryAngleInvalidated = true;
        
        public float lightSecondaryAngle
        {
            get => _lightSecondaryAngle;
            set
            {
                if (_lightSecondaryAngleInvalidated)
                {
                    _lightSecondaryAngle = value;
                    return;
                }
                
                _lightSecondaryAngleInvalidated = true;
                _lightSecondaryAngle = value;
            }
        }
        [System.NonSerialized] private bool _lightSecondaryAngleInvalidated = true;
        
        public VolumetricLightMask lightMask
        {
            get => _lightMask;
            set
            {
                if (_lightMaskInvalidated)
                {
                    _lightMask = value;
                    return;
                }
                
                _lightMaskInvalidated = true;
                _lightMask = value;
            }
        }
        [System.NonSerialized] private bool _lightMaskInvalidated = true;
        
        public bool distanceCullingEnabled
        {
            get => _distanceCullingEnabled;
            set
            {
                if (_distanceCullingEnabledInvalidated)
                {
                    _distanceCullingEnabled = value;
                    return;
                }
                
                _distanceCullingEnabledInvalidated = true;
                _distanceCullingEnabled = value;
            }
        }
        [System.NonSerialized] private bool _distanceCullingEnabledInvalidated = true;
        
        public float softCullingDistance
        {
            get => _softCullingDistance;
            set
            {
                if (_softCullingDistanceInvalidated)
                {
                    _softCullingDistance = value;
                    return;
                }
                
                _softCullingDistanceInvalidated = true;
                _softCullingDistance = value;
            }
        }
        [System.NonSerialized] private bool _softCullingDistanceInvalidated = true;
        
        public float hardCullingDistance
        {
            get => _hardCullingDistance;
            set
            {
                if (_hardCullingDistanceInvalidated)
                {
                    _hardCullingDistance = value;
                    return;
                }
                
                _hardCullingDistanceInvalidated = true;
                _hardCullingDistance = value;
            }
        }
        [System.NonSerialized] private bool _hardCullingDistanceInvalidated = true;
        
        #endregion
    }
}