using NUnit.Framework;
using UnityEngine;
using Unity.Mathematics;
using System.Reflection;

namespace sapra.InfiniteLands.Tests
{
    public class MeshMakerTests
    {
        private MeshMaker _meshMaker;
        private IGraph _mockGraph;
        private MeshSettings _mockSettings;
        private DataManager manager;

        [SetUp]
        public void Setup()
        {
            _meshMaker = new GameObject("MeshMaker").AddComponent<MeshMaker>();
            manager = new DataManager();
            _mockGraph = new MockGraph(); // Create a mock implementation of IGraph
            _mockSettings = new MeshSettings(); // Create appropriate test settings
            _meshMaker.Initialize(_mockGraph, _mockSettings);
        }

        [TearDown]
        public void Teardown()
        {
            _meshMaker.Dispose();
            Object.DestroyImmediate(_meshMaker.gameObject);
            manager.Dispose(default);
            
        }

        [Test]
        public void Initialize_SetsUpMeshMakerProperly()
        {
            // Assert
            Assert.AreEqual(_mockGraph, _meshMaker.graph);
            Assert.AreEqual(_mockSettings, _meshMaker.settings);
        }

        private static float[] vertPos = {
            0, float.MinValue, float.MaxValue, float.NaN,
        };
        private static int[] resolutions = {
            0, 10, int.MinValue, 1000,
        };
        [Test]
        public void MeshSchedule_AddsChunksToProcess([ValueSource(nameof(vertPos))]float position, [ValueSource(nameof(resolutions))]int resolution)
        {
            // Arrange
            MeshSettings settings = new MeshSettings(){Resolution = resolution};
            var chunk1 = new ChunkData(default, settings);
            chunk1.worldFinalData = GetWorldFinalData(manager, new Vector2Int(0,1), settings.Resolution, position);
            var chunk2 = new ChunkData(default, settings);
            chunk2.worldFinalData = GetWorldFinalData(manager, new Vector2Int(0,1), settings.Resolution, position);
            _meshMaker.MaxMeshesPerFrame = 2;
            _meshMaker.AddChunk(chunk1);
            _meshMaker.AddChunk(chunk2);

            MethodInfo method = _meshMaker.GetType().GetMethod("MeshSchedule", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(_meshMaker, new object[] {  });

            // Assert
            Assert.AreEqual(0, _meshMaker.GetChunksToProcess.Count); // All chunks scheduled
            Assert.AreEqual(1, _meshMaker.GetMeshGenerationCalls.Count); // Check one generation call
            
            _meshMaker.UpdateRequests(true);
        }

        [Test]
        public void MeshSchedule_AddsChunksToProcessNulls()
        {
            // Arrange
            var chunk1 = new ChunkData(default, default);
            _meshMaker.MaxMeshesPerFrame = 2;
            _meshMaker.AddChunk(chunk1);

            // Assert
            Assert.AreEqual(0, _meshMaker.GetChunksToProcess.Count); // Chunk not scheduled
        }
        private WorldFinalData GetWorldFinalData(DataManager manager, Vector2Int minMaxValue, int Resolution, float valToAssign){
            Resolution = Mathf.Max(Resolution, 1);
            var values = manager.GetReturnableArray<Vertex>("test", (Resolution+1)*(Resolution+1));
            var minmax = manager.GetReturnableArray<int>("test", 2);
            minmax[0] = minMaxValue.x;
            minmax[1] = minMaxValue.y;

            for(int x = 0; x <= Resolution; x++){
                for(int y = 0; y <= Resolution; y++){
                    int index = x+y*(Resolution+1);
                    values[index] = new Vertex(){
                        position = new float3(x,valToAssign,y),
                        normal = Vector3.up
                    };
                }
            }

            WorldFinalData worldFinalData = new WorldFinalData(manager, "test", minmax, values, minMaxValue, default);
            return worldFinalData;
        }

        [Test]
        public void Consolidate_ProcessesGeneratedMeshes([ValueSource(nameof(vertPos))]float position, [ValueSource(nameof(resolutions))]int resolution)
        {
            // Arrange
            MeshSettings settings = new MeshSettings(){Resolution = resolution};
            var chunk = new ChunkData(default, settings);
            chunk.worldFinalData = GetWorldFinalData(manager, new Vector2Int(0,1), settings.Resolution, position);
            _meshMaker.AddChunk(chunk);
            _meshMaker.UpdateRequests(true);

            // Act
            MethodInfo method = _meshMaker.GetType().GetMethod("Consolidate",  BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(_meshMaker, new object[] { true });

            // Assert
            Assert.AreEqual(0, _meshMaker.GetMeshGenerationCalls.Count);
            Assert.IsTrue(_meshMaker.GetMeshResults.ContainsKey(chunk.terrainConfig.ID));
        }


        [Test]
        public void PhysicsConsolidate_ProcessesPhysicsResults([ValueSource(nameof(vertPos))]float position, [ValueSource(nameof(resolutions))]int resolution)
        {
            // Arrange
            MeshSettings settings = new MeshSettings(){Resolution = resolution};
            var chunk1 = new ChunkData(new TerrainConfig(){ID = new Vector3Int(0,0,0)}, settings);
            var chunk2 = new ChunkData(new TerrainConfig(){ID = new Vector3Int(0,0,1)}, settings);

            _meshMaker.MaxMeshesPerFrame = 2;
            _meshMaker.MaxLODWithPhysics = 0;
            chunk1.worldFinalData = GetWorldFinalData(manager, new Vector2Int(0,1), settings.Resolution, position);
            chunk2.worldFinalData = GetWorldFinalData(manager, new Vector2Int(0,1), settings.Resolution, position);

            MeshResult result1 = null;
            MeshResult result2 = null;
            _meshMaker.onProcessDone += (MeshResult res)=>{
                if(res.ID.z == 0)
                    result1 = res;
                else
                    result2 = res;
            };

 
            _meshMaker.AddChunk(chunk1);
            _meshMaker.AddChunk(chunk2);

            _meshMaker.UpdateRequests(true);
   
            Assert.AreEqual(0, _meshMaker.GetPhysicsCalls.Count);
            Assert.IsNotNull(result1);
            Assert.IsTrue(result1.PhysicsBaked);

            Assert.IsNotNull(result2);
            Assert.IsFalse(result2.PhysicsBaked);
        }
    }
}