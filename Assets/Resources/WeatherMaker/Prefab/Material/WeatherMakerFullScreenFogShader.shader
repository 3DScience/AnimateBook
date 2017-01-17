//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

Shader "WeatherMaker/WeatherMakerFullScreenFogShader"
{
	Properties
	{
		_FogColor("Fog Color", Color) = (0,1,1,1)
		_FogNoise("Fog Noise", 2D) = "white" {}
		_FogNoiseScale("Fog Noise Scale", Range(0.0, 1.0)) = 0.0005
		_FogNoiseMultiplier("Fog Noise Multiplier", Range(0.01, 1.0)) = 0.15
		_FogNoiseVelocity("Fog Noise Time Multiplier", Vector) = (0.01, 0.01, 0, 0)
		_FogNoiseHeight("Fog Noise Height", 2D) = "white" {}
		_FogNoiseHeightScale("Fog Noise Height Scale", Range(0.0, 1.0)) = 0.05
		_FogNoiseHeightMultiplier("Fog Noise Height Multiplier", Range(0.0, 1)) = 0.1
		_FogNoiseHeightVariance("Fog Noise Height Variance", Range(0.0, 100.0)) = 10.0
		_FogDensity("Fog Density", Range(0.0, 1.0)) = 0.05
		_FogHeight("Fog Height", Float) = 0
		_MaxFogFactor("Maximum Fog Facto", Range(0.01, 1)) = 1
		_FarPlaneSunThreshold("Far Plane Sun Threshold", Range(0, 1)) = 0.75
		_SunColor("Sun Color", Vector) = (1.0, 1.0, 1.0, 1.0)
		_SunDirection("Sun Direction", Vector) = (0, 0, 0, 0)
		_DitherLevel("Dither Level", Range(0, 1)) = 0.005
		_LinearFogDistance("Linear Fog Distance", Float) = 1000
	}
	Category
	{
		Cull Back ZWrite Off ZTest Always
		Blend [_SrcBlendMode] [_DstBlendMode]

		SubShader
		{
			Pass
			{
				CGPROGRAM

				#pragma target 3.0
				#pragma vertex fog_full_screen_vertex_shader
				#pragma fragment fog_box_full_screen_fragment_shader
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma glsl_no_auto_normalization
				#pragma multi_compile __ ENABLE_SUN
				#pragma multi_compile __ ENABLE_SCALING
				#pragma multi_compile __ ENABLE_FOG_NOISE
				#pragma multi_compile __ ENABLE_FOG_HEIGHT ENABLE_FOG_HEIGHT_WITH_NOISE
				#pragma multi_compile __ FOG_NONE FOG_EXPONENTIAL FOG_LINEAR FOG_EXPONENTIAL_SQUARED FOG_CONSTANT

				#include "WeatherMakerFogShader.cginc"

				ENDCG
			}
		}
	}
	Fallback "VertexLit"
}
