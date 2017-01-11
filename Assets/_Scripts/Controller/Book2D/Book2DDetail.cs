using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Book.RTDatabase;
using UnityEngine.UI;

public class Book2DDetail : MonoBehaviour {

	public Image img;
	public Text bookDescription;
	public Text bookName;

	private BookGetInfo.BookGetInfoDetail bookGetInfoDetail =(BookGetInfo.BookGetInfoDetail) GlobalVar.shareContext.shareVar["bookGetInfoDetail"];
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
		GlobalVar.shareContext.shareVar.Remove ("bookGetInfoDetail");
		StartCoroutine (loadImg (bookGetInfoDetail.picture_url));
		loadDescription ();
		loadTitle ();
	}
	
	IEnumerator loadImg(string urls){
		Debug.Log ("Book2DDetail ---- : " + urls);
		WWW imgLink = new WWW (urls);
		yield return imgLink;

		img.sprite = Sprite.Create(imgLink.texture, new Rect(0, 0, imgLink.texture.width, imgLink.texture.height), new Vector2(0, 0));

		//img = GameObject.Find ("Canvas/Image");

//		rendererImg = img.GetComponent<Renderer> ();
//		rendererImg.material.mainTexture = imgLink.texture;
	
	}

	private void loadDescription() {
		description = bookGetInfoDetail.description;
		bookDescription.text = "Giới thiệu về sách: \n" + description;
	}

	private void loadTitle() {
		name = bookGetInfoDetail.name;
		version = bookGetInfoDetail.version;
		bookName.text = name + "\nversion: " + version;
	}

}
