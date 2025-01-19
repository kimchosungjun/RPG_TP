using UnityEngine;
namespace sapra.InfiniteLands{
    public class MeshResult {
        public Vector3Int ID;
        public Mesh mesh;
        public bool PhysicsBaked;
        public MeshResult(Vector3Int ID, Mesh mesh, bool PhysicsBaked){
            this.ID = ID;
            this.PhysicsBaked = PhysicsBaked;
            this.mesh = mesh;
        }
    }
}