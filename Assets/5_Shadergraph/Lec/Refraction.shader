Shader"Custom/Refraction"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _NoiseValue("NoiseValue", Range(0, 1)) = 0.0
        _Speed("Speed", float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
LOD 200

        GrabPass
{
} //카메라 화면

        CGPROGRAM
        #pragma surface surf nolight noambient

        #pragma target 2.0

sampler2D _MainTex;
sampler2D _GrabTexture; //메인카메라화면

struct Input
{
    float2 uv_MainTex;
    float4 screenPos;
};

half _Glossiness;
half _Metallic;
fixed4 _Color;

float _NoiseValue;
float _Speed;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

void surf(Input IN, inout SurfaceOutput o)
{
    fixed4 noise = tex2D(_MainTex, IN.uv_MainTex);

    float2 screenUV = IN.screenPos.rgb / IN.screenPos.a;

    o.Emission = tex2D(_GrabTexture, float2(
                (screenUV.x) + noise.x * _NoiseValue * sin(_Time.x * _Speed),
                (screenUV.y) + noise.y * _NoiseValue * sin(_Time.y * _Speed)
                ));
}

float4 Lightingnolight(SurfaceOutput s, float3 lightDir, float atten)
{
    return float4(0, 0, 0, 1);
}

        ENDCG
    }

FallBack"Ragacy Shaders/Transparent/Vertexlit"
}
