/*
*  Analytic Volumetric Lighting - RenderingPass shader
*/

Shader "Hidden/AkiDevCat/AVL/RenderingPass"
{
    // Required for Blit to work properly
    Properties 
    {
        _MainTex ("Screen Texture", 2D) = "white" {}
    }
    
    HLSLINCLUDE

    // ===== Includes =====
    
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
    #include "../Includes/AVLCommon.hlsl"
    #include "../Includes/AVLStructs.hlsl"
    #include "../Includes/AVLLighting.hlsl"

    // ===== Structures =====
    
    struct CustomAttributes
    {
        float4 positionOS       : POSITION;
        float2 uv               : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f
    {
        float4 positionCS    : SV_POSITION;
        float2 uv            : TEXCOORD0;
        float4 ray           : TEXCOORD1;
    };

    // ===== Global Variables =====

    float2                                      _RenderingResolution;
    float4                                      _ClusterSize; // ToDo transfer to AVLClustering.hlsl lib
    float4                                      _CameraWorldSpacePosition;
    float                                       _AirDensity;
    float                                       _FogDensityGlobal;
    float                                       _FogDensityPerLight;
    float                                       _FogScattering;
    float3                                      _FogColor;

    // = Matrices =

    float4x4                                    _CameraNearFrustumMatrix;
    float4x4                                    _CameraToWorldMatrix;

    // = Textures =
    
    TEXTURE2D                                   (_MainTex);
    TEXTURE2D_FLOAT                             (_CameraDepthTexture);
    TEXTURE2D                                   (_AVLBufferTexture);
    TEXTURE2D                                   (_AVLFogTexture);
    
    SAMPLER                                     (sampler_CameraDepthTexture);

    float4                                      _MainTex_TexelSize;
    float4                                      _AVLFogTexture_TexelSize;

    // = Buffers =
    
    StructuredBuffer  <AVL_LightData>           _GlobalLightBuffer;
    uint                                        _GlobalLightBufferCount;
    StructuredBuffer  <AVL_LightClusterData>    _LightClusterBuffer;
    StructuredBuffer  <AVL_MaskData>            _GlobalMaskBuffer;
    StructuredBuffer  <uint>                    _LightIndexBuffer;
    StructuredBuffer  <uint>                    _MaskIndexBuffer;
    StructuredBuffer  <int>                     _MaskInverseIndexBuffer;
    uint                                        _GlobalMaskBufferMaxSize;

    // ===== Shader Functions =====

    float4 CalculateAirLight(float3 rayOrigin, float3 rayDirection, float depth, AVL_LightClusterData lightCluster, uint lightClusterIndex);

    v2f vert(CustomAttributes input)
    {
        v2f output;
        output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
        output.uv = input.uv;
        
        int index = output.uv.x + 2 * output.uv.y;
        output.ray = _CameraNearFrustumMatrix[index];
        
        return output;
    }

    float4 frag_render(v2f input) : SV_Target
    {
        float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_PointClamp, input.uv);
        
        float3 positionWS = ComputeWorldSpacePosition(input.uv.xy, rawDepth, UNITY_MATRIX_I_VP);
        #if UNITY_REVERSED_Z
        float3 nearPlanePosWS = ComputeWorldSpacePosition(input.uv.xy, 1.0f, UNITY_MATRIX_I_VP);
        #else
        float3 nearPlanePosWS = ComputeWorldSpacePosition(input.uv.xy, 0.0f, UNITY_MATRIX_I_VP);
        #endif

        float depth = length(positionWS - nearPlanePosWS);
        
        float3 rayOrigin = nearPlanePosWS;
        float3 rayDir = normalize(positionWS - nearPlanePosWS);
        
        // if (unity_OrthoParams.w > 0.5)
        // {
        //     // Camera is ortho
        //     float3 rayOriginCS = float3((input.uv - float2(0.5, 0.5)) * unity_OrthoParams.xy * 2.0, 0);
        //     
        //     rayOrigin = mul(_CameraToWorldMatrix, float4(rayOriginCS,1)).xyz;
        //     rayDir = -normalize(mul(_CameraToWorldMatrix, float4(0,0,1,0)).xyz);
        // }

        const uint2                 clusterPos = (uint2)(input.uv * _ClusterSize.xy);
        const uint                  clusterIndex = clusterPos.x + clusterPos.y * _ClusterSize.x;
        const AVL_LightClusterData  cluster = _LightClusterBuffer[clusterIndex];

        float4 result = 0;
        
        // Lights
        result += CalculateAirLight(rayOrigin, rayDir, depth, cluster, clusterIndex);

        return result;
    }

    float4 frag_upscale(v2f input) : SV_TARGET
    {
        const float2 uv = input.uv;
        const int2 dsPixelUV = round(input.uv * _MainTex_TexelSize.zw);
        const float rawDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_PointClamp, input.uv).r; // ToDo replace with load
        const float depthEye = LinearEyeDepth(rawDepth, _ZBufferParams);

        float4 result = 0.0;
        float totalWeight = 0.0;

        const int SAMPLES_PER_DIM = 3;

        float contrastFactor = 1.0;
        
        for (int ix = -SAMPLES_PER_DIM / 2; ix <= SAMPLES_PER_DIM / 2; ix++)
        {
            for (int iy = -SAMPLES_PER_DIM / 2; iy <= SAMPLES_PER_DIM / 2; iy++)
            {
                if (abs(ix) == 1 && abs(iy) == 1)
                {
                    continue;
                }
                
                const float2 offset = float2(ix, iy) * _MainTex_TexelSize.xy;
                const float2 dsUV = (float2)dsPixelUV * _MainTex_TexelSize.xy + offset + _MainTex_TexelSize.xy / 2.0;
        
                const float dsDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_PointClamp, dsUV).r; // ToDo replace with load
                const float dsDepthEye = LinearEyeDepth(dsDepth, _ZBufferParams);

                const float4 dsColor = SAMPLE_TEXTURE2D(_MainTex, sampler_PointClamp, dsUV); // ToDo replace with load

                const float2 deltaUV = (dsUV - uv) / _MainTex_TexelSize.xy;
                float uvFactor = saturate(1.0 - abs(deltaUV.x)) * saturate(1.0 - abs(deltaUV.y));
                uvFactor = saturate(uvFactor * 2.0);

                float depthFactor = saturate(1.0 - contrastFactor * abs(dsDepthEye - depthEye));

                float invFactor = saturate(1.0 - totalWeight);
                invFactor = 1.0;
                
                float currentWeight = depthFactor * invFactor * uvFactor;

                result += dsColor * currentWeight;
                totalWeight += currentWeight;
            }
        }

        // ToDo replace with presampled values
        float delta = saturate(1.0 - totalWeight);
        result += SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv) * delta;
        totalWeight += delta;
        
        return float4(result / (totalWeight + 0.0001));
    }

    float4 frag_blend(v2f input) : SV_Target
    {
        float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_PointClamp, input.uv);
        
        return float4(color.rgb, 1.0);
    }

    float4 frag_fog(v2f input) : SV_Target
    {
        float rawDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_PointClamp, input.uv).r;

        float3 positionWS = ComputeWorldSpacePosition(input.uv.xy, rawDepth, UNITY_MATRIX_I_VP);
        #if UNITY_REVERSED_Z
        float3 nearPlanePosWS = ComputeWorldSpacePosition(input.uv.xy, 1.0f, UNITY_MATRIX_I_VP);
        #else
        float3 nearPlanePosWS = ComputeWorldSpacePosition(input.uv.xy, 0.0f, UNITY_MATRIX_I_VP);
        #endif

        float depth = length(positionWS - nearPlanePosWS);
        
        // Global Fog
        float c = depth;
        float f = 1.0 / exp(_FogDensityGlobal * c);
        
        return float4(_FogColor.rgb, 1.0) * saturate(1.0 - f);
    }

    float4 frag_ds(v2f input) : SV_Target
    {
        
        return SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_PointClamp, input.uv);
    }

    /*
     * Returns 3 basis vectors for the default light space. This is a space used for lights having all 3 directions defined.
     */
    float3x3 GetDefaultLightSpaceBasis(const float3 wsLightRight, const float3 wsLightUp, const float3 wsLightForward)
    {
        float3x3 result;
        result[0] = wsLightRight;
        result[1] = wsLightUp;
        result[2] = wsLightForward;
        return result;
    }

    /*
     * Returns 3 basis vectors for the "omni-directional" light space. This is a space used for point lights mainly.
     */
    float3x3 GetOmniLightSpaceBasis(const float3 wsRayOrigin, const float3 wsRayDirection, const float3 wsLightOrigin)
    {
        float3x3 result;
        result[0] = wsRayDirection;
        result[2] = normalize(cross(wsRayDirection, wsLightOrigin - wsRayOrigin));
        result[1] = cross(result[0], result[2]);
        return result;
    }

    /*
     * Returns 3 basis vectors for the "mono-directional" light space. This is a space used for lights having a direction.
     */
    float3x3 GetMonoLightSpaceBasis(const float3 wsRayDirection, const float3 wsLightForward)
    {
        float3x3 result;
        result[1] = wsLightForward;
        result[2] = normalize(cross(wsRayDirection, wsLightForward));
        result[0] = cross(result[1], result[2]);
        return result;
    }

    inline float TransformToLightSpace1D(const float3 wsVector, const float3x3 lsBasis)
    {
        return dot(lsBasis[0], wsVector);
    }

    inline float2 TransformToLightSpace2D(const float3 wsVector, const float3x3 lsBasis)
    {
        return float2(dot(lsBasis[0], wsVector), dot(lsBasis[1], wsVector));
    }
    
    inline float3 TransformToLightSpace3D(const float3 wsVector, const float3x3 lsBasis)
    {
        return float3(dot(lsBasis[0], wsVector), dot(lsBasis[1], wsVector), dot(lsBasis[2], wsVector));
    }

    float4 CalculateAirLightPoint(const AVL_AirLightInput input)
    {
        const float3x3 lsOmniBasis = GetOmniLightSpaceBasis(input.Ray.Origin, input.Ray.Direction, input.Light.Origin);

        float2 lsRayOrigin = TransformToLightSpace2D(input.Ray.Origin - input.Light.Origin, lsOmniBasis);

        float airLight = SolvePointLightIntegral1D(lsRayOrigin.x, lsRayOrigin.y, input.RayLength, input.Light.Scattering);
        
        airLight = SoftenIntegralEdge(airLight, input.RayLength, input.Light.Range, input.Light.Scattering);

        airLight = _AirDensity * max(0.0, airLight);

        return input.Light.Color * airLight;
    }

    float4 CalculateAirLightSpotSoftEdge(AVL_AirLightInput input)
    {
        const float primarySine = input.Light.PrimaryAngle.y;
        const float primaryCosine = input.Light.PrimaryAngle.z;
        const float secondarySine = input.Light.SecondaryAngle.y;
        const float secondaryCosine = input.Light.SecondaryAngle.z;

        const float3x3 lsOmniBasis = GetOmniLightSpaceBasis(input.Ray.Origin, input.Ray.Direction, input.Light.Origin);
        const float3x3 lsMonoBasis = GetMonoLightSpaceBasis(input.Ray.Direction, input.Light.Forward);

        float2 outerIntersections;
        bool2 outerIntersectionsFound = IntersectCone(input.Ray.Origin, input.Ray.Direction, input.Light.Origin, input.Light.Forward, input.Light.Range, primaryCosine, outerIntersections);
        
        // UNITY_BRANCH
        if (!outerIntersectionsFound.x && !outerIntersectionsFound.y)
        {
            return 0.0;
        }

        float2 innerIntersections;
        bool2 innerIntersectionsFound = IntersectCone(input.Ray.Origin, input.Ray.Direction, input.Light.Origin, input.Light.Forward, input.Light.Range, secondaryCosine, innerIntersections);

        float A, B, C, D;
        float integralSum = 0.0;

        A = outerIntersections.x;
        B = innerIntersections.x;
        C = innerIntersections.y;
        D = outerIntersections.y;

        A = min(A, input.RayLength);
        B = min(B, input.RayLength);
        C = min(C, input.RayLength);
        D = min(D, input.RayLength);

        const float3 APos = input.Ray.Origin + input.Ray.Direction * A;
        const float3 BPos = input.Ray.Origin + input.Ray.Direction * B;
        const float3 CPos = input.Ray.Origin + input.Ray.Direction * C;

        float3 delta;
        float2 lsOmniOrigin;
        float3 lsMonoOrigin;
        float2 lsMonoRayDirection = TransformToLightSpace2D(input.Ray.Direction, lsMonoBasis);
        
        float lsIntegralDepth;

        // Calculate edge air light (Segment AB and Segment CD)
        // 4 Points available A B C D
        UNITY_BRANCH
        if (outerIntersectionsFound.y && innerIntersectionsFound.y)
        {
            // Segment AB
            lsIntegralDepth = B - A;
            delta = APos - input.Light.Origin;
            lsOmniOrigin = TransformToLightSpace2D(delta, lsOmniBasis);
            lsMonoOrigin = TransformToLightSpace3D(delta, lsMonoBasis);
            
            // Check for inner case
            if (A < 0.0)
            {
                lsIntegralDepth = B;
                delta = input.Ray.Origin - input.Light.Origin;
                lsOmniOrigin = TransformToLightSpace2D(delta, lsOmniBasis);
                lsMonoOrigin = TransformToLightSpace3D(delta, lsMonoBasis);
            }
            
            lsIntegralDepth = max(0, lsIntegralDepth);
            float integralResult = max(0.0, SolveSpotlightEdgeIntegralV2(lsOmniOrigin.x, lsOmniOrigin.y, lsMonoOrigin.x, lsMonoOrigin.z, lsMonoRayDirection.x, lsIntegralDepth, input.Light.Scattering, primarySine, secondarySine));

            integralSum += integralResult;
            
            // Segment CD
            lsIntegralDepth = D - C;
            delta = CPos - input.Light.Origin;
            lsOmniOrigin = TransformToLightSpace2D(delta, lsOmniBasis);
            lsMonoOrigin = TransformToLightSpace3D(delta, lsMonoBasis);
            
            // Check for inner case
            if (C < 0.0)
            {
                lsIntegralDepth = D;
                delta = input.Ray.Origin - input.Light.Origin;
                lsOmniOrigin = TransformToLightSpace2D(input.Ray.Origin - input.Light.Origin, lsOmniBasis);
                lsMonoOrigin = TransformToLightSpace3D(input.Ray.Origin - input.Light.Origin, lsMonoBasis);
            }

            lsIntegralDepth = max(0, lsIntegralDepth);
            integralResult = max(0.0, SolveSpotlightEdgeIntegralV2(lsOmniOrigin.x, lsOmniOrigin.y, lsMonoOrigin.x, lsMonoOrigin.z, lsMonoRayDirection.x, lsIntegralDepth, input.Light.Scattering, primarySine, secondarySine));

            integralSum += integralResult;
        }
        // Calculate edge air light (Segment AD) when there is no B and C intersections
        // Only 2 points available A D
        UNITY_BRANCH
        if (outerIntersectionsFound.y && !innerIntersectionsFound.y)
        {
            // Segment A D
            lsIntegralDepth = D - A;
            delta = APos - input.Light.Origin;
            lsOmniOrigin = TransformToLightSpace2D(delta, lsOmniBasis);
            lsMonoOrigin = TransformToLightSpace3D(delta, lsMonoBasis);
            
            if (A < 0.0)
            {
                lsIntegralDepth = D;
                delta = input.Ray.Origin - input.Light.Origin;
                lsOmniOrigin = TransformToLightSpace2D(delta, lsOmniBasis);
                lsMonoOrigin = TransformToLightSpace3D(delta, lsMonoBasis);
            }

            lsIntegralDepth = max(0, lsIntegralDepth);
            float integralResult = max(0.0, SolveSpotlightEdgeIntegralV2(lsOmniOrigin.x, lsOmniOrigin.y, lsMonoOrigin.x, lsMonoOrigin.z, lsMonoRayDirection.x, lsIntegralDepth, input.Light.Scattering, primarySine, secondarySine));

            integralSum += integralResult;
        }
        // Calculate inner air light (Segment BC)
        // 4 Points available A B C D
        UNITY_BRANCH
        if (innerIntersectionsFound.y)
        {
            // Segment B C
            lsIntegralDepth = C - B;
            lsOmniOrigin = TransformToLightSpace2D(BPos - input.Light.Origin, lsOmniBasis);
            
            // Check for inner case
            if (B < 0.0)
            {
                lsIntegralDepth = C;
                lsOmniOrigin = TransformToLightSpace2D(input.Ray.Origin - input.Light.Origin, lsOmniBasis);
            }

            lsIntegralDepth = max(0, lsIntegralDepth);
            float integralResult = max(0.0, SolvePointLightIntegral1D(lsOmniOrigin.x, lsOmniOrigin.y, lsIntegralDepth, input.Light.Scattering));

            integralSum += integralResult;
        }

        return input.Light.Color * (_AirDensity * max(0.0, integralSum));
    }

    float4 CalculateAirLightSpotHardEdge(const AVL_AirLightInput input)
    {
        const float primaryCosine = input.Light.PrimaryAngle.z;
        const float secondaryCosine = input.Light.SecondaryAngle.z;

        const float3x3 lsOmniBasis = GetOmniLightSpaceBasis(input.Ray.Origin, input.Ray.Direction, input.Light.Origin);

        float2 outerIntersections;
        bool2 outerIntersectionsFound = IntersectCone(input.Ray.Origin, input.Ray.Direction, input.Light.Origin, input.Light.Forward, input.Light.Range, primaryCosine, outerIntersections);

        UNITY_BRANCH
        if (!outerIntersectionsFound.x && !outerIntersectionsFound.y)
        {
            return 0.0;
        }

        const float2 lsRayOrigin = TransformToLightSpace2D(input.Ray.Origin - input.Light.Origin, lsOmniBasis);

        float2 innerIntersections;
        bool2 innerIntersectionsFound = IntersectCone(input.Ray.Origin, input.Ray.Direction, input.Light.Origin, input.Light.Forward, input.Light.Range, secondaryCosine, innerIntersections);

        float A, B, C, D;
        float integralSum = 0.0;

        A = outerIntersections.x;
        B = innerIntersections.x;
        C = innerIntersections.y;
        D = outerIntersections.y;

        A = min(A, input.RayLength);
        B = min(B, input.RayLength);
        C = min(C, input.RayLength);
        D = min(D, input.RayLength);

        const float3 APos = input.Ray.Origin + input.Ray.Direction * A;
        const float3 BPos = input.Ray.Origin + input.Ray.Direction * B;
        const float3 CPos = input.Ray.Origin + input.Ray.Direction * C;

        float2 lsIntegralOrigin;
        float lsIntegralDepth;

        // Calculate edge air light (Segment AB and Segment CD)
        // 4 Points available A B C D
        UNITY_BRANCH
        if (outerIntersectionsFound.y && innerIntersectionsFound.y)
        {
            // Segment AB
            lsIntegralOrigin = TransformToLightSpace2D(APos - input.Light.Origin, lsOmniBasis);
            lsIntegralDepth = B - A;
            
            // Check for inner case
            if (A < 0.0)
            {
                lsIntegralOrigin = lsRayOrigin;
                lsIntegralDepth = B;
            }

            float integralResult = 0.5 * max(0.0, SolvePointLightIntegral1D(lsIntegralOrigin.x, lsIntegralOrigin.y, lsIntegralDepth, input.Light.Scattering));
            
            integralSum += integralResult;
            
            // Segment CD
            lsIntegralOrigin = TransformToLightSpace2D(CPos - input.Light.Origin, lsOmniBasis);
            lsIntegralDepth = D - C;
            
            // Check for inner case
            if (C < 0.0)
            {
                lsIntegralOrigin = lsRayOrigin;
                lsIntegralDepth = D;
            }

            integralResult = 0.5 * max(0.0, SolvePointLightIntegral1D(lsIntegralOrigin.x, lsIntegralOrigin.y, lsIntegralDepth, input.Light.Scattering));

            integralSum += integralResult;
        }
        // Calculate edge air light (Segment AD) when there is no B and C intersections
        // Only 2 points available A D
        UNITY_BRANCH
        if (outerIntersectionsFound.y && !innerIntersectionsFound.y)
        {
            // Segment A D
            lsIntegralOrigin = TransformToLightSpace2D(APos - input.Light.Origin, lsOmniBasis);
            lsIntegralDepth = D - A;
            
            if (A < 0.0)
            {
                lsIntegralOrigin = lsRayOrigin;
                lsIntegralDepth = D;
            }

            const float integralResult = 0.5 * max(0.0, SolvePointLightIntegral1D(lsIntegralOrigin.x, lsIntegralOrigin.y, lsIntegralDepth, input.Light.Scattering));
            
            integralSum += integralResult;
        }
        // Calculate inner air light (Segment BC)
        // 4 Points available A B C D
        UNITY_BRANCH
        if (innerIntersectionsFound.y)
        {
            // Segment B C
            lsIntegralOrigin = TransformToLightSpace2D(BPos - input.Light.Origin, lsOmniBasis);
            lsIntegralDepth = C - B;
            
            // Check for inner case
            if (B < 0.0)
            {
                lsIntegralOrigin = lsRayOrigin;
                lsIntegralDepth = C;
            }

            const float integralResult = max(0.0, SolvePointLightIntegral1D(lsIntegralOrigin.x, lsIntegralOrigin.y, lsIntegralDepth, input.Light.Scattering));

            integralSum += integralResult;
        }

        return input.Light.Color * (_AirDensity * max(0.0, integralSum));
    }

    float4 CalculateAirLightSpotHardEdgeSingle(const AVL_AirLightInput input)
    {
        const float primaryCosine = input.Light.PrimaryAngle.z;

        float2 outerIntersections;
        bool2 outerIntersectionsFound = IntersectCone(input.Ray.Origin, input.Ray.Direction, input.Light.Origin, input.Light.Forward, input.Light.Range, primaryCosine, outerIntersections);

        UNITY_BRANCH
        if (!outerIntersectionsFound.x && !outerIntersectionsFound.y)
        {
            return 0.0;
        }

        const float3x3 lsOmniBasis = GetOmniLightSpaceBasis(input.Ray.Origin, input.Ray.Direction, input.Light.Origin);

        // const float3 lsRayOrigin = mul(input.WorldToLight, float4(input.Ray.Origin, 1.0)).xyz;
        const float2 lsRayOrigin = TransformToLightSpace2D(input.Ray.Origin - input.Light.Origin, lsOmniBasis);

        float integralSum = 0.0;

        float A = outerIntersections.x;
        float D = outerIntersections.y;

        A = min(A, input.RayLength);
        D = min(D, input.RayLength);

        const float3 APos = input.Ray.Origin + input.Ray.Direction * A;

        // Calculate air light (Segment AD)
        // 2 Points available A D
        UNITY_BRANCH
        if (outerIntersectionsFound.y)
        {
            // float2 lsIntegralOrigin = mul(input.WorldToLight, float4(APos, 1.0));
            float2 lsIntegralOrigin = TransformToLightSpace2D(APos - input.Light.Origin, lsOmniBasis);
            float lsIntegralDepth = D - A;
            
            // Check for inner case
            if (A < 0.0)
            {
                lsIntegralOrigin = lsRayOrigin;
                lsIntegralDepth = D;
            }

            // Check for inner case
            if (A < 0.0)
            {
                lsIntegralOrigin = lsRayOrigin;
                lsIntegralDepth = D;
            }

            float integralResult = max(0.0, SolvePointLightIntegral1D(lsIntegralOrigin.x, lsIntegralOrigin.y, lsIntegralDepth, input.Light.Scattering));

            integralSum += integralResult;
        }

        return input.Light.Color * (_AirDensity * max(0.0, integralSum));
    }

    float4 CalculateAirLightAreaHardEdge(AVL_AirLightInput input)
    {
        const float2 rect = 0.5 * input.Light.Rect;
        
        float3x3 lsBasis = GetDefaultLightSpaceBasis(input.Light.Right, input.Light.Up, input.Light.Forward);

        lsBasis[0] += float3(0, 0, -input.Light.PrimaryAngle.w);
        lsBasis[1] += float3(0, 0, -input.Light.SecondaryAngle.w);
        
        const float3 lsRayOrigin = TransformToLightSpace3D(input.Ray.Origin - input.Light.Origin, lsBasis);
        const float3 lsRayDirection = TransformToLightSpace3D(input.Ray.Direction, lsBasis);
        
        float2 intersections;
        
        UNITY_BRANCH
        if (!IntersectAABB(lsRayOrigin, lsRayDirection, float3(-rect.x, -rect.y, 0.0), float3(rect.x, rect.y, input.Light.Range), intersections))
        {
            return 0;
        }

        float A = intersections.x;
        float B = intersections.y;

        A = min(A, input.RayLength);
        B = min(B, input.RayLength);

        float3 lsAPos = lsRayOrigin + lsRayDirection * A;

        const float rayLength = A < 0.0 ? B : B - A;

        float airLight = SolveAreaHardEdgeLightIntegral1D(A < 0.0 ? lsRayOrigin.z : lsAPos.z, lsRayDirection.z, rayLength, input.Light.Scattering);

        airLight = SoftenIntegralEdge(airLight, rayLength, input.Light.Range, input.Light.Scattering);

        airLight = _AirDensity * max(0.0, airLight);

        return input.Light.Color * airLight;
    }

    float4 CalculateAirLightUniformSphere(AVL_AirLightInput input)
    {
        const float airLight = _AirDensity * input.RayLength;

        return input.Light.Color * airLight;
    }

    float4 CalculateAirLightUniformBox(AVL_AirLightInput input)
    {
        // AVL_Ray lsRay = mul_ray(input.WorldToLight, input.Ray);
        const float3x3 lsBasis = GetDefaultLightSpaceBasis(input.Light.Right, input.Light.Up, input.Light.Forward);
        const float3 lsRayOrigin = TransformToLightSpace3D(input.Ray.Origin - input.Light.Origin, lsBasis);
        const float3 lsRayDirection = TransformToLightSpace3D(input.Ray.Direction, lsBasis);
        
        float2 intersections;
        
        if (!IntersectAABB(lsRayOrigin, lsRayDirection, float3(-input.Light.Rect.x, -input.Light.Rect.y, -input.Light.Range), float3(input.Light.Rect.x, input.Light.Rect.y, input.Light.Range), intersections))
        {
            return 0;
        }
        
        const float airLight = _AirDensity * (intersections.y - max(0.0, intersections.x));

        return input.Light.Color * airLight;
    }

    // For some reason, DX9 fails to compile when using this function from another file
    bool IntersectSphereCopy(float3 rayOrigin, float3 rayDirection, float3 sphereOrigin, float sphereRadius, out float2 intersections)
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
    
    float4 CalculateAirLight(float3 rayOrigin, float3 rayDirection, float depth, AVL_LightClusterData lightCluster, uint lightClusterIndex)
    {
        float4 result = 0.0;

        // ToDo GPU occupancy problem
        float2 maskDepths[MAX_MASKS_PER_CLUSTER];

        UNITY_LOOP
        for (uint i = 0; i < min(lightCluster.MaskCount, MAX_MASKS_PER_CLUSTER); i++)
        {
            const uint maskGlobalIndex = _MaskIndexBuffer[lightCluster.MaskBufferOffset + i];
            const AVL_MaskData mask = _GlobalMaskBuffer[maskGlobalIndex];
        
            const float4 msRayOrigin = mul(mask.WorldToMask, float4(rayOrigin, 1.0));
            const float4 msRayDirection = mul(mask.WorldToMask, float4(rayDirection, 0.0));
        
            float2 intersections = 0;
            bool intersectionResult = false;

            UNITY_BRANCH
            switch (mask.Type)
            {
            case 0: // Cuboid
                intersectionResult = IntersectAABB(msRayOrigin.xyz, msRayDirection.xyz, -0.5, 0.5, intersections);
                break;
            case 1: // Ellipsoid
                intersectionResult = IntersectSphereCopy(msRayOrigin.xyz, msRayDirection.xyz, 0, 0.5, intersections);
                break;
            default: // Ignore
                break;
            }
        
            intersections.x = max(intersections.x, 0.0);
            intersections.y = min(intersections.y, depth);
        
            if (!intersectionResult)
            {
                intersections = 0.0;
            }
            maskDepths[i] = intersections;
        }

        UNITY_LOOP
        for (uint j = 0; j < min(lightCluster.LightCount, MAX_LIGHT_PER_CLUSTER); j++)
        {
            const uint lightGlobalIndex = _LightIndexBuffer[lightCluster.BufferOffset + j];
            const AVL_LightData light = _GlobalLightBuffer[lightGlobalIndex];

            float2 depthRange = float2(0, depth);
            float3 lRayOrigin = rayOrigin;
            
            float2 lightIntersections;

            IntersectSphere(rayOrigin, rayDirection, light.BoundingOrigin, light.BoundingRadius, lightIntersections);

            depthRange.x = max(depthRange.x, lightIntersections.x);
            depthRange.y = min(depthRange.y, lightIntersections.y);
            
            if (light.MaskId > -1)
            {
                int localMaskIndex = _MaskInverseIndexBuffer[lightClusterIndex * _GlobalMaskBufferMaxSize + light.MaskId];

                UNITY_BRANCH
                if (localMaskIndex < 0)
                {
                    continue;
                }
                
                depthRange.x = max(depthRange.x, maskDepths[localMaskIndex].x);
                depthRange.y = min(depthRange.y, maskDepths[localMaskIndex].y);
            }
            
            float lDepth = depthRange.y - depthRange.x;
            lRayOrigin += rayDirection * depthRange.x;

            UNITY_BRANCH
            if (lDepth <= 0.0)
            {
                continue;
            }
            float4 lightContribution = 0.0;

            AVL_Ray ray;
            ray.Origin = lRayOrigin;
            ray.Direction = rayDirection;

            AVL_AirLightInput input;
            input.Light = light;
            input.Ray = ray;
            input.RayLength = lDepth;

            float lightDistance = length(light.Origin - _CameraWorldSpacePosition.xyz);
            float d = _FogScattering * _FogDensityGlobal * _FogDensityPerLight * lightDistance;
            float f = 1.0 / exp(d);
            float f2 = 1.0 / exp(d*d);
            input.Light.Scattering += saturate(1.0 - f2 * 1) * _FogScattering;
            input.Light.Color *= f;

            UNITY_BRANCH
            switch (light.Type)
            {
            case AVL_LIGHT_SHAPE_POINT:
                lightContribution = CalculateAirLightPoint(input);
                break;
            case AVL_LIGHT_SHAPE_SPOT_SOFT_EDGE:
                lightContribution = CalculateAirLightSpotSoftEdge(input);
                break;
            case AVL_LIGHT_SHAPE_SPOT_HARD_EDGE:
                lightContribution = CalculateAirLightSpotHardEdge(input);
                break;
            case AVL_LIGHT_SHAPE_SPOT_HARD_EDGE_SINGLE:
                lightContribution = CalculateAirLightSpotHardEdgeSingle(input);
                break;
            case AVL_LIGHT_SHAPE_AREA_HARD_EDGE:
                lightContribution = CalculateAirLightAreaHardEdge(input);
                break;
            case AVL_LIGHT_SHAPE_UNIFORM_SPHERE:
                lightContribution = CalculateAirLightUniformSphere(input);
                break;
            case AVL_LIGHT_SHAPE_UNIFORM_BOX:
                lightContribution = CalculateAirLightUniformBox(input);
                break;
            default:
                break;
            }

            result += lightContribution * (1.0 - light.CullingFading);
        }

        return result;
    }

    float CorrectDepth(float rawDepth)
    {
        // Orthographic View
        if (unity_OrthoParams.w > 0.5)
        {
            return (_ProjectionParams.z-_ProjectionParams.y)*(1-rawDepth)+_ProjectionParams.y;
        }
        
        return LinearEyeDepth(rawDepth, _ZBufferParams);
    }

    

ENDHLSL
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZTest Always ZWrite Off Cull Back
        
        Pass
        {
            Name "AVL - Rendering"

            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag_render
            ENDHLSL
        }
        Pass
        {
            Name "AVL - Upscaling"
            
            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag_upscale
            ENDHLSL
        }
        Pass
        {
            Name "AVL - Blending"
            
            Blend One One
            
            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag_blend
            ENDHLSL
        }
        Pass
        {
            Name "AVL - Global Fog"
            
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag_fog
            ENDHLSL
        }
    }
}
