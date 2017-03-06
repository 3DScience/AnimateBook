using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProgressBar;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DownloadAsset : MonoBehaviour {
    public Text txtMsg;
    int currentDownloadDependencyIdx = 0;
    string platform = Application.platform.ToString();
    private string assetDataFolder = "";
    private WWW www;
    private string url;
    private bool isSavingFile= false;
	string assetBundleName = "";
    ProgressBarBehaviour barBehaviour;
    private List<BookInfo> dependeciesBook = new List<BookInfo>(); 
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
            if(bookInfo.dependencies != null)
            {
                foreach (var bookId in bookInfo.dependencies)
                {
                    BookInfo dependencyBook = BooksFireBaseDb.getInstance().getBookInfoById(bookId);
                    dependeciesBook.Add(dependencyBook);
                }
            }

        }


       
        if (Application.platform == RuntimePlatform.WindowsEditor)// for testing
        {
            platform = "Android";
        }
        try
        {
            assetDataFolder = GlobalVar.DATA_PATH + "/" ;
            string olderFile = assetDataFolder + assetBundleName;
            if (File.Exists(olderFile))
            {
                File.Delete(olderFile);
                File.Delete(olderFile+".mf");
                File.Delete(olderFile + ".manifest");
                Caching.CleanCache();
            }
        }
        catch (System.Exception ex)
        {
            DebugOnScreen.Log(ex.ToString());
        }

        txtMsg.text = "Downloading contents.";
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
    private void downloadDependencies()
    {
        if(currentDownloadDependencyIdx >= dependeciesBook.Count )
        {
            barBehaviour.m_AttachedText.text = "DONE";

            if (dependeciesBook.Count > 0)
            {
                string[] dependenciesAbName = new string[dependeciesBook.Count];
                int i = 0;
                foreach (BookInfo dep in dependeciesBook)
                {
                    dependenciesAbName[i] = dep.name;
                    i++;
                }
                BookLoader.dependenciesAbName = dependenciesAbName;
            }
            BookLoader.assetBundleName = assetBundleName;
            SceneManager.LoadScene(GlobalVar.BOOK_LOADER_SCENE);
        }else
        {
            isSavingFile = false;

            txtMsg.text = "Downloading dependencies ("+ (currentDownloadDependencyIdx+1)+"/"+dependeciesBook.Count+")";
            BookInfo dependencyBook = dependeciesBook[currentDownloadDependencyIdx];
            
            if( !checkExistAbName(dependencyBook.assetbundle))
            {
                DebugOnScreen.Log("Download dependency book: " + dependencyBook.name);

                url = GlobalVar.BASE_ASSET_DOWNLOAD_URL + dependencyBook.download_url + "/" + platform + ".zip";
                //if (GlobalVar.DEBUG)
                DebugOnScreen.Log("url=" + url);
                www = new WWW(url);
                currentDownloadDependencyIdx++;
            }
            else {
                DebugOnScreen.Log("dependency book \"" + dependencyBook.name +"\" is existing.");
                currentDownloadDependencyIdx++;
                downloadDependencies();
            }

        }
 
    }
    private bool checkExistAbName(string abname)
    {
        string platform = Application.platform.ToString();
        if (platform == RuntimePlatform.IPhonePlayer.ToString())
        {
            platform = "IOS";
        }
        bool r= File.Exists(assetDataFolder +platform+"/"+ abname);
        DebugOnScreen.Log("checking exist ab name " + assetDataFolder + platform + "/" + abname+". Result=" +r);
        return r;
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
            zipFile = assetDataFolder  + assetBundleName + ".zip";
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
        downloadDependencies();
        //barBehaviour.m_AttachedText.text = "DONE";
        //BookLoader.assetBundleName = assetBundleName;
        //SceneManager.LoadScene(GlobalVar.BOOK_LOADER_SCENE);
        yield return null;
    }
}
