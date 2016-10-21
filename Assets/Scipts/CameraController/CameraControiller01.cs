using UnityEngine;
using System.Collections;

public class CameraControiller01 : TouchLogic {

	public float dragSpeed = 1.0f;
	//public float currDist = 0.0f;
	public float cameraY = 0.0f;
	public float deltaMaxCameraX1 = 0f;	// gioi han truc x max cua cameraY khi di chuyen +;
	public float deltaMaxCameraX2 = 0f;	// gioi han truc x max cua cameraY khi di chuyen +;
	public float deltaMaxCameraZ1 = 0f;
	public float deltaMaxCameraZ2 = 0f;
	public float conorA1 = 0;
	public float conorA2 = 0;
	public Vector2 ObjectPostionSart = Vector2.zero;
	public Vector2 currDistVector = Vector2.zero;

	private float limitCameraX1 = 0f;	// gioi han tam nhin cua camrera theo truc x +
	private float limitCameraX2 = 0f;	// gioi han tam nhin cua camrera theo truc x -
	private float limitCameraZ1 = 0f;
	private float limitCameraZ2 = 0f;
	private float cachedRotationCameraX = 90f; 
	private float deltaCameraRotationX = 0f;

	private GameObject _plane;
	private Transform cachedTransform;
	private Vector3 startingPos;

	public float zoomSpeed = 2.0f;
	public float currZoomY = 5f; 
	public float currZoomYMin = 1f;
	public float currZoomYMax = 10f;
	public float currDist = 0.0f,
	lastDist = 0.0f,
	zoomFactor = 0.0f;
	public float _scare =0.0f;

	// Xac dinh vi tri cua touch1 va touch 2;
	private Vector2 currTouch1 = Vector2.zero,
	lastTouch1 = Vector2.zero,
	currTouch2 = Vector2.zero,
	lastTouch2 = Vector2.zero;

	void Start() {
		cachedTransform = transform;
		startingPos = cachedTransform.position;
	}

	void OnTouchBeganAnyWhere() {
		Debug.Log ("FUCK 0");
		ZoomCamera ();
	}

	void OnTouchMoveAnyWhere()
	{
		Debug.Log ("FUCK 1");
		ZoomCamera ();
		if (Input.touchCount == 1) {
			Vector2 _deltaPosition = -Input.GetTouch (0).deltaPosition; // deltaPosition khoang cach giua vi tri cuoi cung va vi tri gan day nhat

			switch (Input.GetTouch (0).phase) {
			case TouchPhase.Began:
				//ObjectPostionSart = - Input.GetTouch (0).position;
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
		Debug.Log ("FUCK 2");
		ZoomCamera ();
	}

	void OnTouchEndedAnyWhere () {
		Debug.Log ("FUCK 3");
		ZoomCamera ();
	}

	void OnTouchBegan3D() {
		Debug.Log ("FUCK 4");
		ZoomCamera ();
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

		currZoomY = transform.position.y;
		zoomFactor =  Mathf.Clamp(lastDist - currDist, -100f, 100f);

		_plane = GameObject.Find ("Plane");
		Vector3 _planceScare = _plane.transform.localScale;

		if (transform.position.y > currZoomYMax) {
			transform.position = new Vector3 (transform.position.x, currZoomYMax, -5);
		}
		else if (transform.position.y < currZoomYMin) {
			transform.position = new Vector3(transform.position.x,currZoomYMin,-2);
		} else {
			Camera.main.transform.Translate(Vector3.back * zoomFactor * zoomSpeed * Time.deltaTime);
		}
	}

	void DragCamera(Vector2 deltaPosition)
	{
		//deltaA = deltaPosition.y;
		Debug.Log ("Camera Y:" + Camera.main.transform.position.y);

		_plane = GameObject.Find ("Plane");
		Vector3 _planceScare = _plane.transform.localScale;
		Renderer planeMesh = _plane.GetComponent<Renderer>();
		Bounds bounds = planeMesh.bounds;

		float cameraFieldofview = Camera.main.fieldOfView;
		float cameraRotationX = Camera.main.transform.eulerAngles.x;
		float cachedFieldofview  = cameraFieldofview / 2;	// ung voi cachedRotationCameraX = 90
		if (cameraRotationX > 90) {
			deltaCameraRotationX = cachedRotationCameraX + cameraRotationX;
		} else {
			deltaCameraRotationX = cachedRotationCameraX - cameraRotationX;
		}
		conorA1 = cachedFieldofview - deltaCameraRotationX;
		conorA2 = cameraFieldofview - conorA1;

		Debug.Log ("conorA1:" + conorA1);
		Debug.Log ("conorA2:" + conorA2);
		Debug.Log ("cameraRotationX:" + cameraRotationX);
		Debug.Log ("deltaCameraRotationX:" + deltaCameraRotationX);
		Debug.Log ("cameraFieldofview:" + cameraFieldofview);
		Debug.Log ("position.y:" + Camera.main.transform.position.y);
		Debug.Log ("deafaultMaxCameraX:" + limitCameraX1);
		limitCameraZ1 = (float)Camera.main.transform.position.y * Mathf.Tan ((conorA1)*Mathf.PI/180);
		limitCameraZ2 = (float)Camera.main.transform.position.y * Mathf.Tan ((conorA2)*Mathf.PI/180);
		Debug.Log ("deafaultMaxCameraZ:" + limitCameraZ1);
		limitCameraX1 = limitCameraZ1 * Screen.width / Screen.height;	// + bb
		limitCameraX2 = limitCameraZ2 * Screen.width / Screen.height;	// - aa
		float cc = (limitCameraX1 + limitCameraX2)/2 + 0.1f;
		limitCameraX1 = cc;
		limitCameraX2 = cc;

		deltaMaxCameraX1 = bounds.size.x / 2 - limitCameraX1;
		deltaMaxCameraX2 = bounds.size.x / 2 - limitCameraX2;
		deltaMaxCameraZ1 = bounds.size.z / 2 - limitCameraZ1;
		deltaMaxCameraZ2 = bounds.size.z / 2 - limitCameraZ2;

//		deltaMaxCameraX1 = bounds.size.x / 3;
//		deltaMaxCameraX2 = bounds.size.x / 3;
//		deltaMaxCameraZ1 = bounds.size.z / 3;
//		deltaMaxCameraZ2 = bounds.size.z / 3;

		float a = Camera.main.transform.position.y / (_planceScare.x * 10);	//ty le toc do a zoom tuong ung voi quang duong zoom Y anh huong toc do di chuyen quang duong X, Z di chuyen dc
		float b = Camera.main.transform.position.y / (_planceScare.z * 10);

		float aX = Mathf.Clamp ((deltaPosition.x * a * dragSpeed * Time.deltaTime) + cachedTransform.position.x,
			startingPos.x - deltaMaxCameraX1, startingPos.x + deltaMaxCameraX2);
		float aZ = Mathf.Clamp ((deltaPosition.y * b * dragSpeed* Time.deltaTime) + cachedTransform.position.z,startingPos.z - deltaMaxCameraZ1, startingPos.z + deltaMaxCameraZ2);

//		float aX = Mathf.Clamp ((deltaPosition.x * a * dragSpeed * Time.deltaTime) + cachedTransform.position.x,
//			-2.5f, 2.5f);
//		float aZ = Mathf.Clamp ((deltaPosition.y * b * dragSpeed* Time.deltaTime) + cachedTransform.position.z,-10f, 0f);
//
		cachedTransform.position = new Vector3(aX,cachedTransform.position.y,aZ);
	}
}
