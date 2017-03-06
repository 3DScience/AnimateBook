using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Book2DDetail : MonoBehaviour {

	public Image img;
	public Text bookDescription1;
	public Text bookDescription2;
	public Text bookContent;
	public Text bookName;

	private BookInfo bookInfo;
	private string description;
	private string min_app_version;
	private string name;
	private string picture_url;
	private float price;
	private int status;
	private string version;
	private string assetbundle;
	private string download_url;

	private Renderer rendererImg;

	// Use this for initialization
	void Start () {
        bookInfo = (BookInfo) GlobalVar.shareContext.shareVar["bookInfo"];
		GlobalVar.shareContext.shareVar.Remove ("bookInfo");
		StartCoroutine (loadImg (bookInfo.picture_url));
		loadContent ();
		loadDescription1 ();
		loadDescription2 ();
		loadTitle ();
	}
	
	IEnumerator loadImg(string urls){
		Debug.Log ("Book2DDetail ---- : " + urls);
		WWW imgLink = new WWW (urls);
		yield return imgLink;

		img.sprite = Sprite.Create(imgLink.texture, new Rect(0, 0, imgLink.texture.width, imgLink.texture.height), new Vector2(0, 0));
	
	}

	private void loadContent() {
		description = bookInfo.detail;
		bookContent.text = "<size=16> \nGiới thiệu về sách: </size> \n\n" + description;
	}

	private void loadDescription1() {
		price = bookInfo.price;
		version = bookInfo.version;
		status = bookInfo.status;
		min_app_version = bookInfo.min_app_version;
		bookDescription1.text = "Ngôn ngữ: Tiếng Việt\n" + "Giá Tiền: " + price + "\nstatus: " + status;
	}

	private void loadDescription2() {
		status = bookInfo.status;
		min_app_version = bookInfo.min_app_version;
		bookDescription2.text = "version: " + version + "\nmin_app_version: " + min_app_version;
	}

	private void loadTitle() {
		name = bookInfo.name;
		bookName.text = name;
	}

	public void onHomeButtonClick()
	{
		SceneManager.LoadScene(GlobalVar.CATEGORY_SCENE);
	}

	public void openBookClick()
	{
		loadBook ();
	}

	public void ReDownloadBook()
	{
		GlobalVar.shareContext.shareVar.Add ("bookInfo", bookInfo);
		SceneManager.LoadScene(GlobalVar.DOWNLOAD_ASSET_SCENE);
	}

	private void loadBook()
	{
		string assetBundleName = "";
		assetBundleName = bookInfo.assetbundle;

		Debug.Log ("assetBundleName Book2dDetail: " + assetBundleName);

		if (checkIsDownloadedAsset(assetBundleName))
		{
			BookLoader.assetBundleName = assetBundleName;
			SceneManager.LoadScene(GlobalVar.BOOK_LOADER_SCENE);
		} 
		else
		{
			GlobalVar.shareContext.shareVar.Add ("bookInfo", bookInfo);
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
