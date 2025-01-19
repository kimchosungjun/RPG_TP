using System;
using UnityEngine;

namespace sapra.InfiniteLands{  
    public abstract class UpdateableSO : ScriptableObject
    {
        public Action OnValuesUpdated;

        protected virtual void OnValidate()
        {
            OnValuesUpdated?.Invoke();
        }
    }
}