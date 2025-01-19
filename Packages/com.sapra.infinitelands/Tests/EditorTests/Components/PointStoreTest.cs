using UnityEngine;
using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;

namespace sapra.InfiniteLands.Tests
{
    public class PointStoreTest
    {    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        private static Vector3[] _positions = {
            new Vector3(0,0,0), 
            new Vector3(1010,0,0), 
            new Vector3(123123,-12312,23423),
        };
        private static Vector3[] _rotations = {
            new Vector3(0,0,0),
            new Vector3(0,0,45),
            new Vector3(79,0,-45),
            new Vector3(1233123,2123,-12312),
        };

        private static float[] _meshSizes = {
            10,1000,123.232f
        };

        private static Vector3Int[] _chunkIndex = {
            new Vector3Int(0,0,0), 
            new Vector3Int(1,1,0),
            new Vector3Int(1,-30,23),
            new Vector3Int(2,5,3),
            new Vector3Int(1231,-1233,4),
        };

        private class FakeMatrix : IControlMatrices
        {
            public Matrix4x4 localToWorldMatrix{get; private set;}
            public Matrix4x4 worldToLocalMatrix{get; private set;}
            public void SetMatrix(Matrix4x4 matrix){
                localToWorldMatrix = matrix.inverse;
                worldToLocalMatrix = matrix;
            }
        }
        
        [Test]
        public void StoreFindsChunksQuad(
            [ValueSource(nameof(_chunkIndex))]Vector3Int terrainIndex, 
            [ValueSource(nameof(_meshSizes))]float MeshSize,
            [ValueSource(nameof(_positions))]Vector3 currentOrigin,
            [ValueSource(nameof(_rotations))]Vector3 currentRotation)
        {
            var origin = new GameObject();
            var store = origin.AddComponent<PointStore>();
            var quad = origin.AddComponent<QuadChunk>();
            StoreFindsChunks(terrainIndex, MeshSize, quad, store, currentOrigin, currentRotation); 
            UnityEngine.Object.DestroyImmediate(origin.gameObject); 
        }

        [Test]
        public void StoreFindsChunksSingle(
            [ValueSource(nameof(_chunkIndex))]Vector3Int terrainIndex, 
            [ValueSource(nameof(_meshSizes))]float MeshSize,
            [ValueSource(nameof(_positions))]Vector3 currentOrigin,
            [ValueSource(nameof(_rotations))]Vector3 currentRotation)
        {
            var origin = new GameObject();
            var store = origin.AddComponent<PointStore>();
            var quad = origin.AddComponent<SingleChunk>();
            StoreFindsChunks(terrainIndex, MeshSize, quad, store, currentOrigin, currentRotation); 
            UnityEngine.Object.DestroyImmediate(origin.gameObject); 
        }

        public void StoreFindsChunks(Vector3Int terrainIndex, float MeshSize, ISetupChunkHirearchy chunk, PointStore store, Vector3 origin, Vector3 rotation)
        {
            MeshSettings settings = new MeshSettings(){
                MeshScale = MeshSize,
                Resolution = 100,
            };
            MeshSettings chunkSettings = chunk.GetMeshSettingsFromID(settings, terrainIndex);
            TerrainConfig config = new TerrainConfig(terrainIndex, new Vector2(0,1), Vector3.up, chunkSettings.MeshScale);

            FakeMatrix matrix = new FakeMatrix();
            matrix.SetMatrix(Matrix4x4.TRS(origin, Quaternion.Euler(rotation), Vector3.one));

            store.InitializePointStore(settings, matrix, chunk);
            store.ConfigureOffset(Vector2.one*settings.MeshScale/2.0f, settings.MeshScale);

            ChunkData tempData = new ChunkData(config, chunkSettings);
            var data = GenerateCoordinateData(settings.Resolution);
            CoordinateDataHolder coordinateData = new CoordinateDataHolder(data, default, "");
            tempData.coordinateData = coordinateData;
            store.AddChunk(tempData);
            store.AddChunk(tempData);

            //Center
            AssertChunkRetrieve(config.Position, config, store, matrix, tempData,chunkSettings.Resolution/2,chunkSettings.Resolution/2);

            var unit = chunkSettings.MeshScale/(chunkSettings.Resolution+1);
            var half = -unit+chunkSettings.MeshScale/2.0f;

            //Corners
            AssertChunkRetrieve(config.Position+new Vector3(-half, 0, -half), config, store, matrix, tempData,01, 01);
            AssertChunkRetrieve(config.Position+new Vector3(half, 0, half), config, store, matrix, tempData,chunkSettings.Resolution-1,chunkSettings.Resolution-1);
            AssertChunkRetrieve(config.Position+new Vector3(-half, 0, half), config, store, matrix, tempData,01,chunkSettings.Resolution-1);
            AssertChunkRetrieve(config.Position+new Vector3(half, 0, -half), config, store, matrix, tempData,chunkSettings.Resolution-1,01);

            //store.RemoveChunk(tempData);
            store.RemoveChunk(tempData);
            data.Dispose();
        }

        private void AssertChunkRetrieve(Vector3 position, TerrainConfig config, PointStore store, IControlMatrices matrix, ChunkData og, int x, int z){
            Debug.LogFormat("Looking at {0}", position);
            Vector3 tp = matrix.localToWorldMatrix.MultiplyPoint(position);
            ChunkData retrieve = store.GetChunkDataAtPosition(tp);
            Assert.AreEqual(og, retrieve);
            Assert.AreEqual(retrieve.terrainConfig, config);

            CoordinateData data = store.GetCoordinateDataAtPosition(tp, out bool found, false, false);
            Assert.AreEqual(x,data.position.x);
            Assert.AreEqual(z,data.position.z);
            Assert.IsTrue(found);
        }

        public NativeArray<CoordinateData> GenerateCoordinateData(int resolution){
            NativeArray<CoordinateData> data = new NativeArray<CoordinateData>((resolution+1)*(resolution+1), Allocator.TempJob);
            for(int x = 0; x <= resolution; x++){
                for(int z = 0; z <= resolution; z++){
                    CoordinateData newData = new CoordinateData(){
                        position = new float3(x, x+z*resolution, z)
                    };
                    data[x+z*(resolution+1)] = newData;
                }
            }
            return data;
        }

    }
}