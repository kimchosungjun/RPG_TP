using System.Collections.Generic;
using UnityEngine;

namespace sapra.InfiniteLands{  
    // Contains the necessary textures used for texturing
    public interface IHoldTextures : IAsset{
        public IShaderTexture[] GetTextures(); // Return all the textures
        public ITextureSettings GetSettings(); // Return the settings for the textures
        public ComputeBuffer CreateTextureCompute(IEnumerable<ITextureSettings> settings); // Creates the buffer and applies the data 
    }
}