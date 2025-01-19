using Unity.Collections;
using UnityEngine;

namespace sapra.InfiniteLands{
    public class CoordinateDataHolder : ProcessableData
    {
        public NativeArray<CoordinateData> points{get; private set;}
        public CoordinateDataHolder(NativeArray<CoordinateData> points, DataManager manager, string idData) : base(manager, idData)
        {
            this.points = points;
        }
    }
}