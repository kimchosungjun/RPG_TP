using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;

namespace sapra.InfiniteLands
{
    internal static class ConfigureMesh{
        public static void Configure(Mesh.MeshData _meshData, int verticesLength, int indicesLength, Bounds _bounds){
            var descriptor = new NativeArray<VertexAttributeDescriptor>(
                3, Allocator.Temp, NativeArrayOptions.UninitializedMemory
            );
            descriptor[0] = new VertexAttributeDescriptor(dimension: 3);
            descriptor[1] = new VertexAttributeDescriptor(VertexAttribute.Normal,
                dimension: 3);

            descriptor[2] = new VertexAttributeDescriptor(
                VertexAttribute.TexCoord0, dimension: 2
            );

            _meshData.SetVertexBufferParams(verticesLength, descriptor);
            descriptor.Dispose();

            _meshData.SetIndexBufferParams(indicesLength, IndexFormat.UInt16);

            _meshData.subMeshCount = 1;
            _meshData.SetSubMesh(0, new SubMeshDescriptor(0, indicesLength)
            {
                bounds = _bounds,
                vertexCount = verticesLength,
            }, MeshGenerator.NoCalculations());
        }
    }
}