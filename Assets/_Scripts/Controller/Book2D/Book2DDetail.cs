using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Book2DDetail : MonoBehaviour {

	public Image img;
	public Text bookDescription;
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
		loadDescription ();
		loadTitle ();
	}
	
	IEnumerator loadImg(string urls){
		Debug.Log ("Book2DDetail ---- : " + urls);
		WWW imgLink = new WWW (urls);
		yield return imgLink;

		img.sprite = Sprite.Create(imgLink.texture, new Rect(0, 0, imgLink.texture.width, imgLink.texture.height), new Vector2(0, 0));
	
	}

	private void loadDescription() {
		description = bookInfo.description;
		price = bookInfo.price;
		version = bookInfo.version;
		bookDescription.text = "Giới thiệu về sách: \n" + description + "\n\nThông tin sách: \nPrice: " + price + "\nversion: " + version;
	}

	private void loadTitle() {
		name = bookInfo.name;
		status = bookInfo.status;
		bookName.text = name + "\nstatus: " + status;
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
