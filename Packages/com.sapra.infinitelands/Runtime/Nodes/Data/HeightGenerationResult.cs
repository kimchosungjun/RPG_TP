using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace sapra.InfiniteLands
{
    public class HeightGenerationResult{
        public JobHandle jobHandle;
        public int OutputResolution;
        public int RequestedResolution;
        
        public int HeightIndex = -1;
        public bool HeightGenerated = false;

        public bool NormalMapGenerated = false;
        public NativeArray<float3> normalMap;
        public JobHandle jobHandleNormal;
        public IndexAndResolution normalIndex;

        public IndexAndResolution JobReady => new IndexAndResolution(HeightIndex,OutputResolution);
        public HeightData CachedHeight => new HeightData(){
            indexData = JobReady,
            jobHandle = jobHandle,
        };

        public HeightData CachedNormal => new HeightData(){
            indexData = normalIndex,
            jobHandle = jobHandleNormal,
        };
        
        public void Reset(){
            HeightGenerated = false;
            NormalMapGenerated = false;
        }
    }
}