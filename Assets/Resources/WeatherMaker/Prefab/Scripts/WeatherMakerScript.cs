//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;
using System;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherMakerScript : MonoBehaviour
    {
        [Header("Precipitation Objects")]
        [Tooltip("Camera the weather should hover over. Defaults to main camera.")]
        public Camera Camera;

        [Tooltip("The sun")]
        public Light Sun;

        [Tooltip("Whether the precipitation collides with the world")]
        public bool CollisionEnabled = true;

        [Tooltip("Whether to enable wind")]
        public bool WindEnabled;

        [Tooltip("Configuration script. Should be deactivated for release builds.")]
        public WeatherMakerConfigurationScript ConfigurationScript;

        [Tooltip("Rain script")]
        public WeatherMakerFallingParticleScript RainScript;

        [Tooltip("Snow script")]
        public WeatherMakerFallingParticleScript SnowScript;

        [Tooltip("Hail script")]
        public WeatherMakerFallingParticleScript HailScript;

        [Tooltip("Sleet script")]
        public WeatherMakerFallingParticleScript SleetScript;

        [Tooltip("Lightning script")]
        public WeatherMakerThunderAndLightningScript LightningScript;

        [Tooltip("Cloud script")]
        public WeatherMakerCloudScript CloudScript;

        [Tooltip("Wind script")]
        public WeatherMakerWindScript WindScript;

        [Tooltip("Sky sphere script, not used in 2D")]
        public WeatherMakerSkySphereScript SkySphereScript;

        [Tooltip("Day night script")]
        public WeatherMakerDayNightCycleScript DayNightScript;

        [Tooltip("Fog script, null for none.")]
        public WeatherMakerFullScreenFogScript FogScript;

        [Tooltip("Intensity of precipitation (0-1)")]
        [Range(0.0f, 1.0f)]
        public float PrecipitationIntensity;

        [Tooltip("How long in seconds to fully change from one precipitation type to another")]
        [Range(0.0f, 300.0f)]
        public float PrecipitationChangeDuration = 4.0f;

        [Tooltip("The threshold change in intensity that will cause a cross-fade between precipitation changes. Intensity changes smaller than this value happen quickly.")]
        [Range(0.0f, 0.2f)]
        public float PrecipitationChangeThreshold = 0.1f;

        [Range(0.0f, 1.0f)]
        [Tooltip("Change the volume of all weather maker sounds.")]
        public float VolumeModifier = 1.0f;

        [NonSerialized]
        private float lastPrecipitationIntensityChange = -1.0f;

        [NonSerialized]
        private float lastVolumeModifier = -1.0f;

        /// <summary>
        /// Whether per pixel lighting is enabled in the shaders for precipitation, clouds, fog, etc. - default is true
        /// </summary>
        public static bool EnablePerPixelLighting = true;

        private void TweenScript(WeatherMakerFallingParticleScript script, float end)
        {
            if (Mathf.Abs(script.Intensity - end) < PrecipitationChangeThreshold)
            {
                script.Intensity = end;
            }
            else
            {
                TweenFactory.Tween("WeatherMakerPrecipitationChange_" + script.gameObject.GetInstanceID(), script.Intensity, end, PrecipitationChangeDuration, TweenScaleFunctions.Linear, (t) =>
                {
                    // Debug.LogFormat("Tween key: {0}, value: {1}, prog: {2}", t.Key, t.CurrentValue, t.CurrentProgress);
                    script.Intensity = t.CurrentValue;
                }, null);
            }
        }

        private void ChangePrecipitation(WeatherMakerFallingParticleScript newPrecipitation)
        {
            if (newPrecipitation != currentPrecipitation && currentPrecipitation != null)
            {
                TweenScript(currentPrecipitation, 0.0f);
                lastPrecipitationIntensityChange = -1.0f;
            }
            currentPrecipitation = newPrecipitation;
        }

        private void UpdateCollision()
        {
			if (RainScript != null) {
	            RainScript.CollisionEnabled = CollisionEnabled;
			}

			if (SnowScript != null) {
	            SnowScript.CollisionEnabled = CollisionEnabled;
			}

			if (HailScript != null) {
	            HailScript.CollisionEnabled = CollisionEnabled;
			}

			if (SleetScript != null) {
	            SleetScript.CollisionEnabled = CollisionEnabled;
			}
        }

        private void UpdateSoundsVolumes()
        {
            LightningScript.VolumeModifier = VolumeModifier;

			if (RainScript != null) {
            	RainScript.AudioSourceLight.VolumeModifier = RainScript.AudioSourceMedium.VolumeModifier = RainScript.AudioSourceHeavy.VolumeModifier = VolumeModifier;
			}

			if (SnowScript != null) {
	            SnowScript.AudioSourceLight.VolumeModifier = SnowScript.AudioSourceMedium.VolumeModifier = SnowScript.AudioSourceHeavy.VolumeModifier = VolumeModifier;
			}

			if (HailScript != null) {
	            HailScript.AudioSourceLight.VolumeModifier = HailScript.AudioSourceMedium.VolumeModifier = HailScript.AudioSourceHeavy.VolumeModifier = VolumeModifier;
			}

			if (SleetScript != null) {
				SleetScript.AudioSourceLight.VolumeModifier = SleetScript.AudioSourceMedium.VolumeModifier = SleetScript.AudioSourceHeavy.VolumeModifier = VolumeModifier;
			}

			if (WindScript != null) {
	            WindScript.AudioSourceWind.VolumeModifier = VolumeModifier;
			}
        }

        private void UpdateWind()
        {
            if (WindScript != null)
            {
                WindScript.gameObject.SetActive(WindEnabled);
                WindScript.EnableWind = WindEnabled;
            }
        }

        private void Awake()
        {
            Instance = this;
            Camera = (Camera == null ? (Camera.main == null ? Camera.current : Camera.main) : Camera);
            if (Camera == null)
            {
                Debug.LogError("Must assign a camera for weather maker to work properly. Tag a camera as main camera, or manually assign the camera property.");
            }
            UpdateCollision();

			if (RainScript != null) {
	            RainScript.Camera = Camera;
			}

			if (SnowScript != null) {
	            SnowScript.Camera = Camera;
			}
            
			if (HailScript != null) {
				HailScript.Camera = Camera;
			}

			if (SleetScript != null) {
	            SleetScript.Camera = Camera;
			}

			if (CloudScript != null) {
	            CloudScript.Camera = Camera;
			}

			if (DayNightScript != null) {
	            DayNightScript.Camera = Camera;
			}

            if (SkySphereScript != null)
            {
                SkySphereScript.Camera = Camera;
            }
            if (WindScript != null)
            {
                WindScript.Camera = Camera;
            }
            if (FogScript != null)
            {
                FogScript.Camera = Camera;
            }
        }

        private void Update()
        {
            if (currentPrecipitation != null && PrecipitationIntensity != lastPrecipitationIntensityChange)
            {
                lastPrecipitationIntensityChange = PrecipitationIntensity;
                TweenScript(currentPrecipitation, PrecipitationIntensity);
            }
            if (VolumeModifier != lastVolumeModifier)
            {
                lastVolumeModifier = VolumeModifier;
                UpdateSoundsVolumes();
            }
            UpdateCollision();
            UpdateWind();
        }

        public WeatherMakerFallingParticleScript CurrentPrecipitation
        {
            get { return currentPrecipitation; }
            set
            {
                if (value != currentPrecipitation)
                {
                    ChangePrecipitation(value);
                }
            }
        }

        private WeatherMakerFallingParticleScript currentPrecipitation;

        /// <summary>
        /// Gets the current time of day in seconds. 86400 seconds per day.
        /// </summary>
        public float TimeOfDay {
			get {

				if (DayNightScript != null) {
					return DayNightScript.TimeOfDay;

				} else {
					return 0.0f;
				}
			}

			set {
				if (DayNightScript != null) {
					DayNightScript.TimeOfDay = value;
				}
			} 
		}

        /// <summary>
        /// Singleton
        /// </summary>
        public static WeatherMakerScript Instance { get; private set; }
    }
}