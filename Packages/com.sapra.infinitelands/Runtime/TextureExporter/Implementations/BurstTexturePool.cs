using System;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.InfiniteLands{
    public class BurstTexturePool : IBurstTexturePool{
        private Dictionary<TextureFormat, List<BurstTexture>> _availableTextures = new Dictionary<TextureFormat, List<BurstTexture>>();

        private List<BurstTexture> _activeTextures = new List<BurstTexture>();
        private int _textureResolution;
        private int _textureLength;
        public int GetTextureLength() => _textureLength;
        public int GetTextureResolution() => _textureResolution;
        public BurstTexturePool(int resolution){
            _textureResolution = resolution;
            _textureLength = (resolution+1)*(resolution+1);

        }
        public BurstTexture GetTexture(string name, FilterMode filter, TextureFormat format= TextureFormat.RGBA32)
        {
            if(!_availableTextures.TryGetValue(format, out List<BurstTexture> textures)){
                textures = new List<BurstTexture>();
                _availableTextures.Add(format, textures);
            }

            BurstTexture newTexture;

            if (textures.Count > 0)
            {
                newTexture = textures[0];
                newTexture.ReuseTexture(name);
                textures.RemoveAt(0);
            }
            else{
                newTexture = new BurstTexture(_textureResolution, name, filter, format);
            }
            _activeTextures.Add(newTexture);
            return newTexture;
        }


        public BurstTexture[] GetTexture(string[] names, FilterMode filter, TextureFormat format)
        {
            BurstTexture[] newTextures = new BurstTexture[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                newTextures[i] = GetTexture(names[i],filter, format);
            }

            return newTextures;
        }

        public void Return(BurstTexture tex)
        {
            if(!_availableTextures.TryGetValue(tex.format, out List<BurstTexture> textures)){
                textures = new List<BurstTexture>();
                _availableTextures.Add(tex.format, textures);
                _activeTextures.Remove(tex);
            }

            textures.Add(tex);
        }

        public void Return(BurstTexture[] texs)
        {
            if(texs == null)    
                return;
            foreach(BurstTexture texture in texs){
                Return(texture);
            }
        }

        public void DestroyBurstTextures(Action<UnityEngine.Object> Destroy)
        {
            foreach(KeyValuePair<TextureFormat, List<BurstTexture>> pair in _availableTextures){
                foreach(BurstTexture tex in pair.Value){
                    Destroy(tex.ApplyTexture());
                }
            }
            _availableTextures.Clear();

            foreach(BurstTexture texture in _activeTextures){
                Destroy(texture.ApplyTexture());
            }
            _activeTextures.Clear();
        }
    }
}