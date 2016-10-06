using UnityEngine;
using System.Collections;
using System.IO;
public class DownloadAsset : MonoBehaviour {
    public string assetBundleName;

    private WWW www;
    private string url;
    private bool isUnzipped = false;
    bool msg = true;
    string msgStr = "";
    // Use this for initialization
    void Start () {
        if (assetBundleName == null || assetBundleName=="")
        {
            assetBundleName = "book1";
        }
        string platform = Application.platform.ToString();
        platform = "Android";
        url = GlobalConfig.BASE_ASSET_DOWNLOAD_URL +assetBundleName + "/" + platform + ".zip";
        www = new WWW(url);
    }
    void OnGUI()
    {
        if (msg)
        {
            GUIStyle gui = new GUIStyle();
            gui.fontSize = 30;
            // string text = System.IO.File.ReadAllText("D:/ping.bat");
            GUI.Label(new Rect(Screen.width / 8, Screen.height / 2, 200f, 200f), msgStr, gui);
        }
    }
    // Update is called once per frame
    void Update()
    {

        if (www.isDone)
        {
            //if (Debug.isDebugBuild)
                //Debug.Log("persistentDataPath=" + Application.persistentDataPath);
                msgStr = "DONE!";
            try
            {
                if (!isUnzipped)
                {
                    if (Debug.isDebugBuild)
                        Debug.Log("dowload file from url "+ url+" complete");
                    byte[] data = www.bytes;
                    string assetDataFolder = GlobalConfig.DATA_PATH + "/" + assetBundleName;
                    if (!Directory.Exists(assetDataFolder))
                    {
                        Directory.CreateDirectory(assetDataFolder);
                    }
                    string zipFile = assetDataFolder + "/"+assetBundleName+".zip";


                    msgStr = "fileDownloaded=" + zipFile;
                    if (Debug.isDebugBuild)
                        Debug.Log("dataFile=" + zipFile);
                    System.IO.File.WriteAllBytes(zipFile, data);
              
              
                   // assetBundlePath = exportLocation;
                    //exportLocation = "C:\\Users\\Public\\Documents\\Unity Projects\\xoa";
                    ZipUtil.Unzip(zipFile, assetDataFolder);
                    if (Debug.isDebugBuild)
                        Debug.Log("unzip done!");
                    isUnzipped = true;

                    BookLoaderScript.assetBundleName = assetBundleName;
                    Application.LoadLevel(GlobalConfig.BOOK_LOADER_SCENE);
                    
                    //if (!isLoadAsset)
                    //{
                    //    StartCoroutine(loadAsset());
                    //    isLoadAsset = true;
                    //}


                }
            }
            catch (System.Exception ex)
            {

                msgStr = ex.Message;

            }



        }
        else
        {
            msgStr = "dowloading " + www.progress * 100 + " %";
            //msgStr = "persistentDataPath=" + Application.persistentDataPath;
            if (Debug.isDebugBuild)
                Debug.Log("downloaded " + www.progress * 100 + " %");
        }


    }

}
