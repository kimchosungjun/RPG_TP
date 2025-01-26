using System.Diagnostics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Debug = UnityEngine.Debug;

namespace AkiDevCat.AVL.Rendering
{
    public class AVLRenderingPass : AVLFeaturePass
    {
        internal static class ShaderConstants
        {
            public static readonly int _AVLBufferTexture = Shader.PropertyToID("_AVLBufferTexture");
            public static readonly int _AVLFogTexture = Shader.PropertyToID("_AVLFogTexture");
            public static readonly int _RenderingResolution = Shader.PropertyToID("_RenderingResolution");
            public static readonly int _ClusterSize = Shader.PropertyToID("_ClusterSize");
            public static readonly int _CameraWorldSpacePosition = Shader.PropertyToID("_CameraWorldSpacePosition");
            public static readonly int _AirDensity = Shader.PropertyToID("_AirDensity");
            public static readonly int _FogDensityGlobal = Shader.PropertyToID("_FogDensityGlobal");
            public static readonly int _FogDensityPerLight = Shader.PropertyToID("_FogDensityPerLight");
            public static readonly int _FogScattering = Shader.PropertyToID("_FogScattering");
            public static readonly int _FogColor = Shader.PropertyToID("_FogColor");
            public static readonly int _CameraNearFrustumMatrix = Shader.PropertyToID("_CameraNearFrustumMatrix");
            public static readonly int _CameraToWorldMatrix = Shader.PropertyToID("_CameraToWorldMatrix");
            public static readonly int _GlobalLightBuffer = Shader.PropertyToID("_GlobalLightBuffer");
            public static readonly int _LightClusterBuffer = Shader.PropertyToID("_LightClusterBuffer");
            public static readonly int _LightIndexBuffer = Shader.PropertyToID("_LightIndexBuffer");
            public static readonly int _GlobalLightBufferCount = Shader.PropertyToID("_GlobalLightBufferCount");
            public static readonly int _GlobalMaskBuffer = Shader.PropertyToID("_GlobalMaskBuffer");
            public static readonly int _MaskIndexBuffer = Shader.PropertyToID("_MaskIndexBuffer");
            public static readonly int _MaskInverseIndexBuffer = Shader.PropertyToID("_MaskInverseIndexBuffer");
            public static readonly int _GlobalMaskBufferMaxSize = Shader.PropertyToID("_GlobalMaskBufferMaxSize");
        }
        
        private const int SP_RENDER_PASS = 0;
        private const int SP_UPSCALE_PASS = 1;
        private const int SP_BLEND_PASS = 2;
        private const int SP_FOG_PASS = 3;

        private const string RENDERING_PROFILING_NAME = nameof(AVLRenderingPass) + " - Rendering";
        private const string UPSCALING_PROFILING_NAME = nameof(AVLRenderingPass) + " - Upscaling";
        private const string BLENDING_PROFILING_NAME  = nameof(AVLRenderingPass) + " - Blending";
        private const string FOG_PROFILING_NAME  = nameof(AVLRenderingPass) + " - Global Fog";
        
        private readonly Shader _shader;
        private readonly Material _passMaterial;

        private readonly RenderTargetIdentifier _bufferRTI;
        private readonly RenderTargetIdentifier _fogRTI;

        public AVLRenderingPass(IRenderingContext context) : base(context)
        {
            this.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;

            _shader = context.Settings.RenderingShader;
            Debug.Assert(_shader, $"[AVL] Rendering shader is missing in the feature settings. Please assign the shader.");
            
            if (_shader == null)
            {
                return;
            }
            
            _passMaterial = CoreUtils.CreateEngineMaterial(_shader);

            _bufferRTI = new RenderTargetIdentifier(ShaderConstants._AVLBufferTexture);
            _fogRTI = new RenderTargetIdentifier(ShaderConstants._AVLFogTexture);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
            
            ConfigureTarget(RenderingContext.CameraColorTarget, RenderingContext.CameraDepthTarget);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_shader == null || _passMaterial == null)
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
            var camera = cameraData.camera;
            
            var commandBuffer = CommandBufferPool.Get();
            
            var dsResX = (int)(cameraData.camera.scaledPixelWidth * RenderingContext.Settings.renderingQuality);
            var dsResY = (int)(cameraData.camera.scaledPixelHeight * RenderingContext.Settings.renderingQuality);
            
            var bufferDSDescriptor = new RenderTextureDescriptor(dsResX, dsResY)
            {
                graphicsFormat = RenderingContext.Settings.enableHDR ? GraphicsFormat.B10G11R11_UFloatPack32 : GraphicsFormat.B8G8R8A8_SRGB, // ToDo
                msaaSamples = 1,
                useMipMap = false
            };
            
            var bufferFSDescriptor = new RenderTextureDescriptor(cameraData.camera.scaledPixelWidth, cameraData.camera.scaledPixelHeight)
            {
                graphicsFormat = RenderingContext.Settings.enableHDR ? GraphicsFormat.B10G11R11_UFloatPack32 : GraphicsFormat.B8G8R8A8_SRGB, // ToDo
                msaaSamples = 1,
                useMipMap = false
            };

            commandBuffer.GetTemporaryRT(ShaderConstants._AVLFogTexture, bufferFSDescriptor);
            commandBuffer.GetTemporaryRT(ShaderConstants._AVLBufferTexture, bufferDSDescriptor);

            SetupMaterial(cameraData, component);

            // Global Fog
            commandBuffer.name = FOG_PROFILING_NAME;
            
            commandBuffer.Blit(null, RenderingContext.CameraColorTarget, _passMaterial, SP_FOG_PASS);
            Profiler.BeginSample(FOG_PROFILING_NAME);
            context.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
            Profiler.EndSample();
            
            // Render AVL
            commandBuffer.name = RENDERING_PROFILING_NAME;
            
            commandBuffer.Blit(null, _bufferRTI, _passMaterial, SP_RENDER_PASS);
            
            Profiler.BeginSample(RENDERING_PROFILING_NAME);
            context.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
            Profiler.EndSample();

            commandBuffer.name = UPSCALING_PROFILING_NAME;
            
            // Upscale AVL
            if (RenderingContext.Settings.enableUpscaling)
                commandBuffer.Blit(_bufferRTI, _fogRTI, _passMaterial, SP_UPSCALE_PASS);
            else
                commandBuffer.Blit(_bufferRTI, _fogRTI);
            
            Profiler.BeginSample(UPSCALING_PROFILING_NAME);
            context.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
            Profiler.EndSample();
            
            commandBuffer.name = BLENDING_PROFILING_NAME;
            
            // Blend AVL
            commandBuffer.Blit(_fogRTI, RenderingContext.CameraColorTarget, _passMaterial, SP_BLEND_PASS);
            
            Profiler.BeginSample(BLENDING_PROFILING_NAME);
            context.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
            Profiler.EndSample();

            CommandBufferPool.Release(commandBuffer);
        }

        public override void Cleanup() // ToDo
        {
            CoreUtils.Destroy(_passMaterial);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            // base.FrameCleanup(cmd);
            
            cmd.ReleaseTemporaryRT(ShaderConstants._AVLFogTexture);
            cmd.ReleaseTemporaryRT(ShaderConstants._AVLBufferTexture);
        }

        // Once per camera every frame
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // base.OnCameraCleanup(cmd);
            
            
        }

        private void SetupMaterial(CameraData cameraData, AVLVolumeComponent component)
        {
            var dsResX = (int)(cameraData.camera.scaledPixelWidth * RenderingContext.Settings.renderingQuality);
            var dsResY = (int)(cameraData.camera.scaledPixelHeight * RenderingContext.Settings.renderingQuality);
            
            var clusterSize = new Vector4(RenderingContext.Settings.cullingClusterSizeX, RenderingContext.Settings.cullingClusterSizeY);
            clusterSize.z = 1.0f / clusterSize.x;
            clusterSize.w = 1.0f / clusterSize.y;
            
            _passMaterial.SetFloat(ShaderConstants._AirDensity, component.airDensity.value);
            _passMaterial.SetFloat(ShaderConstants._FogDensityGlobal, component.fogDensityGlobal.value);
            _passMaterial.SetFloat(ShaderConstants._FogDensityPerLight, component.fogDensityPerLightModifier.value);
            _passMaterial.SetFloat(ShaderConstants._FogScattering, component.fogScattering.value);
            _passMaterial.SetColor(ShaderConstants._FogColor, component.fogColor.value);
            _passMaterial.SetVector(ShaderConstants._RenderingResolution, new Vector4(dsResX, dsResY, 0, 0));
            _passMaterial.SetVector(ShaderConstants._ClusterSize, clusterSize);
            _passMaterial.SetVector(ShaderConstants._CameraWorldSpacePosition, cameraData.camera.transform.position);
            _passMaterial.SetMatrix(ShaderConstants._CameraNearFrustumMatrix, FrustumCorners(cameraData.camera));
            _passMaterial.SetMatrix(ShaderConstants._CameraToWorldMatrix, cameraData.camera.cameraToWorldMatrix);
            _passMaterial.SetBuffer(ShaderConstants._GlobalLightBuffer, RenderingContext.GlobalLightBuffer);
            _passMaterial.SetInt(ShaderConstants._GlobalLightBufferCount, (int)RenderingContext.GlobalLightBufferSize);
            _passMaterial.SetBuffer(ShaderConstants._GlobalMaskBuffer, RenderingContext.GlobalMaskBuffer);
            _passMaterial.SetBuffer(ShaderConstants._LightClusterBuffer, RenderingContext.LightClusterBuffer);
            _passMaterial.SetBuffer(ShaderConstants._LightIndexBuffer, RenderingContext.LightIndexBuffer);
            _passMaterial.SetBuffer(ShaderConstants._MaskIndexBuffer, RenderingContext.MaskIndexBuffer);
            _passMaterial.SetBuffer(ShaderConstants._MaskInverseIndexBuffer, RenderingContext.MaskInverseIndexBuffer);
            _passMaterial.SetInt(ShaderConstants._GlobalMaskBufferMaxSize, RenderingContext.Settings.maxMasks);
        }
        
        private Matrix4x4 FrustumCorners(Camera cam)
        {
            var t = cam.transform;

            var frustumCorners = new Vector3[4];
            cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1),
                cam.nearClipPlane, cam.stereoActiveEye, frustumCorners);

            var frustumVectorsArray = Matrix4x4.identity;

            frustumVectorsArray.SetRow(0, t.TransformPoint(frustumCorners[0]));
            frustumVectorsArray.SetRow(1, t.TransformPoint(frustumCorners[3]));
            frustumVectorsArray.SetRow(2, t.TransformPoint(frustumCorners[1]));
            frustumVectorsArray.SetRow(3, t.TransformPoint(frustumCorners[2]));

            return frustumVectorsArray;
        }
    }
}