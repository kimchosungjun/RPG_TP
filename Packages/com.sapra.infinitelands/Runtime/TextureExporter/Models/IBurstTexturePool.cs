using System;
using UnityEngine;

namespace sapra.InfiniteLands{
    public interface IBurstTexturePool{
        public BurstTexture GetTexture(string name, FilterMode filter, TextureFormat format = TextureFormat.RGBA32);
        public BurstTexture[] GetTexture(string[] names, FilterMode filter, TextureFormat format = TextureFormat.RGBA32);
        public void Return(BurstTexture[] texs);
        public void Return(BurstTexture tex);
        public void DestroyBurstTextures(Action<UnityEngine.Object> Destroy);

        public int GetTextureResolution();
        public int GetTextureLength();
    }
}