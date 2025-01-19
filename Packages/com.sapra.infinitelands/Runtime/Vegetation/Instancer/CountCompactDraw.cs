using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace sapra.InfiniteLands{
    public class CountCompactDraw : ICompact{
        private static readonly int CounterDataSize = sizeof(uint)*4;
        private static readonly int 
            countersID = Shader.PropertyToID("_Counters"),
            shadowCountersID = Shader.PropertyToID("_ShadowCounters"),
            shadowLodOffsetID = Shader.PropertyToID("_ShadowLodOffset"),
            perInstanceDataID = Shader.PropertyToID("_PerInstanceData"),
            targetLodsID = Shader.PropertyToID("_TargetLODs"),
            indexOffsetID = Shader.PropertyToID("_IndexOffset"),
            shadowIndicesID = Shader.PropertyToID("_ShadowIndices"),
            argumentsID = Shader.PropertyToID("_Arguments"),
            subMeshCountID = Shader.PropertyToID("_SubMeshCount"),
            maxInstancesID = Shader.PropertyToID("_MaxInstances"),
            lodValueID = Shader.PropertyToID("_LODValue"),
            lodCountID = Shader.PropertyToID("_LODCount"),
            indicesID = Shader.PropertyToID("_Indices");

        public ComputeBuffer PerInstanceData{get; private set;}
        public VegetationChunk[] ChunksForPosition{get; private set;}
        private List<int> FreeIndices;

        private ComputeBuffer LodBuffer;
        private ComputeBuffer PreVisibleInstances;
        private ComputeBuffer InstanceIndices;
        private ComputeBuffer Counts;
        private GraphicsBuffer Arguments;

        private ComputeBuffer ShadowCounts;
        private ComputeBuffer ShadowIndices;
        private GraphicsBuffer ShadowArguments;

        private int LODLength;
        private int MaxSubMeshCount;

        private VegetationSettings settings;

        private bool ShouldCreateCustomShadows;

        private List<Vector3> Corners;
        private List<int> Skips;
        private Bounds RenderBounds;
        private IAsset Asset;

        private int MaxShadowLOD;
        private int ShadowsLODOffset;
        private MeshLOD[] Lods;

        private int TotalInstancesAdded;
        private LocalKeyword CULLING;

        public CountCompactDraw(IAsset asset, VegetationSettings vegSettings,
            int TotalChunksCount, List<GraphicsBuffer.IndirectDrawIndexedArgs> arguments, MeshLOD[] lods = null,
            bool customShadows = false, int maxShadowLOD = 0, int lodLength = 1, int maxSubMeshCount = 1, int shadowLODOffset = 0){

            Asset = asset;
            Lods = lods;
            settings = vegSettings;
            LODLength = lodLength;
            MaxShadowLOD = maxShadowLOD;
            ChunksForPosition = new VegetationChunk[TotalChunksCount];
            FreeIndices = new();
            for(int i = 0; i < TotalChunksCount; i++){
                FreeIndices.Add(i);
            }

            CULLING = new LocalKeyword(VegetationRenderer.VisibilityCheck, "CULLING");

            Corners = new();
            Skips = new();
            ShadowsLODOffset = shadowLODOffset;
            MaxSubMeshCount = maxSubMeshCount;
            ShouldCreateCustomShadows = customShadows;

            int MaxLength = vegSettings.ChunkInstancesRow*vegSettings.ChunkInstancesRow*TotalChunksCount; 
            PerInstanceData = new ComputeBuffer(MaxLength, InstanceData.size, ComputeBufferType.Structured);
            PerInstanceData.name = string.Format("{0}/Instances", asset.name);
            LodBuffer = new ComputeBuffer(MaxLength, sizeof(int), ComputeBufferType.Structured);
            LodBuffer.name = string.Format("{0}/Lods", asset.name);
            InstanceIndices = new ComputeBuffer(MaxLength, sizeof(int), ComputeBufferType.Structured);
            InstanceIndices.name = string.Format("{0}/InstanceIndices", asset.name);
            PreVisibleInstances = new ComputeBuffer(MaxLength, sizeof(int), ComputeBufferType.Structured);
            PreVisibleInstances.name = string.Format("{0}/PreInstanceIndices", asset.name);

            if(arguments.Count > 0){
                Counts = new ComputeBuffer(LODLength, CounterDataSize,  ComputeBufferType.Structured);
                Counts.name = string.Format("{0}/Counters", asset.name);

                Arguments = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, arguments.Count, GraphicsBuffer.IndirectDrawIndexedArgs.size);
                Arguments.name = string.Format("{0}/Arguments", asset.name);

                Arguments.SetData(arguments);
            }
            
            if(ShouldCreateCustomShadows && arguments.Count > 0){
                ShadowCounts = new ComputeBuffer(LODLength, CounterDataSize,  ComputeBufferType.Structured);                
                ShadowCounts.name = string.Format("{0}/ShadowCounters", asset.name);

                ShadowIndices = new ComputeBuffer(MaxLength, sizeof(int), ComputeBufferType.Structured);
                ShadowIndices.name = string.Format("{0}/ShadowIndices", asset.name);

                ShadowArguments = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, arguments.Count, GraphicsBuffer.IndirectDrawIndexedArgs.size);
                ShadowArguments.name = string.Format("{0}/ShadowArguments", asset.name);

                ShadowArguments.SetData(arguments);
                
            }
        }

        public void InitialCompact(CommandBuffer bf, Matrix4x4 localToWorld)
        {
            Corners.Clear();
            var compute = VegetationRenderer.FillArguments;
            var kernel = VegetationRenderer.InitialCompactKernel;     
            compute.GetKernelThreadGroupSizes(kernel, out uint x, out _, out _);
            int length = ChunksForPosition.Length;
            
            Skips.Clear();
            int chunksToSkip = 0;
            for(int i = 0; i < length; i++)
            {
                VegetationChunk chunk = ChunksForPosition[i];
                //if the chunk is null, dispatch the call
                if(chunk != null && chunk.Visible){
                    Corners.Add(chunk.maxBounds);
                    Corners.Add(chunk.minBounds);
                    Skips.Add(chunksToSkip);
                }else{
                    chunksToSkip++;
                }
            }

            int chunksToAdd = Skips.Count;
            if(chunksToAdd > 0){
                bf.SetBufferData(InstanceIndices, Skips);
                bf.SetComputeBufferParam(compute, kernel, "_Skips", InstanceIndices);
                bf.SetComputeIntParam(compute, "_InstancesPerChunk", settings.ChunkInstances);
                //launch the call
                int bladesInChunk = Mathf.CeilToInt(chunksToAdd*settings.ChunkInstances / (float)x);           

                bf.SetComputeIntParam(compute, maxInstancesID, chunksToAdd*settings.ChunkInstances);
                bf.SetComputeBufferParam(compute, kernel, "_PreVisibleInstances", PreVisibleInstances);
                bf.DispatchCompute(compute,kernel, bladesInChunk, 1, 1);
            }

            TotalInstancesAdded = chunksToAdd*settings.ChunkInstances;
            if(TotalInstancesAdded > 0)
                RenderBounds = GeometryUtility.CalculateBounds(Corners.ToArray(), localToWorld);
        }
        
        public void VisibilityCheck(CommandBuffer bf, bool EnableCulling){
            var compute = VegetationRenderer.VisibilityCheck;
            var kernel = VegetationRenderer.VisibilityCheckKernel;   
            if(TotalInstancesAdded <= 0)
                return;

            compute.GetKernelThreadGroupSizes(kernel, out uint x, out uint _, out _);

            int bladesX = Mathf.CeilToInt(TotalInstancesAdded / (float)x);
            
            bf.SetComputeIntParam(compute, "_TotalInstancesAdded", TotalInstancesAdded);
            bf.SetComputeBufferParam(compute, kernel, "_PreVisibleInstances", PreVisibleInstances);
            bf.SetComputeBufferParam(compute, kernel, perInstanceDataID, PerInstanceData);
            bf.SetComputeBufferParam(compute, kernel, targetLodsID, LodBuffer);
            bf.SetKeyword(compute, CULLING, EnableCulling);
            bf.DispatchCompute(compute,kernel, bladesX, 1, 1);   
        }

        public void OriginShift(CommandBuffer bf,ComputeShader compute, int kernel){
            int totalInstances = settings.ChunkInstancesRow*settings.ChunkInstancesRow*ChunksForPosition.Length; 
            compute.GetKernelThreadGroupSizes(kernel, out uint x, out _, out _);
            
            bf.SetComputeBufferParam(compute, kernel, perInstanceDataID, PerInstanceData);
            bf.SetComputeIntParam(compute, "_ChunkInstancesRow", totalInstances);
            int bladesInChunk = Mathf.CeilToInt(totalInstances / (float)x);
            bf.DispatchCompute(compute, kernel, bladesInChunk, 1, 1);
        }

        public void CountAndCompact(CommandBuffer bf){    
            ComputeShader compute = VegetationRenderer.FillArguments;
            if(TotalInstancesAdded <= 0)
                return;

            Reset(bf, compute);
            Count(bf, compute);
            Sum(bf, compute);
            Compact(bf, compute);
            FillArguments(bf, compute);
        }

        #region CountAndCompact
        private void Reset(CommandBuffer bf, ComputeShader compute){
            int ResetKernel = VegetationRenderer.ResetKernel;     
            bf.SetComputeBufferParam(compute, ResetKernel, countersID, Counts);
            bf.DispatchCompute(compute,ResetKernel, LODLength, 1, 1);

            if(ShouldCreateCustomShadows){    //Reset Shadows    
                bf.SetComputeBufferParam(compute, ResetKernel, countersID, ShadowCounts);
                bf.DispatchCompute(compute,ResetKernel, LODLength, 1, 1);
            }
        }

        private void Count(CommandBuffer bf, ComputeShader compute){
            int CountKernel = VegetationRenderer.CountKernel;     
            
            compute.GetKernelThreadGroupSizes(CountKernel, out uint x, out _, out _);
            bf.SetComputeIntParam(compute, lodCountID, LODLength);
            bf.SetKeyword(compute, VegetationRenderer.ShadowKeyword, ShouldCreateCustomShadows);
            bf.SetComputeIntParam(compute, subMeshCountID, MaxSubMeshCount);
            bf.SetComputeIntParam(compute, maxInstancesID, TotalInstancesAdded);

            bf.SetComputeBufferParam(compute, CountKernel, targetLodsID, LodBuffer);
            bf.SetComputeBufferParam(compute, CountKernel, countersID, Counts);

            if(ShouldCreateCustomShadows){
                bf.SetComputeBufferParam(compute, CountKernel, shadowCountersID, ShadowCounts);
                bf.SetComputeIntParam(compute, shadowLodOffsetID, ShadowsLODOffset);
            }
            int bladesInChunk = Mathf.CeilToInt(TotalInstancesAdded / (float)x);      
            bf.DispatchCompute(compute,CountKernel, bladesInChunk, 1, 1);
        }

        private void Sum(CommandBuffer bf, ComputeShader compute){
            int SumKernel = VegetationRenderer.SumKernel;     
            bf.SetComputeBufferParam(compute, SumKernel, countersID, Counts);
            bf.DispatchCompute(compute,SumKernel, 1, 1, 1);

            if(ShouldCreateCustomShadows){//Sum Shadows
                bf.SetComputeBufferParam(compute, SumKernel, countersID, ShadowCounts);
                bf.DispatchCompute(compute,SumKernel, 1, 1, 1);
            }
        }

        private void Compact(CommandBuffer bf, ComputeShader compute){
            int CompactKernel = VegetationRenderer.CompactKernel;    
            compute.GetKernelThreadGroupSizes(CompactKernel, out uint x, out _, out _);

            bf.SetComputeBufferParam(compute,CompactKernel, "_PreVisibleInstances", PreVisibleInstances);
            bf.SetComputeBufferParam(compute,CompactKernel, targetLodsID, LodBuffer);
            bf.SetComputeBufferParam(compute,CompactKernel, countersID, Counts);
            bf.SetComputeBufferParam(compute,CompactKernel, indicesID, InstanceIndices);
            if(ShouldCreateCustomShadows){
                bf.SetComputeBufferParam(compute,CompactKernel, shadowCountersID, ShadowCounts);
                bf.SetComputeBufferParam(compute,CompactKernel, shadowIndicesID, ShadowIndices);
            }

            int bladesInChunk = Mathf.CeilToInt(TotalInstancesAdded / (float)x);                   
            bf.DispatchCompute(compute,CompactKernel, bladesInChunk, 1, 1);
        }
        private void FillArguments(CommandBuffer bf, ComputeShader compute){
            int FillKernel = VegetationRenderer.FillKernel;     
            bf.SetComputeBufferParam(compute,FillKernel, countersID, Counts);
            bf.SetComputeBufferParam(compute,FillKernel,argumentsID, Arguments);
            bf.DispatchCompute(compute,FillKernel, LODLength,Mathf.Max(MaxSubMeshCount, 1),1); 
            if(ShouldCreateCustomShadows){
                bf.SetComputeBufferParam(compute, FillKernel, countersID, ShadowCounts);
                bf.SetComputeBufferParam(compute,FillKernel,argumentsID, ShadowArguments);
                bf.DispatchCompute(compute,FillKernel, LODLength,Mathf.Max(MaxSubMeshCount, 1),1); 
            }
        }

        public void SetGenerationData(CommandBuffer bf,ComputeShader shader, int kernel, int ChunkIndex){
            bf.SetComputeBufferParam(shader, kernel, perInstanceDataID, PerInstanceData);
            bf.SetComputeIntParam(shader, indexOffsetID, ChunkIndex*settings.ChunkInstancesRow*settings.ChunkInstancesRow);

            ChunksForPosition[ChunkIndex].SetComputeShaderData(bf, shader, kernel, Asset, settings.ChunkInstancesRow, settings.Ratio);
        }
        #endregion

        public int SetChunk(VegetationChunk chunk){
            if(FreeIndices.Count <= 0)
                return -1;
            
            int index = FreeIndices[0];
            ChunksForPosition[index] = chunk;
            FreeIndices.RemoveAt(0);
            return index;
        }


        public void DrawItems(MaterialPropertyBlock propertyBlock, Camera cam){
            if(TotalInstancesAdded <= 0)
                return;

            propertyBlock.Clear();
            
            propertyBlock.SetBuffer("_PreVisibleInstances", PreVisibleInstances);
            propertyBlock.SetBuffer(targetLodsID, LodBuffer);
            propertyBlock.SetBuffer(perInstanceDataID, PerInstanceData);
            propertyBlock.SetInt(lodCountID, LODLength);

            for(int l = 0; l < LODLength; l++){
                MeshLOD lod = Lods[l];
                if(!lod.valid){
                    continue;
                }
                propertyBlock.SetInt(lodValueID, l);
                for(int lm = 0; lm < lod.SubMeshCount; lm++){
                    propertyBlock.SetBuffer(indicesID, InstanceIndices);
                    propertyBlock.SetBuffer(countersID, Counts);
                    propertyBlock.SetInt(shadowLodOffsetID,-1);
                    RenderParams renderParams = new RenderParams(lod.materials[lm]){
                        matProps = propertyBlock,
                        receiveShadows = true,
                        //camera = cam,
                        shadowCastingMode = ShadowCastingMode.Off,
                        worldBounds = RenderBounds,
                    };

                    Graphics.RenderMeshIndirect(renderParams, lod.mesh, Arguments, 1, l*MaxSubMeshCount+lm);

                    if(ShouldCreateCustomShadows && l < MaxShadowLOD){
                        propertyBlock.SetBuffer(indicesID, ShadowIndices);
                        propertyBlock.SetBuffer(countersID, ShadowCounts);
                        propertyBlock.SetInt(shadowLodOffsetID,ShadowsLODOffset);
                        renderParams = new RenderParams(lod.materials[lm]){
                            matProps = propertyBlock,
                            receiveShadows = false,
                            //camera = cam,
                            shadowCastingMode = ShadowCastingMode.ShadowsOnly,
                            worldBounds = RenderBounds
                        };
                        
                        Graphics.RenderMeshIndirect(renderParams, lod.mesh, ShadowArguments, 1, l*MaxSubMeshCount+lm);
                    } 
                }
            }
        }
        public void Dispose(){
            if (Arguments != null)
            {
                Arguments.Release();
                Arguments = null;
            }
            if(PerInstanceData != null){
                PerInstanceData.Release();
                PerInstanceData = null;
            }
            if(LodBuffer!= null){
                LodBuffer.Release();
                LodBuffer = null;
            }
            if(InstanceIndices != null){
                InstanceIndices.Release();
                InstanceIndices = null;
            }
            if(PreVisibleInstances != null){
                PreVisibleInstances.Release();
                PreVisibleInstances = null;
            }
            if(Counts != null){
                Counts.Release();
                Counts = null;
            }  
            if(ShadowCounts != null){
                ShadowCounts.Release();
                ShadowCounts = null;
            }            
            if(ShadowIndices != null){
                ShadowIndices.Release();
                ShadowIndices = null;
            }            
            if(ShadowArguments != null){
                ShadowArguments.Release();
                ShadowArguments = null;
            }            
        }

        public void DrawBounds(){
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(RenderBounds.center, RenderBounds.size);
        }

        public void DisableChunk(int chunkIndex)
        {
            FreeIndices.Add(chunkIndex);
            ChunksForPosition[chunkIndex] = null;
        }
    }
}