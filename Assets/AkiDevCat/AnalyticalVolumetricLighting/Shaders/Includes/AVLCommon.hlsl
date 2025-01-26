#ifndef AVL_COMMON_LIB_INCLUDED
#define AVL_COMMON_LIB_INCLUDED

inline float ClampAway(const float x, const float e, const float eps)
{
    const float delta = x - e;
    
    if (delta >= 0.0 && delta < eps)
    {
        return e + eps;
    }
    
    if (delta < 0.0 && delta > -eps)
    {
        return e - eps;
    }
    
    return x;
}

float3 Barycentric(float3 p, float3 a, float3 b, float3 c)
{
    float3 result;
    float3 v0 = b - a, v1 = c - a, v2 = p - a;
    const float d = v0.x * v1.y - v1.x * v0.y;
    result.x = (v2.x * v1.y - v1.x * v2.y) / d;
    result.y = (v0.x * v2.y - v2.x * v0.y) / d;
    result.z = 1.0f - result.x - result.y;
    return result;
}

float4x4 ConstructWorldToLocalMatrix4x4(float3 xAxis, float3 yAxis, float3 zAxis, float3 wsOrigin)
{
    float4x4 result = {
        xAxis.x, xAxis.y, xAxis.z, 0.0,
        yAxis.x, yAxis.y, yAxis.z, 0.0,
        zAxis.x, zAxis.y, zAxis.z, 0.0,
        0.0, 0.0, 0.0, 1.0
    };
    float3 translation = mul(result, float4(wsOrigin, 0.0)).xyz;
    result[0][3] = -translation.x;
    result[1][3] = -translation.y;
    result[2][3] = -translation.z;
    return result;
}

float PointPlaneDistance(float3 position, float4 plane)
{
    return dot(plane, float4(position, 1.0));
}

float FrustumCullSphere(float4 fPlanes[6], float3 sPosition, float sRadius)
{
    const float d01 = min(PointPlaneDistance(sPosition, fPlanes[0]), PointPlaneDistance(sPosition, fPlanes[1]));
    const float d23 = min(PointPlaneDistance(sPosition, fPlanes[2]), PointPlaneDistance(sPosition, fPlanes[3]));
    const float d45 = min(PointPlaneDistance(sPosition, fPlanes[4]), PointPlaneDistance(sPosition, fPlanes[5]));
 
    return min(min(d01, d23), d45) + sRadius;
}

bool IntersectAABB(float3 rayOrigin, float3 rayDirection, float3 boxMin, float3 boxMax, out float2 intersections)
{
    float3 tMin = (boxMin - rayOrigin) / rayDirection;
    float3 tMax = (boxMax - rayOrigin) / rayDirection;
    float3 t1 = min(tMin, tMax);
    float3 t2 = max(tMin, tMax);
    float tNear = max(max(t1.x, t1.y), t1.z);
    float tFar = min(min(t2.x, t2.y), t2.z);
    intersections = float2(tNear, tFar);
    return tNear <= tFar && tFar >= 0.0;
}

bool IntersectSphere(float3 rayOrigin, float3 rayDirection, float3 sphereOrigin, float sphereRadius, out float2 intersections)
{
    float a = dot(rayDirection, rayDirection);
    float3 s0_r0 = rayOrigin - sphereOrigin;
    float b = 2.0 * dot(rayDirection, s0_r0);
    float c = dot(s0_r0, s0_r0) - sphereRadius * sphereRadius;
    float disc = b * b - 4.0 * a* c;
    
    if (disc < 0.0)
    {
        intersections = 0;
        return false;
    }
    
    intersections = float2(-b - sqrt(disc), -b + sqrt(disc)) / (2.0 * a);
    return true;
}

/**
 * \brief 
 * \param rayOrigin Ray Origin
 * \param rayDirection Ray Direction
 * \param coneOrigin Cone Origin
 * \param coneDirection Cone Direction
 * \param cosAlpha Cosine of the <b>half</b> cone angle
 * \param intersections entry and exit intersection length
 * \return true, if intersection exists
 */
bool2 IntersectCone(float3 rayOrigin, float3 rayDirection, float3 coneOrigin, float3 coneDirection, float coneRange, float cosAlpha, out float2 intersections)
{
    // ToDo Optimize
    float3 D = rayOrigin - coneOrigin;

    float cosa2 = cosAlpha*cosAlpha; // ToDo Optimize
    float rayDotCone = dot(rayDirection,coneDirection);
    float rayDotCone2 = rayDotCone*rayDotCone;
    float DDotCone = dot(D,coneDirection);
    
    float a = rayDotCone2 - cosa2;
    float b = 2. * (rayDotCone*DDotCone - dot(rayDirection,D)*cosa2);
    float c = DDotCone*DDotCone - dot(D,D)*cosa2;

    float det = b*b - 4.*a*c;
    if (det < 0.)
    {
        intersections = 0;
        return false;
    }

    det = sqrt(det);
    float t1 = (-b - det) / (2. * a);
    float t2 = (-b + det) / (2. * a);

    float3 cp = rayOrigin + t1*rayDirection - coneOrigin;
    float h1 = dot(cp, coneDirection);
    cp = rayOrigin + t2*rayDirection - coneOrigin;
    float h2 = dot(cp, coneDirection);

    bool2 result;
    float2 iResults = float2(t2, t1);

    result.y = h1 > 0.0 && t1 >= 0.0;
    result.x = h2 > 0.0 && t2 >= 0.0;

    if (rayDotCone >= cosAlpha)
    {
        result.y = true;
        iResults.y = FLT_INF;
    }
    if (rayDotCone <= -cosAlpha)
    {
        result.x = false;
        iResults.x = -FLT_INF;
    }

    intersections = iResults;
    
    return result;
}

// ====================
// ShaderGraph Wrappers
// ====================

void IntersectAABB_float(float3 rayOrigin, float3 rayDirection, float3 boxMin, float3 boxMax, out bool result, out float2 intersections)
{
    result = IntersectAABB(rayOrigin, rayDirection, boxMin, boxMax, intersections);
}

void ConstructWorldToLocalMatrix4x4_float(float3 xAxis, float3 yAxis, float3 zAxis, float3 wsOrigin, out float4x4 o_matrix)
{
    o_matrix = ConstructWorldToLocalMatrix4x4(xAxis, yAxis, zAxis, wsOrigin);
}

#endif