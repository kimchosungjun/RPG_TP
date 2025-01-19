using UnityEngine;

namespace sapra.InfiniteLands
{
    public struct TerrainConfig
    {
        public Vector3Int ID;
        public Vector3 Position;
        public Vector2 MinMaxHeight;
        public Vector3 TerrainNormal;

        public Bounds WorldSpaceBounds{get; private set;}
        public Bounds ObjectSpaceBounds{get; private set;}
        
        public TerrainConfig(Vector3Int _id, Vector2 _MinMaxHeight, Vector3 worldNormal, float _MeshScale, Vector3 flatPosition)
        {
            this.ID = _id;
            this.Position = flatPosition;
            this.TerrainNormal = worldNormal;
            this.MinMaxHeight = _MinMaxHeight;

            float verticalOffset = (_MinMaxHeight.y + _MinMaxHeight.x)/2f;
            float displacement = _MinMaxHeight.y - _MinMaxHeight.x;

            WorldSpaceBounds = new Bounds(Position+verticalOffset*Vector3.up, new Vector3(_MeshScale, displacement, _MeshScale));;
            ObjectSpaceBounds = new Bounds(verticalOffset*Vector3.up, new Vector3(_MeshScale, displacement, _MeshScale));
        }

        public TerrainConfig(Vector3Int _id, Vector2 _MinMaxHeight, Vector3 worldNormal, float _MeshScale)
        {
            this.ID = _id;
            this.Position = new Vector3(ID.x + .5f, 0, ID.y + .5f) * _MeshScale;
            this.TerrainNormal = worldNormal;
            this.MinMaxHeight = _MinMaxHeight;

            float verticalOffset = (_MinMaxHeight.y + _MinMaxHeight.x)/2f;
            float displacement = _MinMaxHeight.y - _MinMaxHeight.x;

            WorldSpaceBounds = new Bounds(Position+verticalOffset*Vector3.up, new Vector3(_MeshScale, displacement, _MeshScale));;
            ObjectSpaceBounds = new Bounds(verticalOffset*Vector3.up, new Vector3(_MeshScale, displacement, _MeshScale));
        }
    }
}