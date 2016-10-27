using UnityEngine;
using System.Collections;

public class BookInfo : MonoBehaviour {
    public int index;
    public string bookName;
	// Use this for initialization
	void Start () {
        if (Debug.isDebugBuild)
            Debug.Log("BookInfo Start...");
    }
    void OnMouseDown()
    {
        if (Debug.isDebugBuild)
            Debug.Log("BookInfo-OnMouseDown...");
        GameObject.Find("Loader").SendMessage("OnSelectedBook",index,SendMessageOptions.DontRequireReceiver);
    }
    // Update is called once per frame
    void Update () {
	
	}
}
