using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AkiDevCat.AVL.Rendering
{
    public class AVLRenderFeature : ScriptableRendererFeature
    {
        [SerializeField] private AVLFeatureSettings _settings;

        private GraphicsBuffer _globalLightsBuffer;
        private GraphicsBuffer _globalMasksBuffer;

        private Queue<AVLFeaturePass> _renderPasses;
        private AVLRenderingContext _renderContext;
        
        public override void Create()
        {
            if (_settings == null)
            {
                return;
            }
            
            // Cleanup
            FullCleanup();

            // Initialize
            name = "Analytical Volumetric Lighting";
            _renderPasses = new Queue<AVLFeaturePass>();

            #if UNITY_EDITOR
            
            // // Load shaders
            // if (_settings.ClusteringShader == null)
            // {
            //     _settings.ClusteringShader = UnityEditor.AssetDatabase.LoadAssetAtPath<ComputeShader>(
            //         "Assets/AkiDevCat/AnalyticalVolumetricLighting/Shaders/Passes/AVLClusteringPass.compute");
            //     UnityEditor.EditorUtility.SetDirty(this);
            // }
            //
            // if (_settings.RenderingShader == null)
            // {
            //     _settings.RenderingShader = UnityEditor.AssetDatabase.LoadAssetAtPath<Shader>(
            //         "Assets/AkiDevCat/AnalyticalVolumetricLighting/Shaders/Passes/AVLRenderingPass.shader");
            //     UnityEditor.EditorUtility.SetDirty(this);
            // }
            //
            // if (_settings.DebugShader == null)
            // {
            //     _settings.DebugShader = UnityEditor.AssetDatabase.LoadAssetAtPath<Shader>(
            //         "Assets/AkiDevCat/AnalyticalVolumetricLighting/Shaders/Passes/AVLDebugPass.shader");
            //     UnityEditor.EditorUtility.SetDirty(this);
            // }
            //
            // UnityEditor.AssetDatabase.SaveAssets();

            #endif
            
            CreateGraphicsBuffers();

            _renderContext = new AVLRenderingContext(_settings);

            // Create Passes
            _renderPasses.Enqueue(new AVLClusteringPass(_renderContext));
            _renderPasses.Enqueue(new AVLRenderingPass(_renderContext));

            if (_settings.debugMode != DebugMode.None)
            {
                _renderPasses.Enqueue(new AVLDebugPass(_renderContext));
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!_settings.IsValid)
            {
                return;
            }

            // Update rendering context
            _renderContext.Settings = _settings;
            _renderContext.Renderer = renderer;
            _renderContext.GlobalLightBuffer = _globalLightsBuffer;
            _renderContext.GlobalMaskBuffer = _globalMasksBuffer;

            // Enqueue all passes
            foreach (var pass in _renderPasses)
            {
                renderer.EnqueuePass(pass);
            }
        }

        protected override void Dispose(bool disposing) 
        {
            FullCleanup();
        }

        private void CreateGraphicsBuffers()
        {
            _globalLightsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _settings.maxLights, Marshal.SizeOf<LightData>());
            _globalMasksBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _settings.maxMasks, Marshal.SizeOf<MaskData>());
        }

        private void CleanupGraphicsBuffers()
        {
            _globalLightsBuffer?.Dispose();
            _globalMasksBuffer?.Dispose();

            _globalLightsBuffer = null;
            _globalMasksBuffer = null;
        }

        private void FullCleanup()
        {
            // Cleanup render passes
            if (_renderPasses != null)
            {
                foreach (var pass in _renderPasses)
                {
                    pass.Cleanup();
                }
            }

            CleanupGraphicsBuffers();

            _renderContext?.Cleanup();
            _renderContext = null;
        }
    }
}