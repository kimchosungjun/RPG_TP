using System.Collections.Generic;

namespace sapra.InfiniteLands
{
    // Asset is provided that can be later retrieved
    public interface ILoadAsset
    {
        public NoiseOutput ProcessAsset(IAsset asset, GenerationSettings settings);
        public bool AssetExists(IAsset asset);
        public void SetAsset(IAsset asset);
        public IEnumerable<IAsset> GetAssets();
    }
}