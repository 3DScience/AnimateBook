using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* handle sound events */
/* BackgroundVolumeChanged */
/* EffectVolumeChanged */

public class GameSettings : MonoBehaviour {

	public static float backgroundVolume = 1;
	public static float effectVolume = 1;

	public GameObject book;

	public void BackgroundVolumeChanged (float volume) {
		backgroundVolume = volume;
	}

	public void EffectVolumeChanged (float volume) {
		backgroundVolume = volume;
	}
}
