using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace sapra.InfiniteLands{
    public class DataManager : IReturnable
    {
        Dictionary<string, IDisposableJob> Generators = new Dictionary<string, IDisposableJob>();
        List<IReturnable> Returnables = new List<IReturnable>();

        private DisposablePool<G, T> GetDisposablePool<G, T>() where T : IDisposableJob{
            string classType = typeof(G).ToString() + typeof(T).ToString();
            if(!Generators.TryGetValue(classType, out IDisposableJob request)){
                request = new DisposablePool<G,T>();    
                Generators.Add(classType, request);
            }
            return (DisposablePool<G, T>)request;
        }

        private ReturnablePool<T> GetReturnablePool<T>(int size) where T: struct{
            var result = new ReturnablePool<T>(size);
            Returnables.Add(result);
            return result;
        }

        public T GetValue<G,T>(G ID, Func<T> OnGenerate) where T : IDisposableJob{
            DisposablePool<G, T> generator = GetDisposablePool<G, T>();
            return generator.GetValue(ID, OnGenerate);
        }

        public NativeArray<T> GetValue<G,T>(G ID, Func<NativeArray<T>> OnGenerate) where T: struct{
            DisposablePool<G, NativeArrayWrap<T>> generator = GetDisposablePool<G, NativeArrayWrap<T>>();
            return generator.GetValue(ID, () => new NativeArrayWrap<T>(OnGenerate.Invoke())).GetData;
        }
        
        public NativeArray<T> GetReturnableArray<T>(string requestor, T[] data) where T: struct{
            DisposablePool<int, ReturnablePool<T>> generator = GetDisposablePool<int,ReturnablePool<T>>();
            var pool = generator.GetValue(data.Length, () => GetReturnablePool<T>(data.Length));
            var array = pool.GetReturnableArray(requestor);
            array.CopyFrom(data);
            return array;
        }

        public NativeArray<T> GetReturnableArray<T>(string requestor, int size) where T: struct{
            DisposablePool<int, ReturnablePool<T>> generator = GetDisposablePool<int,ReturnablePool<T>>();
            var value = generator.GetValue(size, () => GetReturnablePool<T>(size));
            return value.GetReturnableArray(requestor);
        }
        
        public NativeArray<T> GetReturnableArray<T>(Vector3Int requestor, T[] data) where T: struct => GetReturnableArray(requestor.ToString(), data);
        public NativeArray<T> GetReturnableArray<T>(Vector3Int requestor, int size) where T: struct => GetReturnableArray<T>(requestor.ToString(), size);

        public void Dispose(JobHandle job)
        {
            foreach(KeyValuePair<string, IDisposableJob> disposables in Generators){
                disposables.Value.Dispose(job);
            }
            Generators.Clear();
        }

        public void Return(string requester)
        {
            foreach(IReturnable returnable in Returnables){
                returnable.Return(requester);
            }
        }

        public void Return(Vector3Int requester) => Return(requester.ToString());
    }
}