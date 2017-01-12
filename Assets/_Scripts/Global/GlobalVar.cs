using UnityEngine;
using System.Collections;

public class GlobalVar {
    public static bool DEBUG = false;
    public static ShareContext shareContext;
    public static string DATA_PATH;
	public static string BASE_ASSET_DOWNLOAD_URL = "http://192.168.0.201/unity3d/3dbook_test/";
    //public static string BASE_ASSET_DOWNLOAD_URL = "http://10.11.0.14//unity3d/3dbook_test/";
    public static string BOOK_LOADER_SCENE = "BookLoader";
    public static string DOWNLOAD_ASSET_SCENE = "DownloadAsset";
    public static string CATEGORY_SCENE = "Book2D";
    public static string MAINSCENE = "Home";
	public static string LANGUAGE = "vn";
    public static GameObject SETTING_DIALOG;
    static GlobalVar()
    {
        DATA_PATH = Application.persistentDataPath;
        //DATA_PATH = DATA_PATH.Substring(0, DATA_PATH.Length - 5); // for Androj
		//DATA_PATH = DATA_PATH.Substring(0, DATA_PATH.LastIndexOf("/"));	// for Androj
        DATA_PATH += "/Data";
        GameObject g = new GameObject("ShareContext");
        shareContext=g.AddComponent<ShareContext>();
    }

}
