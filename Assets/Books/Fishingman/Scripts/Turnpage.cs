using UnityEngine;
using System.Collections;


public class Turnpage : MonoBehaviour {

	public float zRotation = 0.00f;
	public float zRotation1 = -181.0f;
	public float zRotation2 = -180.0f;
	public float CurrentZ = 0.0f;
	public float RotationSpeed = 2.0f;


	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {






		


		//CurrentZ += Input.GetAxis("Horizontal");
	//	transform.eulerAngles = new Vector3(0f, 0f, CurrentZ);


		if (Input.GetKey (KeyCode.RightArrow))
		
			//transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRotation1);
			transform.Rotate(Vector3.forward * (RotationSpeed * Time.deltaTime));
			//transform.Rotate(Vector3. * Time.deltaTime, Space.World);

		


		if(Input.GetKey(KeyCode.LeftArrow))
		
//			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRotation);
			transform.Rotate(Vector3.back * (RotationSpeed * Time.deltaTime));

	
	}
	


}
