//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

#include "WeatherMakerShader.cginc"

struct fog_vertex
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
	float3 normal : NORMAL;
};

struct fog_full_screen_fragment
{
	float4 vertex : SV_POSITION;
	float2 uv : TEXCOORD0;
	float3 forwardLine : TEXCOORD1;
};

fixed4 _FogColor;
sampler2D _FogNoise;
float4 _FogNoise_TexelSize;
float _FogNoiseScale;
float _FogNoiseMultiplier;
float3 _FogNoiseVelocity;
sampler2D _FogNoiseHeight;
float _FogNoiseHeightScale;
float _FogNoiseHeightMultiplier;
float _FogNoiseHeightVariance;
float _FogDensity;
float _FogHeight;
float3 _FogBoxMin;
float3 _FogBoxMax;
float _MaxFogFactor;
float _FarPlaneSunThreshold;
float4x4 _CameraInverseMVP;
float4x4 _CameraInverseMV;
float3 _SunDirection;
fixed4 _SunColor;
fixed _DitherLevel;
float _LinearFogDistance;

#if defined(ENABLE_SCALING)

sampler2D _CameraDepthTextureScaled;
//sampler2D _CameraDepthNormalsTextureScaled;

#else

//sampler2D _CameraDepthNormalsTexture;

#endif

inline float CalculateFogFactor(float depth)
{

#if defined(FOG_CONSTANT)

	float fogFactor = _FogDensity * ceil(saturate(depth));

#elif defined(FOG_LINEAR)

	float fogFactor = saturate((depth / _ProjectionParams.z) * (_FogDensity * _LinearFogDistance));

#elif defined(FOG_EXPONENTIAL)

	// simple height formula
	// const float extinction = 0.01;
	// float fogFactor = saturate((_FogDensity * exp(-(_WorldSpaceCameraPos.y - _FogHeight) * extinction) * (1.0 - exp(-depth * rayDir.y * extinction))) / rayDir.y);

	float fogFactor = 1.0 - saturate(1.0 / (exp(depth * _FogDensity)));

#else

	float expFog = exp(depth * _FogDensity);
	float fogFactor = 1.0 - saturate(1.0 / (expFog * expFog));

#endif

	return fogFactor;
}

#if defined(ENABLE_FOG_NOISE)

inline float CalculateFogNoiseBox(sampler2D noiseTex, float3 normal, float3 worldPos, float scale, float3 velocity, float multiplier)
{
	return CalculateNoiseSphere(noiseTex, normal, worldPos, scale, velocity, multiplier);
}

#endif

inline float GetDepth01(float2 uv)
{

#if defined(ENABLE_SCALING)

	// read depth again to get a 32 bit high precision format
	return Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTextureScaled, uv)));

#else

	// read depth again to get a 32 bit high precision format
	return Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, uv)));

#endif

}

// uv are in screen space
inline void CalculateFinalFog(float noise, float2 uv, inout float fogFactor, inout fixed3 fogColor)
{
	fogFactor = clamp(fogFactor * noise, 0.000001f, _MaxFogFactor);
	const fixed3 magic = fixed3(0.06711056, 0.00583715, 52.9829189);
	fixed gradient = frac(magic.z * frac(dot(uv / float2(_ScreenParams.z - 1.0, _ScreenParams.w - 1.0), magic.xy))) * _DitherLevel * (1.0 / fogFactor);
	fogColor.rgb -= gradient.rrr;
}

inline void RaycastFogBoxFullScreen(float3 rayDir, float3 forwardLine, out float3 worldPos, inout float depth, out float noise)
{
	// depth is 0-1 value, which needs to be changed to world space distance
	worldPos = _WorldSpaceCameraPos + (forwardLine * depth);

	// calculate depth exactly in world space
	depth = distance(worldPos, _WorldSpaceCameraPos);

#if defined(ENABLE_FOG_HEIGHT) || defined(ENABLE_FOG_HEIGHT_WITH_NOISE)

	// cast ray, get amount of intersection with the fog box
	float savedDepth = depth;
	float distanceToBox;
	float3 boxMin, boxMax;
	GetFullScreenBoundingBox(_FogHeight, boxMin, boxMax);
	RayBoxIntersect(_WorldSpaceCameraPos, rayDir, savedDepth, boxMin, boxMax, depth, distanceToBox);

	// update world pos with the new intersect point
	worldPos = _WorldSpaceCameraPos + (rayDir * distanceToBox);

	// approaches 1 as we exit the box
	float inBoxMultiplier = saturate(distanceToBox);

#if defined(ENABLE_FOG_NOISE)

#if defined(ENABLE_FOG_HEIGHT_WITH_NOISE)

	// create a noise to move the y value - we use the ray at the base fog height to get a noise value that is consistant
	float fogYNoise = CalculateNoiseXZ(_FogNoiseHeight, worldPos, _FogNoiseHeightScale, _FogNoiseVelocity, _FogNoiseHeightMultiplier) * inBoxMultiplier * _FogNoiseHeightVariance;
	float fogY = _FogHeight + fogYNoise;

	// re-cast the ray with the new y value
	GetFullScreenBoundingBox(fogY, boxMin, boxMax);
	RayBoxIntersect(_WorldSpaceCameraPos, rayDir, savedDepth, boxMin, boxMax, depth, distanceToBox);
	inBoxMultiplier = saturate(distanceToBox);

	// reset the worldPos for noise calculation to the new height point
	// this can be commented out to get a more consistant fog look at different random heights
	worldPos = _WorldSpaceCameraPos + (rayDir * distanceToBox);

#endif

	// calculate noise for the world pos looking at box
	noise = CalculateNoiseXZ(_FogNoise, worldPos, _FogNoiseScale, _FogNoiseVelocity, _FogNoiseMultiplier) * inBoxMultiplier;

	// and finally calculate noise looking in the box out
	float3 rayExitPoint = _WorldSpaceCameraPos + (rayDir * min(depth, 100));
	noise += CalculateNoiseXZ(_FogNoise, rayExitPoint, _FogNoiseScale, _FogNoiseVelocity, _FogNoiseMultiplier) * 0.1 * (1.0 - inBoxMultiplier);

	// cancel out noise where there is no fog
	noise = saturate(ceil(depth)) * saturate(ceil(_FogDensity)) * (1.0 + noise);

#else

	noise = 1.0;

#endif

#elif defined(ENABLE_FOG_NOISE)

	float3 rayExitPoint = _WorldSpaceCameraPos + (rayDir * min(depth, 100));
	noise = CalculateNoiseXZ(_FogNoise, rayExitPoint, _FogNoiseScale, _FogNoiseVelocity, _FogNoiseMultiplier * 0.1);

	// cancel out noise where there is no fog
	noise = saturate(ceil(depth)) * saturate(ceil(_FogDensity)) * (1.0 + noise);

#else

	noise = 1.0;

#endif

}

inline void RaycastFogBox(float3 rayDir, float3 normal, inout float depth, inout float3 worldPos, out float noise)
{	
	// cast ray, get amount of intersection with the fog box
	float savedDepth = depth;
	float distanceToBox;
	RayBoxIntersect(_WorldSpaceCameraPos, rayDir, savedDepth, _FogBoxMin, _FogBoxMax, depth, distanceToBox);

	// update world pos with the new intersect point
	float inBoxMultiplier = saturate(ceil(distanceToBox));
	float outBoxMultiplier = inBoxMultiplier;
	inBoxMultiplier = 1.0 - inBoxMultiplier;
	float inBoxAdder = inBoxMultiplier * 2.0f;
	float outBoxAdder = outBoxMultiplier * (distanceToBox + min(2.0, depth * 0.1));
	worldPos = _WorldSpaceCameraPos + (rayDir * (outBoxAdder + inBoxAdder));

#if defined(ENABLE_FOG_NOISE)

#if defined(ENABLE_FOG_HEIGHT_WITH_NOISE)

	// create a noise value to move the box values using the initial intersect point for the noise calculation
	//float fogNoise = CalculateFogNoiseBox(_FogNoiseHeight, normal, worldPos, _FogNoiseHeightScale, _FogNoiseVelocity, _FogNoiseHeightMultiplier) * _FogNoiseHeightVariance;
	float fogNoise = (CalculateFogNoiseBox(_FogNoiseHeight, normal, worldPos, _FogNoiseHeightScale, _FogNoiseVelocity, _FogNoiseHeightMultiplier) * _FogNoiseHeightVariance * outBoxMultiplier) + (CalculateNoiseAverage(_FogNoise, normal, worldPos, _FogNoiseScale, _FogNoiseVelocity, _FogNoiseMultiplier) * _FogNoiseHeightVariance * inBoxMultiplier);
	float3 newMin = _FogBoxMin - fogNoise;
	float3 newMax = _FogBoxMax + fogNoise;
	RayBoxIntersect(_WorldSpaceCameraPos, rayDir, savedDepth, newMin, newMax, depth, distanceToBox);

#endif

	// calculate noise
	//noise = 1.0 + CalculateFogNoiseBox(_FogNoise, normal, worldPos, _FogNoiseScale, _FogNoiseVelocity, _FogNoiseMultiplier);
	noise = 1.0 + (CalculateFogNoiseBox(_FogNoise, normal, worldPos, _FogNoiseScale, _FogNoiseVelocity, _FogNoiseMultiplier) * outBoxMultiplier) + (CalculateNoiseAverage(_FogNoise, normal, worldPos, _FogNoiseScale, _FogNoiseVelocity, _FogNoiseMultiplier) * inBoxMultiplier);

#else

	noise = 1.0;

#endif

}

// sphere is xyz, w = radius squared
inline float RayMarchFogSphere(volumetric_data i, int iterations, float4 sphere, float density, float outerDensity, out float clarity, out float3 rayDir, out float3 sphereCenterViewSpace, out float maxDistance)
{
	float2 screenUV = i.projPos.xy / i.projPos.w;
	maxDistance = length(DECODE_EYEDEPTH(tex2D(_CameraDepthTexture, screenUV).r) / normalize(i.viewPos).z);
	//float depthBufferDepth = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
	rayDir = normalize(i.viewPos.xyz);
	sphereCenterViewSpace = mul((float3x3)UNITY_MATRIX_V, (_WorldSpaceCameraPos - sphere.xyz));
	float invSphereRadiusSquared = 1.0 / sphere.w;

	// calculate sphere intersection
	float b = -dot(rayDir, sphereCenterViewSpace);
	float c = dot(sphereCenterViewSpace, sphereCenterViewSpace) - sphere.w;
	float d = sqrt((b * b) - c);
	float dist = b - d;
	float dist2 = b + d;

	/*
	float fA = dot(rayDir, rayDir);
	float fB = 2 * dot(rayDir, sphereCenterViewSpace);
	float fC = dot(sphereCenterViewSpace, sphereCenterViewSpace) - sphere.w;
	float fD = fB * fB - 4 * fA * fC;
	// if (fD <= 0.0f) { return; } // not sure if this is needed, doesn't seem to trigger very often
	float recpTwoA = 0.5 / fA;
	float DSqrt = sqrt(fD);
	// the distance to the front of sphere, or 0 if inside the sphere. This is the distance from the camera where sampling begins.
	float dist = max((-fB - DSqrt) * recpTwoA, 0);
	// total distance to the back of the sphere.
	float dist2 = max((-fB + DSqrt) * recpTwoA, 0);
	*/

	// stop at the back of the sphere or depth buffer, whichever is the smaller distance.
	float backDepth = min(maxDistance, dist2);

	// calculate initial sample distance, and the distance between samples.
	float samp = dist;
	float step_distance = (backDepth - dist) / (float)iterations;

	// how much does each step get modified? approaches 1 with distance.
	float step_contribution = (1 - 1 / pow(2, step_distance)) * density;

	// 1 means no fog, 0 means completely opaque fog
	clarity = 1;

	for (int i = 0; i < iterations; i++)
	{
		float3 position = sphereCenterViewSpace + (rayDir * samp);
		float val = saturate(outerDensity * (1.0 - (dot(position, position) * invSphereRadiusSquared)));
		clarity *= (1.0 - saturate(val * step_contribution));
		samp += step_distance;
	}

	return clarity;
}

// VERTEX AND FRAGMENT SHADERS ----------------------------------------------------------------------------------------------------

fog_full_screen_fragment fog_full_screen_vertex_shader(fog_vertex v)
{
	fog_full_screen_fragment o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = v.uv;

#if UNITY_UV_STARTS_AT_TOP

	if (_MainTex_TexelSize.y < 0)
	{
		o.uv.y = 1 - o.uv.y;
	}

#endif

	o.forwardLine = GetFarPlaneVector(_CameraInverseMVP, o.vertex);

	return o;
}

volumetric_data fog_volume_vertex_shader(fog_vertex v)
{
	return GetVolumetricData(v.vertex, v.normal);
}

fixed4 fog_box_full_screen_fragment_shader(fog_full_screen_fragment i) : SV_TARGET
{

#if defined(FOG_NONE)

	return fixed4(0, 0, 0, 0);

#else

	float depth = GetDepth01(i.uv);
	float noise;
	float3 worldPos;
	float3 rayDir = normalize(i.forwardLine);
	RaycastFogBoxFullScreen(rayDir, i.forwardLine, worldPos, depth, noise);
	float fogFactor = CalculateFogFactor(depth);

#if defined(ENABLE_SUN)

	float sunAmount = saturate(dot(rayDir, _SunDirection));
	float sunIntensity;

#if defined(ENABLE_FOG_HEIGHT) || defined(ENABLE_FOG_HEIGHT_WITH_NOISE)

	sunIntensity = _SunColor.a;

#else

	// dim sun as fog gets more dense
	sunIntensity = _SunColor.a * saturate(((1.0 - _FogDensity) + 0.35));

#endif

	float sunDepthMultiplier = 1.0f - saturate(ceil((_ProjectionParams.z * _FarPlaneSunThreshold) - depth));
	float sunLerp = sunIntensity * sunDepthMultiplier * pow(sunAmount, 8.0 - ((_FogDensity - 0.5) * 8));
	float3 fogColor = lerp(_FogColor.rgb * _SunColor.rgb * sunIntensity * noise, _SunColor.rgb * sunIntensity, sunLerp);

#else

	float3 fogColor = _FogColor.rgb * _SunColor.rgb * _SunColor.a * noise;

#endif

	CalculateFinalFog(noise, i.uv, fogFactor, fogColor);
	return fixed4(fogColor, fogFactor);

#endif

}

fixed4 fog_box_fragment_shader(volumetric_data i) : SV_TARGET
{

#if defined(FOG_NONE)

	return fixed4(0, 0, 0, 0);

#else

	float noise;

	// get the depth of this pixel
	float2 screenUV = i.projPos.xy / i.projPos.w;
	float depth = length(DECODE_EYEDEPTH(tex2D(_CameraDepthTexture, screenUV).r) / normalize(i.viewPos).z);

	// world space rayDir
	float3 rayDir = normalize(mul((float3x3)_CameraInverseMV, i.viewPos.xyz));

	// world space ray-depth intersect
	float3 worldPos = _WorldSpaceCameraPos + (rayDir * depth);

	RaycastFogBox(rayDir, i.normal, depth, worldPos, noise);
	float fogFactor = CalculateFogFactor(depth);
	float3 fogColor = _FogColor.rgb * _SunColor.rgb * _SunColor.a * noise;
	CalculateFinalFog(noise, screenUV, fogFactor, fogColor);
	return fixed4(fogColor, fogFactor);

#endif

}