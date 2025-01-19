using UnityEngine;

namespace sapra.InfiniteLands{
    public static class GraphSettingsController
    {
        private static GraphSettings Instance;
        public static void ChangeValueSettings(float meshScale, Vector2 worldOffset, int seed){
            GraphSettings current = GetSettings();
            current.MeshScale = meshScale;
            current.WorldOffset = worldOffset;
            current.Seed = seed;
        }
        public static GraphSettings GetSettings(){
            if(Instance == null)
                Instance = ScriptableObject.CreateInstance<GraphSettings>();
            return Instance;
        }
    } 
}
