using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogMessageController : MonoBehaviour {
    public Text txtMessage;
    public Text buttonText;
    public GameObject backButton;
    public System.Action onOkButtonClickCallback;
    public System.Action onBackButtonClickCallback;
    // Use this for initialization
    void Start () {
		
	}
    public void setActiveBackButton(bool active)
    {
        backButton.SetActive(active);
    }
    public void setButtonText(string text)
    {
        buttonText.text = text;
    }
    public void setMessage(string text)
    {
        txtMessage.text = text;
    }
    public void onOkButtonClickHandle()
    {
        onOkButtonClickCallback();
    }
    public void onBackButtonClickHandle()
    {
        onBackButtonClickCallback();
    }
    public void setActive(bool active)
    {
        gameObject.transform.parent.gameObject.SetActive(active);
    }
    public void DestroyGo()
    {
        DestroyObject(gameObject.transform.parent.gameObject);
        DestroyObject(gameObject);
    }
}
