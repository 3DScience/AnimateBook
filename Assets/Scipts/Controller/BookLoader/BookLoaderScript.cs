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
    public static  string assetBundleName;
    private string metadataAssetBundleName;
    public string jsonAssetBundleMetaDataFileName = "AssetBundleMetaData";
    public string jsonSceneDataFileName = "Scene";
    public int currentSceneIdx = 1;
    public SceneInfo currentScene;
    private UiEventHandler uiEventHandler;
    private AssetBundleInfo assetBundleInfo;
    private AssetBunderHelper assetBunderHelper;
    private Dictionary<string, GameObject> loadedPrefab = new Dictionary<string, GameObject>();
    private GameObject currentTextUIGameObject=null;
    private DisplayTextUiController displayTextUiController;
    // Use this for initialization
    IEnumerator Start()
    {
        if (Debug.isDebugBuild)
            Debug.Log("BookLoaderScript Start assetBundleName="+ assetBundleName);
        assetBunderHelper = new AssetBunderHelper(this);
        uiEventHandler = gameObject.GetComponent<UiEventHandler>();
        if (assetBundleName==null || assetBundleName == "")
        {
              //assetBundleName = "test_book"; 
                assetBundleName = "solar_system_book"; 
        }
        metadataAssetBundleName = assetBundleName + ".metadata";
        // Add DisplayTextUiController 
        displayTextUiController = gameObject.AddComponent<DisplayTextUiController>();
        displayTextUiController.assetBunderHelper = assetBunderHelper;
        displayTextUiController.mainCanvas = mainCanvas;
        displayTextUiController.metadataAssetBundleName = metadataAssetBundleName;
        TouchEventControler touchEventControler = gameObject.GetComponent<TouchEventControler>();
        if (touchEventControler != null)
        {
            touchEventControler.deledateOnTouchNothing = displayTextUiController.onTouchNothing;
        }
        //end
        //DontDestroyOnLoad(gameObject);
        yield return StartCoroutine(assetBunderHelper.InitializeAssetBunder(assetBundleName));
        yield return StartCoroutine(loadAssetBundleMetaData());

        string jsonFileName = jsonSceneDataFileName + currentSceneIdx.ToString();
        yield return StartCoroutine(loadSceneMetaData(jsonFileName));

        //SceneInfo sceneInfo = new SceneInfo();
        //sceneInfo.name = "page1";
        // yield return StartCoroutine(LoadScence(sceneInfo));
        // Load level.
        //yield return StartCoroutine(InitializeLevelAsync("page1", true));


    }
    private void handleOnTouchNothing()
    {
        if (Debug.isDebugBuild)
            Debug.Log("handleOnTouchNothing.");

        if (currentTextUIGameObject != null && currentTextUIGameObject.activeSelf)
        {
            currentTextUIGameObject.SetActive(false);
        }
    }
    protected IEnumerator loadAssetBundleMetaData()
    {
        yield return assetBunderHelper.LoadAsset<TextAsset>(metadataAssetBundleName, jsonAssetBundleMetaDataFileName, textAssetLoaded => {
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
        yield return assetBunderHelper.LoadAsset<TextAsset>(metadataAssetBundleName, jsonFileName, textAssetLoaded => {
            if (Debug.isDebugBuild)
                Debug.Log("json string= " + textAssetLoaded.text);
            currentScene = JsonUtility.FromJson<SceneInfo>(textAssetLoaded.text);
            if (Debug.isDebugBuild)
                Debug.Log("Finished loading currentScene name= " + currentScene.name + ", title=" + currentScene.title + ", info= " + currentScene.info);  
        });
        yield return StartCoroutine(LoadScence(currentScene));
    }
    protected IEnumerator LoadScence(SceneInfo sceneInfo)
    {
       
        LoadingEffect loadingEffect = gLoadingEffect.GetComponent<LoadingEffect>();
        loadingEffect.loading = true;

        if (currentSceneIdx == 1)
        {
            uiEventHandler.ButtonToBegin();
        }
        else if (currentSceneIdx == assetBundleInfo.totalScenes) 
        {
            uiEventHandler.ButtonToEnd();
        }
        else
            uiEventHandler.ButtonToPage();

        float startTime = Time.realtimeSinceStartup;
        // Load level from assetBundle.
        yield return assetBunderHelper.LoadScene(assetBundleName, sceneInfo.name, true);

        uiEventHandler.OndoneLoadScene();
        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        if (Debug.isDebugBuild)
            Debug.Log("Finished loading scene: " + sceneInfo.name + " in " + elapsedTime + " seconds");
        interactiveProcessing(sceneInfo);
        loadingEffect.loading = false;
        //gLoadingEffect.GetComponent<Renderer>().enabled = false;
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
		SceneManager.LoadScene (GlobalConfig.MAINSCENE);
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

    public void OnReplay() {
		SceneManager.UnloadScene(currentScene.name);
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
            //if( mainObject.interactives!=null && mainObject.interactives.Length > 0)
            //{
            //    if (Debug.isDebugBuild)
            //        Debug.Log("interactive "+ mainObject.interactives[0].actions[0].getDictionaryActionParam()["header"]);


            //}
        }
    }
    public void interactiveCallBack(Action action)
    {
        INTERACTIVE_ACTION action_ = (INTERACTIVE_ACTION)System.Enum.Parse(typeof(INTERACTIVE_ACTION), action.actionName, true);
        if (action_ == INTERACTIVE_ACTION.CHANGE_SCENE)
        {
            changeScense(action.actionParams[0].paramValue);
        }
        else if(action_ == INTERACTIVE_ACTION.SHOW_TEXT)
        {
            StartCoroutine(showTextUi(action.getDictionaryActionParam()));
        }

    }

    protected IEnumerator showTextUi(Dictionary<string,string> actionParams)
    {

        GameObject uiGameobject = null;
        string textui = actionParams["displayUI"];
        if (loadedPrefab.ContainsKey(textui))
        {
            uiGameobject = loadedPrefab[textui];
        }
        else
        {
            GameObject prefab = null;
            string[] assetBundleMetaData = textui.Split(new string[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries);
            string commonAssetBundleName = assetBundleMetaData[0];
            string assetName = assetBundleMetaData[1];

            yield return assetBunderHelper.LoadAsset<GameObject>(commonAssetBundleName, assetName, prefabLoaded =>
            {
                prefab = prefabLoaded;
            });
            if (prefab != null)
            {
                uiGameobject = (GameObject)GameObject.Instantiate(prefab, mainCanvas.transform, false);
                loadedPrefab.Add(textui, uiGameobject);
            }
        }
        if (uiGameobject != null)
        {
            currentTextUIGameObject = uiGameobject;
            uiGameobject.SetActive(true);
            TextAsset textContent = null;
            yield return assetBunderHelper.LoadAsset<TextAsset>(metadataAssetBundleName, actionParams["textFile"], textAssetLoaded => {
                textContent = textAssetLoaded;
            });
            Debug.Log("textContent=" + textContent);

            // Text txtTile = uiGameobject.transform.Find("Object").Find("titleBox").Find("txt_title").gameObject.GetComponent<Text>();
            // Text txtTile = uiGameobject.GetComponentInChildren<Text>();
            Text[] textUis = uiGameobject.GetComponentsInChildren<Text>();
            foreach (Text textUi in textUis)
            {
                if (textUi.gameObject.name == "txt_title")
                {
                    textUi.text = actionParams["header"];
                }
                else if (textUi.gameObject.name == "txt_info")
                {
                    textUi.text = textContent.text;
                }
            }
            // now we add eventtrigger for explorerButton
            GameObject explorerButton = uiGameobject.transform.FindChild("explorerButton").gameObject;
            EventTrigger explorerButtonEvenTrigger= explorerButton.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.callback.RemoveAllListeners();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener((eventData) => { OnExplorerButtonClicked(actionParams["explorerSceneIndex"]); });
            explorerButtonEvenTrigger.triggers.Clear();
            explorerButtonEvenTrigger.triggers.Add(entry);
        }
    }
    private void OnExplorerButtonClicked(string sceneIndex)
    {

            changeScense(sceneIndex);
     
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
    //private IEnumerator loadTextUi(SceneInfo sceneInfo)
    //{
    //    TextContent paragraph1 = sceneInfo.texts[0];
    //    string[] assetBundleMetaData= paragraph1.displayUI.Split(new string[]{"/"}, System.StringSplitOptions.RemoveEmptyEntries);
    //    string commonAssetBundleName = assetBundleMetaData[0];
    //    string asset = assetBundleMetaData[1];
    //    AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(commonAssetBundleName, asset, typeof(GameObject));
    //    if (request == null)
    //        yield break;
    //    yield return StartCoroutine(request);

    //    // Get the asset.
    //    GameObject prefab = request.GetAsset<GameObject>();

    //    if (prefab != null)
    //    {
    //        GameObject uiGameobject=(GameObject)GameObject.Instantiate(prefab, mainCanvas.transform, false); 

    //        request = AssetBundleManager.LoadAssetAsync(assetBundleName + ".metadata", paragraph1.textFile, typeof(TextAsset));
    //        if (request == null)
    //            yield break;
    //        yield return StartCoroutine(request);
    //        TextAsset textContent = request.GetAsset<TextAsset>();
    //        Debug.Log("textContent=" + textContent);

    //        // Text txtTile = uiGameobject.transform.Find("Object").Find("titleBox").Find("txt_title").gameObject.GetComponent<Text>();
    //       // Text txtTile = uiGameobject.GetComponentInChildren<Text>();
    //        Text[] textUis = uiGameobject.GetComponentsInChildren<Text>();
    //        foreach (Text textUi in textUis)
    //        {
    //            if(textUi.gameObject.name == "txt_title")
    //            {
    //                textUi.text = paragraph1.header;
    //            }
    //            else if(textUi.gameObject.name == "txt_info")
    //            {
    //                textUi.text = textContent.text;
    //            }
    //        }
          
    //    }

    //}

}
