Shader "Hidden/AkiDevCat/AVL/DebugPass"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM

            #include "../Includes/AVLStructs.hlsl"
            
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 pos : POSITION1;
                float2 uv : TEXCOORD0;
            };

            uint                                        _DebugMode;
            float                                       _DebugModeOpacity;
            float4                                      _ClusterSize;
            
            StructuredBuffer  <AVL_LightClusterData>    _LightClusterBuffer;
            StructuredBuffer  <AVL_LightData>           _GlobalLightBuffer;
            uint                                        _GlobalLightBufferSize;
            StructuredBuffer  <uint>                    _LightIndexBuffer;
            Texture2D<uint>                             _LightClusterDepthTexture;

            TEXTURE2D         (_CameraDepthTexture);
            TEXTURE2D         (_AVLFogTexture);
            SAMPLER           (sampler_CameraDepthTexture);

            float4x4 _CameraViewMatrix;

            float3 HUEtoRGB(in float H)
            {
                float R = abs(H * 6 - 3) - 1;
                float G = 2 - abs(H * 6 - 2);
                float B = 2 - abs(H * 6 - 4);
                return saturate(float3(R,G,B));
            }

            float CorrectDepth(float rawDepth)
            {
                float persp = LinearEyeDepth(rawDepth, _ZBufferParams);
                float ortho = (_ProjectionParams.z-_ProjectionParams.y)*(1-rawDepth)+_ProjectionParams.y;
                return lerp(persp,ortho,unity_OrthoParams.w);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.pos = mul(UNITY_MATRIX_I_VP, float4(o.vertex.xy, -1.0, 1.0));
                o.pos.xyz /= o.pos.w;
                o.uv = (o.vertex.xy / 2.0 + 0.5); // ToDo use Unity integrated methods
                #if UNITY_UV_STARTS_AT_TOP
                o.uv.y = 1.0 - o.uv.y;
                #endif
                return o;
            }

            float4 frag (v2f input) : SV_Target
            {
                uint2 idXY = input.uv * _ClusterSize.xy;
                uint id = idXY.x + idXY.y * _ClusterSize.x;
                AVL_LightClusterData cluster = _LightClusterBuffer[id];
                float hash = Hash(idXY.x + idXY.y * _ClusterSize.x);

                float rawDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv).r;
                float depth = CorrectDepth(rawDepth);

                float3 viewPlane = (input.pos.xyz - _WorldSpaceCameraPos) / dot(input.pos.xyz - _WorldSpaceCameraPos, unity_WorldToCamera._m20_m21_m22);

                depth *= -length(viewPlane);
                
                float3 rayOrigin = input.pos.xyz;
                float3 rayDir = normalize(input.pos.xyz - _WorldSpaceCameraPos);

                float3 wsPos = rayOrigin + rayDir * depth;

                float3 vsPos = mul(_CameraViewMatrix, float4(wsPos.xyz, 1)).xyz;
                vsPos.z = -vsPos.z;

                float4 result = 0;
                
                switch (_DebugMode)
                {
                default:
                case AVL_DEBUG_MODE_NONE:
                    return 0;
                    
                case AVL_DEBUG_MODE_LIGHT_CLUSTERS:
                    return (idXY.x + idXY.y) % 2 == 0 ? float4(1, 0, 1, _DebugModeOpacity) : float4(0, 1, 1, _DebugModeOpacity);

                case AVL_DEBUG_MODE_LIGHT_OVERDRAW:
                    result.rgb = saturate(cluster.LightCount / (float)MAX_LIGHT_PER_CLUSTER * _DebugModeOpacity + (1.0 - _DebugModeOpacity));
                    result.a = saturate(cluster.LightCount / (float)MAX_LIGHT_PER_CLUSTER * (1.0 - _DebugModeOpacity) + _DebugModeOpacity) * _DebugModeOpacity * _DebugModeOpacity;
                    return cluster.LightCount > 0 ? result : 0;

                case AVL_DEBUG_MODE_LIGHT_COUNT:
                    if (cluster.LightCount >= MAX_LIGHT_PER_CLUSTER)
                        return float4(1, 0, 1, saturate(_DebugModeOpacity * 4.0));
                    result.rgb = HUEtoRGB(1.0 / 3.0 - 1.0 / MAX_LIGHT_PER_CLUSTER + cluster.LightCount / (float)MAX_LIGHT_PER_CLUSTER);
                    return cluster.LightCount > 0 ? float4(result.rgb, _DebugModeOpacity) : 0;

                case AVL_DEBUG_MODE_VOLUMETRIC_LIGHT:
                    return SAMPLE_TEXTURE2D(_AVLFogTexture, sampler_PointClamp, input.uv);
                }
                
                return 0;
            }
            ENDHLSL
        }
    }
}