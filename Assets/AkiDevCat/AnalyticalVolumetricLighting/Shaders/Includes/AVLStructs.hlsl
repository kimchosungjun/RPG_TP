#ifndef AVL_STRUCTS_LIB_INCLUDED
#define AVL_STRUCTS_LIB_INCLUDED

/*
 * LIGHT_THREAD_COUNT_PER_GROUP should be less or equal to the warp thread size
 * For Nvidia GPUs starting from CC 1.0 warp size is 32 threads
 * For AMD GPUs it seems to be 64 threads
 * For Intel GPUs it might be 8, 16, 32 threads, most likely 16
 */

/*
 * MAX_LIGHT_PER_CLUSTER also exists on the CPU side inside AVLConstants.cs
 * If you change it, make sure to change it on the CPU side as well!
 */

#define MAX_LIGHT_PER_CLUSTER           256
#define MAX_MASKS_PER_CLUSTER           32
#define LIGHT_THREAD_COUNT_PER_GROUP    24

// AVL_DebugMode enum
#define AVL_DEBUG_MODE                      0
#define AVL_DEBUG_MODE_NONE                 0
#define AVL_DEBUG_MODE_LIGHT_CLUSTERS       1
#define AVL_DEBUG_MODE_LIGHT_OVERDRAW       2
#define AVL_DEBUG_MODE_LIGHT_COUNT          3
#define AVL_DEBUG_MODE_VOLUMETRIC_LIGHT     4
#define AVL_DEBUG_MODE_UPSCALING_EDGES      5
#define AVL_DEBUG_MODE_TRANSPARENT_LIGHT    6

// AVL_LightShape enum
#define AVL_LIGHT_SHAPE_POINT                   0
#define AVL_LIGHT_SHAPE_SPOT_SOFT_EDGE          1
#define AVL_LIGHT_SHAPE_SPOT_HARD_EDGE          2
#define AVL_LIGHT_SHAPE_SPOT_HARD_EDGE_SINGLE   3
#define AVL_LIGHT_SHAPE_AREA_HARD_EDGE          4
#define AVL_LIGHT_SHAPE_UNIFORM_BOX             5
#define AVL_LIGHT_SHAPE_UNIFORM_SPHERE          6

// AVL_MaskShape enum
#define AVL_MASK_SHAPE_SPHERE   0
#define AVL_MASK_SHAPE_BOX      1

struct AVL_LightData
{
    uint   Type;
    float3 Origin;
    float3 Right;
    float3 Up;
    float3 Forward;
    float3 BoundingOrigin;
    float  BoundingRadius;
    float4 Color;
    float  Range;
    float2 Rect;
    float4 PrimaryAngle;
    float4 SecondaryAngle;
    float  SecondaryEnergyModifier;
    float  Scattering;
    float  CullingFading;
    int    MaskId;
};

struct AVL_MaskData
{
    uint Type;
    float4x4 WorldToMask;
    float3 Origin;
    float BoundingRadius;
};

struct AVL_LightClusterData
{
    // Up, Right, Down, Left frustum planes
    float4x4 FrustumPlanes4;
    float FarPlaneDepth;
    uint BufferOffset;
    uint LightCount;
    uint MaskBufferOffset;
    uint MaskCount;
};

struct AVL_Ray
{
    float3 Origin;
    float3 Direction;
};

struct AVL_AirLightInput
{
    AVL_LightData Light;
    AVL_Ray Ray;
    float RayLength;
};

// ===== Functions =====

AVL_Ray mul_ray(float4x4 mat, AVL_Ray ray)
{
    ray.Origin = mul(mat, float4(ray.Origin, 1.0)).xyz;
    ray.Direction = mul(mat, float4(ray.Direction, 0.0)).xyz;
    return ray;
}

#endif