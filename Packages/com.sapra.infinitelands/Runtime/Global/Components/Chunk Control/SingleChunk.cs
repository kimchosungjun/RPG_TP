using UnityEngine;

namespace sapra.InfiniteLands{
    [ExecuteAlways]
    public class SingleChunk : ChunkControl
    {
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
        [SerializeField] public bool DrawSpecificChunk;

        [Header("Components")]
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;
        
        IGenerate<TextureResult> textures;
        IGenerate<MeshResult> meshMaker;

        private bool IsVisible;
        protected override void OnDisable()
        {
            base.OnDisable();
            if(textures != null)
                textures.onProcessDone -= OnTextureGenerated;
            
            if(meshMaker != null)
                meshMaker.onProcessDone -= OnMeshGenerated;

            if (_meshCollider)
            {
                _meshCollider.enabled = false;
                _meshCollider.sharedMesh = null;
            }

            if(_meshFilter)
                _meshFilter.mesh = null;                
        }

        public override bool VisibilityCheck(Vector3 playerPosition, Plane[] planes, float GenerationDistance)
        {
            RequestMesh();
            IsVisible = InView(playerPosition, planes,GenerationDistance);
            _meshRenderer.enabled = IsVisible;
            return IsVisible;
        }

        protected override void Awake()
        {
            base.Awake();
            _meshFilter = GetOrAddComponent(ref _meshFilter);
            _meshRenderer = GetOrAddComponent(ref _meshRenderer);

            textures = transform.root.GetComponent<IGenerate<TextureResult>>();
            meshMaker = transform.root.GetComponent<IGenerate<MeshResult>>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if(textures != null)
                textures.onProcessDone += OnTextureGenerated;
            
            if(meshMaker != null)
                meshMaker.onProcessDone += OnMeshGenerated;
        }

        public void OnTextureGenerated(TextureResult request){
            if(!request.terrainConfig.ID.Equals(config.ID))
                return;

            ApplyMaterial(request.groundMaterial);
        }


        public void OnMeshGenerated(MeshResult request){
            if(!request.ID.Equals(config.ID))
                    return;
            ApplyMesh(request.mesh);
            if(request.PhysicsBaked)
                ApplyPhysics(request.mesh);
        }
        
        void ApplyMaterial(Material material){
            if(_meshRenderer == null)
                return;
            if (Application.isPlaying)
                _meshRenderer.material = material;
            else
                _meshRenderer.sharedMaterial = material;
        }

        void ApplyMesh(Mesh mesh){
            if (Application.isPlaying)
                _meshFilter.mesh = mesh;
            else
                _meshFilter.sharedMesh = mesh;
        }

        void ApplyPhysics(Mesh mesh){
            if(!_meshCollider){
                _meshCollider = GetOrAddComponent(ref _meshCollider);
                _meshCollider.cookingOptions = MeshPhysicsJob.cookingOptions;
            }

            _meshCollider.enabled = true;
            _meshCollider.sharedMesh = mesh;
        }
        
        private void OnDrawGizmos()
        {
            if (generator == null || !generator.DrawGizmos && !DrawSpecificChunk)
                return;

            if(IsVisible)
                Gizmos.color = Color.HSVToRGB(config.ID.z/10.0f, 1f, 1f);
            else
                Gizmos.color = new Color(0,0,0,.5f);

            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}