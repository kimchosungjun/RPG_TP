
Shader "CameraFilterPack/Blur_Focus"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _TimeX("Time", Range(0.0, 1.0)) = 1.0
        _Size("Size", Range(0.0, 1.0)) = 1.0
        _Circle("Circle", Range(0.0, 1.0)) = 1.0
        _Distortion("_Distortion", Range(0.0, 1.0)) = 0.3
        _ScreenResolution("_ScreenResolution", Vector) = (0.,0.,0.,0.)
        _CenterX("_CenterX", Range(-1.0, 1.0)) = 0
        _CenterY("_CenterY", Range(-1.0, 1.0)) = 0
    }
        SubShader
        {
            Pass
            {
                ZTest Always
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #pragma target 3.0
                #pragma glsl
                #include "UnityCG.cginc"

                uniform sampler2D _MainTex;
                uniform float _TimeX;
                uniform float _Distortion;
                uniform float4 _ScreenResolution;
                uniform float _CenterX;
                uniform float _CenterY;
                uniform float _Circle;
                uniform float _Size;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color;

                return o;
            }

            #define tex2D(sampler, uvs) tex2Dlod( sampler, float4( (uvs), 0.0f, 0.0f ))

            float2 barrelDistortion(float2 coord, float amt)
            {
                float cc = coord - float2(0.5 + _CenterX / 2, 0.5 + _CenterY / 2);
                return coord + cc * dot(cc, cc) * amt;
            }

            fixed4 frag(v2f i) : COLOR
            {
                float2 uv = (i.texcoord.xy);
                fixed4 tx = 0.0;

                for (float u = 0.0; u < _Size; u += 0.2)
                {
                    fixed4 a = tex2D(_MainTex, barrelDistortion(uv, u / _Circle));
                    tx += a;
                }

                tx /= _Size * 5;

                return float4( tx.rgb, 1. );
            }
            ENDCG
        }
    }
}
