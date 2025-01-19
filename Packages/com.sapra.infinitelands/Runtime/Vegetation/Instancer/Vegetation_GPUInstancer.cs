using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

namespace sapra.InfiniteLands
{
    public class Vegetation_GPUInstancer : IRenderVegetation
    {
        public static readonly int
            itemIndex = Shader.PropertyToID("_ItemIndex"),
            chunkInstancesRowID = Shader.PropertyToID("_ChunkInstancesRow"),
            viewDistanceID = Shader.PropertyToID("_ViewDistance"),
            distanceBetweenID = Shader.PropertyToID("_DistanceBetween");

        private class ColliderID{
            public Vector3Int instanceID;
            public GameObject instance;
        }

        //Instance Data
        private VegetationSettings vegetationSettings;
        private int CollidersCheckupIndices;

        private List<CameraBuffersManager> buffers = new List<CameraBuffersManager>();
        private List<ColliderID> EnabledColliders = new List<ColliderID>();
        private List<ColliderID> CollidersToDisable = new List<ColliderID>();

        private VisibleChunksLoader ChunkInRangeFinder;
        private VegetationInstancePool InstancePool;

        private GPUInstancing asset;
        
        #region Public Methods
        public Vegetation_GPUInstancer(GPUInstancing asset, IPaintTerrain painter, IGenerate<TextureResult> TextureMaker,PointStore store, VegetationSettings settings, 
            List<Camera> cameras, RenderTexture[] depthtextures, Transform colliderParent){
            
            this.asset = asset;
            vegetationSettings = settings;
            buffers = new List<CameraBuffersManager>();
            ChunkInRangeFinder = new VisibleChunksLoader(vegetationSettings, painter, TextureMaker, store, asset);

            List<GraphicsBuffer.IndirectDrawIndexedArgs> arguments = asset.InitializeMaterialsAndMeshes(painter);
            for(int i = 0; i < cameras.Count; i++){
                Camera cam = cameras[i];
                RenderTexture depth = depthtextures[i];
                CameraBuffersManager buffer = new CameraBuffersManager(cam, depth, asset, vegetationSettings, arguments);
                buffers.Add(buffer);
            }
            CollidersCheckupIndices = Mathf.CeilToInt(asset.CollisionDistance / vegetationSettings.DistanceBetweenItems)+1;      
            if (asset.GenerateColliders && asset.ColliderObject != null && Application.isPlaying && !asset.SkipRendering()){
                InstancePool = new VegetationInstancePool(asset.ColliderObject.gameObject, colliderParent);
            }
        }
        

        public void OnOriginShift(CommandBuffer bf, ComputeShader compute, int kernel, Vector3 offset){
            for(int i = 0; i < buffers.Count; i++){
                CameraBuffersManager buffer = buffers[i];
                if(buffer.camera == null)
                    continue;
                buffer.OriginShift(bf,compute, kernel);
            }

            ChunkInRangeFinder.OnOriginShift(offset);
        }
        public IHoldVegetation GetAsset() => asset;

        public void Render(IControlMatrices matrixControl, MaterialPropertyBlock propertyBlock, bool EnableCulling){
            if(asset.SkipRendering())
                return;

            CommandBuffer bf = CommandBufferPool.Get(asset.name);
            
            bf.SetComputeIntParam(VegetationRenderer.VisibilityCheck,itemIndex, vegetationSettings.ItemIndex);
            bf.SetComputeIntParam(VegetationRenderer.VisibilityCheck,chunkInstancesRowID, vegetationSettings.ChunkInstancesRow);

            //Calculate Positions Data
            bf.SetComputeFloatParam(VegetationRenderer.VisibilityCheck, viewDistanceID, vegetationSettings.ViewDistance);
            bf.SetComputeFloatParam(VegetationRenderer.VisibilityCheck, distanceBetweenID, vegetationSettings.DistanceBetweenItems);

            asset.SetVisibilityShaderData(bf, VegetationRenderer.VisibilityCheck);

            bf.SetComputeMatrixParam(VegetationRenderer.CalculatePositions, "_localToWorld", matrixControl.localToWorldMatrix);
            bf.SetComputeMatrixParam(VegetationRenderer.CalculatePositions, "_worldToLocal", matrixControl.worldToLocalMatrix);

            for(int i = 0; i < buffers.Count; i++){
                CameraBuffersManager buffer = buffers[i];
                if(!buffer.isValid)
                    continue;
                Vector3 camPos = buffer.camera.transform.position;
                Vector3 position = matrixControl.worldToLocalMatrix.MultiplyPoint(camPos);

                //Check visibility and collisions
                Plane[] frustrumPlanes = GlobalHelper.GetFrustrumPlanes(buffer.camera, vegetationSettings.ViewDistance);
                for(int p = 0; p < frustrumPlanes.Length; p++){
                    frustrumPlanes[p] = matrixControl.worldToLocalMatrix.TransformPlane(frustrumPlanes[p]);
                }

                ChunkInRangeFinder.LoadVisibleChunks(bf, frustrumPlanes,position, buffer);

                buffer.CountAndEncapsulateData(bf, matrixControl, EnableCulling);
                buffer.DrawItems(propertyBlock);
            }
            Graphics.ExecuteCommandBuffer(bf);
            CommandBufferPool.Release(bf);
        }        

        public void HandleColliders(Vector3 playerPosition)
        {     
            if(InstancePool != null)
                HandleInstanceColliders(playerPosition);
        }
        public void Dispose()
        {
            ChunkInRangeFinder.Dispose();

            foreach (CameraBuffersManager buf in buffers)
            {
                buf.Dispose();
            }
            buffers.Clear();

            if(InstancePool != null){
                foreach(var instance in EnabledColliders){
                    InstancePool.Return(instance.instance);
                }
                InstancePool.Dispose();
            }
        }
        #endregion

        private void HandleInstanceColliders(Vector3 playerPosition)
        {
            Vector2 simplePos = new Vector2(playerPosition.x, playerPosition.z);
            for(int i = 0; i < buffers.Count; i++){
                CameraBuffersManager buffer = buffers[i];
                ChunkInRangeFinder.CollisionCheck(playerPosition, buffer, InstancePool != null ? asset.CollisionDistance + vegetationSettings.DistanceBetweenItems: -1);
            }
                        
            CollidersToDisable.Clear();
            CollidersToDisable.AddRange(EnabledColliders);
            EnabledColliders.Clear();

            for (int x = -CollidersCheckupIndices; x <= CollidersCheckupIndices; x++)
            {
                for (int y = -CollidersCheckupIndices; y <= CollidersCheckupIndices; y++)
                {
                    Vector2 itemOffset = new Vector2(x,y)*vegetationSettings.DistanceBetweenItems;
                    itemOffset += simplePos-vegetationSettings.gridOffset;

                    Vector2Int posInGrid = Vector2Int.RoundToInt(itemOffset/vegetationSettings.ChunkSize);
                    GPUChunkData foundData = ChunkInRangeFinder.GetInstanceDataAtID(posInGrid);
                    if(foundData != null && foundData.Generated){
                        Vector2 flatten = itemOffset-new Vector2(foundData.Position.x, foundData.Position.z)+vegetationSettings.gridOffset+ Vector2.one*vegetationSettings.ChunkSize/2.0f;
                        Vector2Int actualIndices = Vector2Int.FloorToInt(flatten/vegetationSettings.DistanceBetweenItems);
                        int index = Mathf.Clamp(actualIndices.x+actualIndices.y*vegetationSettings.ChunkInstancesRow, 0, vegetationSettings.ChunkInstances-1);
                        Vector3Int instanceID = new Vector3Int(posInGrid.x, posInGrid.y, index);
                        ColliderID existingOne = CollidersToDisable.FirstOrDefault(a => a.instanceID.Equals(instanceID));
                        if(existingOne == null){
                            existingOne = CreateCollider(foundData.Instances[index], instanceID);
                        }
                        else{
                            CollidersToDisable.Remove(existingOne);
                        }

                        if(existingOne != null)
                            EnabledColliders.Add(existingOne); 
                    }
 
                }
            }

            foreach(ColliderID cll in CollidersToDisable){
                if(cll.instance != null){
                    InstancePool.Return(cll.instance);
                }
            }
        }  
        
        private ColliderID CreateCollider(InstanceData data, Vector3Int id)
        {
            GameObject AvailableCollider = InstancePool.CreateInstance(data);
            return new ColliderID(){
                instanceID = id,
                instance = AvailableCollider
            };
        }

        #region Debugging
        public void DrawGizmos(){
            foreach (CameraBuffersManager buf in buffers){
                if(asset.DrawBoundigBoxes)
                    buf.GizmosDraw();

                asset.GizmosDrawDistances(buf.camera.transform.position);
            }
        }
        #endregion

    }
}