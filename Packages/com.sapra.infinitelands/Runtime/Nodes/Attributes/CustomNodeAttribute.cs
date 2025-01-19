using UnityEngine;
using System;
using System.Linq;

namespace sapra.InfiniteLands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomNodeAttribute : PropertyAttribute
    {
        public string type = "other";
        public string docs = "";
        public string name;
        public bool canCreate = true;
        public bool singleInstance = false;

        private Type[] ValidOnlyIn;

        public bool IsValidInTree(Type treeTyp){
            if(ValidOnlyIn.Length <= 0)
                return true;

            return ValidOnlyIn.Contains(treeTyp);    
        }

        public CustomNodeAttribute(string name, params Type[] ValidOnlyIn)
        {
            this.name = name;
            this.ValidOnlyIn = ValidOnlyIn;
        }
        public CustomNodeAttribute(string name)
        {
            this.name = name;
            this.ValidOnlyIn = new Type[0];
        }
    }
}