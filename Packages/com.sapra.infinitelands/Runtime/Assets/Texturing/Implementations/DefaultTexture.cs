using UnityEngine;

namespace sapra.InfiniteLands{
    public class DefaultTexture : IShaderTexture
    {
        public Texture2D texture;
        public string name;
        public Texture2D defaultTexture;

        public Texture2D GetTexture2D() => texture != null ? texture : defaultTexture;
        public string GetShaderTextureName() => name;

        public DefaultTexture(string TextureName, Texture2D texture2D, Texture2D defaultText){
            this.texture = texture2D;
            this.name = TextureName;
            this.defaultTexture = defaultText;
        }
    }
}