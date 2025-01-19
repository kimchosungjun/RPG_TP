using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Rendering;

namespace sapra.InfiniteLands
{
    public static class MeshGenerator
    {
        public enum MeshType
        {
            Normal,
            Decimated
        };
        
        public static MeshGenerationData ScheduleParallel(ChunkData[] data)
        {
            int count = data.Length;
            MeshGenerationData meshGenerationData = new MeshGenerationData()
            {
                meshDataArray = Mesh.AllocateWritableMeshData(count)
            };

            NativeArray<JobHandle> handles = new NativeArray<JobHandle>(count, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            for (int i = 0; i < count; i++)
            {
                ChunkData specific = data[i];
                JobHandle jobHandle;
                switch (specific.meshSettings.meshType)
                {
                    case MeshSettings.MeshType.Decimated:
                        jobHandle = DecimatedMesh(specific, meshGenerationData.meshDataArray[i]);
                        break;
                    default:
                        jobHandle = StaticMesh(specific, meshGenerationData.meshDataArray[i]);
                        break;
                }

                handles[i] = jobHandle;
            }

            meshGenerationData.handle = JobHandle.CombineDependencies(handles);
            meshGenerationData.generatedChunks = data;
            handles.Dispose();
            return meshGenerationData;
        }

        private static JobHandle DecimatedMesh(ChunkData data, Mesh.MeshData meshData)
        {
            MeshSettings settings = data.meshSettings;
            int vertexCount = (settings.Resolution+1) * (settings.Resolution+1);
            int triangleCountHalf = settings.Resolution * settings.Resolution;
            int patchCountPerLine = Mathf.CeilToInt(settings.Resolution / (float)settings.coreGridSpacing);

            NativeList<Vertex> validPoints = new NativeList<Vertex>(vertexCount, Allocator.Persistent);
            NativeParallelHashMap<int, ushort> validHashMap = new NativeParallelHashMap<int, ushort>(vertexCount, Allocator.Persistent);
            NativeList<ushort3> triangles = new NativeList<ushort3>(triangleCountHalf * 2, Allocator.Persistent);

            JobHandle applyToTheMesh = FindValidDecimatedPoints.ScheduleParallel(validPoints, validHashMap,settings.coreGridSpacing, vertexCount, data);
            JobHandle triangulationHandle = TriangulationJob.ScheduleParallel(validHashMap, triangles, settings, patchCountPerLine, applyToTheMesh);
            JobHandle meshingHandle = DecimatedMeshJob.ScheduleParallel(meshData, data, validPoints, triangles, triangulationHandle);

            triangles.Dispose(meshingHandle);
            validPoints.Dispose(meshingHandle);
            validHashMap.Dispose(meshingHandle);
            return meshingHandle;
        }

        private static JobHandle StaticMesh(ChunkData data, Mesh.MeshData meshData)
        {
            return StaticMeshJob.ScheduleParallel(meshData, data);
        }

        public static MeshGenerationData ScheduleParallel(ChunkData chunk)
        {
            ChunkData[] chunks = new ChunkData[] { chunk };
            return ScheduleParallel(chunks);
        }

        public static Mesh CreateMesh(){
            var current = new Mesh();
            ReuseMesh(current);
            return current;
        }

        public static void ReuseMesh(Mesh current){
            current.name = "Infinite Lands Mesh";
        }

        public static Mesh[] Consolidate(MeshGenerationData data, Mesh[] selection)
        {
            for(int i = 0; i < data.meshDataArray.Length; i++){
                Bounds newMeshBounds = GetBounds(data.generatedChunks[i]);
                selection[i].bounds = newMeshBounds;
            }
            Mesh.ApplyAndDisposeWritableMeshData(data.meshDataArray, selection, NoCalculations());

            return selection;
        }

        
        private static Bounds GetBounds(ChunkData data)
        {
            float MinValue = data.worldFinalData.ChunkMinMax[0];
            float MaxValue = data.worldFinalData.ChunkMinMax[1];

            float verticalOffset = (MaxValue + MinValue)/2f;
            float displacement = MaxValue - MinValue;
            return new Bounds(verticalOffset*Vector3.up, new Vector3(data.meshSettings.MeshScale, displacement, data.meshSettings.MeshScale));;
        }

        public static MeshUpdateFlags NoCalculations()
        {
            return MeshUpdateFlags.DontRecalculateBounds |
                   MeshUpdateFlags.DontValidateIndices |
                   MeshUpdateFlags.DontNotifyMeshUsers |
                   MeshUpdateFlags.DontResetBoneBounds;
        }
    }
}