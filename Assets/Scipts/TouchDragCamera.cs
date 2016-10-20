using UnityEngine;
using System.Collections;

public class TouchDragCamera : MonoBehaviour {

	public float dragSpeed = 1.0f;
	public float currDist = 0.0f,
	cameraY = 0.0f;
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

	Vector2?[] oldTouchPositions = {
		null,
		null
	};
	Vector2 oldTouchVector;
	float oldTouchDistance;

	public float mincameraSize=20;
	public float maxcameraSize=100;
	public float curentCameraSize ;
	public float cameraConnerLimit;
	public float limitCamera;


	void Start() {
		cachedTransform = transform;
		startingPos = cachedTransform.position;
	}

	void OnTouchs () {
		Debug.Log ("FUCK FUCK FUCK FUYCK FUCKKKKK KK K K K KK");
		if (Input.touchCount == 0) {
			oldTouchPositions[0] = null;
			oldTouchPositions[1] = null;
		} else if (Input.touchCount == 1) {
			Vector2 _deltaPosition = - Input.GetTouch (0).deltaPosition; // deltaPosition khoang cach giua vi tri cuoi cung va vi tri gan day nhat

			switch (Input.GetTouch (0).phase) {
			case TouchPhase.Began:
				ObjectPostionSart = - Input.GetTouch (0).position;
				break;
			case TouchPhase.Moved:
				DragCamera(_deltaPosition);
				break;
			case TouchPhase.Ended:
				break;
			}
		} else if (Input.touchCount == 2) {
			if (oldTouchPositions [1] == null) {
				oldTouchPositions [0] = Input.GetTouch (0).position;
				oldTouchPositions [1] = Input.GetTouch (1).position;
				oldTouchVector = (Vector2)(oldTouchPositions [0] - oldTouchPositions [1]);
				oldTouchDistance = oldTouchVector.magnitude;
			} else {
				ZoomCamera ();
			}
		}
	}

	void ZoomCamera () {

		float cameraFieldofview = Camera.main.fieldOfView;

		_plane = GameObject.Find ("Plane");
		Vector3 _planceScare = _plane.transform.localScale;
		Renderer planeMesh = _plane.GetComponent<Renderer> ();
		Bounds bounds = planeMesh.bounds;

		// max conner camera when camera at the vecter3(0,0,0) with connor = 90;  success
		limitCamera = (bounds.size.x / Camera.main.aspect)/2;
		Debug.Log ("limitCamera: " + limitCamera);
		float tangA =limitCamera/(float)Camera.main.transform.position.y;	
		cameraConnerLimit = Mathf.Rad2Deg * Mathf.Atan (tangA);	// Doi ra goc tan(b) = a; => b = Mathf.Rad2Deg * Mathf.Atan (a)
		Debug.Log ("tangA: " + tangA);
		Debug.Log ("Field Of View: " + cameraConnerLimit);
		maxcameraSize = cameraConnerLimit*2;

		//Postion camera when camera at anywhere with connor = 90;
		if (Camera.main.transform.position.x < 0f) {
			limitCamera = limitCamera + (Camera.main.transform.position.x / Camera.main.aspect);
		} else {
			limitCamera = limitCamera - (Camera.main.transform.position.x / Camera.main.aspect);
		}

		Debug.Log ("limitCamera camera at anywhere: " + limitCamera);
		tangA =limitCamera/(float)Camera.main.transform.position.y;	
		cameraConnerLimit = Mathf.Rad2Deg * Mathf.Atan (tangA);	// Doi ra goc tan(b) = a; => b = Mathf.Rad2Deg * Mathf.Atan (a)
		Debug.Log ("tang anywhere: " + tangA);
		Debug.Log ("Field Of View anywhere: " + cameraConnerLimit);
		float c = cameraConnerLimit*2;
		maxcameraSize = c;
		Debug.Log ("maxcameraSize anywhere: " + maxcameraSize);

		// Postion camera when camera at anywhere with connor dynamic 60 -> 90
		float cachedFieldofview  = maxcameraSize;	// ung voi cachedRotationCameraX = 90
		deltaCameraRotationX = cachedRotationCameraX - Camera.main.transform.eulerAngles.x;
		Debug.Log ("deltaCameraRotationX FUCK 0: " + deltaCameraRotationX);

		maxcameraSize = cachedFieldofview - deltaCameraRotationX/2;
		Debug.Log ("maxcameraSize anywhere with connor dynamic: " + maxcameraSize);

		Vector2[] newTouchPositions = {
			Input.GetTouch(0).position,
			Input.GetTouch(1).position
		};
		Vector2 newTouchVector = newTouchPositions[0] - newTouchPositions[1];
		float newTouchDistance = newTouchVector.magnitude;

		// When camera drag to max Z 
		Debug.Log ("DMDMDMDMDMMD: " + deltaMaxCameraZ1);
		float rateZoom = oldTouchDistance / newTouchDistance;	//rateZoom > 1: zoomOut, //rateZoom < 1: zoomIn
		if (deltaMaxCameraZ1 <= Camera.main.transform.position.z & rateZoom > 1) {
			rateZoom = 1;
		}

		float cameraSize = GetComponent<Camera>().fieldOfView * rateZoom;
		if (cameraSize <= mincameraSize)
			cameraSize = mincameraSize;
		else if (cameraSize > maxcameraSize)
			cameraSize = maxcameraSize;

		GetComponent<Camera>().fieldOfView = cameraSize;
		curentCameraSize = cameraSize;

//		oldTouchPositions[0] = newTouchPositions[0];
//		oldTouchPositions[1] = newTouchPositions[1];
//		oldTouchVector = newTouchVector;
//		oldTouchDistance = newTouchDistance;
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

		float a = Camera.main.transform.position.y / (_planceScare.x * 10);	//ty le toc do a zoom tuong ung voi quang duong zoom Y anh huong toc do di chuyen quang duong X, Z di chuyen dc
		float b = Camera.main.transform.position.y / (_planceScare.z * 10);

		float aX = Mathf.Clamp ((deltaPosition.x * a * dragSpeed * Time.deltaTime) + cachedTransform.position.x,
			startingPos.x - deltaMaxCameraX1, startingPos.x + deltaMaxCameraX2);
		float aZ = Mathf.Clamp ((deltaPosition.y * b * dragSpeed* Time.deltaTime) + cachedTransform.position.z,startingPos.z - deltaMaxCameraZ1, startingPos.z + deltaMaxCameraZ2);

		cachedTransform.position = new Vector3(aX,cachedTransform.position.y,aZ);

	}
		
}
