using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
public class GlobalVar {
    public static bool DEBUG = false;
    public static ShareContext shareContext;
    public static string DATA_PATH;
    public static string DB_PATH;
#if DEVELOPMENT_BUILD
		public static string BASE_ASSET_DOWNLOAD_URL = "http://192.168.0.201/unity3d/3dbook_test/";
#else
    public static string BASE_ASSET_DOWNLOAD_URL = "http://www.smallworld3d.com/unity3d/3dbook_test/";
#endif
    public static string BASE_ASSET_URL = "http://192.168.0.201/unity3d/3dbook_test/vn/books/";
    //public static string BASE_ASSET_DOWNLOAD_URL = "http://10.11.0.14//unity3d/3dbook_test/";
    public static string BOOK_LOADER_SCENE = "BookLoader";
    public static string DOWNLOAD_ASSET_SCENE = "DownloadAsset";
    public static string CATEGORY_SCENE = "BookList";
	public static string BOOK2DDETAIL_SCENE = "Book2DDetail";
    public static string MAINSCENE = "Home";
	public static string LANGUAGE = "vn";
    public static GameObject SETTING_DIALOG;
    static GlobalVar()
    {
        DATA_PATH = Application.persistentDataPath;
        //DATA_PATH = DATA_PATH.Substring(0, DATA_PATH.Length - 5); // for Androj
		//DATA_PATH = DATA_PATH.Substring(0, DATA_PATH.LastIndexOf("/"));	// for Androj
        DATA_PATH += "/Data";
        DB_PATH= Application.persistentDataPath+"/db";

        if (!Directory.Exists(DB_PATH))
        {
            Directory.CreateDirectory(DB_PATH);
        }

        GameObject g = new GameObject("ShareContext");
        shareContext=g.AddComponent<ShareContext>();
        shareContext.initLoadingIndicator();

		System.Collections.Generic.Dictionary<string, object> defaults =
			new System.Collections.Generic.Dictionary<string, object>();
		defaults.Add("BASE_ASSET_DOWNLOAD_URL", "http://192.168.0.201/unity3d/3dbook_test/vn/books/");
		Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
		BASE_ASSET_DOWNLOAD_URL = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue("BASE_ASSET_DOWNLOAD_URL").StringValue;
		//DebugOnScreen.Log("RemoteConfig configured and ready with BASE_ASSET_DOWNLOAD_URL : " + BASE_ASSET_DOWNLOAD_URL);

		//DebugOnScreen.Log("RemoteConfig Fetching data...");
		System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(
			TimeSpan.Zero);
		fetchTask.ContinueWith(FetchComplete);
    }

	static void FetchComplete(Task fetchTask) {
		if (fetchTask.IsCanceled) {
			DebugOnScreen.Log("Fetch canceled.");
		} else if (fetchTask.IsFaulted) {
			DebugOnScreen.Log("Fetch encountered an error.");
		} else if (fetchTask.IsCompleted) {
			//DebugOnScreen.Log("Fetch completed successfully!");
		}

		switch (Firebase.RemoteConfig.FirebaseRemoteConfig.Info.LastFetchStatus) {
		case Firebase.RemoteConfig.LastFetchStatus.Success:
			Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched();
			//DebugOnScreen.Log("Remote data loaded and ready.");
			BASE_ASSET_DOWNLOAD_URL = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue("BASE_ASSET_DOWNLOAD_URL").StringValue;
			//DebugOnScreen.Log("RemoteConfig configured and ready with BASE_ASSET_DOWNLOAD_URL 111: " + BASE_ASSET_DOWNLOAD_URL);
			break;
		case Firebase.RemoteConfig.LastFetchStatus.Failure:
			switch (Firebase.RemoteConfig.FirebaseRemoteConfig.Info.LastFetchFailureReason) {
			case Firebase.RemoteConfig.FetchFailureReason.Error:
				DebugOnScreen.Log("Fetch failed for unknown reason");
				break;
			case Firebase.RemoteConfig.FetchFailureReason.Throttled:
				DebugOnScreen.Log("Fetch throttled until " +
					Firebase.RemoteConfig.FirebaseRemoteConfig.Info.ThrottledEndTime);
				break;
			}
			break;
		case Firebase.RemoteConfig.LastFetchStatus.Pending:
			DebugOnScreen.Log("Latest Fetch call still pending.");
			break;
		}
	}

}
