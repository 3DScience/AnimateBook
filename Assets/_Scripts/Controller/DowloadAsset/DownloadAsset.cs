using UnityEngine;
using System.Collections;
using System.IO;
using ProgressBar;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Book.RTDatabase;

public class DownloadAsset : MonoBehaviour {

    private string assetDataFolder = "";
    private WWW www;
    private string url;
    private bool isSavingFile= false;
	string assetBundleName = "";
    ProgressBarBehaviour barBehaviour;
    // Use this for initialization
    void Start () {
        GameObject obj = GameObject.Find("ProgressBarLabelInside");
        barBehaviour = obj.GetComponent<ProgressBarBehaviour>();
        barBehaviour.ProgressSpeed = 10000;



		BookInfo bookInfo = (BookInfo) GlobalVar.shareContext.shareVar["bookInfo"];
		GlobalVar.shareContext.shareVar.Remove ("bookInfo");

		if (bookInfo == null) { // for testing
			//assetBundleName = "test_book";
			assetBundleName = "solar_system_book";
		} else {
			assetBundleName = bookInfo.assetbundle;
		}


        string platform = Application.platform.ToString();
        if (Application.platform == RuntimePlatform.WindowsEditor)// for testing
        {
            platform = "Android";
        }
        try
        {
            assetDataFolder = GlobalVar.DATA_PATH + "/" + assetBundleName;
            if (Directory.Exists(assetDataFolder))
            {
                Directory.Delete(assetDataFolder,true);
            }
        }
        catch (System.Exception ex)
        {
            DebugOnScreen.Log(ex.ToString());
        }

		url = GlobalVar.BASE_ASSET_DOWNLOAD_URL + bookInfo.download_url + "/" + platform + ".zip";
        if (GlobalVar.DEBUG)
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
        BookLoader.assetBundleName = assetBundleName;
        SceneManager.LoadScene(GlobalVar.BOOK_LOADER_SCENE);
        yield return null;
    }
}
