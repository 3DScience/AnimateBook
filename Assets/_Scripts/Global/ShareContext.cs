using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareContext : MonoBehaviour {
    public GameObject loadingIdicator;
	public Dictionary<string, System.Object> shareVar = new Dictionary<string, System.Object>();
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);
        loadingIdicator = Instantiate(Resources.Load("Prefabs/loaddingIndicator", typeof(GameObject)),gameObject.transform) as GameObject;
        loadingIdicator.SetActive(false);
    }
	
}
