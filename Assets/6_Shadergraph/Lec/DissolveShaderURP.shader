Shader "Custom/DissolveShaderURP"
{
    Properties
    {
        _BaseMap ("Base Texture", 2D) = "white" {}
        _DissolveTexture ("Dissolve Texture", 2D) = "white" {} // 기본값 추가
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _EdgeWidth ("Edge Width", Range(0, 0.2)) = 0.05
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        _EmissionStrength ("Emission Strength", Range(0, 5)) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="AlphaTest" "RenderPipeline"="UniversalRenderPipeline" }
        
        Pass
        {
            Name "DissolvePass"
            Tags { "LightMode"="UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_DissolveTexture);
            SAMPLER(sampler_DissolveTexture);
            
            CBUFFER_START(UnityPerMaterial)
            float _DissolveAmount;
            float _EdgeWidth;
            float4 _EdgeColor;
            float _EmissionStrength;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 기본 텍스처 적용
                float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

                // Dissolve Mask 텍스처 적용 (없을 경우 기본값 1로 설정)
                float dissolveValue = SAMPLE_TEXTURE2D(_DissolveTexture, sampler_DissolveTexture, IN.uv).r;
                dissolveValue = max(dissolveValue, 0.5); // 기본값을 0.5로 설정하여 정상 작동 보장

                // Dissolve 값과 비교하여 제거
                float dissolveThreshold = _DissolveAmount;
                float edgeThreshold = _DissolveAmount + _EdgeWidth;

                // 알파 클리핑 처리
                clip(dissolveValue - dissolveThreshold);

                // 경계선 효과 (DissolveAmount 근처에서 EdgeColor 적용)
                float edgeFactor = smoothstep(dissolveThreshold, edgeThreshold, dissolveValue);
                float3 finalColor = baseColor.rgb * (1.0 - edgeFactor) + _EdgeColor.rgb * edgeFactor * _EmissionStrength;

                return half4(finalColor, baseColor.a);
            }
            ENDHLSL
        }
    }
}
