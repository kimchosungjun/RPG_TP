Shader "CS_Mosaic/Mosaic"
{
	Properties
	{
		[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
		_TilingX("TilingX", float) = 1
		_TilingY("TilingY", float) = 1
		_Brightness("Brightness", range(0, 100)) = 1
		_OffsetX("OffsetX", float) = 0
		_OffsetY("OffsetY", float) = 0
	}

    Category
	{
		Tags { "RenderType" = "Queue" }

		SubShader
		{
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float2 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _TilingX, _TilingY;
				float _Brightness;
				float _OffsetX, _OffsetY;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

					UNITY_TRANSFER_FOG(o, o.vertex);

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float4 c = tex2D(_MainTex, float2(_OffsetX, _OffsetY) +
					(floor(i.texcoord * _TilingX) / _TilingX,
						floor(i.texcoord * _TilingY) / _TilingY));

					UNITY_APPLY_FOG(i.fogCoord, col);

					return c * _Brightness;
				}
				ENDCG
			}
		}
	}
}
