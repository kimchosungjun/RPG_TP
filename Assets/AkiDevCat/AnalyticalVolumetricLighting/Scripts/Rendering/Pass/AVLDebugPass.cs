using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AkiDevCat.AVL.Rendering
{
    public class AVLDebugPass : AVLFeaturePass
    {
        internal static class ShaderConstants
        {
            public static readonly int _AVLFogTexture          = Shader.PropertyToID("_AVLFogTexture");
            public static readonly int _LightClusterBuffer     = Shader.PropertyToID("_LightClusterBuffer");
            public static readonly int _ClusterSize            = Shader.PropertyToID("_ClusterSize");
            public static readonly int _CameraViewMatrix       = Shader.PropertyToID("_CameraViewMatrix");
            public static readonly int _GlobalLightBuffer      = Shader.PropertyToID("_GlobalLightBuffer");
            public static readonly int _LightIndexBuffer       = Shader.PropertyToID("_LightIndexBuffer");
            public static readonly int _DebugMode              = Shader.PropertyToID("_DebugMode");
            public static readonly int _GlobalLightBufferSize  = Shader.PropertyToID("_GlobalLightBufferSize");
            public static readonly int _DebugModeOpacity       = Shader.PropertyToID("_DebugModeOpacity");
        }
        
        private const int SP_DEBUG = 0;
        
        private readonly Shader _shader;
        private readonly Material _material;
        
        private readonly RenderTargetIdentifier _fogRTI;

        public AVLDebugPass(IRenderingContext context) : base(context)
        {
            this.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            
            _shader = context.Settings.DebugShader;
            Debug.Assert(_shader, $"[AVL] Debug shader is missing in the feature settings. Please assign the shader.");
            
            if (_shader == null)
            {
                return;
            }
            
            _material = CoreUtils.CreateEngineMaterial(_shader);
            
            _fogRTI = new RenderTargetIdentifier(ShaderConstants._AVLFogTexture);
        }
        
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);
            
            // ToDo?
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
            
            ConfigureTarget(RenderingContext.CameraColorTarget, RenderingContext.CameraDepthTarget);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (RenderingContext.Settings.debugMode == DebugMode.None)
            {
                return;
            }
            
            if (_shader == null || _material == null)
            {
                return;
            }

            if (!renderingData.cameraData.postProcessEnabled)
            {
                return;
            }
            
            var stack = VolumeManager.instance.stack;
            var component = stack.GetComponent<AVLVolumeComponent>();
            if (component == null || !component.IsEnabled)
            {
                return;
            }
            
            ref var cameraData = ref renderingData.cameraData;
            
            var commandBuffer = CommandBufferPool.Get(nameof(AVLDebugPass));

            SetupMaterial(cameraData);

            // Render Debug Overview
            commandBuffer.Blit(null, RenderingContext.CameraColorTarget, _material, SP_DEBUG);
            
            // Restore render targets
            // commandBuffer.SetRenderTarget(RenderingContext.CameraColorTarget, RenderingContext.CameraDepthTarget);

            context.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
            CommandBufferPool.Release(commandBuffer);
        }

        private void SetupMaterial(CameraData cameraData)
        {
            // ToDo get from clustering pass or something
            var clusterSize = new Vector4(RenderingContext.Settings.cullingClusterSizeX, RenderingContext.Settings.cullingClusterSizeY);
            clusterSize.z = 1.0f / clusterSize.x;
            clusterSize.w = 1.0f / clusterSize.y;
            
            _material.SetInt(ShaderConstants._DebugMode, (int)RenderingContext.Settings.debugMode);
            _material.SetInt(ShaderConstants._GlobalLightBufferSize, (int)RenderingContext.GlobalLightBufferSize);
            _material.SetFloat(ShaderConstants._DebugModeOpacity, RenderingContext.Settings.debugOpacity);
            _material.SetMatrix(ShaderConstants._CameraViewMatrix, cameraData.GetViewMatrix());
            _material.SetBuffer(ShaderConstants._LightClusterBuffer, RenderingContext.LightClusterBuffer);
            _material.SetBuffer(ShaderConstants._GlobalLightBuffer, RenderingContext.GlobalLightBuffer);
            _material.SetBuffer(ShaderConstants._LightIndexBuffer, RenderingContext.LightIndexBuffer);
            _material.SetVector(ShaderConstants._ClusterSize, clusterSize);
        }
        
        public override void Cleanup()
        {
            CoreUtils.Destroy(_material);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            // base.FrameCleanup(cmd);
        }
    }
}