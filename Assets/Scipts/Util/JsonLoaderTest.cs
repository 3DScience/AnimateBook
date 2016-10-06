using UnityEngine;
using System.Collections;
using AssetBundles;
using Entities;
public class JsonLoaderTest : MonoBehaviour {
    public const string AssetBundlesOutputPath = "/AssetBundles/";
    public string assetBundleName="bookdemo";
    public string assetName= "TestAssetBundleMetaData";

    // Use this for initialization
    IEnumerator Start()
    {

          assetBundleName = "bookdemo";
           assetName = "TestAssetBundleMetaData";
    yield return StartCoroutine(Initialize());

        // Load asset.
        yield return StartCoroutine(InstantiateGameObjectAsync(assetBundleName, assetName));
    }

    // Initialize the downloading url and AssetBundleManifest object.
    protected IEnumerator Initialize()
    {
        // Don't destroy this gameObject as we depend on it to run the loading script.
        DontDestroyOnLoad(gameObject);

        // With this code, when in-editor or using a development builds: Always use the AssetBundle Server
        // (This is very dependent on the production workflow of the project. 
        // 	Another approach would be to make this configurable in the standalone player.)
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        AssetBundleManager.SetDevelopmentAssetBundleServer();
#else
		// Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
		AssetBundleManager.SetSourceAssetBundleURL(Application.dataPath + "/");
		// Or customize the URL based on your deployment or configuration
		//AssetBundleManager.SetSourceAssetBundleURL("http://www.MyWebsite/MyAssetBundles");
#endif

        // Initialize AssetBundleManifest which loads the AssetBundleManifest object.
        var request = AssetBundleManager.Initialize();
        if (request != null)
            yield return StartCoroutine(request);
    }

    protected IEnumerator InstantiateGameObjectAsync(string assetBundleName, string assetName)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(TextAsset));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);
        TextAsset txt = request.GetAsset<TextAsset>();
        //Debug.Log("json=" + txt.text);


        SceneInfo sceneInfo=JsonUtility.FromJson<SceneInfo>(txt.text);
         Debug.Log("sceneInfo name =" + sceneInfo.title);
        Debug.Log(" game object =" + sceneInfo.mainObjects[0].ObjectName);
        // string json = JsonConvert.SerializeObject(product);
        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
     //   Debug.Log(assetName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
    }


    void createTestDate()
    {

        SceneInfo sceneInfo = new SceneInfo();
        sceneInfo.title = "scence 01";
        sceneInfo.info = " this scence is for testing!";

        MainObject mainObject01 = new MainObject();
        mainObject01.ObjectName = "MainObect";
        mainObject01.movePath = "MovePath";

        Interactive interactive01 = new Interactive();
        interactive01.eventName = "Touch";
      //  interactive01.action = "Scale";
        mainObject01.interactives = new Interactive[] { interactive01 };

        sceneInfo.mainObjects = new MainObject[] { mainObject01 };


        Debug.Log("json=" + JsonUtility.ToJson(sceneInfo, true));
    }
}
