using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
public class IndexPageDialogController : MonoBehaviour {

	public GameObject dialogUi;

	void Start () {
		dialogUi.SetActive(false);
		DontDestroyOnLoad(gameObject.transform.parent.gameObject);
		//ProfileFirebase.getInstance().listenLoginStateChange(stateChangedCallback);

	}

	public void OnSettingClick()
	{
		Debug.Log("OnMouseDown");
		if ( !dialogUi.activeSelf)
		{
			dialogUi.SetActive(true);
		}
	}


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

}
