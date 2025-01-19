namespace sapra.InfiniteLands{
    //
    [System.Serializable]
    public struct DefaultSettings : ITextureSettings
    {
        public float Size;
        public float NormalStrength;

        public static DefaultSettings Default => new DefaultSettings
        {
            Size = 10,
            NormalStrength = 1,
        };

        public float TextureSize => Size;
        public int ObjectByteSize => sizeof(float) * 2;

    }
}