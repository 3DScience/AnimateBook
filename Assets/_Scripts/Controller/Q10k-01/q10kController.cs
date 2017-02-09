using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Fungus;

public class q10kController : MonoBehaviour {

	private int i=0;
	private bool animationAvailable;
	private int swipe = 0;

	public GameObject IndexPageDialogPrefab;

	void Start() {
		IndexPageDialogPrefab.SetActive (true);
	}

	void Update () {
		//open next page
		if (SwipeManager.instance.IsSwiping (SwipeDirection.Left)) {

			if (swipe == 1) {
				i = i - 1;
			}

			animationAvailable = false;
			i = i + 1;
			audioPlay ();
			Debug.Log ("open next page : ");
			if (i > 0 && i < 12) {
				GameObject.Find ("book/page" + i).GetComponent<Animation> ().Play ("flip");
			} else {
				i = 11;
			}
			swipe = 0;		
		}
			
		//back page
		if (SwipeManager.instance.IsSwiping (SwipeDirection.Right)) {

			if (swipe == 1) {
				i = i - 1;
			}
			
			animationAvailable = false;
			Debug.Log ("open back page : ");
			if (i > 0 && i < 12) {
				GameObject.Find ("book/page" + i).GetComponent<Animation> ().Play ("back");
			} else {
				i = 1;
			}
			i = i - 1;
			audioPlay ();
			swipe = 0;
		}
	}

	public void resetAnimationFlag () {
		animationAvailable = true;
		audioPlay ();
	}

	public void onHomeButtonClick()
	{
		IndexPageDialogPrefab.SetActive (false);
		SceneManager.LoadScene(GlobalVar.MAINSCENE);
	}

	private void audioPlay() {
		GameObject lua = GameObject.Find ("LuaControllerAudio");
		LuaEnvironment luaEvironment = lua.GetComponent<LuaEnvironment> ();
		luaEvironment.Interpreter.Globals ["pid"] = i;
		luaEvironment.Interpreter.Globals ["animationAvailable"] = animationAvailable;
		LuaScript luascript = lua.GetComponent<LuaScript> ();
		luascript.OnExecute ();
	}

	public void gotoPage(int Pageid) {
		StartCoroutine (gotoPageFlip (Pageid));
	}

	private int indexPage;
	private IEnumerator gotoPageFlip (int Pageid) {
		
		Debug.Log ("page open iiiii: " + i);
		if (i == 0 || swipe == 0) {
			i = i + 1;
		}

		if (Pageid > i) {
			for(indexPage = i; indexPage < Pageid; indexPage ++)
			{
				Debug.Log ("page open Pageid indexPage: " + indexPage);
				GameObject.Find ("book/page" + indexPage).GetComponent<Animation> ().Play ("flip");
				yield return new WaitForSeconds(0.2f);
			}
		} else if(Pageid < i) {
			for(indexPage = i; indexPage > Pageid; indexPage --)
			{
				int aa = indexPage - 1;
				Debug.Log ("page open indexPage: " + indexPage);
				GameObject.Find ("book/page" + aa).GetComponent<Animation> ().Play ("back");
				yield return new WaitForSeconds(0.2f);
			}
		}

		i = indexPage;

		Debug.Log ("page open Pageid i: " + i);
		Debug.Log ("page open Pageid Pageid: " + Pageid);

		swipe = 1;

		yield return null;
	}
}
