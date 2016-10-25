using UnityEngine;
using System.Collections;

public class CameraController : TouchLogic {

	public float dragSpeed = 1.0f;
	//public float currDist = 0.0f;
	public float cameraY = 0.0f;
	public float deltaMaxCameraX1 = 0f;	// gioi han truc x max cua cameraY khi di chuyen +;
	public float deltaMaxCameraX2 = 0f;	// gioi han truc x max cua cameraY khi di chuyen +;
	public float deltaMaxCameraZ1 = 0f;
	public float deltaMaxCameraZ2 = 0f;
	public float conorA1 = 0;
	public float conorA2 = 0;
	public float zoomSpeed = 2.0f;
	public float currDist = 0.0f,
	lastDist = 0.0f,
	zoomFactor = 0.0f;
	public float _scare =0.0f;

	public Vector2 ObjectPostionSart = Vector2.zero;
	public Vector2 currDistVector = Vector2.zero;

	private GameObject _plane;
	private Transform cachedTransform;
	private Vector3 startingPos;
	// Xac dinh vi tri cua touch1 va touch 2;
	private Vector2 currTouch1 = Vector2.zero,
	lastTouch1 = Vector2.zero,
	currTouch2 = Vector2.zero,
	lastTouch2 = Vector2.zero;

	private float flagCameraExitZoom = 0;
	private float flagCameraZoom = 0;	// 0 normal, 1 camera exit when zoomOut, 0 camea exit when zoomIN
	private float flagCameraExitDrag = 0;
	private float flagCameraDrag = 0;

	void Start() {
		cachedTransform = transform;
		startingPos = cachedTransform.position;
	}

	void OnTouchBeganAnyWhere() {
	}

	void OnTouchMoveAnyWhere()
	{
		Debug.Log ("OnTouchMoveAnyWhereOnTouchMoveAnyWhereOnTouchMoveAnyWhere");
		ZoomCamera ();
		if (Input.touchCount == 1) {
			Vector2 _deltaPosition = -Input.GetTouch (0).deltaPosition; // deltaPosition khoang cach giua vi tri cuoi cung va vi tri gan day nhat

			switch (Input.GetTouch (0).phase) {
			case TouchPhase.Began:
				//ObjectPostionSart = - Input.GetTouch (0).position;
				Debug.Log ("BeganBeganBeganBeganBeganBeganBeganBeganBeganBeganBeganBegan");
				break;
			case TouchPhase.Moved:
				DragCamera (_deltaPosition);
				break;
			case TouchPhase.Ended:

				break;
			}
		}
	}

	void OnTouchStayAnyWhere()
	{
	}

	void OnTouchEndedAnyWhere () {
		if (flagCameraExitDrag == 1 || flagCameraExitZoom == 1) {
			cachedTransform.position = lastCameraPos;
		}
	}
		
	void ZoomCamera ()
	{
		switch (TouchLogic.currTouch) 
		{
		case 0:	// firt touch
			currTouch1 = Input.GetTouch (0).position;
			lastTouch1 = currTouch1 - Input.GetTouch (0).deltaPosition;	// deltaPosition khoang cvi tri gan day nhat va vi tri truoc do
			break;
		case 1:	// second touch
			currTouch2 = Input.GetTouch (1).position;
			lastTouch2 = currTouch2 - Input.GetTouch (1).deltaPosition;
			break;
		}
		// we dont want to find the distance between 1 finger and nothing
		if (TouchLogic.currTouch >= 1) {
			currDist = Vector2.Distance (currTouch1, currTouch2);		// Distance tra ve khoang cach giau a va b
			lastDist = Vector2.Distance (lastTouch1, lastTouch2);
		} else {
			currDist = 0.0f;
			lastDist = 0.0f;
		}
			
		zoomFactor =  Mathf.Clamp(lastDist - currDist, -30f, 30f);
	


		_plane = GameObject.Find ("Plane");

		if (_plane == null) {
			return;
		}

		Vector3 _planceScare = _plane.transform.localScale;

//		if (flagCameraExitZoom == 1 & flagCameraZoom == 0  & zoomFactor > 0) {
//			Debug.Log ("khong zoom out" + zoomFactor);
//			flagCameraZoom = 1;
//		} else if (flagCameraExitZoom == 1 & flagCameraZoom == 0 & zoomFactor < 0) {
//			flagCameraZoom = 2;
//			Debug.Log ("khong zoom in" + zoomFactor);
//		} else if (flagCameraExitZoom == 1 & flagCameraZoom == 2 & zoomFactor > 0) {
//			flagCameraExitZoom = 0;
//			flagCameraZoom = 0;
//			Debug.Log ("tiep tuc zoom out" + zoomFactor);
//			Camera.main.transform.Translate (Vector3.back * zoomFactor * zoomSpeed * Time.deltaTime);
//		} else if (flagCameraExitZoom == 1 & flagCameraZoom == 1 & zoomFactor < 0) {
//			Debug.Log ("tiep tuc zoom in" + zoomFactor);
//			flagCameraExitZoom = 0;
//			flagCameraZoom = 0;
//			Camera.main.transform.Translate(Vector3.back * zoomFactor * zoomSpeed * Time.deltaTime);
//		} else if (flagCameraExitZoom == 0 & flagCameraZoom == 0) {
//			Debug.Log ("tiep tuc zoom nhu binh thuong" + zoomFactor);
//			Camera.main.transform.Translate(Vector3.back * zoomFactor * zoomSpeed * Time.deltaTime);
//		}

		if (flagCameraExitZoom == 1 & flagCameraZoom == 0 & zoomFactor > 0) {
			Debug.Log ("khong zoom out" + zoomFactor);
		} else if (flagCameraExitZoom == 1 & flagCameraZoom == 0 & zoomFactor < 0) {
			Debug.Log ("khong zoom in" + zoomFactor);
		} else {
			Camera.main.transform.Translate(Vector3.back * zoomFactor * zoomSpeed * Time.deltaTime);

		}
	}

	void DragCamera(Vector2 deltaPosition)
	{
		//deltaA = deltaPosition.y;
		Debug.Log ("Camera Y:" + Camera.main.transform.position.y);

		_plane = GameObject.Find ("Plane");

		if (_plane == null) {
			return;
		}

		Vector3 _planceScare = _plane.transform.localScale;
		Renderer planeMesh = _plane.GetComponent<Renderer>();
		Bounds bounds = planeMesh.bounds;

		float a = Camera.main.transform.position.y / (_planceScare.x * 10);	//ty le toc do a zoom tuong ung voi quang duong zoom Y anh huong toc do di chuyen quang duong X, Z di chuyen dc
		float b = Camera.main.transform.position.y / (_planceScare.z * 10);

		float aX = Mathf.Clamp ((deltaPosition.x * a * dragSpeed * Time.deltaTime) + cachedTransform.position.x,
			-bounds.size.x, bounds.size.x);
		float aZ = Mathf.Clamp ((deltaPosition.y * b * dragSpeed* Time.deltaTime) + cachedTransform.position.z,-bounds.size.z, bounds.size.z);

		if (flagCameraExitDrag == 1 & flagCameraDrag == 0 & (aX > 0 || aZ > 0)) {
			Debug.Log ("khong move x+ :" + aX + "khong move Z+ :" + aZ);
		} else if (flagCameraExitDrag == 1 & flagCameraDrag == 0 & (aX < 0 || aZ < 0)) {
			Debug.Log ("khong move x- :" + aX + "khong move Z- :" + aZ);
		} else {
			cachedTransform.position = new Vector3 (aX, cachedTransform.position.y, aZ);

		}
	}
	Vector3 lastCameraPos;
	void OnTriggerExit(Collider other) {	// OnTriggerEnter su kien se xay ra khi doi tuong xay ra va cham voi doi tuong khac
		// tich Is trigger trong Box Collier cua doi tuong Pick Up de kich hoat nhan su kien OnTriggerEnter
		if(other.gameObject.CompareTag("BackGround"))	// xac dinh doi tuong va cham la doi tuong Pick Up
		{

			Debug.Log ("Stop zoom in camera FUCKKCKKKKKKKKK" + other.gameObject);
			if (Input.touchCount == 1) {
				flagCameraExitDrag = 1;
			} else {
				flagCameraExitZoom = 1;

			}


		}
	}


	void OnTriggerStay(Collider other) {	// OnTriggerEnter su kien se xay ra khi doi tuong xay ra va cham voi doi tuong khac
		// tich Is trigger trong Box Collier cua doi tuong Pick Up de kich hoat nhan su kien OnTriggerEnter
		Debug.Log ("Stop zoom in camera OnTriggerStay" );
		lastCameraPos = cachedTransform.position;
			flagCameraExitDrag = 0;
			flagCameraExitZoom = 0;
	}
		
}
