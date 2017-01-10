using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Unity.Editor;
public class ProfileFirebase {
    private bool initedFirebase=false;
    private static ProfileFirebase _instance = null;
    Firebase.Auth.FirebaseAuth auth;
    private System.Action<bool> stateChangedCallback;

    public FirebaseUser user;
    private ProfileFirebase()
    {

    }
    public void getCurrentUser(System.Action callbackWhenDone)
    {
        if (!initedFirebase)
        {
            FirebaseHelper.getInstance().initFirebase(() =>
            {
                initedFirebase = true;
                callbackWhenDone();
            });
        }else
        {
            callbackWhenDone();
        }

    } 

    public static ProfileFirebase getInstance()
    {
        if (_instance == null)
        {
            _instance = new ProfileFirebase();
        }
        return _instance;
    }
    public void listenLoginStateChange(System.Action<bool> stateChangedCallback)
    {
        DebugOnScreen.Log("listenLoginStateChange ....");
        this.stateChangedCallback = stateChangedCallback;
        FirebaseHelper.getInstance().initFirebase(() => {
            DebugOnScreen.Log("init done ....");
            auth = FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;
        });
    }
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        DebugOnScreen.Log("AuthStateChanged ....");
        if (auth.CurrentUser != user)
        {
            if (user == null && auth.CurrentUser != null)
            {
                DebugOnScreen.Log("Signed in " + auth.CurrentUser.DisplayName);
            }
            //else if (user != null && auth.CurrentUser == null)
            //{
            //    Debug.Log("Signed out " + user.DisplayName);
            //}
            user = auth.CurrentUser;
            stateChangedCallback(true);
        }
        stateChangedCallback(false);
    }


    public void Login(string email, string password,System.Action<Task<Firebase.Auth.FirebaseUser>> resultCallback)
    {
        Debug.Log(String.Format("Attempting to sign in as {0}...", email));

        auth.SignInWithEmailAndPasswordAsync(email, password)
          .ContinueWith(resultCallback);
    }
    void HandleSigninResult(Task<Firebase.Auth.FirebaseUser> authTask)
    {

        if (authTask.IsCanceled)
        {
            Debug.Log("SignIn canceled.");
        }
        else if (authTask.IsFaulted)
        {
            Debug.Log("Login encountered an error.");
            Debug.Log(authTask.Exception.ToString());
        }
        else if (authTask.IsCompleted)
        {
            Debug.Log("Login completed.");
            Debug.Log("Signing out.");
            auth.SignOut();
        }
    }

}
