using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class buttonChangeScreen : MonoBehaviour {


	private GameObject RePlay;
	private GameObject Home ;
	private GameObject Pause;
	private GameObject Back;
	private GameObject Next;
	private GameObject Resume;
	private GameObject Play;


	void Start() {
		RePlay = GameObject.Find ("Replay");
		Home = GameObject.Find ("Home");
		Pause = GameObject.Find ("Pause");
		Back = GameObject.Find ("Back");
		Next = GameObject.Find ("Next");
		Resume = GameObject.Find ("Resume");
		Play = GameObject.Find ("Play");
	}

	void ButtonToBegin() {
		RePlay.SetActive(false);
		Pause.SetActive(false);
		Back.SetActive(false);
		Next.SetActive(false);
		Resume.SetActive(false);
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
		Resume.SetActive(false);
		Play.SetActive(false);
	}

	void ButtonToPause() {
		Pause.SetActive(false);
		Resume.SetActive(true);
	}
	void ButtonToResume() {
		Pause.SetActive(true);
		Resume.SetActive(false);
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
		GameObject Loader = GameObject.Find ("Loader");
		Loader.SendMessage ("OnPause");
//		Time.timeScale = 0;
	}

	public void resumeToScene(){
		GameObject Loader = GameObject.Find ("Loader");
		Loader.SendMessage ("OnResume");
//		Time.timeScale = 1;
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
