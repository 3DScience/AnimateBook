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
    public FirebaseAuth auth;
    private FirebaseUser user;
    private System.Action<bool> stateChangedCallback;


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
                auth = FirebaseAuth.DefaultInstance;
                user = auth.CurrentUser;
                auth.StateChanged += AuthStateChanged;
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
        if (GlobalVar.DEBUG)
            DebugOnScreen.Log("ProfileFirebase- Add listenLoginStateChange ....");
        this.stateChangedCallback = stateChangedCallback;
 
    }
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        //DebugOnScreen.Log("AuthStateChanged ");
        //if (user == null && auth.CurrentUser != null)
        //{
        //    DebugOnScreen.Log("Signed in " + auth.CurrentUser.Email);
        //    user = auth.CurrentUser;
        //    if (stateChangedCallback != null)
        //        stateChangedCallback(true);
        //}
        //else
        //{
        //    DebugOnScreen.Log("Signed out " + user.Email);
        //    if (stateChangedCallback != null)
        //        stateChangedCallback(false);
        //}
        if (GlobalVar.DEBUG)
            DebugOnScreen.Log("ProfileFirebase- AuthStateChanged -auth.CurrentUser= " + auth.CurrentUser);
        if (auth.CurrentUser != user)
        {
            if (user == null && auth.CurrentUser != null)
            {
                if (GlobalVar.DEBUG)
                    DebugOnScreen.Log("ProfileFirebase- AuthStateChanged Signed in " + auth.CurrentUser.Email);
                if (stateChangedCallback != null)
                    stateChangedCallback(true);
            }
            else if (user != null && auth.CurrentUser == null)
            {
                if (GlobalVar.DEBUG)
                    DebugOnScreen.Log("ProfileFirebase- AuthStateChanged- Signed out " + user.Email);
                user = null;
                if (stateChangedCallback != null)
                    stateChangedCallback(false);
            }else
            {
                if (GlobalVar.DEBUG)
                    DebugOnScreen.Log("ProfileFirebase- AuthStateChanged- else");
            }
            user = auth.CurrentUser;
        }else
        {
            if (GlobalVar.DEBUG)
                DebugOnScreen.Log("ProfileFirebase- AuthStateChanged- auth.CurrentUser == user ");
        }
    }


    public void Login(string email, string password,System.Action<Task<Firebase.Auth.FirebaseUser>> resultCallback)
    {
        if (GlobalVar.DEBUG)
            Debug.Log(String.Format("Attempting to sign in as {0}...", email));

        auth.SignInWithEmailAndPasswordAsync(email, password)
          .ContinueWith(resultCallback);
    }


}
