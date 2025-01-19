using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
namespace sapra.InfiniteLands
{
    // Modifies the points that are used for the generation of that branch
    public interface IGeneratePoints{
        public JobHandle GiveMePoints(GenerationSettings settings, out NativeArray<float3> points, NativeArray<float3> parent, JobHandle dependancy);
    }
}