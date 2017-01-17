//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

Shader "WeatherMaker/WeatherMakerDepthSamplerShader"
{
	Properties
	{
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ SAMPLE_DEPTH_NORMALS

			#include "UnityCG.cginc"

#if defined(SAMPLE_DEPTH_NORMALS)

			sampler2D _CameraDepthNormalsTexture;

#else

			sampler2D _CameraDepthTexture;

#endif

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_TARGET
			{

#if defined(SAMPLE_DEPTH_NORMALS)

				return tex2D(_CameraDepthNormalsTexture, i.uv);

#else

				return UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));

#endif

			}

			ENDCG
		}
	}
}
