using Unity.Mathematics;
using UnityEngine;

namespace sapra.InfiniteLands
{
    public struct CoordinateData
    {
/*         public int4 vegetationIndices;
        public float4 vegetationWeights;

        public int4 textureIndices;
        public float4 textureWeights; */

        public float3 position;
        public float3 normal;

        public CoordinateData(float3 _position, float3 _normal){
            position = _position;
            normal = _normal;
        }

        public static CoordinateData Default => new CoordinateData()
        {
            position = new float3(0,0,0),
            normal = new float3(0,1,0),
/*             vegetationIndices = -1,
            textureIndices = -1, */
        };

        public static CoordinateData Lerp(CoordinateData a, CoordinateData b, float t){
            float3 po = math.lerp(a.position, b.position, t);
            float3 nor = math.lerp(a.normal, b.normal, t);
            return new CoordinateData(){
                position = po,
                normal = nor,
            };
        }

        public CoordinateData ApplyMatrix(Matrix4x4 localToWorld){
            return new CoordinateData(){
                position = localToWorld.MultiplyPoint(position),
                normal = localToWorld.MultiplyVector(normal)
            };
        }
    }
}