using Unity.Jobs;

namespace sapra.InfiniteLands
{
    public struct NoiseOutput
    {
        public IndexAndResolution globalIndex;
        public JobHandle jobHandle;
    }
}