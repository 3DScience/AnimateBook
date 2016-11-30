using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TalkText : MonoBehaviour {

	private GameObject meoTalk;
	private GameObject rhinoTalk;
	private float meoActive = 1;
	private float rhinoActive = 1;
//	public Text talkText;

	void Start () {
		meoTalk = GameObject.Find ("MeoTalk");
		rhinoTalk = GameObject.Find ("RhinoTalk");
		if (meoTalk == true) {
			meoTalk.SetActive (false);
			meoActive = 0;
		} else if (rhinoTalk == true) {
			rhinoTalk.SetActive (false);
			rhinoActive = 0;
		}
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log ("OnTriggerEnter");
		if (other.gameObject.CompareTag ("Player")) {
			if (meoActive == 0) {
				meoTalk.SetActive (true);
				meoActive = 1;
			} else if (rhinoActive == 0) {
				rhinoTalk.SetActive (true);
				rhinoActive = 1;
			}
			setTalkText ();
		}
	}

	void OnTriggerExit(Collider other) {
		Debug.Log ("OnTriggerExit");
		if (other.gameObject.CompareTag ("Player")) {
			if (meoActive == 1) {
				meoTalk.SetActive (false);
				meoActive = 0;
			} else if (rhinoActive == 1) {
				rhinoTalk.SetActive (false);
				rhinoActive = 0;
			}
		}
	}

	void setTalkText ()
	{
		if ( meoTalk == true ) {
			meoTalk.GetComponent<Text> ().text = "Meow Meow ..";
			Debug.Log ("setTalkText: meow meow" );
		} 
		if ( rhinoTalk == true ) {
			rhinoTalk.GetComponent<Text> ().text = "Ghuww ..";
			Debug.Log ("setTalkText: ghw ghw" );
		}
	}
}
