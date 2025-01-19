using UnityEngine;

namespace sapra.InfiniteLands{  
    public static class GlobalHelper
    {
        public static Plane[] GetFrustrumPlanes(Camera cam, float ViewDistance){
            float viewDistance = cam.farClipPlane;
            cam.farClipPlane = ViewDistance;
            Plane[] frustrumPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
            cam.farClipPlane = viewDistance;
            return frustrumPlanes;
        }
    }
}