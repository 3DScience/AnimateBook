using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
public class FirebaseHelper  {

    private static FirebaseHelper _instance = null;
    private bool initiated = false;
    private FirebaseHelper()
    {

    }
    public static FirebaseHelper getInstance()
    {
        if (_instance == null)
        {
            _instance = new FirebaseHelper();
        }
        return _instance;
    }

    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    public void initFirebase(System.Action callbackWhenDone)
    {
        if (!initiated)
        {
            dependencyStatus = FirebaseApp.CheckDependencies();
            if (dependencyStatus != DependencyStatus.Available)
            {
                FirebaseApp.FixDependenciesAsync().ContinueWith(task =>
                {
                    dependencyStatus = FirebaseApp.CheckDependencies();
                    if (dependencyStatus == DependencyStatus.Available)
                    {
                        initiated = true;
                        _initFirebase(callbackWhenDone);
                    }
                    else
                    {
                        // This should never happen if we're only using Firebase Analytics.
                        // It does not rely on any external dependencies.
                        Debug.LogError(
                            "Could --  not resolve all Firebase dependencies: " + dependencyStatus);
                    }
                });
            }
            else
            {
                _initFirebase(callbackWhenDone);
            }
        }else
        {
            callbackWhenDone();
        }
    }
    private void _initFirebase(System.Action callbackWhenDone)
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;

        app.SetEditorDatabaseUrl("https://smallworld3d-2ac88.firebaseio.com/");
        //app.SetEditorP12FileName ("filebaseTest-2e653eef7319.p12");
        app.SetEditorServiceAccountEmail("smallworld3d-2ac88@appspot.gserviceaccount.com");
        //app.SetEditorP12Password ("2e653eef7319ed39d40ed0a6370d9d222bbb555a");

        //  _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        // Debug.Log("InitializeFirebase:  _databaseReference:" + _databaseReference);
        if (GlobalVar.DEBUG)
            DebugOnScreen.Log("InitializeFirebase done ....");
        initiated = true;
        callbackWhenDone();

    }

}
