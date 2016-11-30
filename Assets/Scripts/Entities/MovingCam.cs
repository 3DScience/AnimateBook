using UnityEngine;
using System.Collections;

public class MovingCam : MonoBehaviour {
	
	public Transform startMarker;
	public Transform endMarker;
	public float speed = 1.0F;
	private float startTime;
	private float journeyLength;
	public Transform from;
	public Transform to;
	public float rotateSpeed = 0.1F;
	private bool flagMovingCam = false;


	void Start() {
		startTime = Time.time;
		journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
	}

	void Update() {

		if (flagMovingCam == false) {
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
			Rotate ();

			float distance = Vector3.Distance(transform.position, endMarker.position);
			if (distance <= 0)
				flagMovingCam = true;
			//	canvas.gameObject.SetActive(true);
		}
	}

	void Rotate() {
		transform.rotation = Quaternion.Slerp(from.rotation, to.rotation, Time.time * rotateSpeed);
	}
}
