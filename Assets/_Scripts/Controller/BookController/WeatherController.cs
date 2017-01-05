using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.WeatherMaker;

public class WeatherController : MonoBehaviour {
	public GameObject weatherController;
	private WeatherMakerConfigurationScript weatherScript;

	void Start () {
		weatherScript = weatherController.GetComponent<WeatherMakerConfigurationScript>();
	}

	public void invokeRainByLevel(bool flag, int lv) {
		if (weatherScript != null) {

			weatherScript.RainToggleChanged(flag);

			switch (lv) {
			case 1:
				weatherScript.IntensitySliderChanged(0.3f);
				break;

			case 2:
				weatherScript.IntensitySliderChanged(0.5f);
				break;

			case 3:
				weatherScript.IntensitySliderChanged(0.8f);
				break;

			case 4:
				weatherScript.IntensitySliderChanged(1.0f);
				break;

			default:
				weatherScript.IntensitySliderChanged(0.5f);
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

	private IEnumerator lightningStrikeInCoInSuccession () {
		for (int count = 0; count < 7; count++) {
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
