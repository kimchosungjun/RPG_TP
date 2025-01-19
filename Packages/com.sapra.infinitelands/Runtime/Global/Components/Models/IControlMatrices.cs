using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;

namespace sapra.InfiniteLands{
    public interface IControlMatrices{
        public Matrix4x4 localToWorldMatrix{get;}
        public Matrix4x4 worldToLocalMatrix{get;}
    }
}