using UnityEngine;
using System.Collections;
using AssetBundles;
public class AssetBundleHelper {
    private static AssetBundleHelper _instance=null;
    private string assetBundleName=null;
    private MonoBehaviour context;
    private AssetBundleHelper()
    {
        context = GlobalVar.shareContext;
    }
    public static AssetBundleHelper getInstance() 
    {
        if(_instance == null)
        {
            _instance = new AssetBundleHelper();
        }
        return _instance;
    }
    public IEnumerator InitializeAssetBunder(string assetBundleName)
    {
        if(this.assetBundleName!=null)
        {
            getInstance().unLoadAssetBundleManager();
        }

        // Don't destroy this gameObject as we depend on it to run the loading script.
        //DontDestroyOnLoad(context.gameObject);
        // With this code, when in-editor or using a development builds: Always use the AssetBundle Server
        // (This is very dependent on the production workflow of the project. 
        // 	Another approach would be to make this configurable in the standalone player.)
        #if DEVELOPMENT_BUILD
                        AssetBundleManager.SetDevelopmentAssetBundleServer();
        #else
                // Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
                //AssetBundleManager.SetSourceAssetBundleURL("http://192.168.0.201/unity3d/3dbook_test/");
                AssetBundleManager.SetSourceAssetBundleURL("file://" + GlobalVar.DATA_PATH + "/" + assetBundleName + "/");
                // Or customize the URL based on your deployment or configuration
                //AssetBundleManager.SetSourceAssetBundleURL("http://www.MyWebsite/MyAssetBundles");
        #endif

        this.assetBundleName=assetBundleName;
        // Initialize AssetBundleManifest which loads the AssetBundleManifest object.
        var request = AssetBundleManager.Initialize();

        if (request != null)
            yield return context.StartCoroutine(request);
    }
    public  IEnumerator LoadAsset<T>(string assetBundleName,string assetName,System.Action<T> callback) where T : Object
    {
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(T));
        if (request == null)
            yield break;
        yield return context.StartCoroutine(request);
        T asset = request.GetAsset<T>();
        callback(asset);
    }
    public IEnumerator LoadScene(string assetBundleName, string scenseName, bool isAddtive)
    {
        AssetBundleManager.UnloadAssetBundle(assetBundleName);
        AssetBundleLoadOperation request = AssetBundleManager.LoadLevelAsync(assetBundleName, scenseName, true);
        if (request == null)
            yield break;
        yield return context.StartCoroutine(request);
    }
    public void unLoadAssetBundleManager()
    {
        AssetBundleManager.UnloadAssetBundle(assetBundleName);
        GameObject assetBundleManager= GameObject.Find("AssetBundleManager");
        AssetBundleManager.DestroyObject(assetBundleManager);
    }
}
