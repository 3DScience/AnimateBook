using System.Collections;
using System.Collections.Generic;
#if !UNITY_WEBGL
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Facebook.Unity;
using UnityEngine.SceneManagement;

public class ProfilePanelController : MonoBehaviour
{
	public GameObject dialogUi;
    public GameObject loginPanel;
    public GameObject profilePanel;
	public GameObject loginButton;
	public GameObject profileButton;
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
			DebugOnScreen.Log ("FUCKKK 1111");
            txtEmail.text = ProfileFirebase.getInstance().auth.CurrentUser.Email;
			DebugOnScreen.Log("ProfilePanelController- OnLoginStateChange, txtEmail.text 111 = "+ txtEmail.text);
        }else
        {
			DebugOnScreen.Log("FUCKKK 22222");
            txtEmail.text = "";
			DebugOnScreen.Log("ProfilePanelController- OnLoginStateChange, txtEmail.text 222 = "+ txtEmail.text);
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
			LoginPanelController.islogin = false;

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
		dialogUi.SetActive (false);
		GlobalVar.login = 2;
		SceneManager.LoadScene(GlobalVar.MAINSCENE);
//        loginPanel.SetActive(true);
//		loginButton.SetActive (true);
//        loginPanel.GetComponent<RectTransform>().SetAsLastSibling();
//        profilePanel.SetActive(false);
//		profileButton.SetActive (false);
    }
}
#endif
