using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    }
	

}
