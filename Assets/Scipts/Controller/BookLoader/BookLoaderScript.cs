using UnityEngine;
using System.Collections;
using AssetBundles;
using Entities;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;
public class BookLoaderScript : MonoBehaviour
{

    public static  string assetBundleName;
    public string jsonAssetBundleMetaDataFileName = "AssetBundleMetaData";
    public string jsonSceneDataFileName = "Scene";
    public int currentSceneIdx = 1;
    public SceneInfo currentScene;
    private UiEventHandler uiEventHandler;
    private AssetBundleInfo assetBundleInfo;
    private bool pauseMovePath = false;
    GameObject mainObject;
    // Use this for initialization
    IEnumerator Start()
    {
        if (Debug.isDebugBuild)
            Debug.Log("BookLoaderScript Start assetBundleName="+ assetBundleName);
        uiEventHandler = gameObject.GetComponent<UiEventHandler>();
        if (assetBundleName==null || assetBundleName == "")
        {
              //assetBundleName = "test_book"; 
                assetBundleName = "solar_system_book"; 
        }

        yield return StartCoroutine(InitializeAssetBunder());
        yield return StartCoroutine(loadAssetBundleMetaData());

        //SceneInfo sceneInfo = new SceneInfo();
        //sceneInfo.name = "page1";
        // yield return StartCoroutine(LoadScence(sceneInfo));
        // Load level.
        //yield return StartCoroutine(InitializeLevelAsync("page1", true));
    }

    // Initialize the downloading url and AssetBundleManifest object.
    protected IEnumerator InitializeAssetBunder()
    {
        // Don't destroy this gameObject as we depend on it to run the loading script.
        DontDestroyOnLoad(gameObject);

        // With this code, when in-editor or using a development builds: Always use the AssetBundle Server
        // (This is very dependent on the production workflow of the project. 
        // 	Another approach would be to make this configurable in the standalone player.)
        #if DEVELOPMENT_BUILD 
                AssetBundleManager.SetDevelopmentAssetBundleServer();
        #else
		                // Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
		                //AssetBundleManager.SetSourceAssetBundleURL("http://192.168.0.201/unity3d/3dbook_test/");
                        AssetBundleManager.SetSourceAssetBundleURL("file://"+GlobalConfig.DATA_PATH+"/"+assetBundleName+"/");
		                // Or customize the URL based on your deployment or configuration
		                //AssetBundleManager.SetSourceAssetBundleURL("http://www.MyWebsite/MyAssetBundles");
        #endif

         
        // Initialize AssetBundleManifest which loads the AssetBundleManifest object.
        var request = AssetBundleManager.Initialize();

        if (request != null)
            yield return StartCoroutine(request);
    }

	public void OnPlay() {
		SceneManager.UnloadScene(currentScene.name);
		currentSceneIdx = 2;
        string jsonFileName = jsonSceneDataFileName + currentSceneIdx.ToString();
        StartCoroutine(loadSceneMetaData(jsonFileName));
	}

	public void OnHome() {
		DestroyObject (gameObject);
		SceneManager.UnloadScene(GlobalConfig.BOOK_LOADER_SCENE);
		DebugOnScreen.Log ("OnHome begin FUCK FUCK FUCK");
		SceneManager.LoadScene (GlobalConfig.MAINSCENE);
		DebugOnScreen.Log ("OnHome END FUCK FUCK FUCK");
	}

	public void OnNext() {
		if (currentSceneIdx < assetBundleInfo.totalScenes) {
			SceneManager.UnloadScene(currentScene.name);
			currentSceneIdx++;
            string jsonFileName = jsonSceneDataFileName + currentSceneIdx.ToString();
            StartCoroutine(loadSceneMetaData(jsonFileName));
		}
	}

	public void OnBack() {
		if (currentSceneIdx > 0) {
			SceneManager.UnloadScene(currentScene.name);
			currentSceneIdx--;
            string jsonFileName = jsonSceneDataFileName + currentSceneIdx.ToString();
            StartCoroutine(loadSceneMetaData(jsonFileName));
		}
	}

	public void OnPause() {
		Time.timeScale = 0;
        uiEventHandler.ButtonToPause();

    }

	public void OnResume() {
		Time.timeScale = 1;
        uiEventHandler.ButtonToPause();

    }
    public void GoToScene(string sceneIdx)
    {
        SceneManager.UnloadScene(currentScene.name);
        currentSceneIdx=int.Parse(sceneIdx);
        string jsonFileName = jsonSceneDataFileName + currentSceneIdx.ToString();
        StartCoroutine(loadSceneMetaData(jsonFileName));
    }
    public void OnReplay() {
		SceneManager.UnloadScene(currentScene.name);
        string jsonFileName = jsonSceneDataFileName + currentSceneIdx.ToString();
        StartCoroutine(loadSceneMetaData(jsonFileName));
	}
		
    protected IEnumerator loadAssetBundleMetaData()
    {
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName+".metadata", jsonAssetBundleMetaDataFileName, typeof(TextAsset));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);
        TextAsset json = request.GetAsset<TextAsset>();
        assetBundleInfo = JsonUtility.FromJson<AssetBundleInfo>(json.text);
        if (Debug.isDebugBuild)
            Debug.Log("Finished loading assetBundleInfo title=" + assetBundleInfo.title+ ", totalScenes" + assetBundleInfo.totalScenes);
        string jsonFileName = jsonSceneDataFileName + currentSceneIdx.ToString();
        yield return  StartCoroutine(loadSceneMetaData(jsonFileName));
    }
    protected IEnumerator loadSceneMetaData(string jsonFileName)
    {
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName+ ".metadata", jsonFileName, typeof(TextAsset));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);
        TextAsset json = request.GetAsset<TextAsset>();
        currentScene = JsonUtility.FromJson<SceneInfo>(json.text);
        if (Debug.isDebugBuild)
            Debug.Log("Finished loading currentScene name= "+ currentScene.name + ", title=" + currentScene.title + ", info= " + currentScene.info);
        yield return StartCoroutine(LoadScence(currentScene));
    }

    protected IEnumerator LoadScence(SceneInfo sceneInfo)
    {
        GameObject gLoadingEffect = GameObject.Find("LoadingEffect");
        LoadingEffect loadingEffect = gLoadingEffect.GetComponent<LoadingEffect>();
        loadingEffect.loading = true;

        if (currentSceneIdx == 1) {
            uiEventHandler.ButtonToBegin();
        } else if (currentSceneIdx == assetBundleInfo.totalScenes) {
            uiEventHandler.ButtonToEnd();
        } else
            uiEventHandler.ButtonToPage();

        float startTime = Time.realtimeSinceStartup;

        // Load level from assetBundle.
      	AssetBundleLoadOperation request = AssetBundleManager.LoadLevelAsync(assetBundleName, sceneInfo.name, true);
        if (request == null)
            yield break;
        yield return StartCoroutine(request);


        uiEventHandler.OndoneLoadScene();
        //  GameObject mainobject = GameObject.Find("MainObject01");
        //  interactivecontroller.addtogameobject(mainobject, "testing param");
        //  iTween.MoveTo(mainobject, iTween.Hash("path", iTweenPath.GetPath("p1"), "time", 10, "easetype", iTween.EaseType.linear, "looptype", "pingpong"));

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        if (Debug.isDebugBuild)
            Debug.Log("Finished loading scene " + sceneInfo.name + " in " + elapsedTime + " seconds");
        interactiveProcessing(sceneInfo);
        loadingEffect.loading = false;
        //gLoadingEffect.GetComponent<Renderer>().enabled = false;
    }
    protected void interactiveProcessing(SceneInfo sceneInfo)
    {
        foreach (MainObject mainObject in sceneInfo.mainObjects)
        {
            InteractiveController interactiveController = InteractiveController.addToGameObject(mainObject, GoToScene);
        }
    }


    protected IEnumerator InitializeLevelAsync(string levelName, bool isAdditive)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load level from assetBundle.
        AssetBundleLoadOperation request = AssetBundleManager.LoadLevelAsync(assetBundleName, levelName, isAdditive);
        if (request == null)
            yield break;
        yield return StartCoroutine(request);


        GameObject mainobject = GameObject.Find("MainObject01");
        //interactivecontroller.addtogameobject(mainobject, "testing param");
        iTween.MoveTo(mainobject, iTween.Hash("path", iTweenPath.GetPath("p1"), "time", 10, "easetype", iTween.EaseType.linear, "looptype", "pingpong"));

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        if (Debug.isDebugBuild)
            Debug.Log("Finished loading scene " + levelName + " in " + elapsedTime + " seconds");
    }

    //void Update1()
    //{

    //    if (Application.platform != RuntimePlatform.Android)
    //    {
    //        // use the input stuff
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            onTouchBegin(Input.mousePosition);
    //        }
    //        if (Input.GetMouseButtonUp(0))
    //        {
    //            onTouchEnd(Input.mousePosition);
    //        }

    //    }
    //    if (Input.touchCount > 0)
    //    {
    //        if (Input.GetTouch(0).phase == TouchPhase.Began)
    //        {
    //            onTouchBegin(Input.GetTouch(0).position);
    //            Debug.Log("Touch Began.............................");
    //        }
    //        else if (Input.GetTouch(0).phase == TouchPhase.Ended)
    //        {
    //            Debug.Log("Touch Ended.............................");
    //            onTouchEnd(Input.GetTouch(0).position);
    //        }

    //    }
    //}
    //void onTouchBegin(Vector3 pos)
    //{

    //    if (isHitOnGameObject("MainObject", pos))
    //    {
    //        iTween.Pause(mainObject);
    //        Debug.Log("This is a Player");
    //        mainObject.transform.localScale = mainObject.transform.localScale * 1.2f;
    //    }
    //    else
    //    {
    //        Debug.Log("This isn't a Player");
    //    }

    //}
    //void onTouchEnd(Vector3 pos)
    //{
    //    if (isHitOnGameObject("MainObject", pos))
    //    {
    //        iTween.Resume(mainObject);
    //        mainObject.transform.localScale = mainObject.transform.localScale / 1.2f;
    //    }
    //}
    //bool isHitOnGameObject(string gameObjectName, Vector3 pos)
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(pos);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        if (hit.transform.name == gameObjectName)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}
}
