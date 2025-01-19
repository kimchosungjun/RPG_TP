using System;

namespace sapra.InfiniteLands.NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SceneAttribute : DrawerAttribute
    {
    }
}