using System;
using System.Collections;
using System.Collections.Generic;

namespace sapra.InfiniteLands
{
    public struct AssetWithType{
        public IEnumerable<IAsset> assets;
        public Type originalType;
    }
}