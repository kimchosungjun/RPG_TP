using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace sapra.InfiniteLands{
    public class ReturnablePool<T> : IDisposableJob, IReturnable where T: struct
    {
        private List<NativeArray<T>> _availableArrays = new();
        private Dictionary<string, List<NativeArray<T>>> _usedArrays = new();

        private int size;

        public ReturnablePool(int size){
            this.size = size;
        }
        
        public void Dispose(JobHandle job)
        {
            foreach(var arr in _availableArrays){
                if(arr.IsCreated)
                    arr.Dispose(job);
            }

            foreach(KeyValuePair<string, List<NativeArray<T>>> arr in _usedArrays){
                foreach(NativeArray<T> value in arr.Value){
                    if(value.IsCreated)
                        value.Dispose(job);
                    Debug.LogWarningFormat("Stuff hasn't been returned. Manually disposing it. {0}", typeof(T));
                }          
            }      

            _availableArrays.Clear();  
            _usedArrays.Clear();
        }

        public NativeArray<T> GetReturnableArray(string requester)
        {
            NativeArray<T> map;
            if (_availableArrays.Count > 0){
                map = _availableArrays[0];
                _availableArrays.RemoveAt(0);
            }
            else
                map = new NativeArray<T>(size, Allocator.Persistent,
                    NativeArrayOptions.UninitializedMemory);
            
            if(!_usedArrays.TryGetValue(requester, out List<NativeArray<T>> usedArray)){
                usedArray = new List<NativeArray<T>>();
                _usedArrays.Add(requester, usedArray);
        
            }
            usedArray.Add(map);
            return map;
        }

        public void Return(string requester){
            
            if(_usedArrays.TryGetValue(requester, out List<NativeArray<T>> usedArray)){
                foreach(NativeArray<T> arr in usedArray){
                    _availableArrays.Add(arr);
                }
                _usedArrays.Remove(requester);
            }
        }
    }
}