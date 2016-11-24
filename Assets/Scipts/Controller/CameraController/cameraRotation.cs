using UnityEngine;
using System.Collections;

public class cameraRotation : MonoBehaviour {
	
	private const float Y_ANGLE_MIN = -90.0f;
	private const float Y_ANGLE_MAX = 90.0f;

	public Transform lookAt;
	public Transform camTransform;

	private Camera cam;

	private float distance = 1000.0f;
	private float currentX = 0.0f;
	private float currentY = 0.0f;

	private float sensitiveX = 1.0f;
	private float sensitiveY = 1.0f;

	// Use this for initialization
	void Start () {
		camTransform = transform;
		cam = Camera.main;
	}

	// Update is called once per frame
	void Update () {
		
		Vector3 offset = transform.position - lookAt.position;
		distance = offset.magnitude;

		if  (Input.touchCount == 1) {

			Touch touch = Input.GetTouch(0);
			if (Input.GetTouch(0).phase == TouchPhase.Moved) {
				Debug.Log("Start begin" + gameObject.transform.position);
				currentX += touch.deltaPosition.x;
				currentY -= touch.deltaPosition.y;

			//				currentY = Mathf.Clamp (currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);

				Vector3 dir = new Vector3 (0,0, -distance);
				Quaternion rotation = Quaternion.Euler (currentY, currentX, 0);
				camTransform.position = lookAt.position + rotation*dir;
				camTransform.LookAt (lookAt.position);

			}
		}
	}
}