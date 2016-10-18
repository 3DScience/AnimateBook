using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
//using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class buttonChangeScreen : MonoBehaviour {

	public int timePause;

	private GameObject RePlay;
	private GameObject Home;
	private GameObject Pause;
	private GameObject Back;
	private GameObject Next;
	private GameObject Play;
	private GameObject Camera;
	private GameObject CameraMain;
	private GameObject CameraMoveFollowObjects;


	void Start() {
		timePause = 0;
		RePlay = GameObject.Find ("Replay");
		Home = GameObject.Find ("Home");
		Pause = GameObject.Find ("Pause");
		Back = GameObject.Find ("Back");
		Next = GameObject.Find ("Next");
		Play = GameObject.Find ("Play");
		Camera = GameObject.Find ("Camera");
		CameraMain = GameObject.Find ("MainCamera");
	}

	void ButtonToBegin() {
		RePlay.SetActive(false);
		Pause.SetActive(false);
		Back.SetActive(false);
		Next.SetActive(false);
		Camera.SetActive (false);
		Play.SetActive(true);
	}

	void ButtonToPage() {
		Play.SetActive(false);
		RePlay.SetActive(true);
		Pause.SetActive(true);
		Back.SetActive(true);
		Next.SetActive(true);
		Camera.SetActive (true);
	}

	void ButtonToEnd() {
		RePlay.SetActive(false);
		Pause.SetActive(false);
		Back.SetActive(false);
		Next.SetActive(false);
		Play.SetActive(false);
		Camera.SetActive (false);

	}

	void ButtonToPause() {
		Button textPause = Pause.GetComponent<Button>();
		if (timePause == 0) {
			timePause = 1;
			textPause.GetComponentInChildren<Text> ().text = "Resume";
		} else if (timePause == 1) {
			timePause = 0;
			textPause.GetComponentInChildren<Text> ().text = "Pause";
		}
	}

	void OndoneLoadScene() {
		ButtonToCamera ();
	}

	void ButtonToCamera() {
		Button textcamera = Camera.GetComponent<Button>();
		string test = textcamera.GetComponentInChildren<Text> ().text;
		CameraMoveFollowObjects = GameObject.Find ("CameraMoveFollowObjects");
		if (CameraMoveFollowObjects != null) { 
			
			CameraMoveFollowObjects cameraMoveFollowObjectsScripts = CameraMoveFollowObjects.AddComponent<CameraMoveFollowObjects> ();
			GameObject starring = GameObject.FindGameObjectWithTag("Starring");

			if (starring != null) {
				cameraMoveFollowObjectsScripts.player = starring;
				if (test.IndexOf ("Camera:On") == 0) {
					textcamera.GetComponentInChildren<Text> ().text = "Camera:Off";
					CameraMoveFollowObjects.GetComponent<Camera> ().enabled = true;
					CameraMain.GetComponent<Camera> ().enabled = false;
				} else if (test.IndexOf ("Camera:Off") == 0) {
					textcamera.GetComponentInChildren<Text> ().text = "Camera:On";
					CameraMoveFollowObjects.GetComponent<Camera> ().enabled = false;
					CameraMain.GetComponent<Camera> ().enabled = true;
				}
			}
		}
	}

	public void playToScene(){
		GameObject Loader = GameObject.Find ("Loader");
		Loader.SendMessage ("OnPlay");
	}

	public void HomeToScene(){
		GameObject Loader = GameObject.Find ("Loader");
		Loader.SendMessage ("OnHome");
	}

	public void replayToScene(){
		GameObject Loader = GameObject.Find ("Loader");
		Loader.SendMessage ("OnReplay");
	}

	public void pauseToScene(){
		if (timePause == 0) {
			GameObject Loader = GameObject.Find ("Loader");
			Loader.SendMessage ("OnPause");
			timePause = 1;
		} else if (timePause == 1) {
			timePause = 0;
			GameObject Loader = GameObject.Find ("Loader");
			Loader.SendMessage ("OnResume");
		}
	}

	public void cameraToScene(){
		Button textcamera = Camera.GetComponent<Button>();
		GameObject Loader = GameObject.Find ("Loader");

		string test = textcamera.GetComponentInChildren<Text> ().text;
		Debug.Log ("text camera:" + test);

		if (test.IndexOf("Camera:On") == 0) {
			Loader.SendMessage ("OnCamera");

		} else if (test.IndexOf("Camera:Off") == 0) {
			Loader.SendMessage ("OffCamera");

		} else {
			Debug.Log ("text camera: none");
		}
	}

	public void resumeToScene(){
		GameObject Loader = GameObject.Find ("Loader");
		Loader.SendMessage ("OnPause");
	}

	public void backToScene(){
		GameObject Loader = GameObject.Find ("Loader");
		Loader.SendMessage ("OnBack");

//		int sceneID = SceneManager.GetActiveScene().buildIndex;
//		if (sceneID > 0) {
//			sceneID = sceneID - 1;
//		}
//		SceneManager.LoadScene (sceneID);
	}

	public void nextToScene(){
		GameObject Loader = GameObject.Find ("Loader");
		Loader.SendMessage ("OnNext");
//		int sceneID = SceneManager.GetActiveScene().buildIndex;
//		if (sceneID < SceneManager.sceneCount) {
//			sceneID = sceneID + 1;
//		}
//		SceneManager.LoadScene (sceneID);
	}
}
