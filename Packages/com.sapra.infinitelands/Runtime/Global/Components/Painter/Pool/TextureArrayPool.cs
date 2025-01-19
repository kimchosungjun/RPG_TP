using System;
using UnityEngine;
using UnityEngine.Pool;

namespace sapra.InfiniteLands{
    public class TextureArrayPool
    {
        int sizeX;
        int sizeY;
        int texturesLength;
        TextureFormat format;
        int mipCount;
        bool linearMode;

        bool mightVarying;
        private ObjectPool<Texture2DArray> arrayPool;

        public void Dispose(){
            if(arrayPool.CountActive > 0)
                Debug.LogErrorFormat("Not all texture arrays have been released: {0}", arrayPool.CountActive);
            arrayPool.Dispose();
        }
        public TextureArrayPool(Texture2D DefaultTexture, int textureLength, bool mightVary, Action<Texture2DArray> OnDestroy, bool _linearMode = false){
            sizeX = DefaultTexture.height;
            sizeY = DefaultTexture.width;
            mipCount = DefaultTexture.mipmapCount;
            format = DefaultTexture.format;
            texturesLength = textureLength;
            linearMode = _linearMode;
            mightVarying = mightVary;

            arrayPool = new ObjectPool<Texture2DArray>(CreateNewTexture2DArray, actionOnDestroy: OnDestroy);
        }
        
        public Texture2DArray GetConfiguredArray(string name, Texture2D[] textures){
            Texture2DArray array = arrayPool.Get();
            if(mightVarying)
                return GenerateTextureArrayVarying(array, name, textures);
            else
                return GenerateTextureArrayFixed(array, name, textures);
        }

        public void Release(Texture2DArray array){
            arrayPool.Release(array);
        }
        public Texture2DArray CreateNewTexture2DArray(){
            Texture2DArray current;
            #if UNITY_2022_1_OR_NEWER
            current = new Texture2DArray(sizeX, sizeY, texturesLength, format, mipCount, linearMode, true);
            #else
            current = new Texture2DArray(sizeX, sizeY, texturesLength, format, mipCount, linearMode);
            #endif
            return current;
        }

        private Texture2DArray GenerateTextureArrayVarying(Texture2DArray current, string name, Texture2D[] textures)
        {
            if (textures.Length != texturesLength){
                Debug.LogErrorFormat("Trying to create an array with {0} textures when it was prepared for {1}", textures.Length, texturesLength);
                return null;
            }                
            current.name = name;

            RenderTexture renderTexture = new RenderTexture(sizeX, sizeY, mipCount);
            Texture2D temporalCopy = new Texture2D(sizeX, sizeY, format, mipCount, linearMode);
            for (int i = 0; i < textures.Length; i++)
            {
                Graphics.Blit(textures[i], renderTexture);

                RenderTexture.active = renderTexture;
                temporalCopy.ReadPixels(new Rect(0, 0, sizeX, sizeY), 0, 0);
                temporalCopy.Apply();

                RenderTexture.active = null;

                Graphics.CopyTexture(temporalCopy, 0, current, i);
            }

            renderTexture.Release();
            if(Application.isPlaying)
                GameObject.Destroy(temporalCopy);
            else
                GameObject.DestroyImmediate(temporalCopy);

            current.filterMode = FilterMode.Bilinear;
            return current;
        }

        private Texture2DArray GenerateTextureArrayFixed(Texture2DArray current, string name, Texture2D[] textures)
        {
            if (textures.Length != texturesLength){
                Debug.LogErrorFormat("Trying to create an array with {0} textures when it was prepared for {1}", textures.Length, texturesLength);
                return null;
            }                
            current.name = name;

            for (int i = 0; i < textures.Length; i++)
            {
                Graphics.CopyTexture(textures[i], 0, current, i);
            }

            current.filterMode = FilterMode.Bilinear;
            return current;
        }
    }
}