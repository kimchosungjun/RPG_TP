using UnityEngine;
using System;

namespace sapra.InfiniteLands
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowIfCustomAttribute : PropertyAttribute
    {
        public readonly string propertyName;
        public ShowIfCustomAttribute(string propertyName)
        {
            this.propertyName = propertyName;
        }
    }
}