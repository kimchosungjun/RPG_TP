using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static sapra.InfiniteLands.CameraBuffersManager;

namespace sapra.InfiniteLands{
    public class VisibleChunksLoader
    {
        private List<Vector2Int> EnabledChunks = new List<Vector2Int>();
        private Dictionary<Vector2Int, GPUChunkData> GameObjectData = new Dictionary<Vector2Int, GPUChunkData>();

        private VegetationSettings settings;
        private IHoldVegetation asset;
        private IPaintTerrain painter;
        private IGenerate<TextureResult> textures;
        private PointStore store;

        private Vector2Int PreviousPositionInGrid;
        private bool MaybeTheresNewData;

        private List<VegetationChunk> ActiveVegetationChunks = new List<VegetationChunk>();
        public VisibleChunksLoader(VegetationSettings settings, IPaintTerrain Painter, IGenerate<TextureResult> TextureMaker,PointStore store, IHoldVegetation asset){
            this.settings = settings;
            this.painter = Painter;
            this.asset = asset;
            this.textures = TextureMaker;
            this.store = store;
            PreviousPositionInGrid = new Vector2Int(int.MinValue, int.MinValue);
            textures.onProcessDone += NewTerrainGenerated;
        }
        
        public void Dispose(){
            textures.onProcessDone -= NewTerrainGenerated;
        }

        private void NewTerrainGenerated(TextureResult texture){
            MaybeTheresNewData = true;
            foreach(VegetationChunk chunk in ActiveVegetationChunks){
                chunk.OnChunkGenerated(texture);
            }
        }
        public void OnOriginShift(Vector3 offset){
            GPUChunkData[] items = GameObjectData.Values.ToArray();
            foreach(GPUChunkData data in items){
                data.OriginShift(offset);
            }
        }

        public GPUChunkData GetInstanceDataAtID(Vector2Int ID){
            if(GameObjectData.TryGetValue(ID, out GPUChunkData data))
                return data;
            return null;
        }
        public void LoadVisibleChunks(CommandBuffer bf, Plane[] frustrumPlanes, Vector3 position, CameraBuffersManager drawer){   
            Vector2Int positionInGrid = Vector2Int.RoundToInt(new Vector2(position.x/settings.ChunkSize, position.z/settings.ChunkSize));
            if(!positionInGrid.Equals(PreviousPositionInGrid)){
                FindChunksInRange(position, drawer);
                PreviousPositionInGrid = positionInGrid;
                MaybeTheresNewData = true;
            }
            
            GenerateNewChunks(bf, drawer);

            drawer.VisibleChunksCheck(position, frustrumPlanes);
        }
        private void FindChunksInRange(Vector3 position, CameraBuffersManager drawer){
            //Free indices
            HashSet<Vector2Int> MustBeDisabled = new HashSet<Vector2Int>(EnabledChunks);
            List<VegetationChunk> ToAssign = new List<VegetationChunk>();

            EnabledChunks.Clear();
            ActiveVegetationChunks.Clear();

            Vector2Int positionInGrid = Vector2Int.RoundToInt(new Vector2(position.x/settings.ChunkSize, position.z/settings.ChunkSize));
            for (int yOffset = -settings.ChunksVisible; yOffset <= settings.ChunksVisible; yOffset++)
            {
                for (int xOffset = -settings.ChunksVisible; xOffset <= settings.ChunksVisible; xOffset++)
                {
                    Vector2Int currentChunkID = positionInGrid+new Vector2Int(xOffset, yOffset);
                    MustBeDisabled.Remove(currentChunkID);
                    EnabledChunks.Add(currentChunkID);

                    VegetationChunk found = drawer.GetVegetationChunk(currentChunkID);
                    if(found == null){
                        Vector2 chunkPosition = new Vector2(currentChunkID.x, currentChunkID.y) * settings.ChunkSize + settings.gridOffset;
                        found = new VegetationChunk(currentChunkID, settings.ChunkSize, chunkPosition, painter, store, asset.ExtraVerticalBound());
                        ToAssign.Add(found);
                    }

                    ActiveVegetationChunks.Add(found);
                }
            }

            foreach(Vector2Int ck in MustBeDisabled){
                drawer.DisableChunk(ck);
            }

            foreach(VegetationChunk chunk in ToAssign){
                drawer.PlaceChunkToBuffers(chunk);    
            }
        }
        private void GenerateNewChunks(CommandBuffer bf, CameraBuffersManager drawer){
            if(!MaybeTheresNewData)
                return;
            
            if(!drawer.AnyWithNewData())
                return;
            
            ComputeShader compute = VegetationRenderer.CalculatePositions;
            int kernel = VegetationRenderer.CalculatePositionsKernel;
            painter.AssignMaterials(bf, compute, kernel);
            drawer.GenerateChunks(bf, compute);
            MaybeTheresNewData = false;
        }

        public void CollisionCheck(Vector3 position, CameraBuffersManager drawer, float DataDistance){
            drawer.IterateOverExistingChunks((VegetationChunk chunk) => Colliders(drawer, chunk, position, DataDistance));
        }
        private void Colliders(CameraBuffersManager drawer, VegetationChunk chunk, Vector3 position, float DataDistance){            
            if(DataDistance <= 0)
                return;

            bool ShouldHaveData = chunk.IsInsideCollision(position, DataDistance);
            bool DataStored = GameObjectData.TryGetValue(chunk.ID, out GPUChunkData dataForColliders);

            if(DataStored && !ShouldHaveData){
                GameObjectData.Remove(chunk.ID);
            }
            else if(ShouldHaveData && !DataStored){
                CommandBuffer bf = CommandBufferPool.Get();

                var ind = drawer.GetCameraBufferIndex(chunk.ID, out bool found);
                if(!found)
                    Debug.LogError("Chunk is non existen, something went wrong");

                int offset = ind.ChunkIndex*settings.ChunkInstancesRow*settings.ChunkInstancesRow;
                dataForColliders = new GPUChunkData(chunk.ID, chunk.Position, settings.ChunkInstances);
                bf.RequestAsyncReadback(drawer.GetBuffer(ind), settings.ChunkInstances*InstanceData.size, offset*InstanceData.size, 
                    (AsyncGPUReadbackRequest request) => CopyDataForColliders(request, dataForColliders));
                Graphics.ExecuteCommandBuffer(bf);
                CommandBufferPool.Release(bf);   
                GameObjectData.Add(chunk.ID, dataForColliders); 
            }  
        }

        private void CopyDataForColliders(AsyncGPUReadbackRequest request, GPUChunkData instancesID){
            if(request.done){
                Unity.Collections.NativeArray<InstanceData> allInstances = request.GetData<InstanceData>(0);
                allInstances.CopyTo(instancesID.Instances);
                instancesID.Generated = true;
                allInstances.Dispose();
            }
        }
        
    }
}