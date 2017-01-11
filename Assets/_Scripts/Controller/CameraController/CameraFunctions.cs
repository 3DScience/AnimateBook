using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* this class is to do some behaviors of camera */

public enum CameraMode {
	Camera3D,
	Camera2D
}

public class CameraFunctions : MonoBehaviour {

	public Camera mainCam;
	public float defaulFieldOfView = 21.0f;

	//flags to activate functions
	public bool switchModeFunctionFlag = false;
	public bool rotateFunctionFlag = false;
	public bool zoomFunctionFlag = false;

	// rotation camera
	public Transform lookAtTarget;
	public float X_ANGLE_MIN = -20;
	public float X_ANGLE_MAX = 20;
	public float Y_ANGLE_MIN = -15;
	public float Y_ANGLE_MAX = 20;

	private float panResistanceX = 5.0f;
	private float panResistanceY = 5.0f;

	private float distance = 1000.0f;
	private float currentX = 0.0f;
	private float currentY = 0.0f;

	private float sensitiveX = 0.2f;
	private float sensitiveY = 0.2f;

	private bool allowRotate;

	private Vector3 firstPos;	//posistion of finger when touch on screen

	//switch mode 2D - 3D
	public GameObject pos2D;
	public GameObject pos3D;

	private float lerpSpeed = 5.0f;


	//zoom
	private float perspectiveZoomSpeed = 0.2f;        // The rate of change of the field of view in perspective mode.
	private float orthoZoomSpeed = 0.1f;        // The rate of change of the orthographic size in orthographic mode.

	int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
	float camRayLength = 100f;          // The length of the ray from the camera into the scene.


	//private variable
	private CameraMode _camMode;
	private bool isMoving;
	private bool isZooming;

	void Awake ()
	{
		// Create a layer mask for the floor layer.
		floorMask = LayerMask.GetMask ("Floor");
	}

	// Use this for initialization
	void Start () {
		allowRotate = false;

		isMoving = true;
		isZooming = false;
		_camMode = CameraMode.Camera3D;
		mainCam.fieldOfView = defaulFieldOfView;

		if (rotateFunctionFlag == true && lookAtTarget != null) {
			transform.LookAt (lookAtTarget.position);
		}
	}

	// Update is called once per frame
	void Update () {

		//rotate camera
		_rotateCameraUpate();

		//switch mode 2D - 3D
		_switchCameraModeUpate();

		//zoom
		_zoomCameraUpdateTouch();


	}

	/* private functions */
	private void _rotateCameraUpate () {
		if (rotateFunctionFlag == true) {
			if (isMoving == false && _camMode == CameraMode.Camera3D &&			//camera can only rotate if it is not moving and camera mode is 3D
				isZooming == false) {	//do not rotate while zooming
				
				if (Input.GetMouseButtonDown(0)) {
					allowRotate = true;
					firstPos = Input.mousePosition;
				}

				if (Input.GetMouseButtonUp(0)) {
					allowRotate = false;
				}

				if (allowRotate == true) {
					Vector2 delta = Input.mousePosition - firstPos;

					if (Mathf.Abs (delta.x) > panResistanceX || Mathf.Abs (delta.y) > panResistanceY) {
						allowRotate = true;

						Vector3 offset = transform.position - lookAtTarget.position;
						distance = offset.magnitude;

						currentX += delta.x * sensitiveX;
						currentY -= delta.y * sensitiveY;

						currentY = Mathf.Clamp (currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);

						currentX = Mathf.Clamp (currentX, X_ANGLE_MIN, X_ANGLE_MAX);

						Vector3 dir = new Vector3 (0,0, -distance);
						Quaternion rotation = Quaternion.Euler (currentY, currentX, 0);
						transform.position = lookAtTarget.position + rotation*dir;
						transform.LookAt (lookAtTarget.position);

						firstPos = Input.mousePosition;
					}

				}
			}	
		}
	}

	private void _switchCameraModeUpate () {
		if (switchModeFunctionFlag == true) {

			if (_camMode == CameraMode.Camera3D) {

				if (isMoving && (mainCam.transform.position != pos3D.transform.position ||
					mainCam.transform.rotation != pos3D.transform.rotation)) {

					mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, pos3D.transform.rotation, Time.deltaTime * lerpSpeed);
					mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, pos3D.transform.position, Time.deltaTime * lerpSpeed);

				} else {
					isMoving = false;
				}

			} else {

				if (isMoving && (mainCam.transform.position != pos2D.transform.position ||
					mainCam.transform.rotation != pos2D.transform.rotation)) {

					mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, pos2D.transform.rotation, Time.deltaTime * lerpSpeed);
					mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, pos2D.transform.position, Time.deltaTime * lerpSpeed);

				} else {
					isMoving = false;
				}
			}
		}
	}

	private void _zoomCameraUpdateTouch () {
		if (zoomFunctionFlag == true &&
			
			isMoving == false && _camMode == CameraMode.Camera3D) {		//camera can only zoom if it is not moving and camera mode is 3D

			// If there are two touches on the device...
			if (Input.touchCount == 2) {
				
				isZooming = true;
				// Store both touches.
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);

				// Find the position in the previous frame of each touch.
				Vector2  touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				//zoom
				// Find the magnitude of the vector (the distance) between the touches in each frame.
				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

				// Find the difference in the distances between each frame.
				float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

				// If the camera is orthographic...
				if (mainCam.orthographic) {
					// ... change the orthographic size based on the change in distance between the touches.
					float size = mainCam.orthographicSize + deltaMagnitudeDiff * orthoZoomSpeed;

					// Make sure the orthographic size never drops below zero.
					mainCam.orthographicSize = Mathf.Max(size, 0.1f);

				} else {
					// Otherwise change the field of view based on the change in distance between the touches.
					float fOv = mainCam.fieldOfView + deltaMagnitudeDiff * perspectiveZoomSpeed;
					
					// Clamp the field of view to make sure it's between 0 and 180.
					mainCam.fieldOfView = Mathf.Clamp(fOv, 5.0f, 30.0f);
				}

				//back to default position after zooming
//				if (touchZero.phase == TouchPhase.Ended ||
//					touchOne.phase == TouchPhase.Ended) {
//
//					switchCameraMode(_camMode);
//				}

			} else {
				isZooming = false;
			}
		}
	}

	/* public functions */
	public void switchCameraMode (CameraMode camMode) {
		_camMode = camMode;
		isMoving = true;
		allowRotate = false;
		mainCam.fieldOfView = defaulFieldOfView;

		if (_camMode == CameraMode.Camera3D) {
			transform.LookAt (lookAtTarget.position);
		}
	}

	public void switchCameraMode () {
		isMoving = true;
		allowRotate = false;
		mainCam.fieldOfView = defaulFieldOfView;

		if (_camMode == CameraMode.Camera3D) {
			_camMode = CameraMode.Camera2D;

		} else {
			_camMode = CameraMode.Camera3D;
			transform.LookAt (lookAtTarget.position);
		}
	}

}
