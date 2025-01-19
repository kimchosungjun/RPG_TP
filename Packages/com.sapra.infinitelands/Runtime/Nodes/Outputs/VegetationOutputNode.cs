using UnityEngine;
using System;
using System.Collections.Generic;

namespace sapra.InfiniteLands
{
    [CustomNodeAttribute("Vegetation Output", type = "Output", docs = "https://ensapra.com/packages/infinite_lands/nodes/vegetation.html")]
    public class VegetationOutputNode : InfiniteLandsNode, ILoadAsset, IHavePreview, IOutput
    {
        public VegetationAsset VegetationItem;
        [Input(typeof(IGive<HeightData>))] public HeightDataGenerator DensityMap;
        protected override InfiniteLandsNode[] Dependancies => new InfiniteLandsNode[]{DensityMap};
        protected override bool ExtraValidationSteps() => VegetationItem != null;

        public Action<bool> OnImageUpdated { get; set; }

        public void GenerateTexture(GenerationSettings settings, IBurstTexturePool TexturePool)
        {}
        
        [HideInInspector][SerializeField] bool showPreview = false;
        
        public bool PreviewButtonEnabled() => VegetationItem != null &&  VegetationItem.Preview() != null;
        public bool ShowPreview() => PreviewButtonEnabled() && showPreview;

        public IEnumerable<IAsset> GetAssets() => new[]{VegetationItem};
        public NoiseOutput ProcessAsset(IAsset asset, GenerationSettings settings)
        {
            HeightData heightJob = DensityMap.RequestHeight(settings);
            return new NoiseOutput
            {
                globalIndex = heightJob.indexData,
                jobHandle = heightJob.jobHandle
            };
        }

        public bool AssetExists(IAsset asset) => VegetationItem.Equals(asset);
        public void SetAsset(IAsset asset) => VegetationItem = (VegetationAsset)asset;

        public void TogglePreview(bool value, bool forcedUpdate = false){
            if(value.Equals(showPreview))
                return;

            showPreview = value;
            OnImageUpdated?.Invoke(value);
        }

        public object GetTemporalTexture(){
            if(!isValid)
                return null;

            if(VegetationItem == null)
                return null;
            
            return VegetationItem.Preview();
        }
        public int PrepareNode(IndexManager manager,ref int currentCount, int resolution, float ratio, string requestGuid)
        {
            if(DensityMap != null)
                return DensityMap.PrepareNode(manager,ref currentCount, resolution, ratio, requestGuid);
            return resolution;
        }
    }
}