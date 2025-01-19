using System;
using Unity.Mathematics;
using UnityEngine;

namespace sapra.InfiniteLands{
    public static class MapTools{
        public static int MaxIncreaseSize => 30;
        public static int IncreaseResolution(int OriginalResolution, int Extra){
            if(Extra > OriginalResolution)
                Debug.LogWarning("Too much resolution gain. Please conside reducing the amount of samples");
            return OriginalResolution + Math.Min(Extra,OriginalResolution)*2;
        }
        public static int2 IndexToVector(int index, int Resolution){
            int x = index % (Resolution+1);
            int y = index / (Resolution+1);
            return new int2(x, y);
        }
        public static int VectorToIndex(int2 indices, int Resolution){
            return indices.x+indices.y*(Resolution+1);
        }

        public static int RemapIndex(int index,int FromResolution, int ToResolution){
            if(FromResolution.Equals(ToResolution))
                return index;
            int2 remapIndex = GetVectorIndex(index, FromResolution, ToResolution);
            return VectorToIndex(remapIndex, ToResolution);
        }
        public static int2 RemapIndex(int2 index, int FromResolution, int ToResolution){
            if(FromResolution.Equals(ToResolution))
                return index;
            int difference = ToResolution-FromResolution;
            int newX = math.clamp(index.x+difference/2,0, ToResolution);
            int newY = math.clamp(index.y+difference/2,0, ToResolution);
            return new int2(newX,newY);
        }
        public static int GetFlatIndex(int2 index, int FromResolution, int ToResolution){
            int2 remaped = RemapIndex(index, FromResolution, ToResolution);
            return VectorToIndex(remaped, ToResolution);
        }
        public static int2 GetVectorIndex(int index, int FromResolution, int ToResolution){
            int2 vectorized = IndexToVector(index, FromResolution);
            return RemapIndex(vectorized, FromResolution, ToResolution);
        }
        public static int LengthFromResolution(int resolution){
            int vertexCount = (resolution + 1) * (resolution + 1);
            if ((vertexCount % 4) != 0)
            {
                var extra = 4 - vertexCount % 4;
                vertexCount += extra;
            }

            return vertexCount;
        }
        public static float2 GetOffsetInGrid(float2 position, float gridSize){
            var value = position/gridSize;
            var fractional = value-math.floor(value);
            return -fractional*gridSize;
        }

    }
}