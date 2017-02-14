using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Facebook.Unity;

public class ProfilePanelController : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject profilePanel;
    public Text txtEmail;
    public Text txtCash;
    // Use this for initialization
    void Start()
    {
        if (GlobalVar.DEBUG)
        {
            Debug.Log("Onstart");
            DebugOnScreen.Log("ProfilePanelController- Onstart ");
        }
        //loadUser();
        ProfileFirebase.getInstance().listenLoginStateChange(OnLoginStateChange);
    }
    public void OnLoginStateChange(bool logedin)
    {
        if (GlobalVar.DEBUG)
            DebugOnScreen.Log("ProfilePanelController- OnLoginStateChange, logedin= "+ logedin);
        if (logedin)
        {
            txtEmail.text = ProfileFirebase.getInstance().auth.CurrentUser.Email;
        }else
        {
            txtEmail.text = "";
        }
    }
    void loadUser()
    {
       // txtEmail.text = ProfileFirebase.getInstance().user.Email;
    }


    public void OnLogOutButtonClick()
    {
        if (GlobalVar.DEBUG)
            DebugOnScreen.Log("ProfilePanelController- OnLogOutButtonClick");
        try
        {
            ProfileFirebase.getInstance().auth.SignOut();
			FB.LogOut();
            deactiveProfilePanel();
        }
        catch (System.Exception ex)
        {
            DebugOnScreen.Log(ex.Message);
            DebugOnScreen.Log(ex.ToString());
        }

    }

    public void deactiveProfilePanel()
    {
        loginPanel.SetActive(true);
        loginPanel.GetComponent<RectTransform>().SetAsLastSibling();
        profilePanel.SetActive(false);
    }
}
