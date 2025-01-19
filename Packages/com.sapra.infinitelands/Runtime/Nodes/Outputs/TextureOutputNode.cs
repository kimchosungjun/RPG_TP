using System;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Texture Output", type = "Output", docs = "https://ensapra.com/packages/infinite_lands/nodes/texture.html")]
    public class TextureOutputNode : InfiniteLandsNode, ILoadAsset, IHavePreview, IOutput
    {
        public TextureAsset TextureItem;
        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator DensityMap;
        protected override InfiniteLandsNode[] Dependancies => new InfiniteLandsNode[]{DensityMap};
        protected override bool ExtraValidationSteps() => TextureItem != null;

        public int PrepareNode(IndexManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            if(DensityMap != null)
                return DensityMap.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid);
            return resolution;
        }

        public IEnumerable<IAsset> GetAssets() => new[]{TextureItem};
        public NoiseOutput ProcessAsset(IAsset asset, GenerationSettings settings)
        {
            HeightData heightJob = DensityMap.RequestHeight(settings);
            return new NoiseOutput
            {
                globalIndex = heightJob.indexData,
                jobHandle = heightJob.jobHandle
            };
        }

        public bool AssetExists(IAsset asset) => TextureItem.Equals(asset);
        public void SetAsset(IAsset asset) => TextureItem = (TextureAsset)asset;

        public Action<bool> OnImageUpdated { get; set; }

        [HideInInspector][SerializeField]bool showPreview = false;
        public void GenerateTexture(GenerationSettings settings, IBurstTexturePool TexturePool)
        {
            OnImageUpdated?.Invoke(true);
        }

        public object GetTemporalTexture() => TextureItem?.Albedo;
        public bool PreviewButtonEnabled() => TextureItem != null &&  TextureItem.Preview() != null;
        public bool ShowPreview() => PreviewButtonEnabled() && showPreview;
        
        public void TogglePreview(bool value, bool forcedUpdate = false)
        {
            if(value.Equals(showPreview))
                return;

            showPreview = value;
            OnImageUpdated?.Invoke(value);
        }
    }
}