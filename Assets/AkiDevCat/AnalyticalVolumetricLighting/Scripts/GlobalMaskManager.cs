using AkiDevCat.AVL.Components;
using System.Collections.Generic;
using UnityEngine.Events;

namespace AkiDevCat.AVL
{
    public static class GlobalMaskManager
    {
        public static readonly UnityEvent<VolumetricLightMask> OnActiveMaskAdded = new();
        public static readonly UnityEvent<VolumetricLightMask> OnActiveMaskRemoved = new();
        
        private static readonly Dictionary<int, VolumetricLightMask> ActiveMasks = new();

        internal static bool AddActiveMask(VolumetricLightMask mask)
        {
            if (ActiveMasks.TryAdd(mask.GetInstanceID(), mask))
            {
                OnActiveMaskAdded.Invoke(mask);
                return true;
            }

            return false;
        }
        
        internal static bool RemoveActiveMask(VolumetricLightMask mask)
        {
            if (ActiveMasks.Remove(mask.GetInstanceID()))
            {
                OnActiveMaskRemoved.Invoke(mask);
                return true;
            }

            return false;
        }

        internal static VolumetricLightMask GetActiveMask(int instanceID)
        {
            if (ActiveMasks.TryGetValue(instanceID, out var result))
            {
                return result;
            }

            return null;
        }
    }
}