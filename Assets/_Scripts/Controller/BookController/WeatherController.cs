using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.WeatherMaker;

public class WeatherController : MonoBehaviour {
	public GameObject weatherController;
	private WeatherMakerConfigurationScript weatherScript;

	public bool snowFlag = false;
	public bool hailFlag = false;
	public bool sleedFlag = false;
	public bool sunFlag = false;
	public bool dayNightCycleFlag = false;
	public bool cloudsFlag = false;
	public bool fogFlag = false;
//	public bool thunderFlag = false;


	void Start () {
		if (weatherController == null) {
			weatherController = Instantiate(Resources.Load("WeatherMaker/Prefab/Prefab/WeatherMakerPrefab", typeof(GameObject))) as GameObject;

			if (weatherController != null) {
				weatherScript = weatherController.GetComponent<WeatherMakerConfigurationScript>();
				Debug.Log ("Load weather component successfully");

				//disable some un-use objects
				GameObject config = weatherController.transform.FindChild("ConfigurationCanvas").gameObject;
				Destroy(config);

				if (snowFlag == false) {
					GameObject snow = weatherController.transform.FindChild("SnowPrefab").gameObject;
					Destroy(snow);
				}

				if (hailFlag == false) {
					GameObject hail = weatherController.transform.FindChild("HailPrefab").gameObject;
					Destroy(hail);
				}

				if (sleedFlag == false) {
					GameObject sleed = weatherController.transform.FindChild("SleetPrefab").gameObject;
					Destroy(sleed);
				}

				if (sunFlag == false) {
					GameObject sun = weatherController.transform.FindChild("Sun").gameObject;
					Destroy(sun);
				}

				GameObject skySphere = weatherController.transform.FindChild("SkySphere").gameObject;
				Destroy(skySphere);

				if (dayNightCycleFlag == false) {
					GameObject dayNightCycle = weatherController.transform.FindChild("DayNightCycle").gameObject;
					Destroy(dayNightCycle);
				}

				if (cloudsFlag == false) {
					GameObject clouds = weatherController.transform.FindChild("Clouds").gameObject;
					Destroy(clouds);
				}

				if (fogFlag == false) {
					GameObject fog = weatherController.transform.FindChild("Fog").gameObject;
					Destroy(fog);
				}

				GameObject thunder = weatherController.transform.FindChild("ThunderAndLightningPrefab").gameObject;
				WeatherMakerThunderAndLightningScript3D script  = thunder.GetComponent<WeatherMakerThunderAndLightningScript3D>();
				script.Camera = Camera.main;

			} else {
				Debug.Log ("Could not load weather component");
			}
		} else {
			weatherScript = weatherController.GetComponent<WeatherMakerConfigurationScript>();
		}
	}

	public void invokeRainByLevel(bool flag, int lv) {
		if (weatherScript != null) {

			weatherScript.RainToggleChanged(flag);

			switch (lv) {
			case 1:
				weatherScript.IntensitySliderChanged(0.1f);
				break;

			case 2:
				weatherScript.IntensitySliderChanged(0.2f);
				break;

			case 3:
				weatherScript.IntensitySliderChanged(0.3f);
				break;

			case 4:
				weatherScript.IntensitySliderChanged(0.5f);
				break;

			case 5:
				weatherScript.IntensitySliderChanged(0.7f);
				break;

			default:
				weatherScript.IntensitySliderChanged(0.2f);
				break;
			}
		}
	}

	public void invokeWind(bool flag) {
		if (weatherScript != null) {

			weatherScript.WindToggleChanged(flag);
		}
	}

	public void invokeLightning() {
		if (weatherScript != null) {

			weatherScript.LightningStrikeButtonClicked();
		}
	}

	public void invokeCloud(bool flag) {
		if (weatherScript != null) {

//			weatherScript.CloudToggleChanged(flag);
		}
	}

	private IEnumerator lightningStrikeInCoInSuccession () {
		for (int count = 0; count < 10; count++) {
			yield return new WaitForSeconds (2.0f);

			invokeLightning();
		}
	}

	public void InvokeLightningStrikeInCoInSuccession () {
		StartCoroutine (lightningStrikeInCoInSuccession());
	}

	public void transitionDurationChanged(int value) {
		weatherScript.TransitionDurationSliderChanged(value);
	}
  
}
