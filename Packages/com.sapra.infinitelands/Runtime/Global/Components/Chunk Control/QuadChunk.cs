using System.Collections.Generic;
using UnityEngine;
using sapra.InfiniteLands.NaughtyAttributes;

namespace sapra.InfiniteLands{  
    public class QuadChunk : ChunkControl
    {
        public override InfiniteSettings GetInfiniteSettings(MeshSettings userData, float ViewDistance)
        {
            int lodLevels = Mathf.CeilToInt(Mathf.Log(ViewDistance / userData.MeshScale, 2)) + 1;
            float MaxScale = Mathf.Pow(2, lodLevels-1)*userData.MeshScale;
            int VisibleChunks = Mathf.CeilToInt(ViewDistance / MaxScale);
            return new InfiniteSettings(lodLevels, VisibleChunks);
        }
        public override MeshSettings GetMeshSettingsFromID(MeshSettings userData, Vector3Int ID)
        {
            MeshSettings selected = userData;
            selected.MeshScale = Mathf.Pow(2, ID.z)*userData.MeshScale;
            return selected;
        }

        public override Vector3Int TransformPositionToID(Vector2 gridPosition, int lod, Vector2 gridOffset, float MeshScale)
        {
            float MeshSize = Mathf.Pow(2, lod) * MeshScale;
            Vector2Int flat = Vector2Int.FloorToInt((gridPosition+gridOffset)/MeshSize);
            return new Vector3Int(flat.x, flat.y, lod);
        }

        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;
        private MeshFilter _meshFilter;
        [SerializeField] public bool DrawSpecificChunk;
        [ReadOnly] public List<QuadChunk> childs = new List<QuadChunk>();
        private float MeshScale;

        private bool childrenReady;
        private bool DataApplied => DataRequested && MeshApplied && MaterialApplied;

        private bool IsVisible;

        public bool MeshApplied;
        public bool MaterialApplied;
        private IGenerate<TextureResult> painter;
        private IGenerate<MeshResult> meshMaker;

        protected override void OnDisable()
        {
            base.OnDisable();
            if(painter != null)
                painter.onProcessDone -= OnTextureCreated;
            
            if(meshMaker != null)
                meshMaker.onProcessDone -= OnMeshCreated;
            CleanUp();
            DestroySmallerChunk();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if(painter != null)
                painter.onProcessDone += OnTextureCreated;
            
            if(meshMaker != null)
                meshMaker.onProcessDone += OnMeshCreated;
        }

        protected override void Awake()
        {
            base.Awake();
            _meshFilter = GetOrAddComponent(ref _meshFilter);
            _meshRenderer = GetOrAddComponent(ref _meshRenderer);     
            
            painter = transform.root.GetComponent<IGenerate<TextureResult>>();
            meshMaker = transform.root.GetComponent<IGenerate<MeshResult>>();
        }
        
        public override void Reuse(TerrainConfig config, MeshSettings meshSettings)
        {
            base.Reuse(config, meshSettings);
            MeshScale = meshSettings.MeshScale;
            childrenReady = false;
            CleanUp();
        }

        private void OnMeshCreated(MeshResult meshResult){
            if(!meshResult.ID.Equals(config.ID))
                return;

            MeshApplied = true;
            if(meshResult.PhysicsBaked){
                if(!_meshCollider){
                    _meshCollider = GetOrAddComponent(ref _meshCollider);
                    _meshCollider.cookingOptions = MeshPhysicsJob.cookingOptions;
                }

                _meshCollider.enabled = true;
                _meshCollider.sharedMesh = meshResult.mesh;
            }

            _meshFilter.mesh = meshResult.mesh;
        }

        private void OnTextureCreated(TextureResult textureResult){
            if(!textureResult.terrainConfig.ID.Equals(config.ID))
                return;

            MaterialApplied = true;
            _meshRenderer.material = textureResult.groundMaterial;
        }

        private float SquareDistance(Vector3 diff)
        {
            return Mathf.Max(Mathf.Abs(diff.x), Mathf.Abs(diff.y), Mathf.Abs(diff.z));
        }

        
        private float DistanceToBounds(Vector3 position)
        {
            Vector3 closestPoint = bounds.ClosestPoint(position);
            //closestPoint.y = 0;
            return SquareDistance(position - closestPoint);
        }
        public override bool VisibilityCheck(Vector3 playerPosition, Plane[] planes, float GenerationDistance){
            return VisibilityCheck(playerPosition, planes, GenerationDistance, true);
        }

        private bool VisibilityCheck(Vector3 playerPosition, Plane[] planes, float GenerationDistance, bool parentDisabled)
        {
            IsVisible = InView(playerPosition, planes, GenerationDistance);
            float distance = DistanceToBounds(playerPosition);
            if (distance < MeshScale / 2 && config.ID.z > 0) //If inside the chunk but can be divided
            {
                if (childrenReady && DataRequested){ //If the children are ready and loaded
                    UnrequestMesh(); //Delete this data
                    CleanUp();
                }

                if (childs.Count <= 0) //If there are no child, generate them
                    GenerateChildChunks();
                else
                    childrenReady = UpdateVisibleChunks(playerPosition, planes, !DataApplied && parentDisabled, GenerationDistance); //Enable the childs or check if they can be activated
            }
            else
            {  
                RequestMesh();
                if(DataApplied){
                    DestroySmallerChunk();
                    childrenReady = false;   
                } 
            }
            _meshRenderer.enabled = IsVisible && parentDisabled;
            return childrenReady || DataApplied;
        }
     
        #region Chunk Generation

        private void GenerateChildChunks()
        {
            for (int yOffset = 0; yOffset < 2; yOffset++)
            {
                for (int xOffset = 0; xOffset < 2; xOffset++)
                {
                    Vector3Int newID = new Vector3Int(config.ID.x * 2 + xOffset, config.ID.y * 2 + yOffset,
                        config.ID.z - 1);
                    QuadChunk chunk = generator.GenerateChunk(newID) as QuadChunk;
                    childs.Add(chunk);
                }
            }
        }

        private bool UpdateVisibleChunks(Vector3 playerPosition, Plane[] planes, bool parentDisable, float GenDistance)
        {
            bool allLoaded = true;
            foreach (QuadChunk chunk in childs)
            {
                allLoaded = chunk.VisibilityCheck(playerPosition, planes, GenDistance, parentDisable) && allLoaded;
            }

            return allLoaded;
        }

        #endregion

        #region Deleting
        private void DestroySmallerChunk()
        {
            if (childs.Count > 0)
            {
                for (int i = 0; i < childs.Count; i++)
                {
                    QuadChunk chunk = childs[i];
                    generator.DisableChunk(chunk);
                }

                childs.Clear();
            }
        }

        private void CleanUp(){

            if(_meshFilter != null)
                _meshFilter.mesh = null;

            if (_meshCollider != null)
            {   
                _meshCollider.enabled = false;
                _meshCollider.sharedMesh = null;
            }

            MeshApplied = false; 
            MaterialApplied = false;
        }
        #endregion

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