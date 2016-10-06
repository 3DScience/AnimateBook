using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
//using UnityEditor.SceneManagement;
using UnityEngine.UI;

public class buttonChangeScreen : MonoBehaviour {

	public int timePause;

	private GameObject RePlay;
	private GameObject Home;
	private GameObject Pause;
	private GameObject Back;
	private GameObject Next;
	private GameObject Play;


	void Start() {
		timePause = 0;
		RePlay = GameObject.Find ("Replay");
		Home = GameObject.Find ("Home");
		Pause = GameObject.Find ("Pause");
		Back = GameObject.Find ("Back");
		Next = GameObject.Find ("Next");
		Play = GameObject.Find ("Play");
	}

	void ButtonToBegin() {
		RePlay.SetActive(false);
		Pause.SetActive(false);
		Back.SetActive(false);
		Next.SetActive(false);
		Play.SetActive(true);
	}

	void ButtonToPage() {
		Play.SetActive(false);
		RePlay.SetActive(true);
		Pause.SetActive(true);
		Back.SetActive(true);
		Next.SetActive(true);
	}

	void ButtonToEnd() {
		RePlay.SetActive(false);
		Pause.SetActive(false);
		Back.SetActive(false);
		Next.SetActive(false);
		Play.SetActive(false);
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
