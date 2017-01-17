//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

Shader "WeatherMaker/WeatherMakerBoxFogShader"
{
	Properties
	{
		_FogColor("Fog Color", Color) = (0,1,1,1)
		_FogNoise("Fog Noise", 2D) = "white" {}
		_FogNoiseScale("Fog Noise Scale", Range(0.0, 1.0)) = 0.0005
		_FogNoiseMultiplier("Fog Noise Multiplier", Range(0.01, 1.0)) = 0.15
		_FogNoiseVelocity("Fog Noise Time Multiplier", Vector) = (0.01, 0.01, 0, 0)
		_FogNoiseHeight("Fog Noise Distortion", 2D) = "white" {}
		_FogNoiseHeightScale("Fog Noise Distortion Scale", Range(0.0, 1.0)) = 0.05
		_FogNoiseHeightMultiplier("Fog Noise Distortion Multiplier", Range(0.0, 1)) = 0.1
		_FogNoiseHeightVariance("Fog Noise Distortion Variance", Range(0.0, 100.0)) = 10.0
		_FogDensity("Fog Density", Range(0.0, 1.0)) = 0.05
		_FogBoxMin("Fog Box Min", Vector) = (0, 0, 0, 0)
		_FogBoxMax("Fog Box Max", Vector) = (10, 10, 10, 0)
		_FogPercentage("Percentage of Box to Fill", Range(0.0, 1.0)) = 0.9
		_MaxFogFactor("Maximum Fog Factor", Range(0.01, 1)) = 1
		_SunColor("Sun Color", Vector) = (1.0, 1.0, 1.0, 1.0)
		_SunDirection("Sun Direction", Vector) = (0, 0, 0, 0)
		_DitherLevel("Dither Level", Range(0, 1)) = 0.005
	}
	Category
	{
		Tags{ "Queue" = "Transparent+98" "IgnoreProjector" = "True" "RenderType" = "Transparent" "LightMode" = "Always" }
		Cull Front Lighting Off ZWrite Off ZTest Always Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		SubShader
		{
			Pass
			{
				CGPROGRAM

				#pragma target 3.0
				#pragma vertex fog_volume_vertex_shader
				#pragma fragment fog_box_fragment_shader
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma glsl_no_auto_normalization
				#pragma multi_compile __ ENABLE_FOG_NOISE
				#pragma multi_compile __ ENABLE_FOG_HEIGHT_WITH_NOISE
				#pragma multi_compile __ FOG_NONE FOG_EXPONENTIAL FOG_LINEAR FOG_EXPONENTIAL_SQUARED FOG_CONSTANT

				#include "WeatherMakerFogShader.cginc"

				ENDCG
			}
		}
	}
	Fallback "VertexLit"
}
