using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
//using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class UiEventHandler : MonoBehaviour {

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
	private GameObject animationText;

    private BookLoaderScript bookLoaderScript;

    void Start() {
        bookLoaderScript = gameObject.GetComponent<BookLoaderScript>();
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

	public void ButtonToBegin() {
		RePlay.SetActive(false);
		Pause.SetActive(false);
		Back.SetActive(false);
		Next.SetActive(false);
		Camera.SetActive (false);
		Play.SetActive(true);
	}

    public void ButtonToPage() {
		Play.SetActive(false);
		RePlay.SetActive(true);
		Pause.SetActive(true);
		Back.SetActive(true);
		Next.SetActive(true);
		Camera.SetActive (true);
	}

    public void ButtonToEnd() {
		RePlay.SetActive(false);
		Pause.SetActive(false);
		Back.SetActive(false);
		Next.SetActive(false);
		Play.SetActive(false);
		Camera.SetActive (false);

	}

    public void ButtonToPause() {
		Button textPause = Pause.GetComponent<Button>();
		animationText = GameObject.Find ("AnimationText");
		if (animationText != null) {
			if (timePause == 0) {
				timePause = 1;
				textPause.GetComponentInChildren<Text> ().text = "Resume";
				animationText.GetComponent<AudioSource> ().Pause ();
			} else if (timePause == 1) {
				timePause = 0;
				textPause.GetComponentInChildren<Text> ().text = "Pause";
				animationText.GetComponent<AudioSource> ().UnPause ();
			}
		}
	}

    public void OndoneLoadScene() {
		ButtonToCamera ();
	}

    public void ButtonToCamera() {
		//DebugOnScreen.Log ("button camera");

		try {
			Button textcamera = Camera.GetComponent<Button>();
			string test = textcamera.GetComponentInChildren<Text> ().text;
			CameraMoveFollowObjects = GameObject.Find ("CameraMoveFollowObjects");
			if (CameraMoveFollowObjects != null) { 

				CameraMoveFollowObjects cameraMoveFollowObjectsScripts = CameraMoveFollowObjects.AddComponent<CameraMoveFollowObjects> ();
				GameObject starring = GameObject.FindGameObjectWithTag("Starring");

				if (starring != null) {
					cameraMoveFollowObjectsScripts.player = starring;
					if (test.IndexOf ("Camera:On") == 0) {
						//DebugOnScreen.Log ("button camera: Off");
						textcamera.GetComponentInChildren<Text> ().text = "Camera:Off";
						CameraMoveFollowObjects.GetComponent<Camera> ().enabled = true;
						CameraMain.GetComponent<Camera> ().enabled = true;
					} else if (test.IndexOf ("Camera:Off") == 0) {
						//DebugOnScreen.Log ("button camera: On");
						textcamera.GetComponentInChildren<Text> ().text = "Camera:On";
						CameraMoveFollowObjects.GetComponent<Camera> ().enabled = false;
						CameraMain.GetComponent<Camera> ().enabled = true;
					}
				}
			}
		} catch (System.Exception ex) {
			DebugOnScreen.Log ("button camera err :" + ex);
		}
			
	}

	public void playToScene(){
        bookLoaderScript.OnPlay();
	}

	public void HomeToScene(){
        bookLoaderScript.OnHome();

    }

	public void replayToScene(){
        bookLoaderScript.OnReplay();

    }

	public void pauseToScene(){
		if (timePause == 0) {
            bookLoaderScript.OnPause();
            timePause = 1;
		} else if (timePause == 1) {
			timePause = 0;
            bookLoaderScript.OnResume();

        }
	}

	public void cameraToScene(){
		Button textcamera = Camera.GetComponent<Button>();

		string test = textcamera.GetComponentInChildren<Text> ().text;
		Debug.Log ("text camera:" + test);

		if (test.IndexOf("Camera:On") == 0) {
            bookLoaderScript.OnCamera();

        } else if (test.IndexOf("Camera:Off") == 0) {
            bookLoaderScript.OffCamera();


        } else {
			Debug.Log ("text camera: none");
		}
	}

	public void resumeToScene(){
        bookLoaderScript.OnPause();

    }

	public void backToScene(){
        bookLoaderScript.OnBack();

//		int sceneID = SceneManager.GetActiveScene().buildIndex;
//		if (sceneID > 0) {
//			sceneID = sceneID - 1;
//		}
//		SceneManager.LoadScene (sceneID);
    }

	public void nextToScene(){
        bookLoaderScript.OnNext();

    }
}
