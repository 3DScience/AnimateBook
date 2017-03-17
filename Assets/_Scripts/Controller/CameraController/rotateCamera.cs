using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateCamera : MonoBehaviour {
    private float speed = 5;
    public Transform target;
    private bool pause = false;
    private float lastTouch = 0;
    public float timeOut=5;
    // Use this for initialization
    void Start () {
		
	}

    void Update()
    {

        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            if ( Input.GetMouseButtonDown(0) || Input.GetTouch(0).phase == TouchPhase.Began )
            {
                pause = true;
                lastTouch = Time.time;
      
            }
        }else
        {
            if(pause==true && Time.time-lastTouch > timeOut)
            {
                pause = false;
            }
        }
        if (pause)
            return;
        transform.RotateAround(target.transform.position, Vector3.up, Time.deltaTime * speed);
    }
}
