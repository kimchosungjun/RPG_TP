using Unity.Jobs;

namespace sapra.InfiniteLands
{
    public struct HeightData
    {
        public IndexAndResolution indexData;
        public JobHandle jobHandle;

        public HeightData(JobHandle job, IndexAndResolution indexData)
        {
            this.jobHandle = job;
            this.indexData = indexData;
        }
    }
}