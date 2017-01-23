using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFlag : MonoBehaviour {

	public void finishAnimation () {
		GameObject parent = (GameObject)gameObject.transform.parent.gameObject;
		//		GameObject parentOfParent = (GameObject)parent.transform.parent.gameObject;

		q10kController t = parent.GetComponent<q10kController>();

		t.resetAnimationFlag();
	}
}
