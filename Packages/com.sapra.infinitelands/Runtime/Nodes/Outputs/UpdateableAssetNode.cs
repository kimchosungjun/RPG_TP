/* using System;
using UnityEngine;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Asset Output", type = "Output", docs = "https://ensapra.com/packages/infinite_lands/nodes/texture.html")]
    public class UpdateableAssetNode : InfiniteLandsNode, ILoadAsset, IHavePreview, IOutput
    {
        public UpdateableSO TextureItem;
        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator DensityMap;
        public IAsset asset{get => TextureItem; set => TextureItem = (UpdateableSO)value;}
        protected override InfiniteLandsNode[] Dependancies => new InfiniteLandsNode[]{DensityMap};
        protected override bool ExtraValidationSteps() => asset != null;

        public int PrepareNode(DataManager manager, ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            if(DensityMap != null)
                return DensityMap.PrepareNode(manager, ref currentCount, resolution, ratio, requestGuid);
            return resolution;
        }

        public NoiseOutput ProcessAsset(GenerationSettings settings)
        {
            HeightData heightJob = DensityMap.RequestHeight(settings);
            return new NoiseOutput
            {
                globalIndex = heightJob.indexData,
                jobHandle = heightJob.jobHandle
            };
        }

        public Action<bool> OnImageUpdated { get; set; }

        [HideInInspector][SerializeField]bool showPreview = false;
        public void GenerateTexture(GenerationSettings settings, IBurstTexturePool TexturePool)
        {
            OnImageUpdated?.Invoke(true);
        }

        public object GetTemporalTexture() => null;

        public bool ShowPreview() => showPreview;
        public void TogglePreview(bool value, bool forcedUpdate = false)
        {
            if(value.Equals(showPreview))
                return;

            showPreview = value;
            OnImageUpdated?.Invoke(value);
        }
    }
} */