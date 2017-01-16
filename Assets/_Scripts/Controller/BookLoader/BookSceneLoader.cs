using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookSceneLoader : MonoBehaviour {

    public string assetBundleName;
    public string sceneName;
    // Use this for initialization

    IEnumerator Start () {
        GlobalVar.shareContext.loadingIdicator.SetActive(true);
        yield return AssetBundleHelper.getInstance().LoadScene(assetBundleName, sceneName, true);
        DestroyObject(this);
        GlobalVar.shareContext.loadingIdicator.SetActive(false);
    }

}
