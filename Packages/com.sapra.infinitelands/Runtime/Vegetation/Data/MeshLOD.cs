using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace sapra.InfiniteLands{
    [System.Serializable]
    public class MeshLOD
    {
        public Mesh mesh;
        public Material[] materials;
        public int SubMeshCount{get; private set;}

        public bool valid => materials != null && mesh != null && materials.Length.Equals(mesh.subMeshCount) && !materials.Any(a => a== null);

        public MeshLOD(Mesh mesh, Material[] materials)
        {
            this.mesh = mesh;
            this.materials = materials;
        }

        public List<GraphicsBuffer.IndirectDrawIndexedArgs> InitializeMeshLOD(IPaintTerrain painter, int MaxSubMeshCount, string assetName = ""){
            if(!valid){
                if(mesh != null){
                    var nonnull = materials.Where(a => a!= null);
                    Debug.LogErrorFormat("Dispartity between number of materials and meshes. There are {0} materials and {1} submeshes in {2}", nonnull.Count(), mesh.subMeshCount, mesh.name);
                }else
                    Debug.LogErrorFormat("Mising a mesh in {0}. Did you delete it?", assetName);
                return DummyData(MaxSubMeshCount);
            }

            var distinctMats = materials.Where(a => a!= null).Distinct().ToArray();
            foreach(Material mat in distinctMats){
                painter.AssignMaterials(mat);
            }
            
            this.SubMeshCount = mesh.subMeshCount;
            List<GraphicsBuffer.IndirectDrawIndexedArgs> arguments = new List<GraphicsBuffer.IndirectDrawIndexedArgs>();
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                var args = new GraphicsBuffer.IndirectDrawIndexedArgs
                {
                    indexCountPerInstance = mesh.GetIndexCount(i),
                    instanceCount = 0,
                    startIndex = mesh.GetIndexStart(i),
                    baseVertexIndex = mesh.GetBaseVertex(i),
                    startInstance = 0
                };
                arguments.Add(args);
            }
            
            if(mesh.subMeshCount < MaxSubMeshCount){
                arguments.AddRange(DummyData(MaxSubMeshCount-mesh.subMeshCount));
            }
            return arguments;
        }

        private List<GraphicsBuffer.IndirectDrawIndexedArgs> DummyData(int Count){
            List<GraphicsBuffer.IndirectDrawIndexedArgs> arguments = new List<GraphicsBuffer.IndirectDrawIndexedArgs>();
            for (int i = 0; i < Count; i++)
            {
                var args = new GraphicsBuffer.IndirectDrawIndexedArgs
                {
                    indexCountPerInstance = 0,
                    instanceCount = 0,
                    startIndex = 0,
                    baseVertexIndex = 0,
                    startInstance = 0
                };
                arguments.Add(args);
            }
            return arguments;            
        }
    }
}