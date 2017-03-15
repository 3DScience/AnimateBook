//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System;

using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherMakerConfigurationScript : MonoBehaviour
    {
        public WeatherMakerScript WeatherScript;
        public float MovementSpeed = 20.0f;
        public bool AllowFlashlight;
        public UnityEngine.UI.Slider TransitionDurationSlider;
        public UnityEngine.UI.Slider IntensitySlider;
        public UnityEngine.UI.Toggle MovementEnabledCheckBox;
        public UnityEngine.UI.Toggle FlashlightToggle;
        public UnityEngine.UI.Toggle TimeOfDayEnabledCheckBox;
        public UnityEngine.UI.Slider DawnDuskSlider;
        public UnityEngine.UI.Text TimeOfDayText;
        public Light Flashlight;
        public GameObject Sun;
        public GameObject Canvas;

        private float initialFogDensity;
        private enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        private RotationAxes axes = RotationAxes.MouseXAndY;
        private float sensitivityX = 15F;
        private float sensitivityY = 15F;
        private float minimumX = -360F;
        private float maximumX = 360F;
        private float minimumY = -60F;
        private float maximumY = 60F;
        private float rotationX = 0F;
        private float rotationY = 0F;
        private Quaternion originalRotation;

        private void UpdateMovement()
        {
            if (MovementSpeed <= 0.0f || MovementEnabledCheckBox == null || !MovementEnabledCheckBox.isOn)
            {
                return;
            }

            float speed = MovementSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.W))
            {
                WeatherScript.Camera.transform.Translate(0.0f, 0.0f, speed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                WeatherScript.Camera.transform.Translate(0.0f, 0.0f, -speed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                WeatherScript.Camera.transform.Translate(-speed, 0.0f, 0.0f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                WeatherScript.Camera.transform.Translate(speed, 0.0f, 0.0f);
            }
        }

        private void UpdateMouseLook()
        {
            if (MovementEnabledCheckBox == null || MovementSpeed <= 0.0f)
            {
                return;
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                MovementEnabledCheckBox.isOn = !MovementEnabledCheckBox.isOn;
            }

            if (!MovementEnabledCheckBox.isOn)
            {
                return;
            }
            else if (axes == RotationAxes.MouseXAndY)
            {
                // Read the mouse input axis
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

                rotationX = ClampAngle(rotationX, minimumX, maximumX);
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

                WeatherScript.Camera.transform.localRotation = originalRotation * xQuaternion * yQuaternion;
            }
            else if (axes == RotationAxes.MouseX)
            {
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationX = ClampAngle(rotationX, minimumX, maximumX);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                WeatherScript.Camera.transform.localRotation = originalRotation * xQuaternion;
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
                WeatherScript.Camera.transform.localRotation = originalRotation * yQuaternion;
            }
        }

        private void UpdateTimeOfDay()
        {
            DawnDuskSlider.value = WeatherScript.TimeOfDay;
			if (TimeOfDayText != null && TimeOfDayText.IsActive())
            {
                TimeSpan t = TimeSpan.FromSeconds(WeatherScript.TimeOfDay);
                TimeOfDayText.text = string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);
            }
        }

        private void Start()
        {
            originalRotation = transform.localRotation;
            IntensitySlider.value = WeatherScript.PrecipitationIntensity;
            DawnDuskSlider.value = WeatherScript.TimeOfDay;
            initialFogDensity = (WeatherScript.FogScript == null ? 0.0f : WeatherScript.FogScript.FogDensity);
        }

        private void Update()
        {
            UpdateMovement();
            UpdateMouseLook();
            if (AllowFlashlight && Flashlight != null)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    FlashlightToggle.isOn = !FlashlightToggle.isOn;

                    // hack: Bug in Unity, doesn't recognize that the light was enabled unless we rotate the camera
                    WeatherScript.Camera.transform.Rotate(0.0f, 0.01f, 0.0f);
                    WeatherScript.Camera.transform.Rotate(0.0f, -0.01f, 0.0f);
                }
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                LightningStrikeButtonClicked();
            }
            if (Input.GetKeyDown(KeyCode.BackQuote) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                Canvas.SetActive(!Canvas.activeInHierarchy); 
            }
            UpdateTimeOfDay();
        }

        public void RainToggleChanged(bool isOn)
        {
            WeatherScript.CurrentPrecipitation = (isOn ? WeatherScript.RainScript : null);
        }

        public void SnowToggleChanged(bool isOn)
        {
            WeatherScript.CurrentPrecipitation = (isOn ? WeatherScript.SnowScript : null);
        }

        public void HailToggleChanged(bool isOn)
        {
            WeatherScript.CurrentPrecipitation = (isOn ? WeatherScript.HailScript : null);
        }

        public void SleetToggleChanged(bool isOn)
        {
            WeatherScript.CurrentPrecipitation = (isOn ? WeatherScript.SleetScript : null);
        }

        public void CloudToggleChanged(bool isOn)
        {
            if (isOn)
            {
                WeatherScript.CloudScript.Reset();
                WeatherScript.CloudScript.CreateClouds();
            }
            else
            {
                WeatherScript.CloudScript.RemoveClouds();
            }
        }

        public void LightningToggleChanged(bool isOn)
        {
            WeatherScript.LightningScript.EnableLightning = isOn;
        }

        public void CollisionToggleChanged(bool isOn)
        {
            WeatherScript.CollisionEnabled = isOn;
        }

        public void WindToggleChanged(bool isOn)
        {
            WeatherScript.WindEnabled = isOn;
        }

        public void TransitionDurationSliderChanged(float val)
        {
            WeatherScript.PrecipitationChangeDuration = val;
        }

        public void IntensitySliderChanged(float val)
        {
            WeatherScript.PrecipitationIntensity = val;
        }

        public void MovementEnabledChanged(bool val)
        {
            MovementEnabledCheckBox.isOn = val;
        }

        public void FlashlightChanged(bool val)
        {
            if (AllowFlashlight && FlashlightToggle != null && Flashlight != null)
            {
                FlashlightToggle.isOn = val;
                Flashlight.enabled = val;
            }
        }

        public void FogChanged(bool val)
        {
            // if fog is not active, set the start fog density to 0, otherwise start at whatever density it is at
            float startFogDensity = (WeatherScript.FogScript.gameObject.activeInHierarchy ? WeatherScript.FogScript.FogDensity : 0.0f);
            float endFogDensity = (val ? initialFogDensity : 0.0f);
            WeatherScript.FogScript.TransitionFogDensity(startFogDensity, endFogDensity, TransitionDurationSlider.value);
        }

        public void TimeOfDayEnabledChanged(bool val)
        {
			if (WeatherScript.DayNightScript != null) {
	            WeatherScript.DayNightScript.Speed = (val ? 10.0f : 0.0f);
			}
        }

        public void LightningStrikeButtonClicked()
        {
            WeatherScript.LightningScript.CallIntenseLightning();
        }

        public void DawnDuskSliderChanged(float val)
        {
			if (WeatherScript.DayNightScript != null) {
	            WeatherScript.DayNightScript.TimeOfDay = val;
			}
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }

            return Mathf.Clamp(angle, min, max);
        }
    }
}