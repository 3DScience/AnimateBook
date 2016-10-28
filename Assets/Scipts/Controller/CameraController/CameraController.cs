using UnityEngine;
using System.Collections;

public class CameraController : TouchLogic {

	public float dragSpeed = 1.0f;
	public float cameraY = 0.0f;

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

	// rotation camera
	private float rotX = 0f;
	private float rotY = 0f;
	private float deltaX = 0f,
	deltaY = 0,
	deltaX1 = 0,
	deltaY1 = 0;
	private Vector3 origRot;
	private Touch initTouch = new Touch();

	public float rotSpeed = 0.2f;
	public float dir =-1;

	Vector2?[] oldTouchPositions = {
		null,
		null
	};
	Vector2 oldTouchVector;
	float oldTouchDistance;


	void Start() {
		cachedTransform = transform;
		startingPos = cachedTransform.position;
		origRot = transform.eulerAngles;
		rotX = origRot.x;
		rotY = origRot.y;
	}

	void OnTouchBeganAnyWhere() {
	}

	void OnTouchMoveAnyWhere()
	{
		if (Input.touchCount == 1) {
			Vector2 _deltaPosition = -Input.GetTouch (0).deltaPosition; // deltaPosition khoang cach giua vi tri cuoi cung va vi tri gan day nhat
			DragCamera (_deltaPosition);
		} else if (Input.touchCount == 2) {
			ZoomCamera ();

			//transform.Rotate (new Vector3 (Time.deltaTime * 50, 0, 0));
			//transform.RotateAround (transform.position, new Vector3 (0, 1, 0), rotSpeed * Time.deltaTime);
			//RotationCamera ();
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

	void RotationCamera ()
	{
		switch (TouchLogic.currTouch) 
		{
		case 0:	// firt touch
			deltaX = Input.GetTouch (0).deltaPosition.x;
			deltaY = Input.GetTouch (0).deltaPosition.y;
			break;
		case 1:	// second touch
			deltaX1 = Input.GetTouch (1).deltaPosition.y;
			deltaY1 = Input.GetTouch (1).deltaPosition.y;
			break;
		}

		Debug.Log ("Rotation Camera: deltaX" + deltaX);
		Debug.Log ("Rotation Camera: deltaX1" + deltaX1);
		Debug.Log ("Rotation Camera: deltaY" + deltaY);
		Debug.Log ("Rotation Camera: deltaY1" + deltaY1);
		Debug.Log ("Rotation Camera: deltaX - deltaX1" + (deltaX - deltaX1) );
		Debug.Log ("Rotation Camera: deltaY - deltaY1" + (deltaX - deltaX1) );
		//rotX -= (deltaY - deltaY1) * Time.deltaTime * rotSpeed * dir;
		rotY -= (deltaX - deltaX1) * Time.deltaTime * rotSpeed * dir;
	
		//transform.eulerAngles = new Vector3 (20f, rotY, 0f);
		//cachedTransform. = new Vector3(20f,rotY,0f);


//		if (Input.touchCount == 0) {
//			oldTouchPositions [0] = null;
//			oldTouchPositions [1] = null;
//		} else if (Input.touchCount == 1) {
//			if (oldTouchPositions [0] == null || oldTouchPositions [1] != null) {
//				oldTouchPositions [0] = Input.GetTouch (0).position;
//				oldTouchPositions [1] = null;
//			}
//		} else {
//			if (oldTouchPositions [1] == null) {
//				oldTouchPositions [0] = Input.GetTouch (0).position;
//				oldTouchPositions [1] = Input.GetTouch (1).position;
//				oldTouchVector = (Vector2)(oldTouchPositions [0] - oldTouchPositions [1]);
//				oldTouchDistance = oldTouchVector.magnitude;
//				Debug.Log ("oldTouchDistance --------: " + oldTouchDistance);
//			} else {
//				Vector2 screen = new Vector2(GetComponent<Camera>().pixelWidth, GetComponent<Camera>().pixelHeight);
//
//				Vector2[] newTouchPositions = {
//					Input.GetTouch (0).position,
//					Input.GetTouch (1).position
//				};
//				Vector2 newTouchVector = newTouchPositions [0] - newTouchPositions [1];
//				float newTouchDistance = newTouchVector.magnitude;
//				Debug.Log ("newTouchDistance --------: " + newTouchDistance);
//
//			//	transform.position += transform.TransformDirection((Vector3)((oldTouchPositions[0] + oldTouchPositions[1] - screen) * GetComponent<Camera>().orthographicSize / screen.y));
////				transform.localRotation *= Quaternion.Euler (new Vector3 (0, 0, Mathf.Asin (Mathf.Clamp ((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f)) / 0.0174532924f));
//				transform.localRotation *= Quaternion.Euler (new Vector3 (0, Mathf.Asin (Mathf.Clamp ((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f)) / 0.0174532924f, 0));
//				//	GetComponent<Camera>().orthographicSize *= oldTouchDistance / newTouchDistance;
//				//transform.position -= transform.TransformDirection((newTouchPositions[0] + newTouchPositions[1] - screen) * GetComponent<Camera>().orthographicSize / screen.y);
//
//				oldTouchPositions [0] = newTouchPositions [0];
//				oldTouchPositions [1] = newTouchPositions [1];
//				oldTouchVector = newTouchVector;
//				oldTouchDistance = newTouchDistance;
//			}
//		}

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
			//Debug.Log ("khong zoom out" + zoomFactor);
		} else if (flagCameraExitZoom == 1 & flagCameraZoom == 0 & zoomFactor < 0) {
			//Debug.Log ("khong zoom in" + zoomFactor);
		} else {
			Camera.main.transform.Translate(Vector3.back * zoomFactor * zoomSpeed * Time.deltaTime);
		}

	}

	void DragCamera(Vector2 deltaPosition)
	{
		_plane = GameObject.Find ("Plane");

		if (_plane == null) {
			return;
		}

		Vector3 _planceScare = _plane.transform.localScale;
		Renderer planeMesh = _plane.GetComponent<Renderer>();
		Bounds bounds = planeMesh.bounds;

		float cameraFieldofview = Camera.main.fieldOfView;
		float cameraRotationX = Camera.main.transform.eulerAngles.x;
		float cachedFieldofview  = cameraFieldofview / 2;	// ung voi cachedRotationCameraX = 90

		float deltaCameraRotationX = 90 - cameraRotationX;

		conorA1 = cachedFieldofview - deltaCameraRotationX;
		conorA2 = cameraFieldofview - conorA1;

//		Debug.Log ("conorA1:" + conorA1);
//		Debug.Log ("conorA2:" + conorA2);
//		Debug.Log ("cameraRotationX:" + cameraRotationX);
//		Debug.Log ("deltaCameraRotationX:" + deltaCameraRotationX);
//		Debug.Log ("cameraFieldofview:" + cameraFieldofview);
//		Debug.Log ("position.y:" + Camera.main.transform.position.y);
		float limitCameraZ1 = (float)Camera.main.transform.position.y * Mathf.Tan ((conorA1)*Mathf.PI/180);
		float limitCameraZ2 = (float)Camera.main.transform.position.y * Mathf.Tan ((conorA2)*Mathf.PI/180);
//		Debug.Log ("deafaultMaxCameraZ:" + limitCameraZ1);
		float limitCameraX1 = limitCameraZ1 * Screen.width / Screen.height;	// + bb
		float limitCameraX2 = limitCameraZ2 * Screen.width / Screen.height;	// - aa
		float cc = (limitCameraX1 + limitCameraX2)/2 + 0.1f;
		limitCameraX1 = cc;
		limitCameraX2 = cc;

		float a = Camera.main.transform.position.y / limitCameraX1;	//ty le toc do a zoom tuong ung voi quang duong zoom Y anh huong toc do di chuyen quang duong X, Z di chuyen dc
		float b = Camera.main.transform.position.y / limitCameraX2;

		float aX = Mathf.Clamp ((deltaPosition.x * a * dragSpeed * Time.deltaTime) + cachedTransform.position.x,
			-bounds.size.x, bounds.size.x);
		float aZ = Mathf.Clamp ((deltaPosition.y * b * dragSpeed* Time.deltaTime) + cachedTransform.position.z,-bounds.size.z, bounds.size.z);

		if (flagCameraExitDrag == 1 & flagCameraDrag == 0 & (aX > 0 || aZ > 0)) {
			//Debug.Log ("khong move x+ :" + aX + "khong move Z+ :" + aZ);
		} else if (flagCameraExitDrag == 1 & flagCameraDrag == 0 & (aX < 0 || aZ < 0)) {
			//Debug.Log ("khong move x- :" + aX + "khong move Z- :" + aZ);
		} else {
			cachedTransform.position = new Vector3 (aX, cachedTransform.position.y, aZ);

		}
	}
	Vector3 lastCameraPos;
	void OnTriggerExit(Collider other) {	// OnTriggerEnter su kien se xay ra khi doi tuong xay ra va cham voi doi tuong khac
		// tich Is trigger trong Box Collier cua doi tuong Pick Up de kich hoat nhan su kien OnTriggerEnter
		if(other.gameObject.CompareTag("BackGround"))	// xac dinh doi tuong va cham la doi tuong Pick Up
		{
			if (Input.touchCount == 1) {
				flagCameraExitDrag = 1;
			} else {
				flagCameraExitZoom = 1;

			}


		}
	}


	void OnTriggerStay(Collider other) {	// OnTriggerEnter su kien se xay ra khi doi tuong xay ra va cham voi doi tuong khac
		// tich Is trigger trong Box Collier cua doi tuong Pick Up de kich hoat nhan su kien OnTriggerEnter
		//Debug.Log ("Stop zoom in camera OnTriggerStay" );
		lastCameraPos = cachedTransform.position;
			flagCameraExitDrag = 0;
			flagCameraExitZoom = 0;
	}
		
}
