using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareContext : MonoBehaviour {
    public GameObject loadingIndicator;
	public Dictionary<string, System.Object> shareVar = new Dictionary<string, System.Object>();
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);
        
    }
    public void initLoadingIndicator()
    {
        loadingIndicator = Instantiate(Resources.Load("Prefabs/loaddingIndicator", typeof(GameObject)), gameObject.transform) as GameObject;
        loadingIndicator.SetActive(false);
    }


}
