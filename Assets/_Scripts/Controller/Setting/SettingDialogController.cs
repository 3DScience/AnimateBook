using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SettingDialogController : MonoBehaviour {

    public GameObject dialogUi;

    public Text txtEmail;
    public Text txtPassword;
    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject.transform.parent.gameObject);
        ProfileFirebase.getInstance().listenLoginStateChange(stateChangedCallback);

    }
    public void OnSettingClick()
    {
        Debug.Log("OnMouseDown");
        if ( !dialogUi.activeSelf)
        {
            dialogUi.SetActive(true);
        }


    }

    void stateChangedCallback(bool logined)
    {
        DebugOnScreen.Log("stateChangedCallback logined="+ logined);
    }
    // Track state changes of the auth object.


    // Update is called once per frame
    void Update () {
		
	}

    public void onCloseButtonClick()
    {
        dialogUi.SetActive(false);
    }
    public void OnLoginButtonClick()
    {
        DebugOnScreen.Log("OnLoginButtonClick, email="+txtEmail.text+ "/ pass="+txtPassword.text);
        ProfileFirebase.getInstance().Login(txtEmail.text, txtPassword.text, HandleSigninResult);
    }

    void HandleSigninResult(Task<Firebase.Auth.FirebaseUser> authTask)
    {

        if (authTask.IsCanceled)
        {
            DebugOnScreen.Log("SignIn canceled.");
        }
        else if (authTask.IsFaulted)
        {
            DebugOnScreen.Log("Login encountered an error.");
            DebugOnScreen.Log(authTask.Exception.ToString());
        }
        else if (authTask.IsCompleted)
        {
            DebugOnScreen.Log("Login completed.");
            DebugOnScreen.Log("Signing out.");
 
        }
    }
    public void OnEdgeClick()
    {
        Debug.Log("OnEdgeClick");
        dialogUi.SetActive(false);
    }
    public void ontest()
    {
        Debug.Log("ontest");
    }
}
