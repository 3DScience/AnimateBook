using UnityEngine;
using System.Collections;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum ChangingPageType
{
    NEXT,
    BACK
}
public class Category :GameObjectTouchedEvent {
    public static string categoryName;
    public GameObject book, page_left, page_right, flip_pageleft, flip_pageright;
    public GameObject contexUiPanel;
    // Use this for initialization
    void Start () {
        book = GameObject.Find("book");
        page_left = GameObject.Find("page_left");
        page_right = GameObject.Find("page_right");
        flip_pageleft = GameObject.Find("flip_pageleft");
        flip_pageright = GameObject.Find("flip_pageright");

        SwipeController sw = gameObject.GetComponent<SwipeController>();
        sw.onSwipeCallBack = OnSwipe;
    }
    void OnSwipe(SwipeController.SwipeType type)
    {
        if (Debug.isDebugBuild)
            Debug.Log("OnSwipe type=" + type);
        switch (type)
        {
            case SwipeController.SwipeType.LEFT:
                ChangePage(ChangingPageType.NEXT);
                break;
            case SwipeController.SwipeType.RIGT:
                ChangePage(ChangingPageType.BACK);
                break;
            case SwipeController.SwipeType.UP:
                break;
            case SwipeController.SwipeType.DOWN:
                break;
            case SwipeController.SwipeType.TOUCH:
                loadBook();
                break;
            default:
                break;
        }
    }
    private void loadBook()
    {
        //String assetBundleName= "test_book";
        String assetBundleName= "solar_system_book";
        if (checkIsDownloadedAsset(assetBundleName))
        {
            BookLoaderScript.assetBundleName = assetBundleName;
            SceneManager.LoadScene(GlobalConfig.BOOK_LOADER_SCENE);
        }
        else
        {
            SceneManager.LoadScene(GlobalConfig.DOWNLOAD_ASSET_SCENE);
        }
    }
    void OnNavigationButtonClick(string param)
    {
        if (Debug.isDebugBuild)
            Debug.Log("navigationButtonClick, param=" + param);

        if(param.Equals("NEXT"))
        {

            if (Debug.isDebugBuild)
                Debug.Log("navigationButtonClick, change page to NEXT");
            ChangePage(ChangingPageType.NEXT);
        }
        else if (param.Equals("BACK"))
        {
            if (Debug.isDebugBuild)
                Debug.Log("navigationButtonClick, change page to BACK");
            ChangePage(ChangingPageType.BACK);
        }
        else if (param.Equals("HOME"))
        {
            if (Debug.isDebugBuild)
                Debug.Log("navigationButtonClick, change page to HOME");
            SceneManager.LoadScene(GlobalConfig.MAINSCENE);
        }
    }
    void OnBookClick(string param)
    {
        if (Debug.isDebugBuild)
            Debug.Log("bookClick param=" + param);
        SceneManager.LoadScene(GlobalConfig.BOOK_LOADER_SCENE);
    }
    public void OnButtonCick(String action)
    {
        if (Debug.isDebugBuild)
            Debug.Log("OnButtonCick action=" + action);
        if (action.Equals("redownload"))
        {
            SceneManager.LoadScene(GlobalConfig.DOWNLOAD_ASSET_SCENE);
        }
        else if (action.Equals("deleteBook"))
        {
            
        }
    }
    protected void ChangePage(ChangingPageType type )
    {
        Material originMaterial, targeMaterialLeft, targeMaterialRight;
        if( type == ChangingPageType.BACK)
        {
            book.GetComponent<Animation>().Play("To left");
            originMaterial = Resources.Load("page2_L", typeof(Material)) as Material;
            targeMaterialLeft = Resources.Load("page1_L", typeof(Material)) as Material;
            targeMaterialRight = Resources.Load("page1_R", typeof(Material)) as Material;


            flip_pageleft.GetComponent<Renderer>().material = targeMaterialRight;
            flip_pageright.GetComponent<Renderer>().material = originMaterial;
            StartCoroutine(delayAddPage(page_left, page_right,targeMaterialLeft, targeMaterialRight));
        }
        else
        {
            book.GetComponent<Animation>().Play("To right");
            originMaterial = Resources.Load("page1_R", typeof(Material)) as Material;
            targeMaterialLeft = Resources.Load("page2_L", typeof(Material)) as Material;
            targeMaterialRight = Resources.Load("page2_R", typeof(Material)) as Material;

            flip_pageleft.GetComponent<Renderer>().material = originMaterial;
            flip_pageright.GetComponent<Renderer>().material = targeMaterialLeft;
             StartCoroutine(delayAddPage(page_right, page_left, targeMaterialRight, targeMaterialLeft));
        }
       
    }

    IEnumerator delayAddPage(GameObject taget1, GameObject taget2, Material pl, Material pr)
    {
        yield return new WaitForSeconds(0.25f);
        taget1.GetComponent<Renderer>().material = pl;
        yield return new WaitForSeconds(0.69f);
        taget2.GetComponent<Renderer>().material = pr;
    }

    public override void OnMouseDown(string action, string param)
    {
        if (Debug.isDebugBuild)
            Debug.Log("OnMouseDown action=" + action);

        if (action.Equals("navigationButtonClick"))
        {
            OnNavigationButtonClick(param);
        }else if (action.Equals("Setting"))
        {
            if (contexUiPanel.activeSelf)
            {
                
                contexUiPanel.SetActive(false);
            }
            else
            {
                contexUiPanel.SetActive(true);
            }
        }
    }

    private bool checkIsDownloadedAsset(string assetBundleName)
    {
        string assetDataFolder = GlobalConfig.DATA_PATH + "/" + assetBundleName;
        if (Directory.Exists(assetDataFolder))
        {
            return true;
        }
        return false;
    }
}
