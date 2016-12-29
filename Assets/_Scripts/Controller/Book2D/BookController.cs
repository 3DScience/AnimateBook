using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

public class BookController : MonoBehaviour {
	public GameObject[] pages;
	public static string catName;

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

		uiDisplay = gameObject.GetComponent<UIDisplay> ();
		yield return uiDisplay.LoadBookData("book/joint1/joint4/LeftItems/Quad","book/joint1/joint4/LeftItems/Canvas/Text",100001,catName);
		yield return uiDisplay.LoadBookData ("page/LeftItems/Quad", "page/LeftItems/Canvas/Text", 100002,catName);
		//GameObject.Find ("book/joint1/joint2/RightItems/").SetActive(false);

		if (pages.Length > 0) {
			animation = GetComponent<Animation>();
			Debug.Log("curPageNumber :: Length " + pages.Length);
		}
		yield return new WaitForSeconds(2.5f);

		GetComponent<Animation> ().Play ("openBook");
		yield return new WaitForSeconds(2.5f);
		animationAvailable = true;
	}

	IEnumerator callLoadBookdataNextPage() {
		Debug.Log("curPageNumber next:: " + curPageNumber );
		Debug.Log("nextPageNumber next:: " + nextPageNumber );
		yield return uiDisplay.LoadBookData("page/RightItems/Quad","page/RightItems/Canvas/Text",curPageNumber,catName);
		yield return uiDisplay.LoadBookData("book/joint1/joint2/RightItems/Quad","book/joint1/joint2/RightItems/Canvas/Text",nextPageNumber,catName);
		yield return new WaitForSeconds(2f);
		yield return uiDisplay.LoadBookData("book/joint1/joint4/LeftItems/Quad","book/joint1/joint4/LeftItems/Canvas/Text",curPageNumber,catName);
		yield return uiDisplay.LoadBookData ("page/LeftItems/Quad", "page/LeftItems/Canvas/Text", nextPageNumber,catName);
	}

	IEnumerator callLoadBookdataBackPage() {
		Debug.Log("curPageNumber back:: " + curPageNumber );
		Debug.Log("nextPageNumber back:: " + nextPageNumber );
		yield return uiDisplay.LoadBookData("page/LeftItems/Quad","page/LeftItems/Canvas/Text",curPageNumber,catName);
		yield return uiDisplay.LoadBookData("book/joint1/joint4/LeftItems/Quad","book/joint1/joint4/LeftItems/Canvas/Text",nextPageNumber,catName);
		yield return new WaitForSeconds(2f);
		yield return uiDisplay.LoadBookData("book/joint1/joint2/RightItems/Quad","book/joint1/joint2/RightItems/Canvas/Text",curPageNumber,catName);
		yield return uiDisplay.LoadBookData ("page/RightItems/Quad", "page/RightItems/Canvas/Text", nextPageNumber,catName);
	}

	IEnumerator delayAddPage() {
		yield return new WaitForSeconds(0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		//open next page
		if (SwipeManager.instance.IsSwiping (SwipeDirection.Left)) {

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
				}
			}
		}


		//back page
		if (SwipeManager.instance.IsSwiping (SwipeDirection.Right)) {
			
			if (animationAvailable == true) {
				if (curPageNumber > 100002) {
					animationAvailable = false;
					curPageNumber = curPageNumber - 1;
					nextPageNumber = curPageNumber - 1;
					StartCoroutine (callLoadBookdataBackPage ());
					StartCoroutine (delayAddPage ());
					GameObject.Find ("book/page").GetComponent<Animation> ().Play ("back");
				} else {
					curPageNumber = 100001;
					nextPageNumber = 100002;
				}
			}
		}
	}

	public void resetAnimationFlag () {
		Debug.Log("resetAnimationFlag");
		animationAvailable = true;
	}

	private void loadBook()
	{
		//String assetBundleName= "test_book";
		string assetBundleName= "solar_system_book";
		if (checkIsDownloadedAsset(assetBundleName))
		{
			BookLoaderScript.assetBundleName = assetBundleName;
			SceneManager.LoadScene(GlobalVar.BOOK_LOADER_SCENE);
		}
		else
		{
			SceneManager.LoadScene(GlobalVar.DOWNLOAD_ASSET_SCENE);
		}
	}

	private bool checkIsDownloadedAsset(string assetBundleName)
	{
		string assetDataFolder = GlobalVar.DATA_PATH + "/" + assetBundleName;
		if (Directory.Exists(assetDataFolder))
		{
			return true;
		}
		return false;
	}

}
