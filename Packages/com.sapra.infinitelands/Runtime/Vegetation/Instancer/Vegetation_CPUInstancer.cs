using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

namespace sapra.InfiniteLands
{
    public class Vegetation_CPUInstancer : IRenderVegetation
    {
        public static readonly int
            chunkInstancesRowID = Shader.PropertyToID("_ChunkInstancesRow");
        //Instance Data
        private VegetationSettings vegetationSettings;

        private List<CameraBuffersManager> buffers = new List<CameraBuffersManager>();
        private List<GPUChunkDataGameObjects> EnabledInstances = new List<GPUChunkDataGameObjects>();
        private VisibleChunksLoader ChunkInRangeFinder;
        private VegetationInstancePool InstancePool;

        private CPUInstancing asset;
        
        #region Public Methods
        public Vegetation_CPUInstancer(CPUInstancing asset, IPaintTerrain painter, IGenerate<TextureResult> TextureMaker,PointStore store, VegetationSettings settings, List<Camera> cameras, RenderTexture[] depthtextures, Transform colliderParent){
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

            if (asset.InstanceObject != null && !asset.SkipRendering() && Application.isPlaying){
                InstancePool = new VegetationInstancePool(asset.InstanceObject, colliderParent);
            }
        }

        public void OnOriginShift(CommandBuffer bf, ComputeShader compute, int kernel, Vector3 offset){
            bf.SetComputeIntParam(compute,chunkInstancesRowID, vegetationSettings.ChunkInstancesRow);
            for(int i = 0; i < buffers.Count; i++){
                CameraBuffersManager buffer = buffers[i];
                if(buffer.camera == null)
                    continue;
                buffer.OriginShift(bf,compute, kernel);
            }

            ChunkInRangeFinder.OnOriginShift(offset);
        }

        public void Render(IControlMatrices matrixControl, MaterialPropertyBlock propertyBlock, bool EnableCulling){
            if(InstancePool == null)
                return;

            CommandBuffer bf = CommandBufferPool.Get(asset.name);
            
            bf.SetComputeMatrixParam(VegetationRenderer.CalculatePositions, "_localToWorld", matrixControl.localToWorldMatrix);
            bf.SetComputeMatrixParam(VegetationRenderer.CalculatePositions, "_worldToLocal", matrixControl.worldToLocalMatrix);
            for(int i = 0; i < buffers.Count; i++){
                CameraBuffersManager buffer = buffers[i];
                if(!buffer.isValid)
                    continue;
                Vector3 camPos = buffer.camera.transform.position;
                Vector3 position = matrixControl.worldToLocalMatrix.MultiplyPoint(camPos);

                //Check visibility and collisions
                Plane[] frustrumPlanes = GlobalHelper.GetFrustrumPlanes(buffer.camera, asset.ViewDistance);
                for(int p = 0; p < frustrumPlanes.Length; p++){
                    frustrumPlanes[p] = matrixControl.worldToLocalMatrix.TransformPlane(frustrumPlanes[p]);
                }

                ChunkInRangeFinder.LoadVisibleChunks(bf, frustrumPlanes,position, buffer);
            }
            Graphics.ExecuteCommandBuffer(bf);
            CommandBufferPool.Release(bf);
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
                foreach(var instance in EnabledInstances){
                    foreach(var gameobject in instance.instances){
                        InstancePool.Return(gameobject);
                    }
                }
                InstancePool.Dispose();
            }
        }
        #endregion        

        public void HandleColliders(Vector3 position)
        {
            for(int i = 0; i < buffers.Count; i++){
                CameraBuffersManager buffer = buffers[i];
                ChunkInRangeFinder.CollisionCheck(position, buffer, vegetationSettings.ViewDistance);
            }
            Vector2Int positionInGrid = Vector2Int.RoundToInt(new Vector2(position.x, position.z)/vegetationSettings.ChunkSize);

            List<GPUChunkDataGameObjects> CurrrentInstances = new List<GPUChunkDataGameObjects>(EnabledInstances);
            EnabledInstances.Clear();
            for (int yOffset = -vegetationSettings.ChunksVisible; yOffset <= vegetationSettings.ChunksVisible; yOffset++)
            {
                for (int xOffset = -vegetationSettings.ChunksVisible; xOffset <= vegetationSettings.ChunksVisible; xOffset++)
                {
                    Vector2Int currentChunkID = positionInGrid+new Vector2Int(xOffset, yOffset);
                    GPUChunkData found = ChunkInRangeFinder.GetInstanceDataAtID(currentChunkID);
                    var theresData = CurrrentInstances.FirstOrDefault(a => a.id.ID.Equals(currentChunkID));
                    if(theresData != null){
                        CurrrentInstances.Remove(theresData);
                    }else{
                        if(found != null && found.Generated){
                            List<GameObject> objectsCreated = new List<GameObject>();
                            foreach(InstanceData data in found.Instances){
                                GameObject createCollider = InstancePool.CreateInstance(data);
                                if(createCollider != null)
                                    objectsCreated.Add(createCollider);
                            }

                            theresData = new GPUChunkDataGameObjects(){
                                id = found,
                                instances = objectsCreated
                            };
                        }
                    }
                    
                    if(theresData != null)
                        EnabledInstances.Add(theresData);
                }
            }

            foreach(GPUChunkDataGameObjects cll in CurrrentInstances){
                foreach(GameObject BJ in cll.instances){
                    BJ.SetActive(false);
                    InstancePool.Return(BJ);
                }
            }
        }  

        
        #region Debugging
        public void DrawGizmos(){
            foreach (CameraBuffersManager buf in buffers){
                if(asset.DrawBoundigBoxes)
                    buf.GizmosDraw();

                asset.GizmosDrawDistances(buf.camera.transform.position);
            }
        }

        public IHoldVegetation GetAsset() => asset;
        #endregion
    }
}