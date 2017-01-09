using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode {
	Camera3D,
	Camera2D
}

public class SwitchCameraMode : MonoBehaviour {

	public GameObject mainCam;
	public GameObject pos2D;
	public GameObject pos3D;

	public float speed = 0.02f;

	private CameraMode _camMode;

	void Update () {

		if (_camMode == CameraMode.Camera3D) {
			
			mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, pos3D.transform.rotation, Time.time * speed);
			mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, pos3D.transform.position, Time.time * speed);

		} else {
			mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, pos2D.transform.rotation, Time.time * speed);
			mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, pos2D.transform.position, Time.time * speed);
		}

	}

	public void switchCameraMode (CameraMode camMode) {
		_camMode = camMode;
	}

	public void switchCameraMode () {
		if (_camMode == CameraMode.Camera3D) {
			_camMode = CameraMode.Camera2D;

		} else {
			_camMode = CameraMode.Camera3D;
		}
	}
}
