//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

// http://rastergrid.com/blog/2010/09/efficient-gaussian-blur-with-linear-sampling/
// _MainTex must be bilinear

Shader "WeatherMaker/WeatherMakerBlurShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "red" {}
	}
	SubShader
	{
		Cull Back ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		CGINCLUDE

		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma multi_compile __ BLUR7 DITHER

		#include "WeatherMakerShader.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

#if defined(DITHER)

		float _DitherLevel;
		sampler2D _DitherTex;

#endif

		v2f vert (appdata v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.uv;

#if UNITY_UV_STARTS_AT_TOP

			if (_MainTex_TexelSize.y < 0)
			{
				o.uv.y = 1 - o.uv.y;
			}

#endif

			return o;
		}

		ENDCG

		// optimized blur
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

#if defined(BLUR7)

				// 7 tap approximation
				float offsetX = _MainTex_TexelSize.x * 0.333333;
				float offsetY = _MainTex_TexelSize.y * 0.333333;
				col += tex2D(_MainTex, float2(i.uv.x - offsetX, i.uv.y - offsetY));
				col += tex2D(_MainTex, float2(i.uv.x + offsetX, i.uv.y + offsetY));
				col *= 0.333333;

#else

				// 17 tap approximation
				// (0.4,-1.2) , (-1.2,-0.4) , (1.2,0.4) and (-0.4,1.2).
				float offsetXSmall = _MainTex_TexelSize.x * 0.4;
				float offsetXLarge = _MainTex_TexelSize.x * 1.2;
				float offsetYSmall = _MainTex_TexelSize.y * 0.4;
				float offsetYLarge = _MainTex_TexelSize.y * 1.2;
				col += tex2D(_MainTex, float2(i.uv.x + offsetXSmall, i.uv.y - offsetYLarge));
				col += tex2D(_MainTex, float2(i.uv.x - offsetXLarge, i.uv.y - offsetYSmall));
				col += tex2D(_MainTex, float2(i.uv.x + offsetXLarge, i.uv.y + offsetYSmall));
				col += tex2D(_MainTex, float2(i.uv.x - offsetXSmall, i.uv.y + offsetYLarge));
				col *= 0.2;

#endif

				return col;
            }

            ENDCG
		}
	}
}
