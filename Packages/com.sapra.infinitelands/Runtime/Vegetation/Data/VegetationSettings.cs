using UnityEngine;

namespace sapra.InfiniteLands{
    public class VegetationSettings 
    {
        public int ChunkInstances;
        public int ChunkInstancesRow;
        public int ChunksRow;
        public int ChunksVisible;
        public float ChunkSize;
        public int ItemIndex;
        public Vector2 gridOffset;

        public float DistanceBetweenItems;
        
        public float ViewDistance;
        private float _ogDistance;
        public float Ratio;

        public VegetationSettings(float meshScale, int itemIndex, float densityPerSize, Vector2 localGridOffset, float distanceBetweenItems, float viewDistance){
            var MaxAvailableSize = densityPerSize * distanceBetweenItems;
            var targetMaxSize = Mathf.Min(viewDistance*2, Mathf.Min(MaxAvailableSize, meshScale));
            var times = meshScale/targetMaxSize;
            
            ChunkSize = meshScale/Mathf.Ceil(times);
            Ratio = Mathf.Ceil(times);
            ChunksVisible = Mathf.CeilToInt(viewDistance / ChunkSize);
            ChunksRow = ChunksVisible+ChunksVisible+1;
            ChunkInstancesRow = Mathf.CeilToInt(ChunkSize/distanceBetweenItems);
            ChunkInstances = ChunkInstancesRow*ChunkInstancesRow;
            ItemIndex = itemIndex;
            ViewDistance = viewDistance;
            _ogDistance = viewDistance;
            DistanceBetweenItems = meshScale/ChunkInstancesRow;
            
            gridOffset = MapTools.GetOffsetInGrid(localGridOffset, ChunkSize);
            if((meshScale/ChunkSize) % 2 == 0){
                gridOffset += Vector2.one*ChunkSize/2.0f;
            }
        }

        public void UpdateViewDistance(float viewDistance){
            this.ViewDistance = viewDistance < 0 ? _ogDistance : Mathf.Min(viewDistance, _ogDistance);
        }
    }
}