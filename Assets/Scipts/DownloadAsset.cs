using UnityEngine;
using System.Collections;
using System.IO;
using ProgressBar;
using UnityEngine.UI;
public class DownloadAsset : MonoBehaviour {
    public string assetBundleName;

    private WWW www;
    private string url;
    private bool isSavingFile= false;

    ProgressBarBehaviour barBehaviour;
    // Use this for initialization
    void Start () {
        GameObject obj = GameObject.Find("ProgressBarLabelInside");
        barBehaviour = obj.GetComponent<ProgressBarBehaviour>();
        barBehaviour.ProgressSpeed = 10000;
        if (assetBundleName == null || assetBundleName=="") // for testing
        {
            assetBundleName = "book1";
        }
        string platform = Application.platform.ToString();
        if (Application.platform == RuntimePlatform.WindowsEditor)// for testing
        {
            platform = "Android";
        }

        url = GlobalConfig.BASE_ASSET_DOWNLOAD_URL +assetBundleName + "/" + platform + ".zip";
        www = new WWW(url);
    }

    // Update is called once per frame
    void Update()
    {

        if (www.isDone)
        {
            if (!isSavingFile)
            {
                barBehaviour.Value =100;
                isSavingFile = true;
                StartCoroutine( saveFileToLocal());
            }

        }
        else
        {
            barBehaviour.Value = www.progress*100;
            if (Debug.isDebugBuild)
                Debug.Log("downloaded " + www.progress * 100 + " %");
        }

    }

    IEnumerator saveFileToLocal()
    {
        //if (Debug.isDebugBuild)
        //Debug.Log("persistentDataPath=" + Application.persistentDataPath);
        string zipFile="";
        string assetDataFolder="";
        barBehaviour.m_AttachedText.text = "Saving...";
        yield return null;
        try
        {
            if (Debug.isDebugBuild)
                Debug.Log("dowload file from url " + url + " complete");
            byte[] data = www.bytes;
            assetDataFolder = GlobalConfig.DATA_PATH + "/" + assetBundleName;
            if (!Directory.Exists(assetDataFolder))
            {
                Directory.CreateDirectory(assetDataFolder);
            }
            zipFile = assetDataFolder + "/" + assetBundleName + ".zip";
            if (Debug.isDebugBuild)
                Debug.Log("dataFile=" + zipFile);
            System.IO.File.WriteAllBytes(zipFile, data);
        }
        catch (System.Exception ex)
        {
            DebugOnScreen.addToGameObject(gameObject).Log(ex.ToString());
            Debug.Log(ex.ToString());
            yield break;
        }

        StartCoroutine(unzipFile(zipFile, assetDataFolder));
        yield return null;

    }
    IEnumerator unzipFile(string zipFile,string path)
    {
        barBehaviour.m_AttachedText.text = "Unzip...";
        yield return null;
        try
        {
            ZipUtil.Unzip(zipFile, path);
        }
        catch (System.Exception ex)
        {
            DebugOnScreen.addToGameObject(gameObject).Log(ex.ToString());
            Debug.Log(ex.ToString());
            yield break;
        }

        if (Debug.isDebugBuild)
            Debug.Log("unzip done!");

        barBehaviour.m_AttachedText.text = "DONE";
        BookLoaderScript.assetBundleName = assetBundleName;
        Application.LoadLevel(GlobalConfig.BOOK_LOADER_SCENE);
        yield return null;
    }
}
