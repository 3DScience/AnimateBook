//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;

namespace DigitalRuby.WeatherMaker
{
    public abstract class WeatherMakerFogScript : MonoBehaviour
    {
        #region Public fields

        [Header("Camera")]
        [Tooltip("Camera the fog will render in.")]
        public Camera Camera;

        [Tooltip("Sun to light the fog")]
        public Light Sun;

        [Header("Fog Appearance")]
        [Tooltip("Fog mode")]
        public WeatherMakerFogMode FogMode = WeatherMakerFogMode.Exponential;

        [Tooltip("Fog density")]
        [Range(0.0f, 1.0f)]
        public float FogDensity = 0.05f;

        [Tooltip("Fog color")]
        public Color FogColor = Color.white;

        [Tooltip("Maximum fog factor, where 1 is the maximum allowed fog.")]
        [Range(0.0f, 1.0f)]
        public float MaxFogFactor = 1.0f;

        [Header("Fog Noise")]
        [Tooltip("Fog noise or null for no noise.")]
        public Texture2D FogNoise;

        [Tooltip("Fog noise scale. Lower values get less tiling. 0 to disable noise.")]
        [Range(0.0f, 1.0f)]
        public float FogNoiseScale = 0.0001f;

        [Tooltip("How much the noise effects the fog.")]
        [Range(0.0f, 1.0f)]
        public float FogNoiseMultiplier = 0.15f;

        [Tooltip("Fog noise velocity, determines how fast the fog moves. Not all fog scripts support 3d velocity, some only support 2d velocity (x and y).")]
        public Vector3 FogNoiseVelocity = new Vector3(0.01f, 0.01f, 0.0f);

        [Header("Fog Distortion")]
        [Tooltip("Fog height noise or null for no noise.")]
        public Texture2D FogNoiseHeight;

        [Tooltip("Fog noise height scale. Lower values get less tiling. 0 to disable fog height noise.")]
        [Range(0.0f, 1.0f)]
        public float FogNoiseHeightScale = 0.05f;

        [Tooltip("Controls how much noise impacts fog height. Not used if FogHeight is 0.")]
        [Range(0.0f, 1.0f)]
        public float FogNoiseHeightMultiplier = 0.15f;

        [Tooltip("Controls how much the fog height can vary.")]
        [Range(0.0f, 100.0f)]
        public float FogNoiseHeightVariance = 10.0f;

        [Header("Fog Rendering")]
        [Tooltip("Fog material")]
        public Material FogMaterial;

        [Tooltip("Material to sample depth buffer")]
        public Material FogDepthSampleMaterial;
        //private Material fogDepthNormalSampleMaterial;

        [Tooltip("Blur material for down-sampled fog to fix artifacts.")]
        public Material FogBlurMaterial;

        [Tooltip("Blur shader type. Determines the strength of the blur if FogBlurMaterial is set and down sample scaling is set. " +
            "GaussianBlur7 does 7 pixels, GaussianBlur17 does 17.")]
        public BlurShaderType BlurShader = BlurShaderType.GaussianBlur17;
        protected BlurShaderType lastBlurShader = (BlurShaderType)0x7FFFFFFF;

        [Range(0.25f, 1.0f)]
        [Tooltip("Down-sample from screen size by this percent. 1 for no scaling.")]
        public float DownSampleScale = 1.0f;
        protected float lastDownSampleScale;

        [Tooltip("Dithering level. 0 to disable dithering.")]
        [Range(0.0f, 1.0f)]
        public float DitherLevel = 0.005f;

        [Tooltip("Distance for linear fog.")]
        public float LinearFogDistance = 1000.0f;

        #endregion Public fields

        #region Public methods

        /// <summary>
        /// Set a new fog density over a period of time - if set to 0, game object will be disabled at end of transition
        /// </summary>
        /// <param name="fromDensity">Start of new fog density</param>
        /// <param name="toDensity">End of new fog density</param>
        /// <param name="transitionDuration">How long to transition to the new fog density in seconds</param>
        public void TransitionFogDensity(float fromDensity, float toDensity, float transitionDuration)
        {
            gameObject.SetActive(true);
            FogDensity = fromDensity;
            UpdateMaterial();
            TweenFactory.Tween("WeatherMakerFog_" + gameObject.name, fromDensity, toDensity, transitionDuration, TweenScaleFunctions.Linear, (v) =>
            {
                FogDensity = v.CurrentValue;
            }, (v) =>
            {
                gameObject.SetActive(v.CurrentValue != 0.0f);
            });
        }

        #endregion Public methods

        #region Protected methods

        protected virtual void Awake()
        {
            Camera = (Camera == null ? (Camera.main == null ? Camera.current : Camera.main) : Camera);

            // ensure depth texture
            Camera.depthTextureMode |= DepthTextureMode.Depth;

            if (Application.isPlaying)
            {
                // clone fog material
                FogMaterial = new Material(FogMaterial);

                // clone depth sampler material for normals
                // fogDepthNormalSampleMaterial = new Material(FogDepthSampleMaterial);

                // clone blur material
                FogBlurMaterial = new Material(FogBlurMaterial);
            }
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            UpdateMaterial();
        }

        protected virtual void LateUpdate()
        {

        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnEnable()
        {
            // https://issuetracker.unity3d.com/issues/screen-dot-width-and-screen-dot-height-values-in-onenable-function-are-incorrect
            // bug in Unity, screen width and height are wrong in OnEnable... sigh...
            lastDownSampleScale = -1.0f; // forces refresh of command buffer or anything else relying on scaling factor
        }

        protected virtual void OnDisable()
        {
        }

        #endregion Protected methods

        #region Private methods

        protected virtual void UpdateMaterial()
        {

#if UNITY_EDITOR

            if (FogMaterial == null)
            {
                Debug.LogError("Must set fog material and fog blur material");
            }

#endif

            FogMaterial.SetColor("_FogColor", FogColor);
            FogMaterial.SetTexture("_FogNoise", FogNoise);
            FogMaterial.SetFloat("_FogNoiseScale", FogNoiseScale);
            FogMaterial.SetFloat("_FogNoiseMultiplier", FogNoiseMultiplier);
            FogMaterial.SetVector("_FogNoiseVelocity", FogNoiseVelocity);
            FogMaterial.SetFloat("_MaxFogFactor", MaxFogFactor);
            FogMaterial.SetMatrix("_CameraInverseMVP", Camera.cameraToWorldMatrix * Camera.projectionMatrix.inverse);
            FogMaterial.SetMatrix("_CameraInverseMV", Camera.cameraToWorldMatrix);
            FogMaterial.DisableKeyword("FOG_NONE");
            FogMaterial.DisableKeyword("FOG_CONSTANT");
            FogMaterial.DisableKeyword("FOG_EXPONENTIAL");
            FogMaterial.DisableKeyword("FOG_LINEAR");
            FogMaterial.DisableKeyword("FOG_EXPONENTIAL_SQUARED");
            if (FogMode == WeatherMakerFogMode.None || FogDensity <= 0.0f || MaxFogFactor <= 0.001f)
            {
                FogMaterial.EnableKeyword("FOG_NONE");
            }
            else if (FogMode == WeatherMakerFogMode.Exponential)
            {
                FogMaterial.EnableKeyword("FOG_EXPONENTIAL");
            }
            else if (FogMode == WeatherMakerFogMode.Linear)
            {
                FogMaterial.EnableKeyword("FOG_LINEAR");
            }
            else if (FogMode == WeatherMakerFogMode.ExponentialSquared)
            {
                FogMaterial.EnableKeyword("FOG_EXPONENTIAL_SQUARED");
            }
            else
            {
                FogMaterial.EnableKeyword("FOG_CONSTANT");
            }
            if (DownSampleScale < 0.99f)
            {
                FogMaterial.EnableKeyword("ENABLE_SCALING");
            }
            else
            {
                FogMaterial.DisableKeyword("ENABLE_SCALING");
            }
            if (FogNoise != null && FogNoiseScale > 0.0f && FogNoiseMultiplier > 0.0f)
            {
                FogMaterial.EnableKeyword("ENABLE_FOG_NOISE");
            }
            else
            {
                FogMaterial.DisableKeyword("ENABLE_FOG_NOISE");
            }
            FogMaterial.SetFloat("_DitherLevel", DitherLevel);
            if (FogBlurMaterial != null)
            {
                if (BlurShader == BlurShaderType.GaussianBlur17)
                {
                    FogBlurMaterial.DisableKeyword("BLUR7");
                }
                else
                {
                    FogBlurMaterial.EnableKeyword("BLUR7");
                }
            }

            FogMaterial.SetFloat("_FogNoiseHeightScale", FogNoiseHeightScale);
            FogMaterial.SetFloat("_FogNoiseHeightMultiplier", FogNoiseHeightMultiplier);
            FogMaterial.SetFloat("_FogNoiseHeightVariance", FogNoiseHeightVariance);
            FogMaterial.SetFloat("_FogDensity", FogDensity);
            FogMaterial.SetFloat("_LinearFogDistance", LinearFogDistance);

            if (FogNoiseHeightScale > 0.0f && FogNoiseHeight != null)
            {
                FogMaterial.SetTexture("_FogNoiseHeight", FogNoiseHeight);
                FogMaterial.EnableKeyword("ENABLE_FOG_HEIGHT_WITH_NOISE");
            }
            else
            {
                FogMaterial.DisableKeyword("ENABLE_FOG_HEIGHT_WITH_NOISE");
            }

            if (Sun != null)
            {
                FogMaterial.SetVector("_SunDirection", -Sun.transform.forward);
                Vector4 sunColor = new Vector4(Sun.color.r, Sun.color.g, Sun.color.b, Sun.intensity);
                FogMaterial.SetVector("_SunColor", sunColor);
            }
        }

        #endregion Private methods
    }
}
