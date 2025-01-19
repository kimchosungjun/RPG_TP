using System;
using sapra.InfiniteLands.NaughtyAttributes;
using UnityEngine;

namespace sapra.InfiniteLands
{
    [Serializable]
    public struct MeshSettings
    {
        public enum MeshType
        {
            Normal,
            Decimated
        };

        public enum GenerationMode
        {
            RelativeToWorld,
            RelativeToTerrain
        };

        public int Seed;
        [Range(1, 254)] public int Resolution;
        public bool CustomTextureResolution;
        [AllowNesting][SerializeField][ShowIf("CustomTextureResolution")] private int _textureResolution;
        public int TextureResolution => CustomTextureResolution ? _textureResolution : Resolution;

        [Min(10)] public float MeshScale;
        public MeshType meshType;
        public GenerationMode generationMode;
        [AllowNesting][ShowIf(nameof(isDecimated))][Min(1)] public int CoreGridSpacing;
        public int coreGridSpacing => Mathf.CeilToInt(Resolution/(float)Mathf.CeilToInt(Resolution/(float)CoreGridSpacing));
        [AllowNesting][ShowIf(nameof(isDecimated))][Range(0,1)]public float NormalReduceThreshold;
        public float ratio => Resolution/MeshScale;
        public static MeshSettings Default => new MeshSettings
        {
            Seed = 0,
            Resolution = 254,
            _textureResolution = 254,
            CustomTextureResolution = false,
            MeshScale = 1000,
            CoreGridSpacing = 10,
            NormalReduceThreshold = 10,
            meshType = MeshType.Normal,
            generationMode = GenerationMode.RelativeToTerrain,
        };
        private bool isDecimated => meshType == MeshType.Decimated;
    }
}