using System.Collections.Generic;
using UnityEngine;

namespace sapra.InfiniteLands{
    public class AssetsData : ProcessableData
    {
        public List<AssetData> assets{get;}

        public AssetsData(List<AssetData> assets, DataManager manager, string idData) : base(manager, idData)
        {
            this.assets = assets;
        }
    }
}