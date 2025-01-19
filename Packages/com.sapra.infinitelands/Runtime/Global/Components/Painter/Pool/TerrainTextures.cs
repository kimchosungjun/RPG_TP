using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace sapra.InfiniteLands
{
    public class TerrainTextures
    {
        private LocalKeyword heightMapBlending;
        public int textureCount;
        public List<Texture2DArray> TexturesAvailable;
        public ComputeBuffer TextureSettings;
        public ComputeBuffer TextureSizes;

        private TextureArrayPool pool;
        public TerrainTextures(IEnumerable<IHoldTextures> allTextures, Action<Texture2DArray> DestroyTextures)
        {
            IEnumerable<IHoldTextures> uniqueTextures = allTextures.Distinct();
/*             List<IHoldTextures> uniqueTextures = new List<IHoldTextures>();
            foreach(var texture in allTextures){
                int index = uniqueTextures.IndexOf(texture);
                if (index < 0)
                {
                    uniqueTextures.Add(texture);
                }
            } */
            
            Dictionary<string, List<Texture2D>> texturePacks = new();
            foreach(var text in uniqueTextures){
                IShaderTexture[] textures = text.GetTextures();
                foreach(IShaderTexture texture in textures){
                    string texName = texture.GetShaderTextureName();
                    Texture2D texture2D = texture.GetTexture2D();
                    if(texturePacks.TryGetValue(texName, out List<Texture2D> name))
                        name.Add(texture2D);
                    else
                        texturePacks.Add(texName, new List<Texture2D>(){texture2D});
                }
            }

           /*  Dictionary<string, List<Texture2D>> texturePacks = new();
            for(int i = 0; i < uniqueTextures.Count; i++){
                IHoldTextures asset = uniqueTextures[i];
                IShaderTexture[] textures = asset.GetTextures();
                foreach(IShaderTexture texture in textures){
                    string texName = texture.GetShaderTextureName();
                    Texture2D texture2D = texture.GetTexture2D();
                    if(texturePacks.TryGetValue(texName, out List<Texture2D> name))
                        name.Add(texture2D);
                    else
                        texturePacks.Add(texName, new List<Texture2D>(){texture2D});
                }
            } */

            TexturesAvailable = new List<Texture2DArray>();
            Texture2D defaultTexture = uniqueTextures.First().GetTextures()[0].GetTexture2D();
            
            int defaultCount = uniqueTextures.Count();

            pool = new TextureArrayPool(defaultTexture, defaultCount, true, DestroyTextures);
            foreach(KeyValuePair<string, List<Texture2D>> nameAndTexture in texturePacks){
                Texture2DArray textureArray = pool.GetConfiguredArray(nameAndTexture.Key, nameAndTexture.Value.ToArray());
                textureArray.wrapMode = TextureWrapMode.Repeat;
                TexturesAvailable.Add(textureArray);
            }

            IEnumerable<ITextureSettings> textureSettings = uniqueTextures.Select(a => a.GetSettings());
            if(textureSettings.Select(a => a.ObjectByteSize).Distinct().Count() >= 2)
                Debug.LogError("Differences of texture settings size");
            
            ComputeBuffer assetSettingsBuffer = allTextures.First().CreateTextureCompute(uniqueTextures.Select(a => a.GetSettings()));
            ComputeBuffer sizesOnly = new ComputeBuffer(allTextures.Count(),
                sizeof(float), ComputeBufferType.Default);
            sizesOnly.SetData(allTextures.Select(a => a.GetSettings().TextureSize).ToArray());

            this.TextureSettings = assetSettingsBuffer;
            this.TextureSizes = sizesOnly;
            this.textureCount = allTextures.Count();
        }

        public void ApplyTextureArrays(Material material){
            for(int i = 0; i < TexturesAvailable.Count; i++){
                Texture2DArray array = TexturesAvailable[i];
                material.SetTexture(array.name, array);
            }
            material.SetBuffer("_texture_settings", TextureSettings);
            material.EnableKeyword("_PROCEDURALTEXTURING");
        }

        public void ApplyTextureArrays(CommandBuffer bf, ComputeShader compute, int kernelIndex){
            if(heightMapBlending == default)
                heightMapBlending = new LocalKeyword(compute, "HEIGHTMAP_ENABLED");

            for(int i = 0; i < TexturesAvailable.Count; i++){
                Texture2DArray array = TexturesAvailable[i];
                bf.SetComputeTextureParam(compute, kernelIndex, array.name, array);
                
                if(array.name.Contains("height"))
                    bf.SetKeyword(compute, heightMapBlending, true);

            }
            bf.SetComputeBufferParam(compute, kernelIndex,"_textureSize", TextureSizes);
        }

        public void Release()
        {
            if (TextureSettings != null)
            {
                TextureSettings.Release();
                TextureSettings = null;
            }
            if (TextureSizes != null)
            {
                TextureSizes.Release();
                TextureSizes = null;
            }

            foreach(Texture2DArray array in TexturesAvailable){
                pool.Release(array);
            }
            pool.Dispose();
        }
    }
}