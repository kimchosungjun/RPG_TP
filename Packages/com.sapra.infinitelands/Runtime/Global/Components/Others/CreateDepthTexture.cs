using sapra.InfiniteLands.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace sapra.InfiniteLands
{
    [RequireComponent(typeof(Camera))]
    public class CreateDepthTexture : MonoBehaviour
    {
        public static readonly int
            depthTextureID = Shader.PropertyToID("depthTexture"),
            rtSizeID = Shader.PropertyToID("_RTSize"),
            maxMipLevelsID = Shader.PropertyToID("_MaxMIPLevel");

        private static readonly int
            inputRT = Shader.PropertyToID("inputRT"),
            outputRT = Shader.PropertyToID("outputRT");

        static int textureResolution = 2048;
        static int MipLevels = 12;
        [ReadOnly] public RenderTexture DepthTexture;
        ComputeShader DownscalerShader;
        private CommandBuffer commandBuffer;

        [Header("Debug")] [SerializeField] private bool Override;
        private Camera Camera;

        private void OnEnable()
        {
            if(GraphicsSettings.defaultRenderPipeline != null)
                return;
            Camera = GetComponent<Camera>();
            DownscalerShader = (ComputeShader)Resources.Load("Computes/Downscaler");
            Camera.depthTextureMode = Camera.depthTextureMode | DepthTextureMode.Depth;

            Shader.SetGlobalInt(rtSizeID, textureResolution);
            Shader.SetGlobalInt(maxMipLevelsID, MipLevels - 1);

            GenerateDepthTexture();
        }

        private void OnDisable()
        {
            if(GraphicsSettings.defaultRenderPipeline != null)
                return;
            ReleaseDepthTexture();
        }

        private void ReleaseDepthTexture()
        {
            if (DepthTexture != null)
                DepthTexture.Release();
            if (commandBuffer != null)
                Camera.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, commandBuffer);
            commandBuffer = null;
        }

        private void GenerateDepthTexture()
        {
            RenderTextureDescriptor descriptor = new RenderTextureDescriptor(textureResolution,
                textureResolution, RenderTextureFormat.RHalf, 16, MipLevels)
            {
                useMipMap = true,
                autoGenerateMips = false,
                enableRandomWrite = true,
 
            };

            DepthTexture = new RenderTexture(descriptor);
            DepthTexture.Create();

            commandBuffer = new CommandBuffer()
            {
                name = "Copying Depth Buffer"
            };

            commandBuffer.Blit(BuiltinRenderTextureType.Depth, DepthTexture);

            int res = (int)textureResolution;
            for (int i = 1; i < MipLevels; i++)
            {
                res /= 2;
                res = Mathf.Max(res, 1);

                int tempName = Shader.PropertyToID("_tempRT" + i);

                commandBuffer.GetTemporaryRT(tempName, res, res, 0,
                    FilterMode.Point,
                    RenderTextureFormat.RHalf,
                    RenderTextureReadWrite.Default,
                    1,
                    true);
                commandBuffer.SetComputeFloatParam(DownscalerShader, "RTSize", res);
                commandBuffer.SetComputeTextureParam(DownscalerShader, 0, inputRT, DepthTexture, i - 1);
                commandBuffer.SetComputeTextureParam(DownscalerShader, 0, outputRT, tempName);

                int groupSize = Mathf.CeilToInt(res / 8f);
                groupSize = Mathf.Max(groupSize, 1);

                commandBuffer.DispatchCompute(DownscalerShader, 0, groupSize, groupSize, 1);
                commandBuffer.CopyTexture(tempName, 0, 0, DepthTexture, 0, i);
                commandBuffer.ReleaseTemporaryRT(tempName);
            }

            Camera.AddCommandBuffer(CameraEvent.AfterDepthTexture, commandBuffer);
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (Override)
                Graphics.Blit(DepthTexture, dest);
            else
                Graphics.Blit(src, dest);
        }
    }
}