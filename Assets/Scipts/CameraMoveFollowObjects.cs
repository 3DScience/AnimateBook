using UnityEngine;
using System.Collections;

public class CameraMoveFollowObjects : MonoBehaviour {

	public GameObject player;

	private GameObject cam;
	private Vector3 offset;

	void Start () {
		cam = GameObject.Find ("CameraFollowObjects");
		cam.GetComponent<Camera> ().enabled = true;
		offset = transform.position - player.transform.position;
	}

	void LateUpdate () {
		transform.position = player.transform.position + offset;
	}
}
