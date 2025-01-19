using UnityEngine;

namespace sapra.InfiniteLands.UnityTerrain{
    public class UnityTerrainChunk : ChunkControl
    {
        public Terrain terrain;
        public TerrainCollider terrainCollider;

        private IGenerate<TerrainWithTextures> finalTerrain;
        protected override void Awake()
        {
            base.Awake();
            terrain = GetComponentInChildren<Terrain>();
            GameObject sub;
            if(terrain == null){
                var gameObject = new GameObject("Terrain");
                gameObject.transform.SetParent(this.transform);
                sub = gameObject;
            }
            else
                sub = terrain.gameObject;
            terrain = GetOrAddComponent(ref terrain, sub);
            terrainCollider = GetOrAddComponent(ref terrainCollider, sub);

            finalTerrain = transform.root.GetComponent<IGenerate<TerrainWithTextures>>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if(finalTerrain != null){
                finalTerrain.onProcessDone += OnMeshGenerated;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if(finalTerrain != null)
                finalTerrain.onProcessDone -= OnMeshGenerated;          
        }

        public void OnMeshGenerated(TerrainWithTextures request){
            if(!request.ID.Equals(config.ID))
                    return;
            
            if(terrain == null)
                return;

            float meshScale = request.data.meshSettings.MeshScale;
            float vertical = request.data.worldFinalData.MinMaxHeight.x;
            terrain.terrainData = request.terrainData;
            Vector3 ps = terrain.transform.localPosition;
            ps.y = vertical;
            ps.x = -meshScale/2f;
            ps.z = -meshScale/2f;
            terrain.transform.localPosition = ps;
            terrain.materialTemplate = request.groundMaterial;
            terrainCollider.terrainData = request.terrainData;
        }

        public override InfiniteSettings GetInfiniteSettings(MeshSettings userData, float ViewDistance)
        {
            int lodLevels = 1;
            int VisibleChunks = Mathf.CeilToInt(ViewDistance / userData.MeshScale);
            return new InfiniteSettings(lodLevels, VisibleChunks);
        }
        
        public override Vector3Int TransformPositionToID(Vector2 gridPosition, int lod, Vector2 gridOffset, float MeshScale)
        {
            Vector2Int flat = Vector2Int.FloorToInt((gridPosition+gridOffset)/MeshScale);
            return new Vector3Int(flat.x, flat.y, lod);
        }

        public override MeshSettings GetMeshSettingsFromID(MeshSettings userData, Vector3Int ID)
        {
            return userData;
        }
        public override bool VisibilityCheck(Vector3 playerPosition, Plane[] planes, float GenerationDistance)
        {
            RequestMesh();
            bool visible = InView(playerPosition, planes,GenerationDistance);
            terrain.enabled = visible;
            return visible;
        }

    }
}