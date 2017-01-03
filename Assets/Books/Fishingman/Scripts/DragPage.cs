using UnityEngine;
using System.Collections;

public class DragPage : MonoBehaviour {

	private float rotationZ = 0f;
	private float sensitivityZ = 7f;

	private float rotationX = 0f;
	private float sensitivityX = 5f;

	private bool isTransferNext;
	private bool isTransferBack;

	private Animator anim;

	// Use this for initialization

	public GameObject[] items;

	void Start ()
	{
		anim = GetComponent<Animator>();
	}
	
	void Update ()
	{
				
		RaycastHit hit;
		
		if (Input.GetMouseButton(0))
		{
//			if (Physics.Raycast (Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
//			{
//				Debug.Log("xxx :: " + hit);
//				float x = -Input.GetAxis("Mouse X");
//				//float y = -Input.GetAxis("Mouse Y");
//				float speed = 10;
//
//				transform.rotation *= Quaternion.AngleAxis(x*speed, Vector3.back);
//				foreach (GameObject obj in items) {
//					obj.transform.rotation *= Quaternion.AngleAxis(-x*speed, Vector3.left);
//				}
//
//			}

//			rotationZ += Input.GetAxis("Mouse X") * -sensitivityZ;
//			rotationZ = Mathf.Clamp (rotationZ, 0, 180);
//
//			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -rotationZ);
//
//			rotationX -= Input.GetAxis("Mouse X") * sensitivityX;
//			rotationX = Mathf.Clamp (rotationX, 0, 90);
//
//			foreach (GameObject obj in items) {
//				obj.transform.localEulerAngles = new Vector3(rotationX, obj.transform.localEulerAngles.y, obj.transform.localEulerAngles.z);
//			}
		}

//		if (Input.GetKey(KeyCode.RightArrow)) {
//			isTransferNext = true;
//			anim.Play("NextPage");
//		}
//
//		if (Input.GetKey(KeyCode.LeftArrow)) {
//			isTransferBack = true;
//			anim.Play("BackPage");
//		}

//		if (isTransferNext) {
//			nextPage ();
//			Debug.Log("nextPage");
//		}
//
//		if (isTransferBack) {
//			backPage ();
//			Debug.Log("backPage");
//		}

	}

	public void nextPageAnimation () {
		anim.Play("NextPage");
	}

	public void backPageAnimation () {
		anim.Play("BackPage");
	}

	public void closePageAnimation () {
		anim.Play("ClosePage");
	}

	public void openPageAnimation () {
		anim.Play("OpenPage");
	}

	public void closeRearPageAnimation () {
		anim.Play("CloseRearPage");
	}

	public void openRearPageAnimation () {
		anim.Play("OpenRearPage");
	}

	void nextPage () {
		Vector3 newPos = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -180);

		transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, newPos, 0.2f * Time.deltaTime);


		foreach (GameObject obj in items) {
			Vector3 newObjPos = new Vector3(90, obj.transform.localEulerAngles.y, obj.transform.localEulerAngles.z);
			obj.transform.localEulerAngles = Vector3.Lerp(obj.transform.localEulerAngles, newObjPos, 2*Time.deltaTime);
		}

		if (transform.localEulerAngles.z <= 180) {
			isTransferNext = false;
		}
	}

	void backPage () {

		Vector3 newPos = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 180);

		transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, newPos, 0.2f * Time.deltaTime);


		foreach (GameObject obj in items) {
			Vector3 newObjPos = new Vector3(90, obj.transform.localEulerAngles.y, obj.transform.localEulerAngles.z);
			obj.transform.localEulerAngles = Vector3.Lerp(obj.transform.localEulerAngles, newObjPos, 2*Time.deltaTime);
		}

		if (transform.localEulerAngles.z == 0) {
			isTransferBack = false;
		}
	}
	
}