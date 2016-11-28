using UnityEngine;
using System.Collections;
using Entities;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class DisplayTextUiController : MonoBehaviour {

    public AssetBundleHelper assetBundleHelper;
    private static Dictionary<string, GameObject> loadedPrefab = new Dictionary<string, GameObject>();
    public string metadataAssetBundleName;
    public Canvas mainCanvas;
    private bool hideOnTouchNothing=true;
    private GameObject currentTextUIGameObject;
    // Use this for initialization
    void Start () {
        

    }
    public void onTouchNothing()
    {
        if (Debug.isDebugBuild)
            Debug.Log("handleOnTouchNothing.");

        if (currentTextUIGameObject != null && currentTextUIGameObject.activeSelf)
        {
            currentTextUIGameObject.SetActive(false);
        }
    }
    public  IEnumerator showTextUi(MainObject mainObject,bool hideOnTouchNothing, System.Action explorerButtonClick)
    {
        this.hideOnTouchNothing = hideOnTouchNothing;
        TextContent textContent = mainObject.texts[mainObject.currentTextIndex];
        GameObject uiGameobject = null;
        string textui = textContent.displayUI;
        if (loadedPrefab.ContainsKey(textui))
        {
            if(Debug.isDebugBuild)
              Debug.Log("[DisplayTextUiController-showTextUi] Got uiGameobject from Dictionary");
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
        }
        if (uiGameobject != null)
        {
            currentTextUIGameObject = uiGameobject;
            uiGameobject.SetActive(true);
            TextAsset textAsset = null;
            yield return assetBundleHelper.LoadAsset<TextAsset>(metadataAssetBundleName, textContent.textFile, textAssetLoaded => {
                textAsset = textAssetLoaded;
            });
            Debug.Log("textContent=" + textContent);

            // Text txtTile = uiGameobject.transform.Find("Object").Find("titleBox").Find("txt_title").gameObject.GetComponent<Text>();
            // Text txtTile = uiGameobject.GetComponentInChildren<Text>();
            Text[] textUis = uiGameobject.GetComponentsInChildren<Text>();
            foreach (Text textUi in textUis)
            {
                if (textUi.gameObject.name == "txt_title")
                {
                    textUi.text = textContent.header;
                }
                else if (textUi.gameObject.name == "txt_info")
                {
                    textUi.text = textAsset.text;
                }
            }
            // now we add eventtrigger for explorerButton
            Transform tranform_explorerButton = uiGameobject.transform.FindChild("explorerButton");
            if( tranform_explorerButton != null)
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
            Debug.Log("DisplayTextUiController Script was destroyed <====================");
        loadedPrefab.Clear();
    }
}
