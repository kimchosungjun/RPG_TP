using UnityEngine;

namespace AkiDevCat.AVL.Components
{
    [AddComponentMenu("AVL/Volumetric Light")]
    [ExecuteInEditMode]
    public partial class VolumetricLight : MonoBehaviour
    {
        [SerializeField] private Light _baseLightComponent = null;

        [SerializeField] private bool _copyLightComponent = false;

        [SerializeField] private LightShape _lightShape = LightShape.Point;
        [SerializeField] private Color _lightColor = Color.white;
        [SerializeField] private bool _useColorTemperature = false;
        [SerializeField] private float _lightColorTemperature = 6000.0f;
        [SerializeField] private float _lightIntensity = 1.0f;
        [SerializeField] private float _lightScattering = 0.01f;
        [SerializeField] private float _lightRange = 10.0f;
        [SerializeField] private Vector2 _lightRect = Vector2.one;
        [SerializeField] private bool _mainLightAlignment = false;

        [SerializeField] private float _lightPrimaryAngle = 90.0f;
        [SerializeField] private float _lightSecondaryAngle = 135.0f;

        [SerializeField] private VolumetricLightMask _lightMask;

        [SerializeField] private bool _distanceCullingEnabled = true;

        [SerializeField] private float _softCullingDistance = 150.0f;
        [SerializeField] private float _hardCullingDistance = 200.0f;

        #region Properties

        public bool LightEnabled => CanCopyLightComponent ? baseLightComponent.enabled && enabled : enabled;

        public Color LightVisibleColor => useColorTemperature ? lightColor * lightIntensity * Mathf.CorrelatedColorTemperatureToRGB(lightColorTemperature) : lightColor * lightIntensity;

        public Vector3 BoundingOrigin => lightShape switch
        {
            LightShape.AreaHardEdge => transform.position + 
                                       (transform.forward + transform.right * Mathf.Tan(_lightPrimaryAngle * Mathf.Deg2Rad) + transform.up * Mathf.Tan(_lightSecondaryAngle * Mathf.Deg2Rad)).normalized 
                                       * lightRange * 0.5f,
            _ => transform.position
        };

        public float BoundingRadius => lightShape switch
        {
            // LightShape.AreaHardEdge => Mathf.Sqrt(lightRange * lightRange * 0.25f + lightRect.x * lightRect.x * 0.25f +
            //                                       lightRect.y * lightRect.y * 0.25f),
            // LightShape.AreaHardEdge => Mathf.Sqrt(
            //     Mathf.Pow(lightRect.x + lightRange * Mathf.Sin(_lightPrimaryAngle * Mathf.Deg2Rad), 2) + 
            //     Mathf.Pow(lightRect.y + lightRange * Mathf.Sin(_lightSecondaryAngle * Mathf.Deg2Rad), 2) + 
            //     Mathf.Pow(lightRange * Mathf.Cos(_lightPrimaryAngle * Mathf.Deg2Rad) * Mathf.Cos(_lightSecondaryAngle * Mathf.Deg2Rad), 2)),
            LightShape.AreaHardEdge => 2.0f * Mathf.Sqrt(
                Mathf.Pow(lightRect.x * 0.5f + lightRange * Mathf.Tan(_lightPrimaryAngle * Mathf.Deg2Rad), 2) + 
                Mathf.Pow(lightRect.y * 0.5f + lightRange * Mathf.Tan(_lightSecondaryAngle * Mathf.Deg2Rad), 2) + 
                Mathf.Pow(lightRange * 0.5f, 2)),
            LightShape.UniformBox => Mathf.Sqrt(lightRange * lightRange + lightRect.x * lightRect.x + lightRect.y * lightRect.y),
            _ => lightRange
        };

        public bool CanCopyLightComponent => copyLightComponent && baseLightComponent != null;

        #endregion

        public void CopyFromLightComponentSettings()
        {
            if (baseLightComponent == null)
            {
                return;
            }
            
            lightShape = baseLightComponent.type switch 
            {
                LightType.Point => LightShape.Point,
                LightType.Spot => lightShape is LightShape.SpotSoftEdge or LightShape.SpotHardEdge or LightShape.SpotHardEdgeSingle ? lightShape : LightShape.SpotSoftEdge,
                LightType.Rectangle => LightShape.AreaHardEdge,
                _ => LightShape.Point
            };
            lightColor = baseLightComponent.color;
            useColorTemperature = baseLightComponent.useColorTemperature;
            lightColorTemperature = baseLightComponent.colorTemperature;
            lightIntensity = baseLightComponent.intensity;
            lightRange = baseLightComponent.range;

            lightPrimaryAngle = baseLightComponent.spotAngle;
            lightSecondaryAngle = baseLightComponent.innerSpotAngle;
        }
        
        public void CopyToLightComponentSettings()
        {
            if (baseLightComponent == null)
            {
                return;
            }
            
            baseLightComponent.type = lightShape switch
            {
                LightShape.Point => LightType.Point,
                LightShape.SpotHardEdge => LightType.Spot,
                LightShape.SpotSoftEdge => LightType.Spot,
                LightShape.SpotHardEdgeSingle => LightType.Spot,
                LightShape.AreaHardEdge => LightType.Rectangle,
                _ => LightType.Point
            };
            baseLightComponent.color = lightColor;
            baseLightComponent.useColorTemperature = useColorTemperature;
            baseLightComponent.colorTemperature = lightColorTemperature;
            baseLightComponent.intensity = lightIntensity;
            baseLightComponent.range = lightRange;

            baseLightComponent.spotAngle = lightPrimaryAngle * Mathf.Rad2Deg;
            baseLightComponent.innerSpotAngle = lightSecondaryAngle * Mathf.Rad2Deg;
        }

        private void Awake()
        {
            UpdateLightCacheForce();
        }

        private void OnEnable()
        {
            GlobalLightManager.AddActiveLight(this);
        }

        private void OnDisable()
        {
            GlobalLightManager.RemoveActiveLight(this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.color = LightVisibleColor;
            
            var v0 = Vector3.zero;
            var f = Vector3.forward;
            var x = Mathf.Sin(lightPrimaryAngle * Mathf.Deg2Rad / 2.0f);
            var y = Mathf.Cos(lightPrimaryAngle * Mathf.Deg2Rad / 2.0f);

            switch (lightShape)
            {
                case LightShape.Point:
                case LightShape.UniformSphere:
                    Gizmos.DrawWireSphere(v0, lightRange);
                    break;
                case LightShape.SpotSoftEdge:
                case LightShape.SpotHardEdge:
                case LightShape.SpotHardEdgeSingle:
                    Gizmos.DrawRay(v0, f * lightRange);
                    Gizmos.DrawRay(v0, new Vector3(x, 0.0f, y) * lightRange);
                    Gizmos.DrawRay(v0, new Vector3(-x, 0.0f, y) * lightRange);
                    Gizmos.DrawRay(v0, new Vector3(0.0f, x, y) * lightRange);
                    Gizmos.DrawRay(v0, new Vector3(0.0f, -x, y) * lightRange);
                    Gizmos.DrawLine(f * lightRange, new Vector3(x, 0.0f, y) * lightRange);
                    Gizmos.DrawLine(f * lightRange, new Vector3(-x, 0.0f, y) * lightRange);
                    Gizmos.DrawLine(f * lightRange, new Vector3(0.0f, x, y) * lightRange);
                    Gizmos.DrawLine(f * lightRange, new Vector3(0.0f, -x, y) * lightRange);
                    break;
                case LightShape.AreaHardEdge:
                    Gizmos.DrawWireCube(Vector3.forward * 0.5f * lightRange, new Vector3(lightRect.x, lightRect.y, lightRange));
                    break;
            }
        }
    }
}