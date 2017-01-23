using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Fungus;

public class q10kController : MonoBehaviour {

	private int i=0;
	private bool animationAvailable;

	void Update () {
		//open next page
		if (SwipeManager.instance.IsSwiping (SwipeDirection.Left)) {
			
			animationAvailable = false;
			i = i + 1;
			audioPlay ();
			Debug.Log ("open next page : ");
			if (i > 0 && i < 12) {
				GameObject.Find ("book/page" + i).GetComponent<Animation> ().Play ("flip");
			} else {
				i = 11;
			}
				
		}
			
		//back page
		if (SwipeManager.instance.IsSwiping (SwipeDirection.Right)) {
			
			animationAvailable = false;
			Debug.Log ("open back page : ");
			if (i > 0 && i < 12) {
				GameObject.Find ("book/page" + i).GetComponent<Animation> ().Play ("back");
			} else {
				i = 1;
			}
			i = i - 1;
			audioPlay ();
		}
	}

	public void resetAnimationFlag () {
		animationAvailable = true;
		audioPlay ();
	}

	public void onHomeButtonClick()
	{
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

	private IEnumerator gotoPageFlip (int Pageid) {
		i = Pageid - 1;
		for(int n = 1; n < Pageid; n ++)
		{
			GameObject.Find ("book/page" + n).GetComponent<Animation> ().Play ("flip");
			yield return new WaitForSeconds(0.2f);
		}
		yield return null;
	}
}
