using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Book.RTDatabase;
using UnityEngine.SceneManagement;
using System.IO;

public class UIDisplay : MonoBehaviour {

	public GameObject bookActiveLeft;
	public GameObject bookActiveRight;

	private GameObject imgLeft;
	private GameObject textLeft;
	private GameObject imgRight;
	private GameObject textRight;

	private bool isLeftPage = true; 
	private string myString;
	private DatabaseReference _databaseReference;

	private Renderer rendererLeft;
	private Text myTextLeft;
	private Renderer rendererRight;

	private string description;
	private string min_app_version;
	private string name;
	private string picture_url;
	private float price;
	private int status;
	private string version;
	private string assetbundle;
	private string download_url;

	private string nameObject;
	private BookGetInfo.BookGetInfoDetail leftData;
	private BookGetInfo.BookGetInfoDetail rightData;


	DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

	void initFizebase() {
		dependencyStatus = FirebaseApp.CheckDependencies ();
		if (dependencyStatus != DependencyStatus.Available) {
			FirebaseApp.FixDependenciesAsync ().ContinueWith (task => {
				dependencyStatus = FirebaseApp.CheckDependencies ();
				if (dependencyStatus == DependencyStatus.Available) {
					InitializeFirebase ();
				} else {
					// This should never happen if we're only using Firebase Analytics.
					// It does not rely on any external dependencies.
					Debug.LogError (
						"Could --  not resolve all Firebase dependencies: " + dependencyStatus);
				}
			});
		} else {
			InitializeFirebase ();
		}
	}

	void InitializeFirebase ()
	{
		FirebaseApp app = FirebaseApp.DefaultInstance;

		app.SetEditorDatabaseUrl ("https://smallworld3d-2ac88.firebaseio.com/");
		//app.SetEditorP12FileName ("filebaseTest-2e653eef7319.p12");
		app.SetEditorServiceAccountEmail ("smallworld3d-2ac88@appspot.gserviceaccount.com");
		//app.SetEditorP12Password ("2e653eef7319ed39d40ed0a6370d9d222bbb555a");

		_databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

		Debug.Log ("InitializeFirebase:  _databaseReference:" + _databaseReference);
	}
		
	public IEnumerator LoadBookData(string nameImg, string nameText, int idBook, string catName, bool isLeftPage)
	{
		if (_databaseReference == null) {
			initFizebase ();
		}
			
		BookGetInfo bookInfo = new BookGetInfo (catName,idBook);
		bookInfo.getFromServer (_databaseReference,gettedData,nameText,nameImg,isLeftPage);

		yield return null;
	}

	private string countBook;
	public void getCountData(string categoryName, System.Action<int> callbackCountBook) {

		if (_databaseReference == null) {
			initFizebase ();
		}

		_databaseReference.Child ("public").Child(GlobalVar.LANGUAGE).Child ("books").OrderByChild ("categories/" + categoryName).EqualTo(1).ValueChanged += (object sender, ValueChangedEventArgs args) => {
			countBook = args.Snapshot.ChildrenCount.ToString();
			callbackCountBook(int.Parse(countBook));
			Debug.Log ("BookController2D.countBook : " + countBook);
		};
	}

	private void gettedData(List<BookGetInfo.BookGetInfoDetail> data, string textObject, string imgObject, bool isLeftPage)
	{
		
			description = data [0].description;
			min_app_version = data [0].min_app_version;
			name = data [0].name;
			price = data [0].price;
			status = data [0].status;
		    picture_url = data [0].picture_url;
			version = data [0].version;
			download_url = data [0].download_url;
			assetbundle = data [0].assetbundle;

			if (isLeftPage) {
				leftData = data [0];
			} else {
				rightData = data [0];
			}

			StartCoroutine (loadImg (picture_url, imgObject));

			textLeft = GameObject.Find (textObject);
			myTextLeft = textLeft.GetComponent<Text> (); 

			myTextLeft.text = "Name: " + name + "\nMin_app_version: " + min_app_version + "\nPrice: " + price + "\nStatus: " + status + "\nVersion: " + version + "\nDescription: " + description;
	}

	IEnumerator loadImg(string urls, string imgObject){

		WWW imgLink = new WWW (urls);
		yield return imgLink;

		imgLeft = GameObject.Find (imgObject);
		rendererLeft = imgLeft.GetComponent<Renderer> ();
		rendererLeft.material.mainTexture = imgLink.texture;
	}

	private float lastClickTimeLeft = 0;
	public void onLeftItemClick () {
		float b = Time.time - lastClickTimeLeft;
		Debug.Log ("Time.time - lastClickTimeLeft: " + b);
		if (Time.time - lastClickTimeLeft < 1) {
			if (leftData != null) {
				if (leftData.status == 1) {
					isLeftPage = true;
					loadBook ();
				} else {
					Debug.Log ("onLeftItemClick: " + leftData.name);
					bookActiveLeft.SetActive (true);
					StartCoroutine (delayAddPage());
				}
			}
		}
		lastClickTimeLeft = Time.time;
	}

	private float lastClickTime = 0;
	public void onRightItemClick () {
		float a = Time.time - lastClickTime;
		Debug.Log ("Time.time - lastClickTime: " + a);
		if (Time.time  - lastClickTime < 1) {
			Debug.Log ("onRightItemClick");
			if (rightData != null) {
				Debug.Log ("onRightItemClick: " + rightData.name);
				if (rightData.status == 1) {
					isLeftPage = false;
					loadBook ();
				} else {
					bookActiveRight.SetActive (true);
					StartCoroutine (delayAddPage());
				}
			}
		}
		lastClickTime = Time.time;
	}

	public void onClickItem() {
		GameObject.Find ("items").SetActive(false);
	}


	private void loadBook()
	{
		string assetBundleName = "";
		BookGetInfo.BookGetInfoDetail dataBook;
		if (isLeftPage == true) {
			assetBundleName = leftData.assetbundle;
			dataBook = leftData;

		} else {
			assetBundleName = rightData.assetbundle;
			dataBook = rightData;
		}

		Debug.Log ("assetBundleName UiDisplay: " + assetBundleName);

		if (checkIsDownloadedAsset(assetBundleName))
		{
			BookLoader.assetBundleName = assetBundleName;
			SceneManager.LoadScene(GlobalVar.BOOK_LOADER_SCENE);
		} 
		else
		{
			GlobalVar.shareContext.shareVar.Add ("bookGetInfoDetail",dataBook);
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

	IEnumerator delayAddPage() {
		yield return new WaitForSeconds(2f);
		bookActiveLeft.SetActive (false);
		bookActiveRight.SetActive (false);
	}

}
