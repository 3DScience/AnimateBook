using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_WEBGL // If the Ads service is not enabled...
using UnityEngine.Advertisements;
#endif

public class AdsController : MonoBehaviour {

    public string gameId = "1224976"; // Set this value from the inspector.
    public bool enableTestMode = false;

#if !UNITY_WEBGL // 
    IEnumerator Start()
    {
#if !UNITY_ADS // If the Ads service is not enabled...
        if (Advertisement.isSupported)
        { // If runtime platform is supported...
            Advertisement.Initialize(gameId, enableTestMode); // ...initialize.
        }
#endif

        // Wait until Unity Ads is initialized,
        //  and the default ad placement is ready.
        while (!Advertisement.isInitialized || !Advertisement.IsReady())
        {
            yield return new WaitForSeconds(0.5f);
        }

        // Show the default ad placement.
        var options = new ShowOptions { resultCallback = HandleShowResult };
        Advertisement.Show();
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
        Destroy(this);
    }
#endif
}
