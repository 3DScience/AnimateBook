using UnityEngine;
using System.Collections;

public class GlobalConfig {

    public static string DATA_PATH;
    public static string BASE_ASSET_DOWNLOAD_URL = "http://192.168.0.201/unity3d/3dbook_test/";
    public static string BOOK_LOADER_SCENE = "BookLoader";
    public static string DOWNLOAD_ASSET_SCENE = "DownloadAsset";
	public static string MAINSCENE = "MainScene";
    static GlobalConfig()
    {
        DATA_PATH = Application.persistentDataPath;
        DATA_PATH = DATA_PATH.Substring(0, DATA_PATH.Length - 5);
        DATA_PATH = DATA_PATH.Substring(0, DATA_PATH.LastIndexOf("/"));
        DATA_PATH += "/Data";
    }
}
