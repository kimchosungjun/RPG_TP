#ifndef USEFULTEXTURES_INCLUDED
#define USEFULTEXTURES_INCLUDED    
#define LevelSample

#include "GetTextureIndicesCommon.hlsl"
StructuredBuffer<float> _textureSize;

Texture2DArray _splatMap; 
SamplerState sampler_splatMap;

#if HEIGHTMAP_ENABLED
Texture2DArray _height_textures; 
SamplerState sampler_height_textures;
#endif

float4 VectorSumNormalize(float4 weights){
    float sum = weights.x+weights.y+weights.z+weights.w;
    return weights/sum;
}
void GetTexturesIndices(in float2 correctUV, out int4 highestIndices, out float4 highestWeights){  
    SampleIndicesLevel(correctUV,_splatMap, sampler_splatMap, highestIndices, highestWeights);
    //Can i do this better?
    int4 copyIndex = 0;
    float4 copyWeight = 0.01;

    float SizeX = _textureSize[highestIndices.x];
    float SizeY = _textureSize[highestIndices.y];
    float SizeZ = _textureSize[highestIndices.z];
    float SizeW = _textureSize[highestIndices.w];

    #if HEIGHTMAP_ENABLED
    highestWeights.x = highestWeights.x*_height_textures.SampleLevel(sampler_height_textures, float3(correctUV/SizeX, highestIndices.x),0).r;
    highestWeights.y = highestWeights.y*_height_textures.SampleLevel(sampler_height_textures, float3(correctUV/SizeY, highestIndices.y),0).r;
    highestWeights.z = highestWeights.z*_height_textures.SampleLevel(sampler_height_textures, float3(correctUV/SizeZ, highestIndices.z),0).r;
    highestWeights.w = highestWeights.w*_height_textures.SampleLevel(sampler_height_textures, float3(correctUV/SizeW, highestIndices.w),0).r;
    #endif

    highestWeights = VectorSumNormalize(highestWeights);
}
#endif //MYHLSLINCLUDE_INCLUDED