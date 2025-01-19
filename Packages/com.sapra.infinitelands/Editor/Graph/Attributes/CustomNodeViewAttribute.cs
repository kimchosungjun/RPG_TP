using UnityEngine;
using System;

namespace sapra.InfiniteLands.Editor{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomNodeViewAttribute : PropertyAttribute
    {        
        readonly Type nodeType;        
        public Type target => nodeType;

        public CustomNodeViewAttribute(Type nodeType)
        {
            this.nodeType = nodeType;
        }
    }
}