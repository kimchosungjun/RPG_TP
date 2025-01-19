// https://twitter.com/Cyanilux/status/1396848736022802435?s=20

#ifndef GRASS_INSTANCED_INCLUDED
#define GRASS_INSTANCED_INCLUDED
#define mask 0xFF
#include "..\Helpers\Quaternion.cginc"

// ----------------------------------------------------------------------------------

// Graph should contain Boolean Keyword, "PROCEDURAL_INSTANCING_ON", Global, Multi-Compile.
// Must have two Custom Functions in vertex stage. One is used to attach this file (see Instancing_float below),
// and another to set #pragma instancing_options :

// It must use the String mode as this cannot be defined in includes.
// Without this, you will get "UNITY_INSTANCING_PROCEDURAL_FUNC must be defined" Shader Error.
/*
Out = In;
#pragma instancing_options procedural:vertInstancingSetup
*/
// I've found this works fine, but it might make sense for the pragma to be defined outside of a function,
// so could also use this slightly hacky method too
/*
Out = In;
}
#pragma instancing_options procedural:vertInstancingSetup
void dummy(){
*/

// ----------------------------------------------------------------------------------

struct InstanceData {
    float3 position;
    uint2 quaternionScale;
    uint normalandTexture;
};

struct SumPack{
    uint startBase;
    uint countBase;

    uint startTrans;
    uint countTrans;
};

StructuredBuffer<InstanceData> _PerInstanceData;
StructuredBuffer<uint> _PreVisibleInstances;
StructuredBuffer<uint> _Indices;
StructuredBuffer<uint> _TargetLODs;

StructuredBuffer<SumPack> _Counters;

int _LODValue;
int _LODCount;
int _ShadowLodOffset;


// Stores the matrices (and possibly other data) sent from the C# side via material.SetBuffer, in Start/OnEnable.
// See : https://gist.github.com/Cyanilux/e7afdc5c65094bfd0827467f8e4c3c54

#if UNITY_ANY_INSTANCING_ENABLED
	// Updates the unity_ObjectToWorld / unity_WorldToObject matrices so our matrix is taken into account

	// Based on : 
	// https://github.com/Unity-Technologies/Graphics/blob/master/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/ParticlesInstancing.hlsl
	// and/or
	// https://github.com/TwoTailsGames/Unity-Built-in-Shaders/blob/master/CGIncludes/UnityStandardParticleInstancing.cginc

/* 	void vertInstancingMatrices(out float4x4 objectToWorld, out float4x4 worldToObject) {
		InstanceData data = _PerInstanceData[_SortedKeys[unity_InstanceID]];
		objectToWorld = data.m;

		// Inverse transform matrix
		float3x3 w2oRotation;
		w2oRotation[0] = objectToWorld[1].yzx * objectToWorld[2].zxy - objectToWorld[1].zxy * objectToWorld[2].yzx;
		w2oRotation[1] = objectToWorld[0].zxy * objectToWorld[2].yzx - objectToWorld[0].yzx * objectToWorld[2].zxy;
		w2oRotation[2] = objectToWorld[0].yzx * objectToWorld[1].zxy - objectToWorld[0].zxy * objectToWorld[1].yzx;

		float det = dot(objectToWorld[0].xyz, w2oRotation[0]);
		w2oRotation = transpose(w2oRotation);
		w2oRotation *= rcp(det);
		float3 w2oPosition = mul(w2oRotation, -objectToWorld._14_24_34);

		worldToObject._11_21_31_41 = float4(w2oRotation._11_21_31, 0.0f);
		worldToObject._12_22_32_42 = float4(w2oRotation._12_22_32, 0.0f);
		worldToObject._13_23_33_43 = float4(w2oRotation._13_23_33, 0.0f);
		worldToObject._14_24_34_44 = float4(w2oPosition, 1.0f);
	} */

	void vertInstancingSetup() {
		//vertInstancingMatrices(unity_ObjectToWorld, unity_WorldToObject);
	}

#endif

float4x4 inverse(float4x4 input)
{
#define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
 
    float4x4 cofactors = float4x4(
        minor(_22_23_24, _32_33_34, _42_43_44),
        -minor(_21_23_24, _31_33_34, _41_43_44),
        minor(_21_22_24, _31_32_34, _41_42_44),
        -minor(_21_22_23, _31_32_33, _41_42_43),
 
        -minor(_12_13_14, _32_33_34, _42_43_44),
        minor(_11_13_14, _31_33_34, _41_43_44),
        -minor(_11_12_14, _31_32_34, _41_42_44),
        minor(_11_12_13, _31_32_33, _41_42_43),
 
        minor(_12_13_14, _22_23_24, _42_43_44),
        -minor(_11_13_14, _21_23_24, _41_43_44),
        minor(_11_12_14, _21_22_24, _41_42_44),
        -minor(_11_12_13, _21_22_23, _41_42_43),
 
        -minor(_12_13_14, _22_23_24, _32_33_34),
        minor(_11_13_14, _21_23_24, _31_33_34),
        -minor(_11_12_14, _21_22_24, _31_32_34),
        minor(_11_12_13, _21_22_23, _31_32_33)
        );
#undef minor
    return transpose(cofactors) / determinant(input);
}

// Shader Graph Functions

// Obtain InstanceID. e.g. Can be used as a Seed into Random Range node to generate random data per instance
void GetInstanceID_float(out float Out){
	Out = 0;
	#ifndef SHADERGRAPH_PREVIEW
	#if UNITY_ANY_INSTANCING_ENABLED
	Out = unity_InstanceID;
	#endif
	#endif
}
float4x4 MakeTRSMatrix(float3 pos, float4 rotQuat, float3 scale)
{
    float4x4 rotPart = QuatToMatrix(rotQuat);
    float4x4 trPart = float4x4(
        float4(scale.x, 0, 0, 0), 
        float4(0, scale.y, 0, 0), 
        float4(0, 0, scale.z, 0), 
        float4(pos, 1));
    return mul(rotPart, trPart);
}
void UnpackRotationScale(in uint2 quat_scale, out float4 quaternion, out float3 scale){
	uint qx = quat_scale.x & 0xFFFF;
	uint qy = (quat_scale.x >> 16) & 0xFFFF;
	uint qz = quat_scale.y & 0xFFFF;

	float3 qxyz = (float3(qx,qy,qz)/0xFFFF)*2.0f-1.0f;
	float qw = 1-(qxyz.x*qxyz.x+qxyz.y*qxyz.y+qxyz.z*qxyz.z);
	
	quaternion = float4(qxyz, qw > 0.0 ? sqrt(qw):0.0);

	float s = f16tof32((quat_scale.y >> 16) & 0xFFFF);
	scale = float3(s,s,s);
}

void UnpackNormalIndex(in uint normal_index, out float3 normal, out int index){
	uint nx = normal_index & 0xFFF;
	uint nz = (normal_index >> 12) & 0xFFF;
	index = ((normal_index >> 24) & 0xFF) - 1;

	float2 nxz = (float2(nx,nz)/0xFFF)*2.0f-1.0f;
	float ny = 1.0-(nxz.x*nxz.x+nxz.y*nxz.y);
	normal = normalize(float3(nxz.x, ny > 0.0 ? sqrt(ny):0.0, nxz.y));
}

float UnMask(in uint value){
	uint NormalIndex = value & 0xFF;
	if(_ShadowLodOffset >= 0){
		float ShadowTransition = ((value >> 23) & 0xFF)/255.0f;
		return NormalIndex+_ShadowLodOffset+ShadowTransition;
	}
	else{
		float NormalTransition = ((value >> 8) & 0x3FFF)/16383.0f;
		return NormalIndex+NormalTransition;
	}
	
}

void TransfromPosition_float(in float3 objectPosition, out float3 position, out float3 worldPosition, out float transition, out int textureIndex, out float3 groundNormal){
	#if UNITY_ANY_INSTANCING_ENABLED
	int instanceIndex = _Indices[_Counters[_LODValue].startBase+unity_InstanceID];
	InstanceData data = _PerInstanceData[_PreVisibleInstances[instanceIndex]];
	worldPosition = data.position;
	
	float4 rotation;
	float3 scale;
	UnpackRotationScale(data.quaternionScale, rotation, scale);
	float4x4 trs = transpose(MakeTRSMatrix(data.position, rotation, scale));
	#undef unity_ObjectToWorld
    #undef unity_WorldToObject
    unity_ObjectToWorld = trs;
    unity_WorldToObject = inverse(trs);
	
	#if SHADERPASS == SHADERPASS_MOTION_VECTORS && defined(SHADERPASS_CS_HLSL)
		unity_MatrixPreviousM = unity_ObjectToWorld;
		unity_MatrixPreviousMI = unity_WorldToObject;
	#endif
	
	
	float current = UnMask(_TargetLODs[instanceIndex]);
	transition = saturate(abs(_LODValue-current)*2.0-1.0);
	
	UnpackNormalIndex(data.normalandTexture, groundNormal, textureIndex);
	
	#else
	worldPosition = 0;
	transition = 0;
	textureIndex = 0;
	groundNormal = 0;
	#endif

	position = objectPosition;
}

// Just passes the position through, allows us to actually attach this file to the graph.
// Should be placed somewhere in the vertex stage, e.g. right before connecting the object space position.
void Instancing_float(float3 Position, out float3 Out){
	Out = Position;
}

#endif