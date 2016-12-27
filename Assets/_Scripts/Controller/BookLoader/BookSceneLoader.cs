using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookSceneLoader : MonoBehaviour {

    public string assetBundleName;
    public string sceneName;
    // Use this for initialization
    IEnumerator Start () {
        yield return AssetBundleHelper.getInstance().LoadScene(assetBundleName, sceneName, true);
        DestroyObject(this);
	}

}
