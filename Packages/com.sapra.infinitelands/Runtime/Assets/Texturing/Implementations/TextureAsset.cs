using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace sapra.InfiniteLands{  
    [AssetNodeAttribute(typeof(TextureOutputNode))]
    [CreateAssetMenu(menuName = "InfiniteLands/Assets/Texture")]
    public class TextureAsset : UpdateableSO, IHoldTextures
    {
        public Texture2D Albedo;
        public Texture2D HeightMap;
        public Texture2D NormalMap;
        public Texture2D OcclusionMap;
        public DefaultSettings settings = DefaultSettings.Default;

        public IShaderTexture[] GetTextures() => new IShaderTexture[]{
            new DefaultTexture("_albedo_textures", Albedo, Texture2D.blackTexture), 
            new DefaultTexture("_height_textures", HeightMap, Texture2D.grayTexture), 
            new DefaultTexture("_normal_textures", NormalMap, Texture2D.normalTexture), 
            new DefaultTexture("_occlusion_textures", OcclusionMap, Texture2D.whiteTexture)
        };

        public ITextureSettings GetSettings() => settings;

        public ComputeBuffer CreateTextureCompute(IEnumerable<ITextureSettings> settingsArray)
        {
            ComputeBuffer assetSettingsBuffer = new ComputeBuffer(settingsArray.Count(),
                settings.ObjectByteSize, ComputeBufferType.Default);
            assetSettingsBuffer.SetData(settingsArray.Cast<DefaultSettings>().ToArray());
            return assetSettingsBuffer;
        }

        public object Preview() => Albedo;
    }
}