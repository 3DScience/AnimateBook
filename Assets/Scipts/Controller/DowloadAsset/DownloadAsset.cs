using UnityEngine;
using System.Collections;
using System.IO;
using ProgressBar;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class DownloadAsset : MonoBehaviour {
    public string assetBundleName;
    private string assetDataFolder = "";
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
            assetBundleName = "test_book";
        }
        string platform = Application.platform.ToString();
        if (Application.platform == RuntimePlatform.WindowsEditor)// for testing
        {
            platform = "Android";
        }
        try
        {
            assetDataFolder = GlobalConfig.DATA_PATH + "/" + assetBundleName;
            if (Directory.Exists(assetDataFolder))
            {
                Directory.Delete(assetDataFolder,true);
            }
        }
        catch (System.Exception ex)
        {
            DebugOnScreen.Log(ex.ToString());
        }

        url = GlobalConfig.BASE_ASSET_DOWNLOAD_URL +assetBundleName + "/" + platform + ".zip";
        DebugOnScreen.Log("url=" + url);
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
    
        barBehaviour.m_AttachedText.text = "Saving...";
        yield return null;
        try
        {
            if (Debug.isDebugBuild)
                Debug.Log("dowload file from url " + url + " complete");
            byte[] data = www.bytes;
            
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
            DebugOnScreen.Log(ex.ToString());
            yield break;
        }

        StartCoroutine(unzipFile(zipFile, assetDataFolder));
        yield return null;

    }
    IEnumerator unzipFile(string zipFile,string path)
    {
        barBehaviour.m_AttachedText.text = "extracting...";
        yield return null;
        try
        {
            ZipUtil.Unzip(zipFile, path);
            File.Delete(zipFile);
        }
        catch (System.Exception ex)
        {
            DebugOnScreen.Log(ex.ToString());
            yield break;
        }

        if (Debug.isDebugBuild)
            Debug.Log("unzip done!");

        barBehaviour.m_AttachedText.text = "DONE";
        BookLoaderScript.assetBundleName = assetBundleName;
        SceneManager.LoadScene(GlobalConfig.BOOK_LOADER_SCENE);
        yield return null;
    }
}
