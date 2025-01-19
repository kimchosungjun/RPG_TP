using Unity.Collections;
using Unity.Jobs;

namespace sapra.InfiniteLands{
    public class NativeArrayWrap<T> : IDisposableJob where T : struct
    {
        private NativeArray<T> data;
        public NativeArrayWrap(NativeArray<T> rry)
        {
            data = rry;
        }
        public void Dispose(JobHandle job)
        {
            data.Dispose(job);
        }
        public NativeArray<T> GetData => data;
    }
}