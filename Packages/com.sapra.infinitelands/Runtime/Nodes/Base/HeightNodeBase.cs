using Unity.Jobs;

namespace sapra.InfiniteLands
{
    public abstract class HeightNodeBase : HeightDataGenerator
    {
        public override HeightData RequestHeight(HeightGenerationResult ind, GenerationSettings settings)
        {
            if (!ind.HeightGenerated)
            {
                IndexAndResolution real = ind.JobReady;
                ind.jobHandle = ProcessData(real, settings);
                ind.HeightGenerated = true;
            }
            
            return ind.CachedHeight;
        }

        public abstract JobHandle ProcessData(IndexAndResolution target, GenerationSettings settings);
    }
}