using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace sapra.InfiniteLands
{
    public class CameraBuffersManager{
        public struct CameraBufferIndex{
            public int BufferIndex;
            public int ChunkIndex;
        }
        private static readonly int
            cameraPositionID = Shader.PropertyToID("_CameraPosition"),
            matrixID = Shader.PropertyToID("_MATRIX_VP"),
            minmaxBufferID = Shader.PropertyToID("_MinMaxBuffer"),
            itemIndex = Shader.PropertyToID("_ItemIndex"),
            chunkInstancesRowID = Shader.PropertyToID("_ChunkInstancesRow"),
            verticalPositionID = Shader.PropertyToID("_VerticalPosition"),
            distanceBetweenID = Shader.PropertyToID("_DistanceBetween"),
            textureRandomnessDistanceID = Shader.PropertyToID("_TextureRandomnessDistance"),
            positionRandomnessID = Shader.PropertyToID("_PositionRandomness"),
            lodCountID = Shader.PropertyToID("_LODCount"),
            densityIsHeightID = Shader.PropertyToID("_DensityIsHeight"),
            alignToGroundID = Shader.PropertyToID("_AlignToGround"),
            sizeID = Shader.PropertyToID("_Size");

        public Camera camera;
        private Texture DepthTexture;
        private ICompact[] Buffers = new ICompact[0];

        private VegetationSettings settings;

        private Dictionary<Vector2Int, CameraBufferIndex> PositionsChunk = new Dictionary<Vector2Int, CameraBufferIndex>();

        private List<Vector3> FrustrumCorners = new();
        public ComputeBuffer TriangleCorners;

        public IHoldVegetation asset;
        public bool isValid => validArguments && camera != null;
        private bool validArguments;

        private ProfilingSampler InitialCompactSampler;
        private ProfilingSampler VisibilityCheckSampler;
        private ProfilingSampler PrepareDrawCallSampler;
        private ProfilingSampler CalculatePositionsSampler;

        public CameraBuffersManager(Camera cam, RenderTexture depthTexture, IHoldVegetation asset, VegetationSettings settings, 
            List<GraphicsBuffer.IndirectDrawIndexedArgs> arguments){
            this.camera = cam;
            this.settings = settings;
            long oneRow = settings.ChunkInstancesRow*settings.ChunksRow;
            double actualSpaceBytes = oneRow*oneRow*InstanceData.size;
            long maxSize = SystemInfo.maxGraphicsBufferSize/8;
            int howManyNeeded = (int)Math.Ceiling(actualSpaceBytes/maxSize);
            int BufferCount = howManyNeeded;
            int ChunksPerBuffer = Mathf.CeilToInt(settings.ChunksRow*settings.ChunksRow/(float)BufferCount);

            validArguments = arguments.Count > 0;
            TriangleCorners = new ComputeBuffer(12, sizeof(float)*9);
            this.asset = asset;
            Buffers = new ICompact[BufferCount]; 


            for(int i = 0; i < BufferCount; i++){
                Buffers[i] = asset.GetCompactor(settings, ChunksPerBuffer, arguments);
            }
            //DepthTexture = depthTexture;

            InitialCompactSampler = new ProfilingSampler(string.Format("{0}/Initial Compact", camera.name));
            VisibilityCheckSampler = new ProfilingSampler(string.Format("{0}/Visibility Check", camera.name));
            PrepareDrawCallSampler = new ProfilingSampler(string.Format("{0}/Prepare Draw Call", camera.name));
            CalculatePositionsSampler = new ProfilingSampler(string.Format("{0}/Calculate Positions", camera.name));
        }

        
        private void SetPositionShaderData(CommandBuffer bf, ComputeShader compute){
            bf.SetComputeFloatParam(compute, distanceBetweenID, settings.DistanceBetweenItems);
            bf.SetComputeIntParam(compute,itemIndex, settings.ItemIndex);
            bf.SetComputeIntParam(compute,chunkInstancesRowID, settings.ChunkInstancesRow);
                       
            bf.SetComputeFloatParam(compute, verticalPositionID, asset.GetVerticalPosition());
            bf.SetComputeFloatParam(compute, distanceBetweenID, asset.GetDistanceBetweenItems());
            bf.SetComputeFloatParam(compute, textureRandomnessDistanceID, asset.GetTextureRandomness());
            bf.SetComputeFloatParam(compute, positionRandomnessID, asset.GetPositionRandomness());
            bf.SetComputeIntParam(compute, densityIsHeightID, (int)asset.GetDensityMode());
            bf.SetComputeIntParam(compute, alignToGroundID, (int)asset.GetAlignmentMode());
            bf.SetComputeVectorParam(compute, sizeID, asset.GetMinimumMaximumScale());
        }
       
        #region DataControl
        public void OriginShift(CommandBuffer bf, ComputeShader compute, int kernel){
            foreach(ICompact buffer in Buffers){
                buffer.OriginShift(bf, compute, kernel);
            }
        }

        public void GenerateChunks(CommandBuffer bf, ComputeShader compute){
            using(new ProfilingScope(bf, CalculatePositionsSampler))
            {
                SetPositionShaderData(bf, compute);
                IterateOverExistingChunks((VegetationChunk c) => GenerateChunkData(c, bf));
            }
        }

        public void IterateOverExistingChunks(Action<VegetationChunk> action){
            for(int b = 0; b < Buffers.Length; b++){
                ICompact buffer = Buffers[b];
                VegetationChunk[] chunks = buffer.ChunksForPosition;
                for(int c = 0; c < chunks.Length; c++){
                    if(chunks[c] != null)
                        action(chunks[c]);
                }
            }
        }

        public bool AnyWithNewData(){            
            for(int b = 0; b < Buffers.Length; b++){
                ICompact buffer = Buffers[b];
                VegetationChunk[] chunks = buffer.ChunksForPosition;
                for(int c = 0; c < chunks.Length; c++){
                    if(chunks[c] != null && chunks[c].NewData != null)
                        return true;
                }
            }
            return false;
        }

        public void VisibleChunksCheck(Vector3 position, Plane[] planes){
            IterateOverExistingChunks((VegetationChunk c) => c.IsVisible(position, planes));
        }
        public void DisableChunk(Vector2Int ID){
            if(PositionsChunk.TryGetValue(ID, out CameraBufferIndex ind)){
                Buffers[ind.BufferIndex].DisableChunk(ind.ChunkIndex);
                PositionsChunk.Remove(ID);
            }
        }

        public VegetationChunk GetVegetationChunk(Vector2Int ID){
            var ind = GetCameraBufferIndex(ID, out bool found);
            if(found)
                return Buffers[ind.BufferIndex].ChunksForPosition[ind.ChunkIndex];
            else
                return null;
        }

        public CameraBufferIndex GetCameraBufferIndex(Vector2Int ID, out bool found){
            if(PositionsChunk.TryGetValue(ID, out CameraBufferIndex ind)){
                found = true;
                return ind;
            }
            found = false;
            return default;
        }

        public ComputeBuffer GetBuffer(CameraBufferIndex index){
            return Buffers[index.BufferIndex].PerInstanceData;
        }

        private CameraBufferIndex PlaceChunkIntoBuffers(VegetationChunk chunk){
            for(int i = 0; i < Buffers.Length; i++){
                int subIndex = Buffers[i].SetChunk(chunk);
                if(subIndex >= 0){
                    CameraBufferIndex newInd = new CameraBufferIndex(){
                        BufferIndex = i,
                        ChunkIndex = subIndex};
                    PositionsChunk.Add(chunk.ID, newInd);
                    return newInd;
                }
            }

            Debug.LogError("No More space in buffers");
            return new CameraBufferIndex(){
                BufferIndex = -1,
                ChunkIndex = -1
            };
        }
        

        public void Dispose(){
            for(int i = 0; i < Buffers.Length; i++){
                Buffers[i].Dispose();
            }
            if(TriangleCorners != null){
                TriangleCorners.Release();
                TriangleCorners = null;
            }
        }
        #endregion
        #region Steps
        public CameraBufferIndex PlaceChunkToBuffers(VegetationChunk chunk){
            if(!PositionsChunk.TryGetValue(chunk.ID, out CameraBufferIndex ind))
                return PlaceChunkIntoBuffers(chunk);
            
            Debug.LogError("Should have the data yet, wtf");
            return ind;
        }

        public void GenerateChunkData(VegetationChunk chunk, CommandBuffer bf){
            if(chunk.NewData == null)
                return;

            if(!PositionsChunk.TryGetValue(chunk.ID, out CameraBufferIndex index))
                Debug.LogError("No index assigned");
            
            ComputeShader shader = VegetationRenderer.CalculatePositions;
            int kernel = VegetationRenderer.CalculatePositionsKernel;
            
            chunk.RecalculateTextureData();            
            Buffers[index.BufferIndex].SetGenerationData(bf, shader, kernel, index.ChunkIndex);

            ComputeBuffer MinMaxBuffer = new ComputeBuffer(2, sizeof(int));
            bf.SetBufferData(MinMaxBuffer, new int[]{int.MaxValue, int.MinValue});
            bf.SetComputeBufferParam(shader, kernel, minmaxBufferID, MinMaxBuffer);                
            
            int bladesInChunk = Mathf.CeilToInt(settings.ChunkInstancesRow / 8f);
            bf.DispatchCompute(shader, kernel, bladesInChunk, bladesInChunk, 1);
            bf.RequestAsyncReadback(MinMaxBuffer, (AsyncGPUReadbackRequest result) => LoadMinMaxBuffer(result, chunk, MinMaxBuffer));
        }
        

        public void CountAndEncapsulateData(CommandBuffer bf, IControlMatrices matrixControl, bool EnableCulling = true){
            using(new ProfilingScope(bf, InitialCompactSampler))
            {
                for(int b = 0; b < Buffers.Length; b++){
                    ICompact buffer = Buffers[b];
                    buffer.InitialCompact(bf, matrixControl.localToWorldMatrix);
                }
            }

            using(new ProfilingScope(bf, VisibilityCheckSampler))
            {
                SetVisibilityBufferData(bf, matrixControl.worldToLocalMatrix);

                for(int b = 0; b < Buffers.Length; b++){
                    ICompact buffer = Buffers[b];
                    buffer.VisibilityCheck(bf, EnableCulling);
                }
            }
        
            using(new ProfilingScope(bf, PrepareDrawCallSampler)){
                for(int i = 0; i < Buffers.Length; i++){
                    Buffers[i].CountAndCompact(bf);
                }
            }
        }

        public void DrawItems(MaterialPropertyBlock propertyBlock){
            for(int i = 0; i < Buffers.Length; i ++){
                Buffers[i].DrawItems(propertyBlock, camera);
            }
        }
        #endregion

        #region Callbacks
        private void LoadMinMaxBuffer(AsyncGPUReadbackRequest result, VegetationChunk chunk, ComputeBuffer buffer){
            if(result.done){
                NativeArray<int> data = result.GetData<int>(0);
                chunk.UpdateBounds(data[0], data[1]);
                data.Dispose();
                buffer.Release();
            }
        }
        #endregion

        #region Compute Shader Dataa
        private void SetVisibilityBufferData(CommandBuffer bf, Matrix4x4 worldToLocalMatrix){
            ComputeShader VisibilityCheck = VegetationRenderer.VisibilityCheck;

            Matrix4x4 VP = GL.GetGPUProjectionMatrix(camera.projectionMatrix, false) * camera.worldToCameraMatrix;
            GetFrustumCorners(camera);
            Triangle[] tris = GetTriangles(FrustrumCorners);
            TriangleCorners.SetData(tris);

            bf.SetComputeBufferParam(VisibilityCheck, VegetationRenderer.VisibilityCheckKernel, "_FrustrumTriangles", TriangleCorners);
            bf.SetComputeVectorParam(VisibilityCheck, cameraPositionID, camera.transform.position);//worldToLocalMatrix.MultiplyPoint(camera.transform.position));
            bf.SetComputeMatrixParam(VisibilityCheck, matrixID, VP);

            if(DepthTexture == null){
                DepthTexture = Shader.GetGlobalTexture("_CameraDepthTexture");
            }

            if(DepthTexture != null)
                VisibilityCheck.SetTextureFromGlobal(VegetationRenderer.VisibilityCheckKernel, "_DepthTexture", "_CameraDepthTexture");
        }

        void GetFrustumCorners(Camera cam)
        {
            FrustrumCorners.Clear();
            Vector3[] frustumCornersNear = new Vector3[4];

            // Near plane corners
            cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornersNear);

            // Near plane corners in world space
            for (int i = 0; i < 4; i++)
            {
                frustumCornersNear[i] = cam.transform.TransformPoint(frustumCornersNear[i]);
            }

            Vector3[] frustumCornersFar = new Vector3[4];

            // Far plane corners
            cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornersFar);

            // Far plane corners in world space
            for (int i = 0; i < 4; i++)
            {
                frustumCornersFar[i] = cam.transform.TransformPoint(frustumCornersFar[i]);
            }
            FrustrumCorners.AddRange(frustumCornersNear);
            FrustrumCorners.AddRange(frustumCornersFar);
        }

        Triangle[] GetTriangles(List<Vector3> frustumCorners)
        {
            // Define frustum triangles for each side
            Triangle[] frustumTriangles = {
                // Left side
                new(frustumCorners[0], frustumCorners[1], frustumCorners[5]),
                new(frustumCorners[0], frustumCorners[5], frustumCorners[4]),
                // Right side
                new(frustumCorners[2], frustumCorners[3], frustumCorners[7]),
                new(frustumCorners[2], frustumCorners[7], frustumCorners[6]),
                // Top side
                new(frustumCorners[1], frustumCorners[2], frustumCorners[6]),
                new(frustumCorners[1], frustumCorners[6], frustumCorners[5]),
                // Bottom side
                new (frustumCorners[0], frustumCorners[3], frustumCorners[7]),
                new (frustumCorners[0], frustumCorners[7], frustumCorners[4]),
                // Near plane
                new (frustumCorners[0], frustumCorners[1], frustumCorners[2]),
                new(frustumCorners[2], frustumCorners[3], frustumCorners[0]),
                // Far plane
                new (frustumCorners[4], frustumCorners[5], frustumCorners[6]),
                new (frustumCorners[6], frustumCorners[7], frustumCorners[4])
            };
            return frustumTriangles;
        }

        struct Triangle{
            public Vector3 C1;
            public Vector3 C2;
            public Vector3 C3;
            public Triangle(Vector3 c1, Vector3 c2, Vector3 c3) { 
                C1 = c1;
                C2 = c2;
                C3 = c3;
            }
        }
        #endregion

        public void GizmosDraw(){
            IterateOverExistingChunks((VegetationChunk c) => c.DrawGizmos());

            foreach(ICompact buffer in Buffers){
                buffer.DrawBounds();
            }
        }
    }
}