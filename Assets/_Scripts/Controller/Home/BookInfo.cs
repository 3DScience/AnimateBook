using UnityEngine;
using System.Collections;

public class CategoryInfo : MonoBehaviour {
    public int index;
    public string categoryName;

    public System.Action<int> callback;
	// Use this for initialization
	void Start () {
        if (Debug.isDebugBuild)
            Debug.Log("CategoryInfo Start...");
    }
    void OnMouseDown()
    {
        if (Debug.isDebugBuild)
            Debug.Log("CategoryInfo-OnMouseDown...");
        if (callback != null)
        {
            callback(index);
        }
    }

}
