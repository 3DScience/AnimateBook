using UnityEngine;
using System.Collections;
using System.IO;
public class MainScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	//
	}
	
	// Update is called once per frame
	void Update () {
	//
	}

    public void ButtonClick(string assetBundleName)
    {

     
        if(checkIsDownloadedAsset(assetBundleName))
        {
            BookLoaderScript.assetBundleName = assetBundleName;
            Application.LoadLevel(GlobalConfig.BOOK_LOADER_SCENE);
            //test 2
        }
        else
        {
            Application.LoadLevel(GlobalConfig.DOWNLOAD_ASSET_SCENE);
        }
    }

    private bool checkIsDownloadedAsset(string assetBundleName)
    {
        string assetDataFolder = GlobalConfig.DATA_PATH + "/" + assetBundleName;
        if (Directory.Exists(assetDataFolder))
        {
            return true;
        }
        return false;
    }
     
}
