using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BookLoader : MonoBehaviour {
    public GameObject homeButton;
    public static string assetBundleName;
    public static string[] dependenciesAbName;
    // Use this for initialization
    IEnumerator Start () {
		Debug.Log("RUN PLATFORM WEBGL 1");
        // GameObject loadingEffect =GameObject.Instantiate(AssetDataDase.LoadAssetAtPath("Assets/Prefabs/myPrefab.prefab", typeof(GameObject)) as GameObject;
        //GameObject textObject = (GameObject)Instantiate(Resources.Load("RichTextUi_1"));
        //  GameObject instance = Instantiate(Resources.Load("RichTextUi_1", typeof(GameObject))) as GameObject;

        //Caching.CleanCache();
        if (assetBundleName == null || assetBundleName == "")
        {
            //assetBundleName = "test_book"; 
            assetBundleName = "solar_system_book";
        }
#if !UNITY_WEBGL
       // DebugOnScreen.Log("init mainfest 10");
        yield return AssetBundleHelper.getInstance().InitializeAssetBunder(assetBundleName);
		try
		{

		//DebugOnScreen.Log("BookLoader Start assetBundleName=" + assetBundleName);
		BookSceneLoader sceneLoader = gameObject.AddComponent<BookSceneLoader>();
		sceneLoader.assetBundleName = assetBundleName;
		sceneLoader.sceneName = "page1";
		}
		catch (System.Exception ex)
		{

		DebugOnScreen.Log(ex.ToString());
		}
#else
        /*
         * 
        homeButton.SetActive(false);
        string url = Application.absoluteURL;
        Dictionary<string, string> httpGetData=null;
        if(url != null)
        {
            httpGetData = parseHttpGetData(url);
        }
        string assetBundleUrl = "vn/books/science/230002_solar_system_book/";
        assetBundleName = "solar_system_book";
		DebugOnScreen.Log("assetBundleUrl: " + assetBundleUrl);
        if (httpGetData!=null && httpGetData.ContainsKey("assetBundleName")&& httpGetData.ContainsKey("assetBundleUrl"))
        {
            
            assetBundleName = httpGetData["assetBundleName"];
            assetBundleUrl = httpGetData["assetBundleUrl"];
           DebugOnScreen.Log("getted assetBundleName=" + assetBundleName+ ", assetBundleUrl=" + assetBundleUrl);
        }
		yield return AssetBundleHelper.getInstance().InitializeAssetBunderWebGL(assetBundleName, assetBundleUrl);
    	*/
        //DebugOnScreen.Log("RUN PLATFORM WEBGL 5");
        Debug.Log("RUN PLATFORM WEBGL 2");
       yield return null;
#endif

       

    }
    Dictionary<string,string> parseHttpGetData(string url)
    {
        Dictionary<string, string> res = new Dictionary<string, string>();
        if (!url.Contains("?"))
            return res;
        url = url.Split('?')[1];

        string[] pas=url.Split('&');
        foreach (var pa in pas)
        {
            if (!url.Contains("="))
                continue;
            string[] strs = pa.Split('=');
            res.Add(strs[0], strs[1]);
        }
        return res;
    }
    public void onHomeButtonClick()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("DestroyOnHome");
        foreach (var go in gos)
        {
            DestroyObject(go);
        }
        SceneManager.UnloadSceneAsync(GlobalVar.BOOK_LOADER_SCENE);
        SceneManager.LoadScene(GlobalVar.BOOK2DDETAIL_SCENE);
    }
		
	public void ReceiveVariableFromJS(string a)
	{
		DebugOnScreen.Log ("ReceiveVariableFromJS:" + a);
		ParameterFormJson json = JsonUtility.FromJson <ParameterFormJson>(a);

		DebugOnScreen.Log ("ReceiveVariableFromJS 1: " + json.assetBundleName);
		DebugOnScreen.Log ("ReceiveVariableFromJS 2: " + json.assetBundleUrl);

		StartCoroutine( loadBook(json) );

	}

	IEnumerator loadBook(ParameterFormJson json)
	{
		yield return AssetBundleHelper.getInstance().InitializeAssetBunderWebGL(json.assetBundleName, json.assetBundleUrl);

		try
		{

			DebugOnScreen.Log("BookLoader Start assetBundleName=" + assetBundleName);
			BookSceneLoader sceneLoader = gameObject.AddComponent<BookSceneLoader>();
			sceneLoader.assetBundleName = json.assetBundleName;
			sceneLoader.sceneName = "page1";
		}
		catch (System.Exception ex)
		{

			DebugOnScreen.Log(ex.ToString());
		}
	}
		
}

[Serializable]
class ParameterFormJson 
{
	public string assetBundleName;
	public string assetBundleUrl;
}
