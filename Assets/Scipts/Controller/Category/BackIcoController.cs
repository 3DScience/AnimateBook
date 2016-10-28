using UnityEngine;
using System.Collections;

public class BackIcoController : MonoBehaviour {
	
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
		book.GetComponent<Animation>().Play("To left");

		Material material1, material2, material3;
		material1 = Resources.Load("page2_L", typeof(Material)) as Material;
		material2 = Resources.Load("page1_L", typeof(Material)) as Material;
		material3 = Resources.Load("page1_R", typeof(Material)) as Material;

		flip_pageleft.GetComponent<Renderer> ().material = material3;
		flip_pageright.GetComponent<Renderer> ().material = material1;
		StartCoroutine(delayAddPage(material2, material3));

		Debug.Log ("Left To Right");
	}

	IEnumerator delayAddPage(Material pl, Material pr) {
		yield return new WaitForSeconds(0.25f);
		page_left.GetComponent<Renderer> ().material = pl;
		yield return new WaitForSeconds(0.69f);
		page_right.GetComponent<Renderer> ().material = pr;
	}

}
