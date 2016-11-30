using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimationText : MonoBehaviour {

	public Text textArea;
	public float speed = 0.1f;
	public float waitTime = 2f;
	public string[] strings;
	public AudioClip[] audio;

	private float deltaTime = 0;
	private float endTime = 0;
	private float timePlay = 0;

	int stringIndex = 0;
	int characterIndex = 0;

	void Start () {
		StartCoroutine (DisplayTimer ());
		timePlay = audio [stringIndex].length;
		speed = timePlay / strings [stringIndex].Length - 0.02f;
	}

	IEnumerator DisplayTimer() {
		while (1 == 1) {
			yield return new WaitForSeconds (speed);
			if (characterIndex > strings [stringIndex].Length) {
				continue;
			}
			textArea.text = strings [stringIndex].Substring (0, characterIndex);
			characterIndex++;
		}
	}

	void Update () {
		deltaTime = Time.realtimeSinceStartup;
		//deltaTime = GetComponent<AudioSource> ().clip.length;
		if (characterIndex == 0) {
			GetComponent<AudioSource> ().Stop();
			GetComponent<AudioSource> ().PlayOneShot (audio [stringIndex]);
//			GetComponent<AudioSource> ().loop = true;
		}

		if (characterIndex == (strings [stringIndex].Length)) {
			endTime = deltaTime + waitTime;
		}

		if ( characterIndex == (strings [stringIndex].Length + 1) & (strings.Length - 1) > stringIndex &  deltaTime >= endTime) {
			if (characterIndex < strings [stringIndex].Length) {
				characterIndex = strings [stringIndex].Length;
			} else if (stringIndex < strings.Length) {
				stringIndex++;
				characterIndex = 0;
			}
		}
	}
}
