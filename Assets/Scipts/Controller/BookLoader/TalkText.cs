using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TalkText : MonoBehaviour {

	private GameObject talk;
	public Text talkText;

	void Start () {
		talk = GameObject.Find ("Talk");
		talk.SetActive (false);
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log ("OnTriggerEnter");
		if (other.gameObject.CompareTag ("Player")) {
			talk.SetActive (true);
			setTalkText ();
		}
	}

	void OnTriggerExit(Collider other) {
		Debug.Log ("OnTriggerExit");
		if (other.gameObject.CompareTag ("Player")) {
			talk.SetActive (false);
		}
	}

	void setTalkText ()
	{
		talkText.text = "Meow Meow ..";
	}
}
