using UnityEngine;

namespace AkiDevCat.AVL.Rendering
{
    [System.Serializable]
    public class AVLFeatureSettings
    {
        [Header("Shaders")] 
        public ComputeShader ClusteringShader;
        public Shader RenderingShader;
        public Shader DebugShader;
        
        [Header("General Rendering")] 
        [Range(0.1f, 1.0f)] public float renderingQuality = 0.5f;
        public bool enableHDR = true;
        public int maxLights = 4096;
        public int maxMasks = 128;

        [Header("Upscaling Pass")] 
        public bool enableUpscaling = true;

        [Header("Culling Pass")] 
        public bool enableDepthCulling = true;
        [Range(1, 256)] public int cullingClusterSizeX = 16;
        [Range(1, 256)] public int cullingClusterSizeY = 9;
        
        // [Header("Transparent Pass")] 
        // public bool enableTransparentPass = false;

        [Header("Debug Settings")]
        public DebugMode debugMode = DebugMode.None;
        [Range(0.0f, 1.0f)]
        public float debugOpacity = 1.0f / 25.0f;

        public bool IsValid => ClusteringShader != null && RenderingShader != null & DebugShader != null;
    }
}