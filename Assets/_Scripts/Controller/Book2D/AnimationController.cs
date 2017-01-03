using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

	public void finishAnimation () {
		GameObject parent = (GameObject)gameObject.transform.parent.gameObject;
//		GameObject parentOfParent = (GameObject)parent.transform.parent.gameObject;

		BookController2D t = parent.GetComponent<BookController2D>();

		t.resetAnimationFlag();
	}
}
