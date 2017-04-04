#if !UNITY_WEBGL
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProgressBar;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DownloadAsset : MonoBehaviour {
    public Text txtMsg;
    public GameObject dialogMessagePref;
    int currentDownloadDependencyIdx = 0;
    string platform = Application.platform.ToString();
    private string assetDataFolder = "";
    private WWW www;
    private string url;
    private bool isSavingFile= false;
	string assetBundleName = "";
    ProgressBarBehaviour barBehaviour;
    private List<BookInfo> dependeciesBook = new List<BookInfo>();
    private DialogMessageController dialogMessageController;
    // Use this for initialization
    void Start () {
        GameObject obj = GameObject.Find("ProgressBarLabelInside");
        barBehaviour = obj.GetComponent<ProgressBarBehaviour>();
        barBehaviour.ProgressSpeed = 10000;



		BookInfo bookInfo = (BookInfo) GlobalVar.shareContext.shareVar["bookInfo"];
		//GlobalVar.shareContext.shareVar.Remove ("bookInfo");

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
		if (platform == RuntimePlatform.IPhonePlayer.ToString())
		{
			platform = "iOS";
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
		url = GlobalVar.BASE_ASSET_DOWNLOAD_URL + bookInfo.download_url + "/" + bookInfo.assetbundle + "_" + platform + ".zip";
        //if (GlobalVar.DEBUG)
            DebugOnScreen.Log("url 1 =" + url);
        startDownload();
    }
    private void startDownload()
    {
        if (checkNetwork())
        {
            www = new WWW(url);
        }
        else
        {
            if (dialogMessageController == null)
            {
                initDialogMessage();
            }
            dialogMessageController.setActive(true);
            dialogMessageController.setMessage("No Internet connection!");
            dialogMessageController.setButtonText("Retry");



        }
    }

    private void initDialogMessage()
    {
        GameObject dialogMsg = GameObject.Instantiate(dialogMessagePref);
        dialogMessageController = dialogMsg.GetComponentInChildren<DialogMessageController>();
        dialogMessageController.onOkButtonClickCallback = dialogOkButtonClick;
        dialogMessageController.onBackButtonClickCallback = dialogBackButtonClick;
    }

    private void dialogOkButtonClick()
    {
        dialogMessageController.setActive(false);
        barBehaviour.Value = 0;
        barBehaviour.m_AttachedText.text = "0%";
        startDownload();
    }
    private void dialogBackButtonClick()
    {
        SceneManager.UnloadSceneAsync(GlobalVar.DOWNLOAD_ASSET_SCENE);
        SceneManager.LoadScene(GlobalVar.BOOK2DDETAIL_SCENE);
    }
    private bool checkNetwork()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        return true;
    }
    private void handleDownloadError()
    {
       
        if (dialogMessageController == null)
        {
            initDialogMessage();
        }
        dialogMessageController.setActive(true);
        dialogMessageController.setMessage("There are some errors when loading!");
        dialogMessageController.setButtonText("Retry");
       
#if DEVELOPMENT_BUILD
        DebugOnScreen.Log("download err:" + www.error);
#endif
        www = null;
    }
    // Update is called once per frame
    void Update()
    {

        if (www == null)
            return;
        if (www.isDone)
        {
            if( www.error!=null)
            {
                handleDownloadError();
                return;
            }else  if (!isSavingFile)
            {
                barBehaviour.Value =100;
                isSavingFile = true;
                StartCoroutine( saveFileToLocal());
            }

        }
        else
        {
            if(www.error != null)
            {
                handleDownloadError();
                return;
            }
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
                //DebugOnScreen.Log("Download dependency book: " + dependencyBook.name);

				url = GlobalVar.BASE_ASSET_DOWNLOAD_URL + dependencyBook.download_url + "/" + dependencyBook.assetbundle + "_" + platform + ".zip";
				//url = GlobalVar.BASE_ASSET_DOWNLOAD_URL + dependencyBook.download_url + "/" + platform + ".zip";
                //if (GlobalVar.DEBUG)
                    DebugOnScreen.Log("url 2=" + url);
                www = new WWW(url);
                currentDownloadDependencyIdx++;
            }
            else {
                //DebugOnScreen.Log("dependency book \"" + dependencyBook.name +"\" is existing.");
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
        //DebugOnScreen.Log("checking exist ab name " + assetDataFolder + platform + "/" + abname+". Result=" +r);
        return false;
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
    void OnDestroy()
    {
        if(dialogMessageController != null)
        {
            dialogMessageController.DestroyGo();
        }
    }
}
#endif