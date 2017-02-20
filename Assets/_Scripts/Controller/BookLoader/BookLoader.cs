using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BookLoader : MonoBehaviour {

    public static string assetBundleName;
    // Use this for initialization
    IEnumerator Start () {
        // GameObject loadingEffect =GameObject.Instantiate(AssetDataDase.LoadAssetAtPath("Assets/Prefabs/myPrefab.prefab", typeof(GameObject)) as GameObject;
        //GameObject textObject = (GameObject)Instantiate(Resources.Load("RichTextUi_1"));
        //  GameObject instance = Instantiate(Resources.Load("RichTextUi_1", typeof(GameObject))) as GameObject;


        if (assetBundleName == null || assetBundleName == "")
        {
            //assetBundleName = "test_book"; 
            assetBundleName = "solar_system_book";
        }
#if !UNITY_WEBGL
               yield return AssetBundleHelper.getInstance().InitializeAssetBunder(assetBundleName);
#else
        string assetBundleUrl = "vn/books/science/230002_solar_system_book/";
        yield return AssetBundleHelper.getInstance().InitializeAssetBunder(assetBundleUrl);
        assetBundleName = "solar_system_book";
        DebugOnScreen.Log("RUN PLATFORM WEBGL");
        Debug.Log("RUN PLATFORM WEBGL");
#endif

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

    }

    public void onHomeButtonClick()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("DestroyOnHome");
        foreach (var go in gos)
        {
            DestroyObject(go);
        }
        SceneManager.UnloadScene(GlobalVar.BOOK_LOADER_SCENE);
        SceneManager.LoadScene(GlobalVar.MAINSCENE);
    }
	

}
