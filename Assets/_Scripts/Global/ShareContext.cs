using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareContext : MonoBehaviour {

	public Dictionary<string, System.Object> shareVar = new Dictionary<string, System.Object>();
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);
	}
	
}
