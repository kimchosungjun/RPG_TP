using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace sapra.InfiniteLands{
    [ExecuteAlways]
    public class MeshMaker : ChunkProcessor<ChunkData>,IGenerate<MeshResult>
    {
        public struct PhysicsResult{
            public NativeArray<int> meshIDs;
            public List<MeshResult> results;
            public JobHandle handle;
        }

        private ObjectPool<Mesh> _meshPool;
        public Action<MeshResult> onProcessDone{get;set;}
        public Action<MeshResult> onProcessRemoved{get;set;}
        public Action<IGraph, MeshSettings> onReload { get; set; }

        public IGraph graph{get; private set;}
        public MeshSettings settings{get; private set;}

        [Min(1)] public int MaxMeshesPerFrame = 1;
        [Min(-1)] public int MaxLODWithPhysics = 0;

        private Dictionary<Vector3Int, MeshResult> MeshResults = new();
        private List<ChunkData> ChunksToProcess = new();
        private List<MeshGenerationData> MeshGenerationCalls = new();
        private List<MeshResult> PhysicsToProcess = new();
        private List<PhysicsResult> PhysicsCalls = new();

        private HashSet<Vector3Int> RemoveAfterGeneration = new();

        public IReadOnlyDictionary<Vector3Int, MeshResult> GetMeshResults => MeshResults;
        public IReadOnlyList<ChunkData> GetChunksToProcess => ChunksToProcess;
        public IReadOnlyList<MeshResult> GetPhysicsToProcess => PhysicsToProcess;
        public IReadOnlyList<MeshGenerationData> GetMeshGenerationCalls => MeshGenerationCalls;
        public IReadOnlyList<PhysicsResult> GetPhysicsCalls => PhysicsCalls;

        public override void Initialize(IGraph generator, MeshSettings settings)
        {
            this.graph = generator;
            this.settings = settings;

            if(_meshPool == null)
                _meshPool = new ObjectPool<Mesh>(MeshGenerator.CreateMesh, actionOnGet: MeshGenerator.ReuseMesh, actionOnDestroy: AdaptiveDestroy);
            onReload?.Invoke(generator, settings);
        }
        protected override void OnProcessAdded(ChunkData chunk) => AddChunk(chunk);
        
        protected override void OnProcessRemoved(ChunkData chunk) => RemoveChunk(chunk);

        public void AddChunk(ChunkData chunk){
            if(chunk.worldFinalData == null)
                return;

            chunk.worldFinalData.AddProcessor(this);
            ChunksToProcess.Add(chunk);
            if(chunk.InstantGeneration)
                UpdateRequests(true);
        }

        public void RemoveChunk(ChunkData chunk)
        {
            var wasBeeingProcessed = MeshGenerationCalls.SelectMany(a => a.generatedChunks).Any(a => a.terrainConfig.ID.Equals(chunk.terrainConfig.ID));
            wasBeeingProcessed = wasBeeingProcessed || PhysicsCalls.SelectMany(a => a.results).Any(a => a.ID.Equals(chunk.terrainConfig.ID));
            if(wasBeeingProcessed){
                RemoveAfterGeneration.Add(chunk.terrainConfig.ID);
            }

            var chunkToProcess = ChunksToProcess.Where(a => a.terrainConfig.ID.Equals(chunk.terrainConfig.ID)).ToArray();
            foreach(var result in chunkToProcess){
                result.worldFinalData.RemoveProcessor(this);
                ChunksToProcess.Remove(result);
            }

            var physicsToProcess = PhysicsToProcess.Where(a => a.ID.Equals(chunk.terrainConfig.ID)).ToArray();
            foreach(var result in physicsToProcess){
                _meshPool.Release(result.mesh);
                PhysicsToProcess.Remove(result);
            }
            
            if(MeshResults.TryGetValue(chunk.terrainConfig.ID, out MeshResult finalMesh)){
                onProcessRemoved?.Invoke(finalMesh);
                _meshPool.Release(finalMesh.mesh);
                MeshResults.Remove(chunk.terrainConfig.ID);
            }
        }

        void Update(){
            UpdateRequests(false);
        }

        public void UpdateRequests(bool instantGeneration){
            MeshSchedule();
            Consolidate(instantGeneration);
            PhysicsSchedule();
            PhysicsConsolidate(instantGeneration);
        }

        void MeshSchedule(){
            if(ChunksToProcess.Count <= 0)
                return;

            List<ChunkData> subset = ChunksToProcess.GetRange(0, Mathf.Min(ChunksToProcess.Count, MaxMeshesPerFrame));
            MeshGenerationData _meshData = MeshGenerator.ScheduleParallel(subset.ToArray());
            MeshGenerationCalls.Add(_meshData);
            ChunksToProcess.RemoveRange(0, subset.Count);
        }

        void Consolidate(bool instantGeneration){
            if(MeshGenerationCalls.Count <= 0)
                return;
            
            for(int d = MeshGenerationCalls.Count-1; d >= 0; d--){
                MeshGenerationData _meshData = MeshGenerationCalls[d];
                if(_meshData.handle.IsCompleted || instantGeneration){
                    _meshData.handle.Complete();
                    
                    Mesh[] selection = new Mesh[_meshData.meshDataArray.Length];
                    for(int i = 0; i < selection.Length; i++){
                        selection[i] = _meshPool.Get();
                    }
                
                    Mesh[] mesh = MeshGenerator.Consolidate(_meshData, selection);
                    
                    for(int m = 0; m < mesh.Length; m++){
                        ChunkData chunk = _meshData.generatedChunks[m];
                        MeshResult finalMesh = new MeshResult(chunk.terrainConfig.ID, mesh[m], false);
                        if(chunk.terrainConfig.ID.z <= MaxLODWithPhysics){
                            PhysicsToProcess.Add(finalMesh);
                        }
                        else
                            InformMeshCreated(finalMesh);

                        chunk.worldFinalData.RemoveProcessor(this);
                    }
                    MeshGenerationCalls.RemoveAt(d);
                }
            }
        }

        void PhysicsSchedule(){
            if(PhysicsToProcess.Count <= 0)
                return;
            
            NativeArray<int> meshIDs = new NativeArray<int>(PhysicsToProcess.Select(a => a.mesh.GetInstanceID()).ToArray(), Allocator.Persistent);
            JobHandle processPhysics = MeshPhysicsJob.ScheduleParallel(meshIDs, default);
            PhysicsResult result = new PhysicsResult(){
                meshIDs = meshIDs,
                handle = processPhysics,
                results = new List<MeshResult>(PhysicsToProcess)            
            };
            meshIDs.Dispose(processPhysics);
            PhysicsCalls.Add(result);
            PhysicsToProcess.Clear();
        }

        void PhysicsConsolidate(bool instantGeneration){
            if(PhysicsCalls.Count <= 0)
                return;
            
            for(int d = PhysicsCalls.Count-1; d >=0; d--){
                PhysicsResult _meshData = PhysicsCalls[d];
                if(_meshData.handle.IsCompleted || instantGeneration){
                    _meshData.handle.Complete();

                    for(int m = 0; m < _meshData.results.Count; m++){
                        _meshData.results[m].PhysicsBaked = true;
                        InformMeshCreated(_meshData.results[m]);
                    }
                    PhysicsCalls.RemoveAt(d);
                }
            }
        }
        private void InformMeshCreated(MeshResult finalMesh){
            if(RemoveAfterGeneration.Contains(finalMesh.ID)){
                _meshPool.Release(finalMesh.mesh);
                RemoveAfterGeneration.Remove(finalMesh.ID);
            }
            else{
                MeshResults.TryAdd(finalMesh.ID, finalMesh);
                onProcessDone?.Invoke(finalMesh);
            }
        }

        public override void Dispose()
        {   
            foreach(var result in PhysicsToProcess){
                _meshPool.Release(result.mesh);
            }

            foreach(var result in ChunksToProcess){
                result.worldFinalData.RemoveProcessor(this);            
            }

            foreach(var result in PhysicsCalls){
                result.handle.Complete();
                foreach(var meshes in result.results){
                    _meshPool.Release(meshes.mesh);
                }           
            }

            foreach(var result in MeshGenerationCalls){
                result.handle.Complete();    
                foreach(var chunk in result.generatedChunks){
                    chunk.worldFinalData.RemoveProcessor(this);            
                }
            }

            foreach(var result in MeshResults){
                _meshPool.Release(result.Value.mesh);
            }

            MeshResults.Clear();
            PhysicsToProcess.Clear();
            ChunksToProcess.Clear();
            MeshGenerationCalls.Clear();
            PhysicsCalls.Clear();
            RemoveAfterGeneration.Clear();

            if(_meshPool != null){    
                if(_meshPool.CountActive > 0)
                    Debug.LogWarning("Not all meshes have been returned");
                _meshPool.Dispose();
            }
        }

    }
}