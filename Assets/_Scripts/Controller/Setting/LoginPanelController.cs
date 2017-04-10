﻿using System.Collections;
using System.Collections.Generic;
#if !UNITY_WEBGL
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Facebook.Unity;
using Firebase.Database;
using UnityEngine.SceneManagement;

public class LoginPanelController : MonoBehaviour {
	public GameObject uiLogin;
	public GameObject usersLoginPanel;
	public GameObject usersLoginButton;
    public GameObject loginPanel;
    public GameObject profilePanel;
	public GameObject loginButton;
	public GameObject profileButton;
    public Text txtEmail;
    public InputField txtPassword;
    public Text txtError;
    public GameObject loadingPanel;
    void OnEnable()
    {

        if (GlobalVar.DEBUG)
            DebugOnScreen.Log("LoginPanelController-OnEnable- OnBecameVisible     ");
        txtError.gameObject.SetActive(false);
    }
        // Use this for initialization
        void Start () {
        if (GlobalVar.DEBUG)
            DebugOnScreen.Log("LoginPanelController- Onstart ");
    }

	void Awake ()
	{
		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			DebugOnScreen.Log("Initialize the Facebook SDK ");
			FB.Init(InitCallback, OnHideUnity);
		} else {
			// Already initialized, signal an app activation App Event
			DebugOnScreen.Log("Already initialized, signal an app activation App Event");
			FB.ActivateApp();
		}
	}
	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
		} else {
			DebugOnScreen.Log("Failed to Initialize the Facebook SDK");
		}
	}
	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	public void OnUsersLoginButtonClick()
	{
		usersLoginPanel.SetActive (true);
	}

	public void DisableUsersLoginPanel()
	{
		usersLoginPanel.SetActive (false);
	}

	public void OnLoginButtonGuestClick()
	{
			if (GlobalVar.DEBUG)
				DebugOnScreen.Log ("OnLoginButtonGuestClick");
			ProfileFirebase.getInstance ().loginAsAnnonymousUser (userInfo => {
				//DebugOnScreen.Log("OnLoginButtonGuestClick- loginAsAnnonymousUser :: " +userInfo.userID);
				deactiveLoginPanel();
			});
	}

	void HandleAction (UserInfo obj)
	{
		
	}

	public static bool islogin = false;
    public void OnLoginButtonClick()
    {
        loadingPanel.SetActive(true);
        if (GlobalVar.DEBUG)
            DebugOnScreen.Log("LoginPanelController- OnLoginButtonClick, email=" + txtEmail.text + "/ pass=" + txtPassword.text);
			ProfileFirebase.getInstance ().Login (txtEmail.text, txtPassword.text, HandleSigninResult);
    }
		
	public void OnLoginButtonFBClick()
	{
		if (islogin == false) {
			islogin = true;
			var perms = new List<string> (){ "public_profile", "email", "user_friends" };
			FB.LogInWithReadPermissions (perms, AuthCallback);
		}
	}
	private void AuthCallback (ILoginResult result) {
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			//var aToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
			// Print current access token's User ID
			//DebugOnScreen.Log(aToken.UserId);
			//DebugOnScreen.Log (aToken);
			// Print current access token's granted permissions
//			foreach (string perm in aToken.Permissions) {
//				DebugOnScreen.Log(perm);
//			}

			ProfileFirebase.getInstance ().LoginWithFaceBook (aToken.TokenString, HandleSigninResult);

		} else {
			islogin = false;
			Debug.Log("User cancelled login");
		}
	}

    void HandleSigninResult(Task<Firebase.Auth.FirebaseUser> authTask)
    {

        loadingPanel.SetActive(false);
        if (authTask.IsCanceled)
        {
            if (GlobalVar.DEBUG)
                DebugOnScreen.Log("LoginPanelController- SignIn canceled.");
        }
        else if (authTask.IsFaulted)
        {
            if (GlobalVar.DEBUG)
            {
                DebugOnScreen.Log("LoginPanelController- Login encountered an error.");
                DebugOnScreen.Log(authTask.Exception.ToString());
            }
            txtError.text = "Your email or password is not correct!";
            txtError.gameObject.SetActive(true);
        }
        else if (authTask.IsCompleted)
        {
            if (GlobalVar.DEBUG)
                DebugOnScreen.Log("LoginPanelController- Login completed.");
            deactiveLoginPanel();
        }
    }

    public void deactiveLoginPanel()
    {
		//uiLogin.SetActive (false);
		GlobalVar.login = 1;
		SceneManager.LoadScene(GlobalVar.MAINSCENE);
//        loginPanel.SetActive(false);
//		loginButton.SetActive (false);
//        profilePanel.SetActive(true);
//		profileButton.SetActive (true);
//        profilePanel.GetComponent<RectTransform>().SetAsLastSibling();
    }
		

}
#endif