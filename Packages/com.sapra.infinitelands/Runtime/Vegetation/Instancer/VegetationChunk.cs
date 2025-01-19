using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace sapra.InfiniteLands{
    public class VegetationChunk
    {
        private static readonly int 
            gridOffsetID = Shader.PropertyToID("_ChunkPosition");
        public Vector2Int ID{get; private set;}
        public Vector3 Position{get; private set;}
        public Vector3 minBounds{get; private set;}
        public Vector3 maxBounds{get; private set;}
        public bool Visible{get; private set; }
        public TextureResult TerrainTexture{get; private set; }
        public TextureResult NewData{get; private set;}

        private Vector2 FlatPosition;
        private Vector3 ChunkOffset;
        private Bounds Bounds;
        private bool ContainsInstances;

        private float MaxAssetBounds;
        private float ChunkSize;
        private PointStore pointStore;
        private IGenerate<TextureResult> textureGenerator;
        
        public VegetationChunk(Vector2Int id, float chunkSize,Vector2 position, IPaintTerrain painter, PointStore store, float maxBounds){
            ID = id;
            Position = new Vector3(position.x, 0, position.y);
            FlatPosition = position;
            pointStore = store;

            ChunkSize = chunkSize;
            MaxAssetBounds = maxBounds;
            ContainsInstances = false;

            TextureResult initial = painter.GetTextureAt(FlatPosition);
            if(initial != null)
                NewData = initial;
        }
        
        public void RecalculateTextureData(){
            if(NewData == null)
                return;
                
            TerrainTexture = NewData;
            NewData = null;

            var dif = (ChunkSize-TerrainTexture.settings.MeshScale)/(2f*ChunkSize);
            var precIssues = Position-TerrainTexture.terrainConfig.Position;
            var ind = (precIssues/ChunkSize)-new Vector3(dif,0,dif);
            var rnd = Vector3Int.RoundToInt(ind);
            ChunkOffset = new Vector3(rnd.x+dif,0,rnd.z+dif)*ChunkSize;

            Vector2 TerrainMinMaxHeight = TerrainTexture.terrainConfig.MinMaxHeight;
            float verticalOffset = TerrainMinMaxHeight.y + TerrainMinMaxHeight.x;
            float displacement = TerrainMinMaxHeight.y - TerrainMinMaxHeight.x;
            Bounds = new Bounds(new Vector3(Position.x, verticalOffset / 2f+MaxAssetBounds/2f, Position.z),
                new Vector3(ChunkSize, displacement+MaxAssetBounds, ChunkSize));
            
            minBounds = Bounds.min;
            maxBounds = Bounds.max;
        }

        public void OnChunkGenerated(TextureResult result){
            if(result == null)
                return;

            if(TerrainTexture != null && result.terrainConfig.ID.z >= TerrainTexture.terrainConfig.ID.z)
                return;

            if(!pointStore.IsPointInChunkAtGrid(FlatPosition, result.terrainConfig))
                return;
           
            NewData = result;
        }
        public bool IsVisible(Vector3 cameraPosition, Plane[] planes)
        {
            if(!ContainsInstances)
                return false;
            Vector3 closestPoint = Bounds.ClosestPoint(cameraPosition);
            float distance = Vector3.Distance(cameraPosition, closestPoint);
            bool InView = GeometryUtility.TestPlanesAABB(planes, Bounds);

            bool result = InView || distance < ChunkSize / 2;
            Visible = result;
            return result;
        }

        public void UpdateBounds(int MinHeight, int MaxHeight){
            ContainsInstances = MinHeight != int.MaxValue || MaxHeight != int.MinValue;
            Vector3 center = new Vector3(Bounds.center.x, (MaxHeight + MinHeight) / 2f, Bounds.center.z);
            Vector3 size = new Vector3(Bounds.size.x, MaxHeight - MinHeight+MaxAssetBounds, Bounds.size.z);
            Bounds = new Bounds(center, size);
            this.minBounds = Bounds.min;
            this.maxBounds = Bounds.max;
        }        

        public bool IsInsideCollision(Vector3 playerPosition, float CollisionDistance)
        {
            if(TerrainTexture == null)
                return false;
            Vector3 closestPoint = Bounds.ClosestPoint(playerPosition);
            float distance = Vector3.Distance(playerPosition, closestPoint);
            return distance < CollisionDistance && ContainsInstances;
        }

        public void SetComputeShaderData(CommandBuffer bf, ComputeShader compute, int kernelIndex, IAsset asset, int chunkInstancesRow, float chunkCount){
            bf.SetComputeVectorParam(compute, gridOffsetID, ChunkOffset);
            bf.SetComputeIntParam(compute,"_TotalInstances", (int)(Mathf.Pow(2, TerrainTexture.terrainConfig.ID.z)*chunkInstancesRow*chunkCount));
            TerrainTexture.DynamicMeshResultApply(bf, compute, kernelIndex, asset);
        }

        public void DrawGizmos(){
            if(Visible)
                Gizmos.color = Color.blue;
            else if(!ContainsInstances)
                Gizmos.color = Color.black;
            else
                Gizmos.color = Color.gray;
            Gizmos.DrawWireCube(Bounds.center, Bounds.size);
        }
    }
}