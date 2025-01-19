using System;
using System.Collections.Generic;
using Unity.Jobs;

namespace sapra.InfiniteLands{
    public class DisposablePool<G, T> : IDisposableJob where T : IDisposableJob
    {
        Dictionary<G, T> GenerationValues = new Dictionary<G, T>();
        public T GetValue(G guid, Func<T> generate){
            if(!GenerationValues.TryGetValue(guid, out T request)){
                request = generate();    
                GenerationValues.Add(guid, request);
            }
            return request;
        }
        public void Dispose(JobHandle job){
            foreach(KeyValuePair<G, T> curves in GenerationValues){
                curves.Value.Dispose(job);
            }
            GenerationValues.Clear();
        }
    }
}