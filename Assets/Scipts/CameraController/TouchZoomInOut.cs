using UnityEngine;
using System.Collections;

public class TouchZoomInOut : TouchLogic {

	public float zoomSpeed = 5.0f;
	public float currZoomY = 5f; 
	public float currZoomYMin = 0f;
	public float currZoomYMax = 0f;
	public float currDist = 0.0f,
	lastDist = 0.0f,
	zoomFactor = 0.0f;
	public float _scare =0.0f;

	// Xac dinh vi tri cua touch1 va touch 2;
	private Vector2 currTouch1 = Vector2.zero,
	lastTouch1 = Vector2.zero,
	currTouch2 = Vector2.zero,
	lastTouch2 = Vector2.zero;

	private GameObject _plane;
	private Transform cachedTransform;
	private Vector3 startingPos;

	void Start() {
		cachedTransform = transform;
		startingPos = cachedTransform.position;
	}


	void OnTouchMoveAnyWhere()
	{
		Zoom ();
	}

	void OnTouchStayAnyWhere()
	{
		Zoom ();
	}

	void Zoom_1 ()
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
		zoomFactor =  Mathf.Clamp(lastDist - currDist, -30f, 30f);
		currZoomYMin = 4;
		currZoomYMax = 0;

		_plane = GameObject.Find ("Plane");
		Vector3 _planceScare = _plane.transform.localScale;
		if (_planceScare.x == _planceScare.z) { 
			currZoomYMax = _planceScare.x * 10; 
		} else if (_planceScare.x != _planceScare.z & _planceScare.x == 1) {
			currZoomYMax = 10;
		} else if (_planceScare.x != _planceScare.z & _planceScare.x > 1) {
			currZoomYMax = 17;
		}

//		if (_planceScare.x >= _planceScare.z) { 
//			currZoomYMax = _planceScare.z * 10; 
//		} else
//			currZoomYMax = _planceScare.x * 10;

		// apply zoom to our camera

		if (transform.position.y > currZoomYMax) {
			transform.position = new Vector3 (transform.position.x, currZoomYMax, transform.position.z);
		}
		else if (transform.position.y < 4) {
			transform.position = new Vector3(transform.position.x,4,transform.position.z);
		} else {
			Camera.main.transform.Translate(Vector3.back * zoomFactor * zoomSpeed * Time.deltaTime);
		}
	}


	void Zoom ()
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
		zoomFactor =  Mathf.Clamp(lastDist - currDist, -5f, 5f);
		currZoomYMin = 4;
		currZoomYMax = 0;

		_plane = GameObject.Find ("Plane");
		Vector3 _planceScare = _plane.transform.localScale;



		Renderer planeMesh = _plane.GetComponent<Renderer>();
		Bounds bounds = planeMesh.bounds;

		float conorB = Camera.main.fieldOfView;
		float conorA = ((conorB) * Screen.width) / Screen.height;

		Debug.Log ("conorA A:" + conorA);
		Debug.Log ("conorB B:" + conorB);

		Debug.Log ("position.y:" + Camera.main.transform.position.y);
		Debug.Log ("Tan (conorA / 2):" + Mathf.Tan ((conorA / 2)*Mathf.PI/180));
		float deltaCameraZ = (float)Camera.main.transform.position.y * Mathf.Tan ((conorB / 2)*Mathf.PI/180);
		Debug.Log ("deafaultMaxCameraZ:" + deltaCameraZ);
		float deltaCameraX = deltaCameraZ * Screen.width / Screen.height;

		float maxCameraX = bounds.size.x - deltaCameraX;
		float maxCameraZ = bounds.size.z - deltaCameraZ;




		if (_planceScare.x >= _planceScare.z) { 
			//currZoomYMax = _planceScare.x * 10; 
			currZoomYMax =maxCameraZ;
		} else if (_planceScare.x < _planceScare.z) {
			//currZoomYMax = _planceScare.z * 10;
			currZoomYMax =maxCameraX;
		}
			
		if (transform.position.y > currZoomYMax) {
			transform.position = new Vector3 (transform.position.x, currZoomYMax, transform.position.z);
		}
		else if (transform.position.y < 4) {
			transform.position = new Vector3(transform.position.x,4,transform.position.z);
		} else {
			Camera.main.transform.Translate(Vector3.back * zoomFactor * zoomSpeed * Time.deltaTime);
		}
	}
		
}
