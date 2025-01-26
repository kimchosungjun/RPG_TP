using System;
using System.Collections;
using System.Collections.Generic;
using AkiDevCat.AVL.Components;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AkiDevCat.AVL.Examples
{
    
    public class PerformanceExampleManager : MonoBehaviour
    {
        [SerializeField] private uint _lightCount = 100;

        private Camera _mainCamera;
        private List<VolumetricLight> _lights;
        
        private void Start()
        {
            ResetLights();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetLights();
            }

            // var radius = ((Mathf.Sin(Time.time) + 1.0f) * 0.25f + 0.5f) * 400.0f;
            var radius = 400.0f;
            var camPos = new Vector3(Mathf.Cos(Time.time) * radius, radius / 2.0f, Mathf.Sin(Time.time) * radius);
            _mainCamera.transform.position = camPos;
            _mainCamera.transform.LookAt(Vector3.zero);
        }

        private void ResetLights()
        {
            _mainCamera = Camera.main;
            
            if (_lights != null)
            {
                foreach (var light in _lights)
                {
                    Destroy(light.gameObject);
                }
            }

            _lights = new List<VolumetricLight>();

            var perAxis = Mathf.CeilToInt(Mathf.Pow(_lightCount, 1.0f / 3.0f));

            for (var z = 0; z < perAxis; z++)
            {
                for (var y = 0; y < perAxis; y++)
                {
                    for (var x = 0; x < perAxis; x++)
                    {
                        var pos = new Vector3(x / (float)(perAxis - 1), 
                                                          y / (float)(perAxis - 1),
                                                          z / (float)(perAxis - 1));
                        pos = pos * 2.0f - Vector3.one;
                        pos *= 300.0f;
                        
                        var lightObj = new GameObject("AVL Light");
                        var light = lightObj.AddComponent<VolumetricLight>();

                        light.lightIntensity = 5.0f + Random.value * 2.0f;
                        if (Random.value > 0.5)
                            light.lightColor = Random.ColorHSV(-15.0f / 360.0f, 25.0f / 360.0f, 0.3f, 0.9f, 1.0f, 1.0f);
                        else
                            light.lightColor = Random.ColorHSV(220.0f / 360.0f, 230.0f / 360.0f, 0.3f, 0.9f, 1.0f, 1.0f);
                        light.distanceCullingEnabled = false;
                        light.lightRange = 30.0f;
                        light.lightShape = LightShape.Point;
                        light.lightPrimaryAngle = 120.0f;
                        light.lightSecondaryAngle = 90.0f;
                        light.lightScattering = 0.01f;
                        light.lightRect = new Vector2(2.0f, 2.0f);
                        lightObj.transform.rotation = Random.rotation;

                        lightObj.transform.position = pos;
                
                        _lights.Add(light);
                    }
                }
            }
            
            // for (var i = 0; i < _lightCount; i++)
            // {
            //     var lightObj = new GameObject("AVL Light");
            //     var light = lightObj.AddComponent<VolumetricLight>();
            //
            //     light.lightIntensity = 5.0f + Random.value * 2.0f;
            //     if (Random.value > 0.5)
            //         light.lightColor = Random.ColorHSV(-15.0f / 360.0f, 25.0f / 360.0f, 0.3f, 0.9f, 1.0f, 1.0f);
            //     else
            //         light.lightColor = Random.ColorHSV(220.0f / 360.0f, 230.0f / 360.0f, 0.3f, 0.9f, 1.0f, 1.0f);
            //     light.distanceCullingEnabled = false;
            //     light.lightRange = 30.0f;
            //     light.lightShape = LightShape.Point;
            //     light.lightPrimaryAngle = 120.0f;
            //     light.lightSecondaryAngle = 90.0f;
            //     light.lightScattering = 0.01f;
            //     light.lightRect = new Vector2(2.0f, 2.0f);
            //     lightObj.transform.rotation = Random.rotation;
            //
            //     lightObj.transform.position = Random.insideUnitSphere * 300.0f;
            //     
            //     _lights.Add(light);
            // }
        }
    }
}