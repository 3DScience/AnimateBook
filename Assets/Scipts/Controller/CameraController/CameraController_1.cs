using UnityEngine;
using System.Collections;

public class CameraController_1 : MonoBehaviour,TouchEventInterface {

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
//	private Vector3 origRot;
//	private Touch initTouch = new Touch();

	public float rotSpeed = 0.2f;
	public float dir =-1;
	public int currTouch = 0;

	Vector2?[] oldTouchPositions = {
		null,
		null
	};
	Vector2 oldTouchVector;
	float oldTouchDistance;

	void Start() {
		cachedTransform = transform;
		startingPos = cachedTransform.position;
//		origRot = transform.eulerAngles;
	}

	public void OnTouchs()
	{
		for (int i = 0; i < Input.touchCount; i++) {
			Debug.Log (i); // kiem tra xem co may touch dang cham vao man hinh 
			currTouch = i;
			Debug.Log ("OnTouchsOnTouchsOnTouchs");
			if (Input.GetTouch (i).phase == TouchPhase.Began) {	// khi bat dau cham vao man hinh
				OnTouchBeganAnyWhere ();
			}
			if (Input.GetTouch (i).phase == TouchPhase.Ended) {	// khi ket thuc cham vao man hinh
				OnTouchEndedAnyWhere ();
			}
			if (Input.GetTouch (i).phase == TouchPhase.Moved) {	// khi cham vao man hinh va di chuyen
				OnTouchMoveAnyWhere ();
			}
			if (Input.GetTouch (i).phase == TouchPhase.Stationary) {	// khi cham vao man hinh am ko di chuyen
				OnTouchStayAnyWhere ();
			}
		}
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
		} else if (Input.touchCount >= 3) {
			RotationCamera (); // camera rotaion round its
			// camera rotaion round object
			//transform.RotateAround(Vector3.zero, Vector3.down, 5 * Time.deltaTime);
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

		if (oldTouchPositions[1] == null) {
			oldTouchPositions[0] = Input.GetTouch(0).position;
			oldTouchPositions[1] = Input.GetTouch(1).position;
			oldTouchVector = (Vector2)(oldTouchPositions[0] - oldTouchPositions[1]);
			oldTouchDistance = oldTouchVector.magnitude;
		}
		else {
				//Vector2 screen = new Vector2(GetComponent<Camera>().pixelWidth, GetComponent<Camera>().pixelHeight);

			Vector2[] newTouchPositions = {
				Input.GetTouch(0).position,
				Input.GetTouch(1).position
			};
			Vector2 newTouchVector = newTouchPositions[0] - newTouchPositions[1];
			float newTouchDistance = newTouchVector.magnitude;

			//transform.position += transform.TransformDirection((Vector3)((oldTouchPositions[0] + oldTouchPositions[1] - screen) * GetComponent<Camera>().orthographicSize / screen.y));
			//transform.localRotation *= Quaternion.Euler(new Vector3(0, Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f)) / 0.0174532924f, 0));
			//GetComponent<Camera>().orthographicSize *= oldTouchDistance / newTouchDistance;
			//transform.position -= transform.TransformDirection((newTouchPositions[0] + newTouchPositions[1] - screen) * GetComponent<Camera>().orthographicSize / screen.y);
			transform.RotateAround(Vector3.zero, new Vector3(0, Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -2f, 2f)) / 0.0174532924f, 0), 50 * Time.deltaTime);

//			Debug.Log ("transform.localRotation 1 : " + transform.localRotation);
//			Debug.Log ("transform.localRotation 2 : " + (oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y));

			oldTouchPositions[0] = newTouchPositions[0];
			oldTouchPositions[1] = newTouchPositions[1];
			oldTouchVector = newTouchVector;
			oldTouchDistance = newTouchDistance;
		}


	}
		
	void ZoomCamera ()
	{
		switch (currTouch) 
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
		if (currTouch >= 1) {
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

		if (flagCameraExitZoom == 1 & flagCameraZoom == 0 & zoomFactor > 0) {
		} else if (flagCameraExitZoom == 1 & flagCameraZoom == 0 & zoomFactor < 0) {
		} else {
			Camera.main.transform.Translate(Vector3.back * zoomFactor * zoomSpeed * Time.deltaTime);
		}

		//RotationCamera ();

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

		float aX = Mathf.Clamp ((deltaPosition.x * dragSpeed * Time.deltaTime) + cachedTransform.position.x,
			-bounds.size.x, bounds.size.x);
		float aY = Mathf.Clamp ((deltaPosition.y * dragSpeed* Time.deltaTime) + cachedTransform.position.y,-bounds.size.y, bounds.size.y);

		if (flagCameraExitDrag == 1 & flagCameraDrag == 0 & (aX > 0 || aY > 0)) {
			//Debug.Log ("khong move x+ :" + aX + "khong move Z+ :" + aZ);
		} else if (flagCameraExitDrag == 1 & flagCameraDrag == 0 & (aX < 0 || aY < 0)) {
			//Debug.Log ("khong move x- :" + aX + "khong move Z- :" + aZ);
		} else {
			//cachedTransform.position = new Vector3 (aX, cachedTransform.position.y, aZ);
			cachedTransform.position = new Vector3 (aX, aY, cachedTransform.position.z);

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
