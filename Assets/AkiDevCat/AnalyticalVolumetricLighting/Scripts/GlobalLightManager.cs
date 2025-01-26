using System.Collections;
using System.Collections.Generic;
using AkiDevCat.AVL.Components;
using UnityEngine.Events;

namespace AkiDevCat.AVL
{
    public static class GlobalLightManager
    {
        public static readonly UnityEvent<VolumetricLight> OnActiveLightAdded = new();
        public static readonly UnityEvent<VolumetricLight> OnActiveLightRemoved = new();
        
        private static readonly Dictionary<int, VolumetricLight> ActiveLights = new();

        internal static IEnumerable<KeyValuePair<int, VolumetricLight>> AsEnumerable() => ActiveLights;

        internal static bool AddActiveLight(VolumetricLight light)
        {
            if (ActiveLights.TryAdd(light.GetInstanceID(), light))
            {
                OnActiveLightAdded.Invoke(light);
                return true;
            }

            return false;
        }
        
        internal static bool RemoveActiveLight(VolumetricLight light)
        {
            if (ActiveLights.Remove(light.GetInstanceID()))
            {
                OnActiveLightRemoved.Invoke(light);
                return true;
            }

            return false;
        }

        internal static VolumetricLight GetActiveLight(int instanceID)
        {
            if (ActiveLights.TryGetValue(instanceID, out var result))
            {
                return result;
            }

            return null;
        }
    }
}