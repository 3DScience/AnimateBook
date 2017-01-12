using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;

public class LoginPanelController : MonoBehaviour {
    public GameObject loginPanel;
    public GameObject profilePanel;
    public Text txtEmail;
    public Text txtPassword;
    public Text txtError;
    public GameObject loadingPanel;
    void OnEnable()
    {

        if (GlobalVar.DEBUG)
        DebugOnScreen.Log("OnEnable- OnBecameVisible     ");
        txtError.gameObject.SetActive(false);
    }
        // Use this for initialization
        void Start () {
        if (GlobalVar.DEBUG)
            DebugOnScreen.Log("LoginPanelController- Onstart ");
    }
    public void OnLoginButtonClick()
    {
        loadingPanel.SetActive(true);
        if (GlobalVar.DEBUG)
            DebugOnScreen.Log("LoginPanelController- OnLoginButtonClick, email=" + txtEmail.text + "/ pass=" + txtPassword.text);
        ProfileFirebase.getInstance().Login(txtEmail.text, txtPassword.text, HandleSigninResult);
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
        loginPanel.SetActive(false);
        profilePanel.SetActive(true);
        profilePanel.GetComponent<RectTransform>().SetAsLastSibling();
    }
}
