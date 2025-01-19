using Unity.Jobs;
using UnityEngine;

namespace sapra.InfiniteLands
{
    public class MeshGenerationData
    {
        public JobHandle handle;
        public Mesh.MeshDataArray meshDataArray;
        public ChunkData[] generatedChunks;

        public void Dispose(){
            foreach(ChunkData chunk in generatedChunks){
                chunk.Return();
            }
        }
    }
}