using System;
namespace sapra.InfiniteLands.Editor
{
    public static class EditorTools
    {
        public static M GetAttribute<M>(Type type)
        {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            foreach (Attribute attribute in attrs)
            {
                if (attribute is M customAttribute)
                {
                    return customAttribute;
                }
            }

            return default;
        }
        
    }
}