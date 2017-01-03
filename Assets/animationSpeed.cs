using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationSpeed : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Animation>()["earth_moon"].speed = 0.1f;
       
        GetComponent<Animation>()[ "earth_moon"].time=Random.Range(1,1000);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
