using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using Entities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class BookLoaderScript : MonoBehaviour
{
    public GameObject gLoadingEffect;
    public Canvas mainCanvas;
    public static string assetBundleName;
    private string metadataAssetBundleName;
    public string jsonAssetBundleMetaDataFileName = "AssetBundleMetaData";
    public string jsonSceneDataFileName = "Scene";
    public int currentSceneIdx = 1;
    public SceneInfo currentScene;
    private UiEventHandler uiEventHandler;
    private AssetBundleInfo assetBundleInfo;

    private Dictionary<string, SceneInfo> loadedSceneInfo = new Dictionary<string, SceneInfo>();

    private DisplayTextUiController displayTextUiController;
    // Use this for initialization
    IEnumerator Start()
    {
        if (Debug.isDebugBuild)
            Debug.Log("BookLoaderScript Start assetBundleName=" + assetBundleName);

        uiEventHandler = gameObject.GetComponent<UiEventHandler>();
        if (assetBundleName == null || assetBundleName == "")
        {
            //assetBundleName = "test_book"; 
            assetBundleName = "solar_system_book";
        }
        metadataAssetBundleName = assetBundleName + ".metadata";
        // Add DisplayTextUiController 
        displayTextUiController = gameObject.AddComponent<DisplayTextUiController>();

        displayTextUiController.mainCanvas = mainCanvas;
        displayTextUiController.metadataAssetBundleName = metadataAssetBundleName;
        //end
        //DontDestroyOnLoad(gameObject);
        yield return StartCoroutine(AssetBundleHelper.getInstance().InitializeAssetBunder(assetBundleName));
        yield return StartCoroutine(loadAssetBundleMetaData());

        string jsonFileName = jsonSceneDataFileName + currentSceneIdx.ToString();
        yield return StartCoroutine(loadSceneMetaData(jsonFileName));

        //SceneInfo sceneInfo = new SceneInfo();
        //sceneInfo.name = "page1";
        // yield return StartCoroutine(LoadScence(sceneInfo));
        // Load level.
        //yield return StartCoroutine(InitializeLevelAsync("page1", true));


    }
    protected IEnumerator loadAssetBundleMetaData()
    {
        yield return AssetBundleHelper.getInstance().LoadAsset<TextAsset>(metadataAssetBundleName, jsonAssetBundleMetaDataFileName, textAssetLoaded =>
        {
            assetBundleInfo = JsonUtility.FromJson<AssetBundleInfo>(textAssetLoaded.text);
            string dependenciesAsset = "";
            foreach (string depend in assetBundleInfo.dependenciesAsset)
            {
                dependenciesAsset += depend + " ";
            }
            if (Debug.isDebugBuild)
                Debug.Log("Finished loading assetBundleInfo title=" + assetBundleInfo.title + ", totalScenes" + assetBundleInfo.totalScenes + dependenciesAsset + ", dependenciesAsset=" + dependenciesAsset);
        });

    }
    protected IEnumerator loadSceneMetaData(string jsonFileName)
    {
       
        if (!loadedSceneInfo.ContainsKey(jsonFileName))
        {
            yield return AssetBundleHelper.getInstance().LoadAsset<TextAsset>(metadataAssetBundleName, jsonFileName, textAssetLoaded =>
            {
                if (Debug.isDebugBuild)
                    Debug.Log("json string= " + textAssetLoaded.text);
                currentScene = JsonUtility.FromJson<SceneInfo>(textAssetLoaded.text);
                loadedSceneInfo.Add(jsonFileName, currentScene);
                if (Debug.isDebugBuild)
                    Debug.Log("Finished loading currentScene name= " + currentScene.name + ", title=" + currentScene.title + ", info= " + currentScene.info);
            });
        }
        else
        {
            currentScene = loadedSceneInfo[jsonFileName];
            if (Debug.isDebugBuild)
                Debug.Log("Loaded from catch currentScene name= " + currentScene.name + ", title=" + currentScene.title + ", info= " + currentScene.info);
        }
        yield return StartCoroutine(LoadScence(currentScene));
    }
    protected IEnumerator LoadScence(SceneInfo sceneInfo)
    {
        displayTextUiController.onChangeScene();
        LoadingEffect loadingEffect = gLoadingEffect.GetComponent<LoadingEffect>();
        loadingEffect.loading = true;
		uiEventHandler.onSceneChange (currentSceneIdx,assetBundleInfo.totalScenes);
        float startTime = Time.realtimeSinceStartup;
        // Load level from assetBundle.
        yield return AssetBundleHelper.getInstance().LoadScene(assetBundleName, sceneInfo.name, true);

        uiEventHandler.OndoneLoadScene();
        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        if (Debug.isDebugBuild)
            Debug.Log("Finished loading scene: " + sceneInfo.name + " in " + elapsedTime + " seconds");
        interactiveProcessing(sceneInfo);
        loadingEffect.loading = false;
        //gLoadingEffect.GetComponent<Renderer>().enabled = false;
    }
    public void OnPlay()
    {
        SceneManager.UnloadScene(currentScene.name);
        currentSceneIdx = 2;
        string jsonFileName = jsonSceneDataFileName + currentSceneIdx.ToString();
        StartCoroutine(loadSceneMetaData(jsonFileName));
    }

    public void OnHome()
    {
        DestroyObject(gameObject);
        SceneManager.UnloadScene(GlobalVar.BOOK_LOADER_SCENE);
        SceneManager.LoadScene(GlobalVar.MAINSCENE);
    }

    public void OnNext()
    {
        if (currentSceneIdx < assetBundleInfo.totalScenes)
        {
            SceneManager.UnloadScene(currentScene.name);
            currentSceneIdx++;
            string jsonFileName = jsonSceneDataFileName + currentSceneIdx.ToString();
            StartCoroutine(loadSceneMetaData(jsonFileName));
        }
    }

    public void OnBack()
    {
        if (currentSceneIdx > 0)
        {
            SceneManager.UnloadScene(currentScene.name);
            currentSceneIdx--;
            string jsonFileName = jsonSceneDataFileName + currentSceneIdx.ToString();
            StartCoroutine(loadSceneMetaData(jsonFileName));
        }
    }

    public void OnPause()
    {
        Time.timeScale = 0;
        uiEventHandler.ButtonToPause();

    }

    public void OnResume()
    {
        Time.timeScale = 1;
        uiEventHandler.ButtonToPause();

    }

    public void OnReplay()
    {
        SceneManager.UnloadScene(currentScene.name);
        string jsonFileName = jsonSceneDataFileName + currentSceneIdx.ToString();
        loadedSceneInfo.Remove(jsonFileName);
        StartCoroutine(loadSceneMetaData(jsonFileName));
        
    }

	public void OnSystemSolar()
	{
		SceneManager.UnloadScene(currentScene.name);
		currentSceneIdx = 2;
		string jsonFileName = jsonSceneDataFileName + currentSceneIdx.ToString();
		StartCoroutine(loadSceneMetaData(jsonFileName));
	}

    protected void interactiveProcessing(SceneInfo sceneInfo)
    {
        foreach (MainObject mainObject in sceneInfo.mainObjects)
        {
            InteractiveController interactiveController = InteractiveController.addToGameObject(mainObject);
            interactiveController.interactiveCallBack = interactiveCallBack;
            interactiveController.displayTextUiController = displayTextUiController;
        }
    }
    public void interactiveCallBack(Action action)
    {
        INTERACTIVE_ACTION action_ = (INTERACTIVE_ACTION)System.Enum.Parse(typeof(INTERACTIVE_ACTION), action.actionName, true);
        if (action_ == INTERACTIVE_ACTION.CHANGE_SCENE)
        {
            changeScense(action.actionParams[0].paramValue);
        }
    }

    private void changeScense(string sceneIndex)
    {
        try
        {

            SceneManager.UnloadScene(currentScene.name);
            currentSceneIdx = int.Parse(sceneIndex);
            string jsonFileName = jsonSceneDataFileName + currentSceneIdx.ToString();
            StartCoroutine(loadSceneMetaData(jsonFileName));
        }
        catch (System.Exception)
        {
        }

    }
    void OnDestroy()
    {
     
        if (Debug.isDebugBuild)
            Debug.Log("BookLoaderScript  was destroyed <====================");
        AssetBundleHelper.getInstance().unLoadAssetBundleManager();
        loadedSceneInfo.Clear();
    }
}
