#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace sapra.InfiniteLands{
    [ExecuteAlways]
    public abstract class ChunkControl : MonoBehaviour, IRenderChunk, ISetupChunkHirearchy
    {
        protected Bounds bounds;
        protected TerrainConfig config;
        protected ChunkData data;
        protected IVisualizeTerrain generator;

        protected bool DataRequested;
        public abstract InfiniteSettings GetInfiniteSettings(MeshSettings userData, float ViewDistance);
        public abstract MeshSettings GetMeshSettingsFromID(MeshSettings userData, Vector3Int ID);
        public abstract Vector3Int TransformPositionToID(Vector2 gridPosition, int lod, Vector2 gridOffset, float MeshScale);
        public abstract bool VisibilityCheck(Vector3 playerPosition, Plane[] planes, float GenerationDistance);
        
        protected virtual void OnEnable()
        {
            if(generator == null)
                Awake();
            
            if(generator != null)
                generator.onProcessDone += OnTerrainCreated;
        }
        protected virtual void OnDisable()
        {
            if(generator != null)
                generator.onProcessDone -= OnTerrainCreated;

            UnrequestMesh();
        }

        protected virtual void Awake()
        {
            generator = transform.root.GetComponent<IVisualizeTerrain>();
        }

        public virtual void Reuse(TerrainConfig config, MeshSettings meshSettings)
        {
            this.bounds = config.WorldSpaceBounds;
            this.config = config;
            this.data = null;
            this.DataRequested = false;
        }

        private void OnTerrainCreated(ChunkData data){
            #if UNITY_EDITOR
            if(!Application.isPlaying){
                Awake();
            }
            #endif

            if(!data.terrainConfig.ID.Equals(config.ID))
                return;
            this.data = data;
            bounds = data.terrainConfig.WorldSpaceBounds;
            config = data.terrainConfig;
        }
        

        protected void RequestMesh(){
            if(!DataRequested){
                generator?.RequestMesh(config);
                DataRequested = true;
            }
        }

        protected void UnrequestMesh(){
            if(!DataRequested)
                return;

            generator?.UnrequestMesh(config, data); //Unrequest the mesh in case it's on the queue, we will not needed it anymore
            DataRequested = false;
        }
        
        protected T GetOrAddComponent<T>(ref T comp) where T : Component{
            if(comp != null)
                return comp;
            T found = GetComponent<T>();
            if(found == null)
                found = gameObject.AddComponent<T>();
            return found;
        }

        protected T GetOrAddComponent<T>(ref T comp, GameObject from) where T : Component{
            if(comp != null)
                return comp;
            T found = from.GetComponent<T>();
            if(found == null)
                found = from.AddComponent<T>();
            return found;
        }

        protected bool InView(Vector3 playerPosition, Plane[] planes, float GenerationDistance){
            Vector3 p = bounds.ClosestPoint(playerPosition);
            //p.y = 0;
            float dist = Vector3.Distance(playerPosition, p);

            if(dist < GenerationDistance){
                if(planes != null){
                    return GeometryUtility.TestPlanesAABB(planes, bounds);
                }
            }
            return false;
        }
    }
}