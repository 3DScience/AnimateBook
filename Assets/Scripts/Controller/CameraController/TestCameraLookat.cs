using UnityEngine;
using System.Collections;

public class TestCameraLookat : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.transform.LookAt(GameObject.Find("sun 1").transform);
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}
