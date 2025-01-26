using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AkiDevCat.AVL.Components;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AkiDevCat.AVL.Rendering
{
    public class AVLClusteringPass : AVLFeaturePass
    {
        internal static class ShaderConstants
        {
            public static readonly int _ProjectionParams = Shader.PropertyToID("_ProjectionParams");
            public static readonly int _ClusterSize = Shader.PropertyToID("_ClusterSize");
            public static readonly int _InvProjectionMatrix = Shader.PropertyToID("_InvProjectionMatrix");
            public static readonly int _GlobalLightBuffer = Shader.PropertyToID("_GlobalLightBuffer");
            public static readonly int _GlobalLightBufferSize = Shader.PropertyToID("_GlobalLightBufferSize");
            public static readonly int _GlobalMaskBuffer = Shader.PropertyToID("_GlobalMaskBuffer");
            public static readonly int _GlobalMaskBufferSize = Shader.PropertyToID("_GlobalMaskBufferSize");
            public static readonly int _LightClusterBuffer = Shader.PropertyToID("_LightClusterBuffer");
            public static readonly int _CulledLightsCountBuffer = Shader.PropertyToID("_CulledLightsCountBuffer");
            public static readonly int _LightIndexCount = Shader.PropertyToID("_LightIndexCount");
            public static readonly int _LightIndexBuffer = Shader.PropertyToID("_LightIndexBuffer");
            public static readonly int _MaskIndexBuffer = Shader.PropertyToID("_MaskIndexBuffer");
            public static readonly int _NearFrustumPlane = Shader.PropertyToID("_NearFrustumPlane");
            public static readonly int _FarFrustumPlane = Shader.PropertyToID("_FarFrustumPlane");
            public static readonly int _ViewMatrix = Shader.PropertyToID("_ViewMatrix");
            public static readonly int _RenderingResolution = Shader.PropertyToID("_RenderingResolution");
            public static readonly int _LightClusterDepthTexture = Shader.PropertyToID("_LightClusterDepthTexture");
            public static readonly int _CameraDepthTexture = Shader.PropertyToID("_CameraDepthTexture");
            public static readonly int _RenderingQuality = Shader.PropertyToID("_RenderingQuality");
            public static readonly int _MaskInverseIndexBuffer = Shader.PropertyToID("_MaskInverseIndexBuffer");
            public static readonly int _GlobalMaskBufferMaxSize = Shader.PropertyToID("_GlobalMaskBufferMaxSize");
        }
        
        private const string CLUSTERING_PROFILING_NAME = nameof(AVLClusteringPass) + " - Clustering";
        private const string DEPTH_ALIGNMENT_PROFILING_NAME = nameof(AVLClusteringPass) + " - Depth Alignment";
        private const string LIGHT_LOOP_PROFILING_NAME = nameof(AVLClusteringPass) + " - Light Loop";
        private const string CULLING_LIGHTS_PROFILING_NAME = nameof(AVLClusteringPass) + " - Culling Lights";
        private const string CULLING_MASKS_PROFILING_NAME = nameof(AVLClusteringPass) + " - Culling Masks";

        private const string DEPTH_CULLING_KEYWORD = "DEPTH_CULLING_ON";

        private readonly GraphicsBuffer _GlobalLightBuffer;
        private readonly GraphicsBuffer _GlobalMaskBuffer;
        private readonly GraphicsBuffer _LightIndexBuffer;
        private readonly GraphicsBuffer _MaskIndexBuffer;
        private readonly GraphicsBuffer _LightClusterBuffer;
        private readonly GraphicsBuffer _CulledLightsCountBuffer;
        private readonly GraphicsBuffer _MaskInverseIndexBuffer;

        private readonly List<LightData> _globalLightList;
        private readonly List<MaskData> _globalMaskList;
        
        private readonly RenderTargetIdentifier _lightClusterDepthRTI;

        private readonly ComputeShader _shader;
        private readonly int _clusterFrustumKernel;
        private readonly int _depthAlignmentKernel;
        private readonly int _cullLightsKernel;
        private readonly int _cullMasksKernel;

        public AVLClusteringPass(IRenderingContext renderingContext) : base(renderingContext)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingSkybox;

            _shader = renderingContext.Settings.ClusteringShader;
            Debug.Assert(_shader, $"[AVL] Clustering shader is missing in the feature settings. Please assign the shader.");
            
            if (_shader == null)
            {
                return;
            }

            _clusterFrustumKernel = _shader.FindKernel("ClusterFrustumMain");
            _depthAlignmentKernel = _shader.FindKernel("DepthAlignmentMain");
            _cullLightsKernel = _shader.FindKernel("CullLightsMain");
            _cullMasksKernel = _shader.FindKernel("CullMasksMain");

            _globalLightList = new List<LightData>();
            _globalMaskList = new List<MaskData>();
            RenderingContext.GlobalLightList = _globalLightList;
            RenderingContext.GlobalMaskList = _globalMaskList;

            _lightClusterDepthRTI = new RenderTargetIdentifier(ShaderConstants._LightClusterDepthTexture);
            
            _GlobalLightBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured,
                RenderingContext.Settings.maxLights,
                Marshal.SizeOf<LightData>());
            RenderingContext.GlobalLightBuffer = _GlobalLightBuffer;
            
            _LightIndexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured,
                // RenderingContext.Settings.maxLights * (int)AVLConstants.MAX_LIGHT_PER_CLUSTER,
                renderingContext.Settings.cullingClusterSizeX * renderingContext.Settings.cullingClusterSizeY * (int)AVLConstants.MAX_LIGHT_PER_CLUSTER,
                Marshal.SizeOf<uint>());
            RenderingContext.LightIndexBuffer = _LightIndexBuffer;
            
            _LightClusterBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured,
                renderingContext.Settings.cullingClusterSizeX * renderingContext.Settings.cullingClusterSizeY,
                Marshal.SizeOf<LightClusterData>());
            RenderingContext.LightClusterBuffer = _LightClusterBuffer;
            
            _GlobalMaskBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured,
                RenderingContext.Settings.maxMasks,
                Marshal.SizeOf<MaskData>());
            RenderingContext.GlobalMaskBuffer = _GlobalMaskBuffer;
            
            _MaskIndexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured,
                renderingContext.Settings.cullingClusterSizeX * renderingContext.Settings.cullingClusterSizeY * (int)AVLConstants.MAX_MASKS_PER_CLUSTER,
                Marshal.SizeOf<uint>());
            RenderingContext.MaskIndexBuffer = _MaskIndexBuffer;
            
            _MaskInverseIndexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured,
                renderingContext.Settings.cullingClusterSizeX * renderingContext.Settings.cullingClusterSizeY * RenderingContext.Settings.maxMasks,
                Marshal.SizeOf<int>());
            RenderingContext.MaskInverseIndexBuffer = _MaskInverseIndexBuffer;

            _CulledLightsCountBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 1, sizeof(uint)); // ToDo remove?
        }
        
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
            
            ConfigureTarget(RenderingContext.CameraColorTarget, RenderingContext.CameraDepthTarget);
        }
        
        private void SetupDepthAlignmentKernel(CameraData cameraData)
        {
            var far = cameraData.camera.farClipPlane;
            var near = cameraData.camera.nearClipPlane;
            
            var projectionParams = new Vector4(1.0f, near, far, 1.0f / far);
            
            var zBufferParams = new Vector4(-1.0f + far / near, 1.0f, 0.0f, 1.0f / far);
            zBufferParams.z = zBufferParams.x / far;

            var clusterSize = new Vector4(RenderingContext.Settings.cullingClusterSizeX, RenderingContext.Settings.cullingClusterSizeY);
            clusterSize.z = 1.0f / clusterSize.x;
            clusterSize.w = 1.0f / clusterSize.y;
            
            var dsResX = (int)(cameraData.camera.scaledPixelWidth * RenderingContext.Settings.renderingQuality);
            var dsResY = (int)(cameraData.camera.scaledPixelHeight * RenderingContext.Settings.renderingQuality);
            
            _shader.SetFloat(ShaderConstants._RenderingQuality, RenderingContext.Settings.renderingQuality);
            _shader.SetVector(ShaderConstants._RenderingResolution, new Vector4(dsResX, dsResY, 0, 0));
            _shader.SetVector(ShaderConstants._ClusterSize, clusterSize);
            _shader.SetMatrix(ShaderConstants._InvProjectionMatrix, cameraData.GetProjectionMatrix().inverse); // ToDo heavy calculation, double
            _shader.SetVector(ShaderConstants._ProjectionParams, projectionParams);
        }
        
        private void SetupClusterFrustumKernel(CameraData cameraData)
        {
            var far = cameraData.camera.farClipPlane;
            var near = cameraData.camera.nearClipPlane;
            
            var projectionParams = new Vector4(1.0f, near, far, 1.0f / far);
            
            var zBufferParams = new Vector4(-1.0f + far / near, 1.0f, 0.0f, 1.0f / far);
            zBufferParams.z = zBufferParams.x / far;

            var clusterSize = new Vector4(RenderingContext.Settings.cullingClusterSizeX, RenderingContext.Settings.cullingClusterSizeY);
            clusterSize.z = 1.0f / clusterSize.x;
            clusterSize.w = 1.0f / clusterSize.y;
            
            _shader.SetVector(ShaderConstants._ProjectionParams, projectionParams);
            _shader.SetVector(ShaderConstants._ClusterSize, clusterSize);
            _shader.SetMatrix(ShaderConstants._InvProjectionMatrix, cameraData.GetProjectionMatrix().inverse); // ToDo heavy calculation
            _shader.SetBuffer(_clusterFrustumKernel, ShaderConstants._LightClusterBuffer, _LightClusterBuffer);
            _shader.SetBuffer(_clusterFrustumKernel, ShaderConstants._LightIndexBuffer, _LightIndexBuffer);
            // For some reasons, Unity requires to create LocalKeyword object every frame
            _shader.SetKeyword(new LocalKeyword(_shader, DEPTH_CULLING_KEYWORD), RenderingContext.Settings.enableDepthCulling);
        }
        
        private void SetupCullLightsKernel(CameraData cameraData)
        {
            var camera = cameraData.camera;
            
            var frustumCorners = new Vector3[4];
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), 
                camera.nearClipPlane, camera.stereoActiveEye, frustumCorners);

            Vector4 nearFrustumPlane = Vector3.Normalize(Vector3.Cross(
                frustumCorners[0] - frustumCorners[1], 
                frustumCorners[2] - frustumCorners[1]));
            nearFrustumPlane = -nearFrustumPlane;
            nearFrustumPlane.w = Vector3.Dot(frustumCorners[0], nearFrustumPlane);
            
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), 
                camera.farClipPlane, camera.stereoActiveEye, frustumCorners);
            
            Vector4 farFrustumPlane = Vector3.Normalize(Vector3.Cross(
                frustumCorners[0] - frustumCorners[1], 
                frustumCorners[2] - frustumCorners[1]));
            farFrustumPlane.w = Vector3.Dot(frustumCorners[0], farFrustumPlane);
            
            _CulledLightsCountBuffer.SetData(new List<uint> { 0 });

            _shader.SetInt(ShaderConstants._GlobalLightBufferSize, (int)RenderingContext.GlobalLightBufferSize); // ToDo
            // _shader.SetInt(ShaderConstants._LightIndexCount, 0);
            _shader.SetVector(ShaderConstants._NearFrustumPlane, nearFrustumPlane);
            _shader.SetVector(ShaderConstants._FarFrustumPlane, farFrustumPlane);
            _shader.SetMatrix(ShaderConstants._ViewMatrix, cameraData.GetViewMatrix());
            _shader.SetBuffer(_cullLightsKernel, ShaderConstants._GlobalLightBuffer, RenderingContext.GlobalLightBuffer);
            _shader.SetBuffer(_cullLightsKernel, ShaderConstants._LightClusterBuffer, RenderingContext.LightClusterBuffer);
            _shader.SetBuffer(_cullLightsKernel, ShaderConstants._LightIndexBuffer, RenderingContext.LightIndexBuffer);
            // _shader.SetBuffer(_cullLightsKernel, ShaderConstants._CulledLightsCountBuffer, _CulledLightsCountBuffer);
        }
        
        private void SetupCullMasksKernel(CameraData cameraData)
        {
            var camera = cameraData.camera;
            
            var frustumCorners = new Vector3[4];
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), 
                camera.nearClipPlane, camera.stereoActiveEye, frustumCorners);

            Vector4 nearFrustumPlane = Vector3.Normalize(Vector3.Cross(
                frustumCorners[0] - frustumCorners[1], 
                frustumCorners[2] - frustumCorners[1]));
            nearFrustumPlane = -nearFrustumPlane;
            nearFrustumPlane.w = Vector3.Dot(frustumCorners[0], nearFrustumPlane);
            
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), 
                camera.farClipPlane, camera.stereoActiveEye, frustumCorners);
            
            Vector4 farFrustumPlane = Vector3.Normalize(Vector3.Cross(
                frustumCorners[0] - frustumCorners[1], 
                frustumCorners[2] - frustumCorners[1]));
            farFrustumPlane.w = Vector3.Dot(frustumCorners[0], farFrustumPlane);
            
            _CulledLightsCountBuffer.SetData(new List<uint> { 0 });

            _shader.SetInt(ShaderConstants._GlobalMaskBufferSize, (int)RenderingContext.GlobalMaskBufferSize); // ToDo
            _shader.SetVector(ShaderConstants._NearFrustumPlane, nearFrustumPlane);
            _shader.SetVector(ShaderConstants._FarFrustumPlane, farFrustumPlane);
            _shader.SetMatrix(ShaderConstants._ViewMatrix, cameraData.GetViewMatrix());
            _shader.SetBuffer(_cullMasksKernel, ShaderConstants._GlobalMaskBuffer, RenderingContext.GlobalMaskBuffer);
            _shader.SetInt(ShaderConstants._GlobalMaskBufferMaxSize, RenderingContext.Settings.maxMasks);
            _shader.SetBuffer(_cullMasksKernel, ShaderConstants._LightClusterBuffer, RenderingContext.LightClusterBuffer);
            _shader.SetBuffer(_cullMasksKernel, ShaderConstants._MaskIndexBuffer, RenderingContext.MaskIndexBuffer);
            _shader.SetBuffer(_cullMasksKernel, ShaderConstants._MaskInverseIndexBuffer, RenderingContext.MaskInverseIndexBuffer);
            // _shader.SetBuffer(_cullLightsKernel, ShaderConstants._CulledLightsCountBuffer, _CulledLightsCountBuffer);
        }
        
        private void ExecuteDepthAlignmentKernel(CommandBuffer cmd, CameraData cameraData)
        {
            var dsResX = (int)(cameraData.camera.scaledPixelWidth * RenderingContext.Settings.renderingQuality);
            var dsResY = (int)(cameraData.camera.scaledPixelHeight * RenderingContext.Settings.renderingQuality);
            
            cmd.DispatchCompute(_shader, _depthAlignmentKernel, 
                // Mathf.CeilToInt(dsResX / 4.0f), 
                // Mathf.CeilToInt(dsResY / 4.0f), 
                Mathf.CeilToInt(RenderingContext.Settings.cullingClusterSizeX / 4.0f), 
                Mathf.CeilToInt(RenderingContext.Settings.cullingClusterSizeY / 4.0f), 
                1);
        }
        
        private void ExecuteClusterFrustumKernel(CommandBuffer cmd)
        {
            cmd.DispatchCompute(_shader, _clusterFrustumKernel, 
                Mathf.CeilToInt(RenderingContext.Settings.cullingClusterSizeX / 4.0f), 
                Mathf.CeilToInt(RenderingContext.Settings.cullingClusterSizeY / 4.0f), 
                1);
        }

        private void ExecuteCullLightsKernel(CommandBuffer cmd)
        {
            cmd.DispatchCompute(_shader, _cullLightsKernel, 
                RenderingContext.Settings.cullingClusterSizeX, 
                RenderingContext.Settings.cullingClusterSizeY, 
                1);
        }
        
        private void ExecuteCullMasksKernel(CommandBuffer cmd)
        {
            cmd.DispatchCompute(_shader, _cullMasksKernel, 
                RenderingContext.Settings.cullingClusterSizeX, 
                RenderingContext.Settings.cullingClusterSizeY, 
                1);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_shader == null)
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
            
            var commandBuffer = CommandBufferPool.Get(nameof(AVLClusteringPass));
            
            var dsResX = (int)(cameraData.camera.scaledPixelWidth * RenderingContext.Settings.renderingQuality);
            var dsResY = (int)(cameraData.camera.scaledPixelHeight * RenderingContext.Settings.renderingQuality);
            
            var bufferDSDescriptor = new RenderTextureDescriptor(dsResX, dsResY)
            {
                graphicsFormat = GraphicsFormat.R32_UInt,
                msaaSamples = 1,
                useMipMap = false,
                enableRandomWrite = true
            };

            /*
             * Align cluster frustums' far Z plane
             */

            if (RenderingContext.Settings.enableDepthCulling)
            {
                commandBuffer.GetTemporaryRT(ShaderConstants._LightClusterDepthTexture, bufferDSDescriptor);
                commandBuffer.SetRenderTarget(_lightClusterDepthRTI);
                commandBuffer.ClearRenderTarget(false, true, Color.black);

                SetupDepthAlignmentKernel(cameraData);
                ExecuteDepthAlignmentKernel(commandBuffer, cameraData);

                commandBuffer.name = DEPTH_ALIGNMENT_PROFILING_NAME;

                Profiler.BeginSample(DEPTH_ALIGNMENT_PROFILING_NAME);
                context.ExecuteCommandBuffer(commandBuffer);
                commandBuffer.Clear();
                Profiler.EndSample();
            }

            /*
             * Reconstruct cluster frustums
             * This seems to be very inexpensive, so we'll run it every frame
             * Theoretically, this can be called once the camera updates its projection matrix
             */
            
            SetupClusterFrustumKernel(cameraData);
            ExecuteClusterFrustumKernel(commandBuffer);

            commandBuffer.name = CLUSTERING_PROFILING_NAME;
            
            Profiler.BeginSample(CLUSTERING_PROFILING_NAME);
            context.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
            Profiler.EndSample();

            /*
             * Setup global light list
             */
            
            Profiler.BeginSample(LIGHT_LOOP_PROFILING_NAME);
            SetupGlobalLightList(camera);
            Profiler.EndSample();

            /*
             * Conduct the actual light culling
             */

            SetupCullLightsKernel(cameraData);
            ExecuteCullLightsKernel(commandBuffer);
            
            commandBuffer.name = CULLING_LIGHTS_PROFILING_NAME;
            
            Profiler.BeginSample(CULLING_LIGHTS_PROFILING_NAME);
            context.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
            Profiler.EndSample();

            SetupCullMasksKernel(cameraData);
            ExecuteCullMasksKernel(commandBuffer);
            
            commandBuffer.name = CULLING_MASKS_PROFILING_NAME;
            
            Profiler.BeginSample(CULLING_MASKS_PROFILING_NAME);
            context.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
            Profiler.EndSample();

            CommandBufferPool.Release(commandBuffer);
        }

        private void SetupGlobalLightList(Camera camera) // ToDo review
        {
            _globalLightList.Clear();
            _globalMaskList.Clear();

            // InstanceID, Reference, LocalID
            Dictionary<int, (VolumetricLightMask, int)> masksRegistry = new();

            var maskIdMax = -1;

            var camPos = camera.transform.position;

            foreach (var (instanceID, light) in GlobalLightManager.AsEnumerable())
            {
                if (!light.LightEnabled)
                    continue;
                
                if (_globalLightList.Count >= RenderingContext.Settings.maxLights)
                    continue;
                
                // Update light cache for this frame if required
                light.UpdateLightCache();

                var cullingFading = 0.0f;
                var maskId = -1;

                if (light.distanceCullingEnabled)
                {
                    var camDistanceSqr = (light.TransformCached.position - camPos).sqrMagnitude;
                    var lightHardCullingSqr = light.hardCullingDistance * light.hardCullingDistance;

                    if (camDistanceSqr > lightHardCullingSqr)
                    {
                        continue;
                    }

                    var lightSoftCullingSqr = light.softCullingDistance * light.softCullingDistance;
                    cullingFading = Mathf.Clamp01((camDistanceSqr - lightSoftCullingSqr) /
                                                  (lightHardCullingSqr - lightSoftCullingSqr));
                }

                if (light.lightMask != null && light.lightMask.isActiveAndEnabled)
                {
                    var maskInstanceID = light.lightMask.GetInstanceID();
                    if (masksRegistry.TryGetValue(maskInstanceID, out var foundMask))
                    {
                        maskId = foundMask.Item2;
                    }
                    else
                    {
                        var md = new MaskData
                        {
                            Type = (uint) light.lightMask.shape,
                            WorldToMask = light.lightMask.worldToMaskMatrix,
                            BoundingRadius = light.lightMask.BoundingRadius,
                            Origin = light.lightMask.transform.position
                        };
                        _globalMaskList.Add(md);
                        masksRegistry.Add(maskInstanceID, (light.lightMask, ++maskIdMax));
                        maskId = maskIdMax;
                    }
                }

                var ld = new LightData
                {
                    Type = (uint) light.lightShape,
                    Origin = light.TransformCached.position,
                    Right = light.TransformRightCached,
                    Up = light.TransformUpCached,
                    Forward = light.TransformForwardCached,
                    BoundingOrigin = light.BoundingOriginCached,
                    BoundingRadius = light.BoundingRadiusCached,
                    Color = light.LightVisibleColorCached,
                    SecondaryAngle = light.SecondaryAngleRadCached,
                    PrimaryAngle = light.PrimaryAngleRadCached,
                    Scattering = light.LightScatteringSqrCached,
                    CullingFading = cullingFading,
                    MaskID = maskId,
                    Range = light.lightRange,
                    Rect = light.lightRect
                };

                _globalLightList.Add(ld);
            }
            
            RenderingContext.GlobalLightBuffer.SetData(_globalLightList);
            RenderingContext.GlobalLightBufferSize = (uint)_globalLightList.Count;
            RenderingContext.GlobalMaskBuffer.SetData(_globalMaskList);
            RenderingContext.GlobalMaskBufferSize = (uint)_globalMaskList.Count;
        }
        
        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
            
            cmd.ReleaseTemporaryRT(ShaderConstants._LightClusterDepthTexture);
            
            _globalLightList.Clear();
            _globalMaskList.Clear();
        }

        public override void Cleanup()
        {
            _globalLightList?.Clear();
            RenderingContext.GlobalLightList = null;
            
            _globalMaskList?.Clear();
            RenderingContext.GlobalMaskList = null;
            
            _GlobalLightBuffer?.Release();
            RenderingContext.GlobalLightBuffer = null;
            
            _LightIndexBuffer?.Release();
            RenderingContext.LightIndexBuffer = null;
            
            _MaskIndexBuffer?.Release();
            RenderingContext.MaskIndexBuffer = null;
            
            _MaskInverseIndexBuffer?.Release();
            RenderingContext.MaskInverseIndexBuffer = null;
            
            _GlobalMaskBuffer?.Release();
            RenderingContext.GlobalMaskBuffer = null;
            
            _CulledLightsCountBuffer?.Release();

            _LightClusterBuffer?.Release();
            RenderingContext.LightClusterBuffer = null; // ToDo I don't like that
        }
    }
}