using UnityEngine;

namespace sapra.InfiniteLands{  
    public interface IShaderTexture
    {
        public string GetShaderTextureName();
        public Texture2D GetTexture2D(); 
    }
}