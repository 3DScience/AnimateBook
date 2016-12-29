using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Book.RTDatabase;

public class UIDisplay : MonoBehaviour {

	public GameObject imgLeft;
	public GameObject textLeft;
	public GameObject imgRight;
	public GameObject textRight;

	private string myString;
	private string url = "http://hstatic.net/846/1000030846/10/2015/9-24/cute-child-pictures.jpg";
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

	private string nameObject;

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
						"Could not resolve all Firebase dependencies: " + dependencyStatus);
				}
			});
		} else {
			InitializeFirebase ();
		}
	}

	void InitializeFirebase ()
	{
		FirebaseApp app = FirebaseApp.DefaultInstance;

		app.SetEditorDatabaseUrl ("https://filebasetest-7c55d.firebaseio.com/");
		app.SetEditorP12FileName ("filebaseTest-2e653eef7319.p12");
		app.SetEditorServiceAccountEmail ("filebasetest-7c55d@appspot.gserviceaccount.com");
		app.SetEditorP12Password ("2e653eef7319ed39d40ed0a6370d9d222bbb555a");

		_databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

		Debug.Log ("InitializeFirebase:  _databaseReference:" + _databaseReference);
	}
		
	public IEnumerator LoadBookData(string nameImg, string nameText, int idBook, string catName)
	{
		if (_databaseReference == null) {
			initFizebase ();
		}
			
		BookGetInfo bookInfoLeft = new BookGetInfo (catName,idBook);
		bookInfoLeft.getFromServer (_databaseReference,gettedData,nameText,nameImg);

		yield return null;
	}

	private void gettedData(List<BookGetInfo.BookGetInfoDetail> data, string textObject, string imgObject)
	{
		description = data [0].description;
		min_app_version = data [0].min_app_version;
		name = data [0].name;
		price = data [0].price;
		status = data [0].status;
		picture_url = data [0].picture_url;
		version = data [0].version;

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

}
