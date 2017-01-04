using System.Collections;
using UnityEngine;

public class FishTouch : MonoBehaviour {
	private GameObject book;
	// Update is called once per frame
//	void Update () {
//		if (Input.GetMouseButton(0)) {
//			
//		}
//	}

	void Start () {
		book = GameObject.Find("TableBook/book");
	}

	void OnMouseDown () {
		Debug.Log ("Send :: FishMouseButton");
		book.SendMessage ("FishMouseButton", SendMessageOptions.DontRequireReceiver);
	}
}
