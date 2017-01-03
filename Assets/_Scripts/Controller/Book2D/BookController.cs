using UnityEngine;
using System.Collections;

public class BookController : MonoBehaviour {
	public GameObject[] pages;
	public static string catName;
	public GameObject bookActive;

	public int i =0;
	private int isLoadPage = 0;
	private int curPageNumber = 100001;
	private int nextPageNumber = 100002;

	private DragPage curAnimator;
	private DragPage nextAnimator;

	private GameObject curPage;
	private GameObject nextPage;

	private Animation animation;
	private Animation curPageAnimation;
	private Animation nextPageAnimation;

	private UIDisplay uiDisplay;

	private GameObject objectAnimation;

	private bool animationAvailable;

	// Use this for initialization
	IEnumerator Start () {
		if (catName == null) {
			catName = "science";
		}
		uiDisplay = gameObject.GetComponent<UIDisplay> ();
		yield return uiDisplay.LoadBookData("book/joint1/joint4/LeftItems/Quad","book/joint1/joint4/LeftItems/Canvas/Text",100001,catName, true);
		yield return uiDisplay.LoadBookData ("page/LeftItems/Quad", "page/LeftItems/Canvas/Text", 100002,catName,false);
		//GameObject.Find ("book/joint1/joint2/RightItems/").SetActive(false);

		if (pages.Length > 0) {
			animation = GetComponent<Animation>();
			Debug.Log("curPageNumber :: Length " + pages.Length);
		}

		if (catName == "science") {
			yield return new WaitForSeconds(2.5f);
			GetComponent<Animation> ().Play ("openBook");
		} else {
			bookActive.SetActive (true);
		}
		yield return new WaitForSeconds(2.5f);
		bookActive.SetActive (false);
		animationAvailable = true;
	}

	IEnumerator callLoadBookdataNextPage() {
		Debug.Log("curPageNumber next:: " + curPageNumber );
		Debug.Log("nextPageNumber next:: " + nextPageNumber );
		yield return uiDisplay.LoadBookData("page/RightItems/Quad","page/RightItems/Canvas/Text",curPageNumber,catName,true);
		yield return uiDisplay.LoadBookData("book/joint1/joint2/RightItems/Quad","book/joint1/joint2/RightItems/Canvas/Text",nextPageNumber,catName,false);
		yield return new WaitForSeconds(2f);
		yield return uiDisplay.LoadBookData("book/joint1/joint4/LeftItems/Quad","book/joint1/joint4/LeftItems/Canvas/Text",curPageNumber,catName,true);
		yield return uiDisplay.LoadBookData ("page/LeftItems/Quad", "page/LeftItems/Canvas/Text", nextPageNumber,catName,false);
	}

	IEnumerator callLoadBookdataBackPage() {
		Debug.Log("curPageNumber back:: " + curPageNumber );
		Debug.Log("nextPageNumber back:: " + nextPageNumber );
		yield return uiDisplay.LoadBookData ("page/RightItems/Quad", "page/RightItems/Canvas/Text", nextPageNumber+2,catName,true);
		yield return uiDisplay.LoadBookData("book/joint1/joint2/RightItems/Quad","book/joint1/joint2/RightItems/Canvas/Text",nextPageNumber+3,catName,false);
		yield return uiDisplay.LoadBookData("page/LeftItems/Quad","page/LeftItems/Canvas/Text",curPageNumber,catName,false);
		yield return uiDisplay.LoadBookData("book/joint1/joint4/LeftItems/Quad","book/joint1/joint4/LeftItems/Canvas/Text",nextPageNumber,catName,true);
		yield return new WaitForSeconds(2f);
//		yield return uiDisplay.LoadBookData("book/joint1/joint2/RightItems/Quad","book/joint1/joint2/RightItems/Canvas/Text",curPageNumber,catName,false);
//		yield return uiDisplay.LoadBookData ("page/RightItems/Quad", "page/RightItems/Canvas/Text", nextPageNumber,catName,true);
	}

	IEnumerator delayAddPage() {
		yield return new WaitForSeconds(1f);
	}
	
	// Update is called once per frame
	void Update () {
		//open next page
		if (SwipeManager.instance.IsSwiping (SwipeDirection.Left)) {
			i = 0;
			if (animationAvailable == true) {

				if (nextPageNumber < curPageNumber) {
					curPageNumber = nextPageNumber;
					nextPageNumber = curPageNumber + 1;
				}

				if (nextPageNumber < 100004) {
					animationAvailable = false;
					curPageNumber = nextPageNumber + 1;
					nextPageNumber = curPageNumber + 1;
					StartCoroutine (callLoadBookdataNextPage ());
					StartCoroutine (delayAddPage ());
					GameObject.Find ("book/page").GetComponent<Animation> ().Play ("flip");
				} else {
				}
			}
		}


		//back page
		if (SwipeManager.instance.IsSwiping (SwipeDirection.Right)) {
			Debug.Log ("back page iiii: " + i);
			if (animationAvailable == true) {
				if (curPageNumber > 100002) {
					animationAvailable = false;
					if (i == 0) { 
						curPageNumber = curPageNumber - 1;
						nextPageNumber = curPageNumber - 1;
					} else {
						curPageNumber = nextPageNumber - 1;
						nextPageNumber = curPageNumber - 1;
					}
					StartCoroutine (callLoadBookdataBackPage ());
					StartCoroutine (delayAddPage ());
					GameObject.Find ("book/page").GetComponent<Animation> ().Play ("back");
				} else {
					curPageNumber = 100001;
					nextPageNumber = 100002;
				}
			}
			i = i+1;
		}
	}

	public void resetAnimationFlag () {
		Debug.Log("resetAnimationFlag");
		animationAvailable = true;
	}

}
