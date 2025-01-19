using System;
using UnityEngine;

namespace sapra.InfiniteLands{
    public interface IGenerate<T>
    {   
        public Action<T> onProcessDone{get;set;}
        public Action<T> onProcessRemoved{get; set;}
        public Action<IGraph, MeshSettings> onReload{get; set;}

        public IGraph graph{get;} 
        public MeshSettings settings{get;} 
    }
}