using UnityEngine;
using System.Collections;

public class CameraController_1 : MonoBehaviour,TouchEventInterface {

	private float dragSpeed = 10.0f;
	private float cameraY = 0.0f;

	private float conorA1 = 0;
	private float conorA2 = 0;
	private float zoomSpeed = 150.0f;
	private float currDist = 0.0f,
	lastDist = 0.0f,
	zoomFactor = 0.0f;
	private float _scare =0.0f;

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

	public int currTouch = 0;

	// rotation camera
	public Transform lookAt;
	private const float Y_ANGLE_MIN = -80;
	private const float Y_ANGLE_MAX = 80;

	private float distance = 1000.0f;
	private float currentX = 0.0f;
	private float currentY = 0.0f;

	private float sensitiveX = 0.2f;
	private float sensitiveY = 0.2f;

	void Start() {
		cachedTransform = transform;
		startingPos = cachedTransform.position;
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
		if (flagCameraExitZoom == 1) {
			cachedTransform.position = lastCameraPos;
		}

		if (Input.touchCount == 1) {
			RotationCamera ();
		} else if (Input.touchCount == 2) {
			BoxCollider[] myColliders = gameObject.GetComponents<BoxCollider>();
			foreach(BoxCollider bc in myColliders) bc.enabled = true;
			ZoomCamera ();
		}
	}

	void OnTouchStayAnyWhere()
	{
		if (flagCameraExitZoom == 1) {
			cachedTransform.position = lastCameraPos;
		}
	}

	void OnTouchEndedAnyWhere () {
		if (flagCameraExitZoom == 1) {
			cachedTransform.position = lastCameraPos;
		}
	}
		
	void RotationCamera ()
	{	
		Vector3 offset = transform.position - lookAt.position;
		distance = offset.magnitude;

		if  (Input.touchCount == 1) {

			Touch touch = Input.GetTouch(0);
			if (Input.GetTouch(0).phase == TouchPhase.Moved) {
				Debug.Log("Start begin" + gameObject.transform.position);
				currentX += touch.deltaPosition.x * sensitiveX;
				currentY -= touch.deltaPosition.y * sensitiveY;

				currentY = Mathf.Clamp (currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);

				Vector3 dir = new Vector3 (0,0, -distance);
				Quaternion rotation = Quaternion.Euler (currentY, currentX, 0);
				cachedTransform.position = lookAt.position + rotation*dir;
				cachedTransform.LookAt (lookAt.position);

			}
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

	}
		

	Vector3 lastCameraPos;
	void OnTriggerExit(Collider other) {
		if(other.gameObject.CompareTag("BackGround"))	// xac dinh doi tuong va cham la doi tuong Pick Up
			Debug.Log("OnTriggerExit Camera");
		{
			if (Input.touchCount == 1) {
				flagCameraExitDrag = 1;
			} else {
				flagCameraExitZoom = 1;

			}
		}
	}


	void OnTriggerStay(Collider other) {
		lastCameraPos = cachedTransform.position;
			flagCameraExitDrag = 0;
			flagCameraExitZoom = 0;
	}
		
}
