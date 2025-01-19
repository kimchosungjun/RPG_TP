using UnityEngine;
using System;

namespace sapra.InfiniteLands
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InputAttribute : PropertyAttribute
    {
        public readonly Type type;
        public readonly bool optional;

        public InputAttribute(Type type, bool optional = false)
        {
            this.type = type;
            this.optional = optional;
        }
    }
}