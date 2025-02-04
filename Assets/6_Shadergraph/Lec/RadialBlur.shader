Shader "Custom/RadialBlur"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _BlurStrength ("Blur Strength", Range(0, 1)) = 0.5
        _BlurSamples ("Blur Samples", Range(1, 20)) = 10
        _Center ("Blur Center", Vector) = (0.5, 0.5, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Pass
        {
            Name "RadialBlurPass"
            ZTest Always
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLINCLUDE
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

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurStrength;
            int _BlurSamples;
            float2 _Center;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 dir = IN.uv - _Center;
                float distance = length(dir);
                dir = normalize(dir + 0.0001); // 0으로 나누기 방지

                half4 color = tex2D(_MainTex, IN.uv);
                for (int i = 1; i < _BlurSamples; i++)
                {
                    float scale = i * (_BlurStrength / _BlurSamples);
                    color += tex2D(_MainTex, IN.uv - dir * scale);
                }

                color /= _BlurSamples;
                return color;
            }
            ENDHLSL

            // Shader 단계 연결
            Cull Off ZWrite Off ZTest Always
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }
    FallBack Off
}
