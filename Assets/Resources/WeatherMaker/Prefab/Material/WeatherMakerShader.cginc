//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
#include "UnityDeferredLibrary.cginc"

// constants
#define MIE_G (-0.990)
#define MIE_G2 0.9801
#define SKY_GROUND_THRESHOLD 0.02

#ifndef SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
#if defined(SHADER_API_MOBILE)
#define SKYBOX_COLOR_IN_TARGET_COLOR_SPACE 1
#else
#define SKYBOX_COLOR_IN_TARGET_COLOR_SPACE 0
#endif
#endif

#if defined(UNITY_COLORSPACE_GAMMA)
#define GAMMA 2
#define COLOR_2_GAMMA(color) color
#define COLOR_2_LINEAR(color) color * color
#define LINEAR_2_OUTPUT(color) sqrt(color)
#else
#define GAMMA 2.2
// HACK: to get gfx-tests in Gamma mode to agree until UNITY_ACTIVE_COLORSPACE_IS_GAMMA is working properly
#define COLOR_2_GAMMA(color) ((unity_ColorSpaceDouble.r>2.0) ? pow(color,1.0/GAMMA) : color)
#define COLOR_2_LINEAR(color) color
#define LINEAR_2_LINEAR(color) color
#endif

struct procedural_sky_vertex
{
	float3 ray : NORMAL;
	fixed4 vertexColor : COLOR0;
	fixed4 sunColor : COLOR1;
};

struct vertex_only_input_data
{
	float4 vertex : POSITION;
};

struct volumetric_data
{
	float4 vertex : SV_POSITION;
	float3 normal : NORMAL;
	float4 viewPos : TEXCOORD0;
	float4 projPos : TEXCOORD1;
	float4 worldPos : TEXCOORD2;
};

fixed4 _TintColor;
fixed3 _EmissiveColor;
fixed _Intensity;
float _DirectionalLightMultiplier = 1;
float _PointSpotLightMultiplier = 1;
float _AmbientLightMultiplier = 1;

sampler2D _MainTex;
float4 _MainTex_ST;
float4 _MainTex_TexelSize;

#if defined(SOFTPARTICLES_ON)

float _InvFade;

#endif

inline fixed LerpFade(float4 c, float t)
{
	// the vertex will fade in, stay at full color, then fade out
	// x = creation time seconds
	// y = fade time in seconds
	// z = life time seconds

	// debug
	// return 1;

	float peakFadeIn = c.x + c.y;
	float startFadeOut = c.x + c.z - c.y;
	float endTime = c.x + c.z;
	float lerpMultiplier = saturate(ceil(t - peakFadeIn) + 0.000001);
	float lerp1Scalar = saturate(((t - c.x) / max(0.000001, (peakFadeIn - c.x))));
	float lerp2Scalar = saturate(max(0, ((t - startFadeOut) / max(0.000001, (endTime - startFadeOut)))));
	float lerp1 = lerp1Scalar * (1.0 - lerpMultiplier);
	float lerp2 = (1.0 - lerp2Scalar) * lerpMultiplier;
	return lerp1 + lerp2;
}

fixed3 ApplyLight(float4 lightPos, float4 lightDir, fixed3 lightColor, half4 lightAtten, float3 viewPos, float3 viewNormal)
{
	float3 toLight = (lightPos.xyz - (viewPos * lightPos.w));
	float lengthSq = dot(toLight, toLight);
	float atten = (1.0 / (1.0 + (lengthSq * lightAtten.z)));
	toLight = normalize(toLight);

#if defined(ORTHOGRAPHIC_MODE)

	// ignore view normal and point straight out along z axis
	float3 normal = fixed3(0, 0, 1);
	float diff = max(lightPos.w, dot(normal, toLight));
	return lightColor.rgb * (diff * atten);

#else

	// calculate modifier for non-directional light
	float modifierNonDirectionalLight = lightPos.w;

	// will be 1 if normal was 0, 0, 0, otherwise 0
	float normalZeroMultiplier = 1.0 - saturate(ceil(abs(viewNormal.x) + abs(viewNormal.y) + abs(viewNormal.z)));

	// if normal is 0,0,0 point the normal at the non-directional light
	viewNormal += (modifierNonDirectionalLight * normalZeroMultiplier * toLight);

	// spot light calculation - will be 1 for non-spot lights
	float rho = max(0, dot(toLight, lightDir.xyz));
	float spotAtt = max(0, (rho - lightAtten.x) * lightAtten.y);

	// calculate modifier for directional light, will be 0 for non-directional light
	float modifierDirectionalLight = 1.0 - modifierNonDirectionalLight;

	// if normal is 0,0,0 point the normal straight up for the directional light
	viewNormal += (modifierDirectionalLight * normalZeroMultiplier * float3(0, 1, 0));

	// apply spot modifier last
	modifierNonDirectionalLight *= spotAtt;

	// reduce based on normal
	atten *= max(0, dot(viewNormal, toLight));

	return lightColor.rgb * ((atten * modifierNonDirectionalLight) + modifierDirectionalLight);

#endif

}

inline fixed4 CalculateVertexColor(float3 viewPos, float3 viewNormal)
{
	fixed3 vertexColor = UNITY_LIGHTMODEL_AMBIENT.rgb * _AmbientLightMultiplier;
	vertexColor += ApplyLight(unity_LightPosition[0], unity_SpotDirection[0], unity_LightColor[0], unity_LightAtten[0], viewPos, viewNormal);
	vertexColor += ApplyLight(unity_LightPosition[1], unity_SpotDirection[1], unity_LightColor[1], unity_LightAtten[1], viewPos, viewNormal);
	vertexColor += ApplyLight(unity_LightPosition[2], unity_SpotDirection[2], unity_LightColor[2], unity_LightAtten[2], viewPos, viewNormal);
	vertexColor += ApplyLight(unity_LightPosition[3], unity_SpotDirection[3], unity_LightColor[3], unity_LightAtten[3], viewPos, viewNormal);
	vertexColor = clamp(vertexColor, 0, 3);
	return fixed4(vertexColor, 1);
}

inline float3 RotateVertexLocalQuaternion(float3 position, float3 axis, float angle)
{
	float half_angle = angle * 0.5;
	float _sin, _cos;
	sincos(half_angle, _sin, _cos);
	float4 q = float4(axis.xyz * _sin, _cos);
	return position + (2.0 * cross(cross(position, q.xyz) + (q.w * position), q.xyz));
}

inline fixed GetMieScattering(float cosAngle)
{
	const float MIEGV_COEFF = 0.1;
	const float4 MIEGV = float4(1 - (MIEGV_COEFF * MIEGV_COEFF), 1 + (MIEGV_COEFF * MIEGV_COEFF), 2 * MIEGV_COEFF, 1.0f / (4.0f * 3.14159265358979323846));
	return MIEGV.w * (MIEGV.x / (pow(MIEGV.y - MIEGV.z * cosAngle, 1.5)));
}

inline fixed GetMiePhase(fixed size, fixed eyeCos, fixed eyeCos2)
{
	fixed temp = 1.0 + MIE_G2 - 2 * MIE_G * eyeCos;
	temp = max(1.0e-4, smoothstep(0.0, 0.005, temp) * temp);
	return size * ((1.0 - MIE_G2) / (2.0 + MIE_G2)) * (1.0 + eyeCos2) / temp;
}

inline fixed GetRayleighPhase(fixed eyeCos2)
{
	return 0.75 + 0.75 * eyeCos2;
}

inline fixed GetRayleighPhase(fixed3 light, fixed3 ray)
{
	fixed eyeCos = dot(light, ray);
	return GetRayleighPhase(eyeCos * eyeCos);
}

inline fixed CalcSunSpot(fixed size, fixed3 vec1, fixed3 vec2)
{
	fixed3 delta = vec1 - vec2;
	fixed dist = length(delta);
	fixed spot = 1.0 - smoothstep(0.0, size * 3, dist);
	return 100 * spot * spot;
}

inline fixed4 GetSunColorHighQuality(float3 sunNormal, fixed4 sunColor, fixed size, float3 ray)
{
	ray = normalize(ray);
	fixed eyeCos = dot(sunNormal, ray);
	fixed eyeCos2 = eyeCos * eyeCos;
	fixed mie = GetMiePhase(size, eyeCos, eyeCos2);
	return (mie * sunColor);
}

inline fixed4 GetSunColorFast(float3 sunNormal, fixed4 sunColor, fixed size, float3 ray)
{
	ray = normalize(ray);
	fixed eyeCos = dot(sunNormal, ray);
	fixed eyeCos2 = eyeCos * eyeCos;
	fixed mie = CalcSunSpot(size, sunNormal, -ray);
	return (mie * sunColor);
}

inline float GetSkyScale(float inCos)
{
	float x = 1.0 - inCos;
#if defined(SHADER_API_N3DS)
	// The polynomial expansion here generates too many swizzle instructions for the 3DS vertex assembler
	// Approximate by removing x^1 and x^2
	return 0.25 * exp(-0.00287 + x * x * x * (-6.80 + x * 5.25));
#else
	return 0.25 * exp(-0.00287 + x * (0.459 + x * (3.83 + x * (-6.80 + x * 5.25))));
#endif

}

fixed GetSunLightSkyMultiplier(float3 lightPos, float3 skyVertex)
{
	fixed3 toLight = (lightPos - skyVertex);
	fixed lengthSq = dot(toLight, toLight);
	fixed atten = 1.0 / (1.0 + (lengthSq * 1));
	return clamp(atten * 1.5, 1.0, 1.4);
}

procedural_sky_vertex CalculateSkyVertex(float3 lightPos, fixed3 lightColor, fixed3 groundColor, float3 skyVertex, fixed3 skyTintColor, float atmosphereThickness)
{
	procedural_sky_vertex o;

	static const float3 kDefaultScatteringWavelength = float3(.65, .57, .475);
	static const float3 kVariableRangeForScatteringWavelength = float3(.15, .15, .15);
	static const float OUTER_RADIUS = 1.065;
	static const float kOuterRadius = OUTER_RADIUS;
	static const float kOuterRadius2 = OUTER_RADIUS * OUTER_RADIUS;
	static const float kInnerRadius = 1.0;
	static const float kInnerRadius2 = 1.0;
	static const float kCameraHeight = 0.0001;
	static const float kMIE = 0.0010; // Mie constant
	static const float kSUN_BRIGHTNESS = 20.0; // Sun brightness
	static const float kMAX_SCATTER = 50.0; // Maximum scattering value, to prevent math overflows on Adrenos
	static const half kSunScale = 400.0 * kSUN_BRIGHTNESS;
	static const float kKmESun = kMIE * kSUN_BRIGHTNESS;
	static const float kKm4PI = kMIE * 4.0 * 3.14159265;
	static const float kScale = 1.0 / (OUTER_RADIUS - 1.0);
	static const float kScaleDepth = 0.25;
	static const float kScaleOverScaleDepth = (1.0 / (OUTER_RADIUS - 1.0)) / 0.25;
	static const float kSamples = 2.0; // THIS IS UNROLLED MANUALLY, DON'T TOUCH
	float kRAYLEIGH = (lerp(0, 0.0025, pow(atmosphereThickness, 2.5))); // Rayleigh constant
	float3 kSkyTintInGammaSpace = COLOR_2_GAMMA(skyTintColor); // convert tint from Linear back to Gamma
	float3 kScatteringWavelength = lerp(
		kDefaultScatteringWavelength - kVariableRangeForScatteringWavelength,
		kDefaultScatteringWavelength + kVariableRangeForScatteringWavelength,
		float3(1, 1, 1) - kSkyTintInGammaSpace); // using Tint in sRGB gamma allows for more visually linear interpolation and to keep (.5) at (128, gray in sRGB) point
	float3 kInvWavelength = 1.0 / pow(kScatteringWavelength, 4);
	float kKrESun = kRAYLEIGH * kSUN_BRIGHTNESS;
	float kKr4PI = kRAYLEIGH * 4.0 * 3.14159265;
	float3 cameraPos = float3(0, kInnerRadius + kCameraHeight, 0);
	// The camera's current position
	// Get the ray from the camera to the vertex and its length (which is the far point of the ray passing through the atmosphere)
	float3 eyeRay = normalize(mul((float3x3)unity_ObjectToWorld, skyVertex));
	o.ray = -eyeRay;
	float far = 0.0;
	fixed3 cIn, cOut;
	if (eyeRay.y >= 0.0)
	{
		// Sky
		// Calculate the length of the "atmosphere"
		far = sqrt(kOuterRadius2 + kInnerRadius2 * eyeRay.y * eyeRay.y - kInnerRadius2) - kInnerRadius * eyeRay.y;
		float3 pos = cameraPos + far * eyeRay;

		// Calculate the ray's starting position, then calculate its scattering offset
		float height = kInnerRadius + kCameraHeight;
		float depth = exp(kScaleOverScaleDepth * (-kCameraHeight));
		float startAngle = dot(eyeRay, cameraPos) / height;
		float startOffset = depth * GetSkyScale(startAngle);

		// Initialize the scattering loop variables
		float sampleLength = far / kSamples;
		float scaledLength = sampleLength * kScale;
		float3 sampleRay = eyeRay * sampleLength;
		float3 samplePoint = cameraPos + sampleRay * 0.5;

		// Now loop through the sample rays
		float3 frontColor = float3(0.0, 0.0, 0.0);
		{
			float height = length(samplePoint);
			float depth = exp(kScaleOverScaleDepth * (kInnerRadius - height));
			float lightAngle = dot(lightPos, samplePoint) / height;
			float cameraAngle = dot(eyeRay, samplePoint) / height;
			float scatter = (startOffset + depth * (GetSkyScale(lightAngle) - GetSkyScale(cameraAngle)));
			float3 attenuate = exp(-clamp(scatter, 0.0, kMAX_SCATTER) * (kInvWavelength * kKr4PI + kKm4PI));
			frontColor += attenuate * (depth * scaledLength);
			samplePoint += sampleRay;
		}
		{
			float height = length(samplePoint);
			float depth = exp(kScaleOverScaleDepth * (kInnerRadius - height));
			float lightAngle = dot(lightPos, samplePoint) / height;
			float cameraAngle = dot(eyeRay, samplePoint) / height;
			float scatter = (startOffset + depth * (GetSkyScale(lightAngle) - GetSkyScale(cameraAngle)));
			float3 attenuate = exp(-clamp(scatter, 0.0, kMAX_SCATTER) * (kInvWavelength * kKr4PI + kKm4PI));
			frontColor += attenuate * (depth * scaledLength);
			samplePoint += sampleRay;
		}

		// Finally, scale the Mie and Rayleigh colors and set up the varying variables for the pixel shader
		cIn = frontColor * (kInvWavelength * kKrESun);
		cOut = frontColor * kKmESun;
	}
	else
	{
		// Ground
		far = (-kCameraHeight) / (min(-0.001, eyeRay.y));
		float3 pos = cameraPos + far * eyeRay;

		// Calculate the ray's starting position, then calculate its scattering offset
		float depth = exp((-kCameraHeight) * (1.0 / kScaleDepth));
		float cameraAngle = dot(o.ray, pos);
		float lightAngle = dot(lightPos, pos);
		float cameraScale = GetSkyScale(cameraAngle);
		float lightScale = GetSkyScale(lightAngle);
		float cameraOffset = depth * cameraScale;
		float temp = (lightScale + cameraScale);

		// Initialize the scattering loop variables
		float sampleLength = far / kSamples;
		float scaledLength = sampleLength * kScale;
		float3 sampleRay = eyeRay * sampleLength;
		float3 samplePoint = cameraPos + sampleRay * 0.5;

		// Now loop through the sample rays
		float3 frontColor = float3(0.0, 0.0, 0.0);
		float3 attenuate;
		{
			float height = length(samplePoint);
			float depth = exp(kScaleOverScaleDepth * (kInnerRadius - height));
			float scatter = depth * temp - cameraOffset;
			attenuate = exp(-clamp(scatter, 0.0, kMAX_SCATTER) * (kInvWavelength * kKr4PI + kKm4PI));
			frontColor += attenuate * (depth * scaledLength);
			samplePoint += sampleRay;
		}
		cIn = frontColor * (kInvWavelength * kKrESun + kKmESun);
		cOut = clamp(attenuate, 0.0, 1.0);
	}

	fixed rayLeigh = GetRayleighPhase(lightPos, o.ray);
	fixed atten = GetSunLightSkyMultiplier(lightPos, skyVertex);
	o.vertexColor = fixed4((cIn * rayLeigh) + (cIn + (groundColor)* cOut), 1);
	o.sunColor = fixed4((1.0 - cOut) * lightColor, atten);

#if defined(UNITY_COLORSPACE_GAMMA) && SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
	o.vertexColor = sqrt(o.vertexColor);
	o.sunColor = sqrt(o.sunColor);
#endif

	return o;
}

inline volumetric_data GetVolumetricData(float4 vertex, float3 normal)
{
	volumetric_data o;
	o.worldPos = mul(unity_ObjectToWorld, vertex);
	o.vertex = UnityObjectToClipPos(vertex);
	o.projPos = ComputeScreenPos(o.vertex);
	o.viewPos = mul(UNITY_MATRIX_MV, vertex);
	o.worldPos.w = normalize(o.viewPos).z;
	o.normal = UnityObjectToWorldNormal(normal);
	return o;
}

float3 GetFarPlaneVector(float4x4 inverseMVP, float4 clip)
{
	// create x,y clip coordinates (render space), divide by w to get near plane
	float2 clipFront = clip.xy / clip.w;

	// create clip coordinates (render space) at the back of the viewing frustum
	// 1.0, 1.0 puts it at the back
	float4 clipBack = float4(clipFront, 1.0, 1.0);

	// handle flipped projection matrixes (https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html)
	clipBack.y *= _ProjectionParams.x;

	// convert from clip space to world space
	float4 farPlanePosition = mul(inverseMVP, clipBack);

	// get actual world position by dividing by w
	farPlanePosition.xyz /= farPlanePosition.w;

	// return the vector that points from the camera to the back of the frustum
	return farPlanePosition.xyz - _WorldSpaceCameraPos;
}

inline void RayBoxIntersect(float3 rayOrigin, float3 rayDir, float rayLength, float3 boxMin, float3 boxMax, out float intersectAmount, out float distanceToBox)
{
	// https://tavianator.com/fast-branchless-raybounding-box-intersections/

	/*
	Aos::Vector3 t1(Aos::mulPerElem(m_min - ray.m_pos, ray.m_invDir));
	Aos::Vector3 t2(Aos::mulPerElem(m_max - ray.m_pos, ray.m_invDir));

	Aos::Vector3 tmin1(Aos::minPerElem(t1, t2));
	Aos::Vector3 tmax1(Aos::maxPerElem(t1, t2));

	float tmin = Aos::maxElem(tmin1);
	float tmax = Aos::minElem(tmax1);

	return tmax >= std::max(ray.m_min, tmin) && tmin < ray.m_max;
	*/

	float3 invRayDir = 1.0 / rayDir;
	float3 t1 = (boxMin - rayOrigin) * invRayDir;
	float3 t2 = (boxMax - rayOrigin) * invRayDir;
	float3 tmin1 = min(t1, t2);
	float3 tmax1 = max(t1, t2);
	float tmin = max(max(tmin1.x, tmin1.y), tmin1.z);
	float tmax = min(min(tmax1.x, tmax1.y), tmax1.z);
	float2 tt0 = max(tmin1.xx, tmin1.yz);
	distanceToBox = max(0, max(tt0.x, tt0.y));
	tt0 = min(tmax1.xx, tmax1.yz);
	float tt1 = min(tt0.x, tt0.y);
	tt1 = min(tt1, rayLength);
	intersectAmount = max(0, tt1 - distanceToBox);
}

inline float RandomFloat(float3 v)
{
	return frac(sin(dot(v.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
}

inline void GetFullScreenBoundingBox(float height, out float3 boxMin, out float3 boxMax)
{
	boxMin = float3(_WorldSpaceCameraPos.x - _ProjectionParams.z, -_ProjectionParams.z, _WorldSpaceCameraPos.z - _ProjectionParams.z);
	boxMax = float3(_WorldSpaceCameraPos.x + _ProjectionParams.z, height, _WorldSpaceCameraPos.z + _ProjectionParams.z);
}

inline float CalculateNoiseSphere(sampler2D noiseTex, float3 normal, float3 worldPos, float scale, float3 velocity, float multiplier)
{
	const float divider = 32.0; // 256
	const float oneOverDivider = 1.0 / divider;
	const float bias = -999;
	const float aOffX = 23.0;
	const float aOffY = 29.0;
	const float b_off_adder = 1.0;

	float t = _Time.x;
	worldPos *= scale;
	worldPos += (t * velocity);
	worldPos.z = frac(worldPos.z) * divider;
	float iz = floor(worldPos.z);
	float fz = frac(worldPos.z);
	float2 a_off = float2(aOffX, aOffY) * (iz)* oneOverDivider;
	float2 b_off = float2(aOffX, aOffY) * (iz + b_off_adder) * oneOverDivider;
	float a = tex2Dlod(noiseTex, float4(worldPos.xy + a_off, 0, bias)).r;
	float b = tex2Dlod(noiseTex, float4(worldPos.xy + b_off, 0, bias)).r;
	return (lerp(a, b, fz) - 0.65) * multiplier;
}

inline float CalculateNoisePerlinSphere(sampler2D noiseTex, float3 normal, float3 worldPos, float scale, float3 velocity, float multiplier)
{
	const float powersOfTwo[6] = { pow(2.0, 0), pow(2.0, 1), pow(2.0, 2), pow(2.0, 3), pow(2.0, 4), pow(2.0, 5) };
	const float powersOfPoint5[6] = { pow(0.5, 0), pow(0.5, 1), pow(0.5, 2), pow(0.5, 3), pow(0.5, 4), pow(0.5, 5) };
	return
		CalculateNoiseSphere(noiseTex, normal, worldPos * powersOfTwo[0], scale, velocity, multiplier) * powersOfPoint5[0] +
		CalculateNoiseSphere(noiseTex, normal, worldPos * powersOfTwo[1], scale, velocity, multiplier) * powersOfPoint5[1] +
		CalculateNoiseSphere(noiseTex, normal, worldPos * powersOfTwo[2], scale, velocity, multiplier) * powersOfPoint5[2] +
		CalculateNoiseSphere(noiseTex, normal, worldPos * powersOfTwo[3], scale, velocity, multiplier) * powersOfPoint5[3] +
		CalculateNoiseSphere(noiseTex, normal, worldPos * powersOfTwo[4], scale, velocity, multiplier) * powersOfPoint5[4] +
		CalculateNoiseSphere(noiseTex, normal, worldPos * powersOfTwo[5], scale, velocity, multiplier) * powersOfPoint5[5];
}

inline float CalculateNoiseAverage(sampler2D noiseTex, float3 normal, float3 worldPos, float scale, float2 velocity, float multiplier)
{
	const float subtractor = 0.65;
	const float divider = 0.333333;
	const float normalMinMultiplier = 1.0;
	// normal = abs(normal);
	float t = _Time.x;
	float noise = 0.0;
	worldPos *= scale;
	float2 vt = (t.xx * velocity.xy), noiseUV;

	// create 3 samples of noise and average them
	noiseUV = float2(worldPos.x, worldPos.z);
	noiseUV += vt;
	noise = (tex2D(noiseTex, noiseUV).r - subtractor) * multiplier;// *max(normalMinMultiplier, normal.y);
	noiseUV = float2(worldPos.y, worldPos.x);
	noiseUV += vt;
	noise += (tex2D(noiseTex, noiseUV).r - subtractor) * multiplier;// *max(normalMinMultiplier, normal.z);
	noiseUV = float2(worldPos.z, worldPos.y);
	noiseUV += vt;
	noise += (tex2D(noiseTex, noiseUV).r - subtractor) * multiplier;// *max(normalMinMultiplier, normal.x);
	return noise * divider;
}

inline float CalculateNoiseXZ(sampler2D noiseTex, float3 worldPos, float scale, float2 velocity, float multiplier)
{
	float t = _Time.x;
	float2 noiseUV = float2(worldPos.x * scale, worldPos.z * scale);
	noiseUV += (t.xx * velocity.xy);
	return ((tex2D(noiseTex, noiseUV).r - 0.5) * multiplier);
}

/*
inline float sphereDistance(float3 p)
{
return distance(p, _FogSphere.xyz) - _FogSphere.w;
}

// there is a bug with this right at the edge of the sphere where the outer part looks funny, almost like a second sphere
// need to fix this bug before this function can be used
inline float fastFogCalculation(float3 rayDir, float depth)
{
float radiusSquared = _FogSphere.w * _FogSphere.w;
float3 m = -_FogSphere.xyz;
float b = dot(m, rayDir);
float nb = -b;
float c = dot(m, m) - radiusSquared;

// Exit if r’s origin outside s (c > 0) and r pointing away from s (b > 0)
//if (c > 0 && b > 0) return 0;

float discr = b * b - c;

// A negative discriminant corresponds to ray missing sphere
//if (discr < 0.0f) return 0;

// Ray now found to intersect sphere, compute smallest t value of intersection
float t = nb - sqrt(discr);

// If t is negative, ray started inside sphere so clamp t to zero
//if (t < 0.0f) t = 0.0f;

float3 hitPoint = (t * rayDir);
float3 normal = normalize(_FogSphere.xyz - hitPoint);// (hitPoint - rayOrigin) / _FogParam.w;
float e = (1 - (1 / exp(discr * 0.5 * length(normal))));
return (saturate(nb) * e * saturate(depth - t));
}
*/