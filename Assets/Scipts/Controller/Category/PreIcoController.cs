using UnityEngine;
using System.Collections;

public class PreIcoController : MonoBehaviour {

	public GameObject book, page_left, page_right, flip_pageleft, flip_pageright;

	// Use this for initialization
	void Start () {
		book = GameObject.Find("book");
		page_left = GameObject.Find("page_left");
		page_right = GameObject.Find("page_right");
		flip_pageleft = GameObject.Find("flip_pageleft");
		flip_pageright = GameObject.Find("flip_pageright");
	}

	// Update is called once per frame
	void Update () {

	}

	void OnMouseDown() {
		book.GetComponent<Animation>().Play("To right");

		Material material1, material2, material3;
		material1 = Resources.Load("page1_R", typeof(Material)) as Material;
		material2 = Resources.Load("page2_L", typeof(Material)) as Material;
		material3 = Resources.Load("page2_R", typeof(Material)) as Material;

		flip_pageleft.GetComponent<Renderer> ().material = material1;
		flip_pageright.GetComponent<Renderer> ().material = material2;
		StartCoroutine(delayAddPage(material2, material3));

		Debug.Log ("Right To Left");
	}

	IEnumerator delayAddPage(Material pl, Material pr) {
		yield return new WaitForSeconds(0.3f);
		page_right.GetComponent<Renderer> ().material = pr;
		yield return new WaitForSeconds(0.69f);
		page_left.GetComponent<Renderer> ().material = pl;
	}

}
