using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController3D : MonoBehaviour {

	public void finishAnimation () {
		GameObject parent = (GameObject)gameObject.transform.parent.gameObject;
		GameObject parentOfParent = (GameObject)parent.transform.parent.gameObject;

		BookController t = parentOfParent.GetComponent<BookController>();

		t.resetAnimationFlag();
	}
}
