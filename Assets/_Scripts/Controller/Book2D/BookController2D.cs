using UnityEngine;
using System.Collections;

public class BookController2D : MonoBehaviour {
	public GameObject[] pages;
	public static string catName;
	public GameObject bookActive;

	public int i =0;
	private int isLoadPage = 0;
	private int curPageNumber = 0;
	private int nextPageNumber = 0;
	private int minPage = 0;
	private int maxPage = 0;
	private string pageLeftImg = "page/LeftItems/Quad";
	private string pageLeftText = "page/LeftItems/Canvas/Text";
	private string pageRightImg = "page/RightItems/Quad";
	private string pageRightText = "page/RightItems/Canvas/Text";
	private string bookLeftImg = "book/joint1/joint4/LeftItems/Quad";
	private string bookLeftText = "book/joint1/joint4/LeftItems/Canvas/Text";
	private string bookRightImg = "book/joint1/joint2/RightItems/Quad";
	private string bookRightText = "book/joint1/joint2/RightItems/Canvas/Text";

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

//		//test when start from scene book2D
//		if (catName == null) {
//			catName = "science";
//		}
//		// end test

		uiDisplay = gameObject.GetComponent<UIDisplay> ();
	
		getPage();
		yield return uiDisplay.LoadBookData(bookLeftImg,bookLeftText,curPageNumber,catName, true);
		yield return uiDisplay.LoadBookData (pageLeftImg, pageLeftText, nextPageNumber,catName,false);

		if (pages.Length > 0) {
			animation = GetComponent<Animation>();
		}

		uiDisplay.getCountData(catName, (int count)=>{
			if (count > 0) {
				StartCoroutine (delayAddPage ());
				GetComponent<Animation> ().Play ("openBook");
			} else {
				bookActive.SetActive (true);
				StartCoroutine (delayAddPage ());
			}
		});
		yield return new WaitForSeconds (2f);
		bookActive.SetActive (false);
		animationAvailable = true;
	}

	IEnumerator callLoadBookdataNextPage() {
		Debug.Log("curPageNumber next:: " + curPageNumber );
		Debug.Log("nextPageNumber next:: " + nextPageNumber );
		yield return uiDisplay.LoadBookData(pageRightImg,pageRightText,curPageNumber,catName,true);
		yield return uiDisplay.LoadBookData(bookRightImg,bookRightText,nextPageNumber,catName,false);
		yield return new WaitForSeconds(2f);
		yield return uiDisplay.LoadBookData(bookLeftImg,bookLeftText,curPageNumber,catName,true);
		yield return uiDisplay.LoadBookData(pageLeftImg,pageLeftText,nextPageNumber,catName,false);
	}

	IEnumerator callLoadBookdataBackPage() {
		Debug.Log("curPageNumber back:: " + curPageNumber );
		Debug.Log("nextPageNumber back:: " + nextPageNumber );
		yield return uiDisplay.LoadBookData(pageLeftImg,pageLeftText,curPageNumber,catName,false);
		yield return uiDisplay.LoadBookData(bookLeftImg,bookLeftText,nextPageNumber,catName,true);
		yield return new WaitForSeconds(2f);
//		yield return uiDisplay.LoadBookData(pageRightImg,pageRightText,curPageNumber,catName,true);
//		yield return uiDisplay.LoadBookData(bookRightImg,bookRightText,nextPageNumber,catName,false);
	}

	IEnumerator delayAddPage() {
		yield return new WaitForSeconds(2f);
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

				if (nextPageNumber < maxPage && nextPageNumber > curPageNumber) {
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


				Debug.Log("curPageNumber back 0:: " + curPageNumber );
				Debug.Log("nextPageNumber back 0:: " + nextPageNumber );
				
				if (curPageNumber > minPage+1) {
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
					curPageNumber = minPage;
					nextPageNumber = curPageNumber + 1;
				}
			}
			i = i+1;
		}
	}

	public void resetAnimationFlag () {
		animationAvailable = true;
	}

	public void getPage() {
			
		if (catName == "science") {
			curPageNumber = 230001;
			minPage = 230001;
			
		} else if (catName == "nature") {
			curPageNumber = 210001;
			minPage = 210001;
		} else if (catName == "fairytale") {
			curPageNumber = 220001;
			minPage = 220001;
		} else if (catName == "fiction") {
			curPageNumber = 240001;
			minPage = 240001;
		}
		nextPageNumber = curPageNumber + 1;

		Debug.Log ("catName:== " + catName);

		uiDisplay.getCountData(catName, (int count)=>{
			maxPage = minPage + count -1;
		});
			
	}
}
