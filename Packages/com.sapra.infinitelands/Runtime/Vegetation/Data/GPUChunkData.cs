using UnityEngine;

namespace sapra.InfiniteLands{
    public class GPUChunkData{
        public Vector2Int ID;
        public Vector3 Position;
        public InstanceData[] Instances;
        public bool Generated; 
        public GPUChunkData(Vector2Int iD, Vector3 position, int Length){
            ID = iD;
            Position = position;
            Instances = new InstanceData[Length];
            Generated = false;
        }
        public void OriginShift(Vector3 offset){
            for(int i = 0; i < Instances.Length; i++){
                InstanceData data = Instances[i];
                data.PerformShift(offset);
                Instances[i] = data;
            }
        }
    }
}