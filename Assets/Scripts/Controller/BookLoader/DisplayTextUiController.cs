using UnityEngine;
using System.Collections;
using Entities;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class DisplayTextUiController : MonoBehaviour {
    private string NAME_BUTTON_LAYOUT = "buttonLayout";
    private string NAME_EXPLORER_BUTTON = "explorerButton";
    private string NAME_BACK_BUTTON = "backButton";
    private string NAME_PRE_BUTTON = "buttonPre";
    private string NAME_NEXT_BUTTON = "buttonNext";
    private string NAME_TEXT_TITLE = "txt_title";
    private string NAME_TEXT_HEADER = "txtHeader";
    private string NAME_TEXT_BODY = "txt_info";
    private string NAME_SCROLL_REC = "infomationBox/ScroolRect";

    public AssetBundleHelper assetBundleHelper;
    private static Dictionary<string, GameObject> loadedPrefab = new Dictionary<string, GameObject>();
    private static Dictionary<string, string> loadedTexfile = new Dictionary<string, string>();
    public string metadataAssetBundleName;
    public Canvas mainCanvas;
    private bool hideOnTouchNothing=true;
    private GameObject currentTextUIGameObject;
    private MainObject currentManinObject;
    // Use this for initialization
    void Start () {
        TouchEventControler touchEventControler = gameObject.GetComponent<TouchEventControler>();
        if (touchEventControler != null)
        {
            touchEventControler.onTouchNothing = onTouchNothing;
        }

    }
    public void onTouchNothing()
    {
        if (Debug.isDebugBuild)
            Debug.Log("handleOnTouchNothing.");

        hideTextUi();
    }
    public void hideTextUi()
    {
        if (currentTextUIGameObject != null && currentTextUIGameObject.activeSelf)
        {
            currentTextUIGameObject.SetActive(false);
            Camera.main.GetComponent<CameraController_1>().OnMainObjectUnTouched();
        }
    }
    public  IEnumerator showTextUi(MainObject mainObject,bool hideOnTouchNothing, System.Action explorerButtonClick)
    {
        
        if(currentManinObject!=null && currentManinObject.Equals(mainObject) && currentTextUIGameObject.activeSelf)
        {
            if (Debug.isDebugBuild)
                Debug.Log("[DisplayTextUiController-showTextUi] show text already!");
            hideTextUi();
            yield break;
        }
        currentManinObject = mainObject;
        this.hideOnTouchNothing = hideOnTouchNothing;
        TextContent textContent = mainObject.texts[mainObject.currentTextIndex];
        GameObject uiGameobject = null;
        string textui = textContent.displayUI;
        if (loadedPrefab.ContainsKey(textui))
        {
            if(Debug.isDebugBuild)
              Debug.Log("[DisplayTextUiController-showTextUi] Got uiGameobject from cache!");
            uiGameobject = loadedPrefab[textui];
        }
        else
        {
            if (Debug.isDebugBuild)
                Debug.Log("[DisplayTextUiController-showTextUi] Load uiGameobject from assetBundle!");
            GameObject prefab = null;
            string[] assetBundleMetaData = textui.Split(new string[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries);
            string commonAssetBundleName = assetBundleMetaData[0];
            string assetName = assetBundleMetaData[1];

            yield return assetBundleHelper.LoadAsset<GameObject>(commonAssetBundleName, assetName, prefabLoaded =>
            {
                prefab = prefabLoaded;
            });
            if (prefab != null)
            {
                uiGameobject = (GameObject)GameObject.Instantiate(prefab, mainCanvas.transform, false);
                loadedPrefab.Add(textui, uiGameobject);
            }
            AddEventTriggerToButtons(uiGameobject);
        }
        if (uiGameobject != null)
        {
            currentTextUIGameObject = uiGameobject;
            // now we add eventtrigger for explorerButton
            AddEventTrigeerToExplorerButton(uiGameobject, explorerButtonClick);
            yield return loadTextToUi(uiGameobject, textContent);
 
            if(Camera.main.GetComponent<CameraController_1>())
                Camera.main.GetComponent<CameraController_1> ().OnMainObjectTouched ();
        }
    }
    private IEnumerator loadTextToUi(GameObject uiGameobject,TextContent textContent)
    {
        uiGameobject.SetActive(true);
        string text = null;
        if (loadedTexfile.ContainsKey(textContent.textFile))
        {
            if (Debug.isDebugBuild)
                Debug.Log("[DisplayTextUiController-loadTextToUi] load text from cache." );
            text = loadedTexfile[textContent.textFile];
        }
        else
        {
            yield return assetBundleHelper.LoadAsset<TextAsset>(metadataAssetBundleName, textContent.textFile, textAssetLoaded =>
            {
                text = textAssetLoaded.text;
                loadedTexfile.Add(textContent.textFile, text);
            });
        }
        // Text txtTile = uiGameobject.transform.Find("Object").Find("titleBox").Find("txt_title").gameObject.GetComponent<Text>();
        // Text txtTile = uiGameobject.GetComponentInChildren<Text>();
        Text[] textUis = uiGameobject.GetComponentsInChildren<Text>();
        foreach (Text textUi in textUis)
        {
            if (textUi.gameObject.name == NAME_TEXT_TITLE)
            {
                textUi.text = currentManinObject.Title;
            }
            else if (textUi.gameObject.name == NAME_TEXT_HEADER)
            {
                textUi.text = textContent.header;
            }
            else if (textUi.gameObject.name == NAME_TEXT_BODY)
            {
                textUi.text = text;
            }
        }
        forceScrollTop(uiGameobject);
    }
    void forceScrollTop(GameObject uiGameobject) 
    {
      //  yield return new WaitForSeconds(0.1f);
        Canvas.ForceUpdateCanvases();
        Transform panel = uiGameobject.transform.Find(NAME_SCROLL_REC);
        panel.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
        Canvas.ForceUpdateCanvases();
    }
    private void AddEventTrigeerToExplorerButton(GameObject uiGameobject, System.Action explorerButtonClick)
    {
        Transform tranform_explorerButton = uiGameobject.transform.FindChild(NAME_BUTTON_LAYOUT + "/" + NAME_EXPLORER_BUTTON);
        if (tranform_explorerButton != null)
        {
            GameObject explorerButton = tranform_explorerButton.gameObject;
            EventTrigger explorerButtonEvenTrigger = explorerButton.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.callback.RemoveAllListeners();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener((eventData) => { explorerButtonClick(); });
            explorerButtonEvenTrigger.triggers.Clear();
            explorerButtonEvenTrigger.triggers.Add(entry);
        }
    }
    private void AddEventTriggerToButtons(GameObject uiGameobject)
    {
        Transform tranformButton = uiGameobject.transform.FindChild(NAME_BUTTON_LAYOUT + "/" + NAME_PRE_BUTTON);
        if (tranformButton != null)
        {
            if (Debug.isDebugBuild)
                Debug.Log("[DisplayTextUiController-showTextUi] AddButtonEventTrigger for pre   ");
            GameObject explorerButton = tranformButton.gameObject;
            EventTrigger explorerButtonEvenTrigger = explorerButton.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.callback.RemoveAllListeners();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener((eventData) => { onPreButtonClick(); });
            explorerButtonEvenTrigger.triggers.Clear();
            explorerButtonEvenTrigger.triggers.Add(entry);
        }
        tranformButton = uiGameobject.transform.FindChild(NAME_BUTTON_LAYOUT + "/" + NAME_NEXT_BUTTON);
        if (tranformButton != null)
        {
            GameObject explorerButton = tranformButton.gameObject;
            EventTrigger explorerButtonEvenTrigger = explorerButton.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.callback.RemoveAllListeners();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener((eventData) => { onNextButtonClick(); });
            explorerButtonEvenTrigger.triggers.Clear();
            explorerButtonEvenTrigger.triggers.Add(entry);
        }
    }
    private void onPreButtonClick()
    {
        if (Debug.isDebugBuild)
            Debug.Log("[DisplayTextUiController-showTextUi] onPreButtonClick");
        int idx = currentManinObject.currentTextIndex;
        idx--;
        if (idx<0)
        {
            return;
        }
        currentManinObject.currentTextIndex = idx;
        TextContent textContent = currentManinObject.texts[currentManinObject.currentTextIndex];
        StartCoroutine(loadTextToUi(currentTextUIGameObject, textContent));
    }
    private void onNextButtonClick() {
        if (Debug.isDebugBuild)
            Debug.Log("[DisplayTextUiController-showTextUi] onNextButtonClick");
        int idx = currentManinObject.currentTextIndex;
        idx++;
        if (idx > currentManinObject.texts.Length-1)
        {
            return;
        }
        currentManinObject.currentTextIndex= idx;
        TextContent textContent = currentManinObject.texts[currentManinObject.currentTextIndex];
        StartCoroutine(loadTextToUi(currentTextUIGameObject, textContent));
    }
    public void onChangeScene()
    {
        if (currentTextUIGameObject != null && currentTextUIGameObject.activeSelf)
        {
            currentTextUIGameObject.SetActive(false);
        }
    }
    //private void addEventTrigger
    void OnDestroy()
    {
        if (Debug.isDebugBuild)
            Debug.Log("[DisplayTextUiController] Script was destroyed <====================");
        loadedPrefab.Clear();
        loadedTexfile.Clear();
    }
}
