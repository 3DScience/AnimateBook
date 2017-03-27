using System.Collections;
using System.Collections.Generic;
#if !UNITY_WEBGL
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
public class SettingDialogController : MonoBehaviour {

    public GameObject dialogUi;
    public GameObject loginPanel;
    private LoginPanelController loginPanelController;
    private ProfilePanelController profilePanelController;
    // Use this for initialization
    void Start () {
        dialogUi.SetActive(false);
        DontDestroyOnLoad(gameObject.transform.parent.gameObject);
        //ProfileFirebase.getInstance().listenLoginStateChange(stateChangedCallback);

        loginPanelController = loginPanel.GetComponent<LoginPanelController>();
        profilePanelController = GetComponent<ProfilePanelController>();
        ProfileFirebase.getInstance().getCurrentUser(gettedUser);

    }

    void gettedUser()
    {
        FirebaseUser user = ProfileFirebase.getInstance().auth.CurrentUser;
        if (user != null)
        {
			//GlobalVar.login = 1;
            //if (GlobalVar.DEBUG)
                DebugOnScreen.Log("SettingDialogController-CurrentUser: Email=" + user.Email + ", DisplayName=" + user.DisplayName);
            loginPanelController.deactiveLoginPanel();
            profilePanelController.OnLoginStateChange(true);
        }
        else
        {
			//GlobalVar.login = 2;
			//DebugOnScreen.Log("SettingDialogController-CurrentUser= null " + GlobalVar.login);
            profilePanelController.deactiveProfilePanel();
            //if (GlobalVar.DEBUG)
        }
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

#endif