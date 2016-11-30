using UnityEngine;
using System.Collections;

public class MovingCam : MonoBehaviour {
	

	public Transform startMarker;
	public Transform endMarker;
    public System.Action onMoveCameraEnd;
	public float speed = 100f;
	private float startTime;
	private float journeyLength;
	public float rotateSpeed = 0.1F;
	private bool flagMovingCam = false;
    private Vector3 startPos;
    private Vector3 targetPos;
    Quaternion rotateStart;
    Quaternion rotateTarget;
    void Start() {
		startTime = Time.time;
		journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
        startPos = new Vector3(startMarker.position.x, startMarker.position.y, startMarker.position.z);
        targetPos = new Vector3(endMarker.position.x, endMarker.position.y, endMarker.position.z);
        rotateStart = startMarker.rotation;
        rotateTarget= endMarker.rotation;
    }

	void Update() {

		if (flagMovingCam == false) {
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp(startPos, targetPos, fracJourney);
			Rotate ();

			float distance = Vector3.Distance(transform.position, targetPos);
			if (distance <= 0)
				flagMovingCam = true;
            //	canvas.gameObject.SetActive(true);
        }else
        {
            if(onMoveCameraEnd != null) {
                onMoveCameraEnd();
            }
            Destroy(this);
        }
	}

	void Rotate() {
		transform.rotation = Quaternion.Slerp(rotateStart, rotateTarget, Time.time * rotateSpeed);
	}
    void OnDestroy()
    {
        if (Debug.isDebugBuild)
            Debug.Log("[MovingCam] Script was destroyed <====================");

    }
}
