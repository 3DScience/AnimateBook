using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* this class is to do some behaviors of camera */
public enum CameraMode {
	Camera3D,
	Camera2D
}

public class CameraFunctions : MonoBehaviour {

	//flags to activate functions
	public bool switchModeFunctionFlag = false;
	public bool rotateFunctionFlag = false;

	// rotation camera
	public Transform lookAtTarget;
	public float X_ANGLE_MIN = -20;
	public float X_ANGLE_MAX = 20;
	public float Y_ANGLE_MIN = -10;
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
	public GameObject mainCam;
	public GameObject pos2D;
	public GameObject pos3D;

	public float speed = 5.0f;

	private CameraMode _camMode;
	private bool isMoving;

	// Use this for initialization
	void Start () {
		allowRotate = false;

		isMoving = true;
		_camMode = CameraMode.Camera3D;

		if (rotateFunctionFlag == true && lookAtTarget != null) {
			transform.LookAt (lookAtTarget.position);
		}
	}

	// Update is called once per frame
	void Update () {

		//rotate camera
		if (rotateFunctionFlag == true) {
			if (isMoving == false && _camMode == CameraMode.Camera3D) {	//camera can only rotate if it is not moving and camera mode is 3D
				if (Input.GetMouseButtonDown(0)) {
					allowRotate = true;
					firstPos = Input.mousePosition;
				}

				if (Input.GetMouseButtonUp(0)) {
					allowRotate = false;
				}

				if (allowRotate == true) {
					Debug.Log("allowRotate :: " + allowRotate);
					Vector2 delta = Input.mousePosition - firstPos;
					Debug.Log("delta :: " + delta);

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

		//switch mode 2D - 3D
		if (switchModeFunctionFlag == true) {
			if (_camMode == CameraMode.Camera3D) {

				if (isMoving && (mainCam.transform.position != pos3D.transform.position ||
					mainCam.transform.rotation != pos3D.transform.rotation)) {

					mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, pos3D.transform.rotation, Time.deltaTime * speed);
					mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, pos3D.transform.position, Time.deltaTime * speed);

				} else {
					isMoving = false;
				}

			} else {

				if (isMoving && (mainCam.transform.position != pos2D.transform.position ||
					mainCam.transform.rotation != pos2D.transform.rotation)) {

					mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, pos2D.transform.rotation, Time.deltaTime * speed);
					mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, pos2D.transform.position, Time.deltaTime * speed);

				} else {
					isMoving = false;
				}
			}
		}
	}

	public void switchCameraMode (CameraMode camMode) {
		_camMode = camMode;
		isMoving = true;
		allowRotate = false;

		if (_camMode == CameraMode.Camera3D) {
			transform.LookAt (lookAtTarget.position);
		}
	}

	public void switchCameraMode () {
		isMoving = true;
		allowRotate = false;

		if (_camMode == CameraMode.Camera3D) {
			_camMode = CameraMode.Camera2D;

		} else {
			_camMode = CameraMode.Camera3D;
			transform.LookAt (lookAtTarget.position);
		}
	}

}
