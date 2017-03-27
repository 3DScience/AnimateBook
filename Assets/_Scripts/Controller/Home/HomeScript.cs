﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
public class HomeScript : MonoBehaviour {
    public GameObject settingDialogPrefab;
	public GameObject uiLogin;
    private bool selectedCategory=false;
	ArrayList listBookInfo = new ArrayList();
	// Use this for initialization
	void Start () {

        if(GlobalVar.SETTING_DIALOG == null)
        {
            GlobalVar.SETTING_DIALOG = GameObject.Instantiate(settingDialogPrefab);
        }
		if (GlobalVar.login == 2) {
			DebugOnScreen.Log("Home : " + GlobalVar.login);
			uiLogin.SetActive(true);
			//GameObject.Instantiate (uiLogin);
		} else if (GlobalVar.login == 1) {
			DebugOnScreen.Log("Home : " + GlobalVar.login);
			//GameObject.DestroyObject (uiLogin);
			uiLogin.SetActive(false);
		}

        if (Debug.isDebugBuild)
            Debug.Log("HomeScript Start...");
        loadAllCategory();

        Analytics.CustomEvent("Scene", new Dictionary<string, object>
      {
        { "user", "user1" },
        { "scene", "Home" }
      });

    }
    void loadAllCategory()
    {
        GameObject animal_book = GameObject.Find("animal_book");
        CategoryInfo cat1 = animal_book.AddComponent<CategoryInfo>();
        cat1.index = 0;
		cat1.categoryName = "nature";
        cat1.callback = OnSelectedBook;
        listBookInfo.Add(cat1);

        GameObject fairy_book = GameObject.Find("fairy_book");
        CategoryInfo cat2 = fairy_book.AddComponent<CategoryInfo>();
        cat2.index = 1;
		cat2.categoryName = "fairytale";
        cat2.callback = OnSelectedBook;
        listBookInfo.Add(cat2);

        GameObject science_book = GameObject.Find("science_book");
        CategoryInfo cat3 = science_book.AddComponent<CategoryInfo>();
        cat3.index = 2;
		cat3.categoryName = "science";
        cat3.callback = OnSelectedBook;
        listBookInfo.Add(cat3);

        GameObject fiction_book = GameObject.Find("fiction_book");
        CategoryInfo cat4 = fiction_book.AddComponent<CategoryInfo>();
        cat4.index = 3;
		cat4.categoryName = "fiction";
        cat4.callback = OnSelectedBook;
        listBookInfo.Add(cat4);

    }
    void OnSelectedBook(int index)
    {
        if (Debug.isDebugBuild)
            Debug.Log("OnSelectedBook "+ index);
        if (selectedCategory)
            return;

        selectedCategory = true;
        CategoryInfo categoryInfo = (CategoryInfo)listBookInfo[index];
#if !UNITY_WEBGL
		BookListController.catName = categoryInfo.categoryName;
        LoadBookSelected(categoryInfo.categoryName);
#endif
    }
    public void ButtonClick(string assetBundleName)
    {
        Debug.Log("ButtonClick1 ");
        if (Debug.isDebugBuild)
        {

        }
            
    }
    public void LoadBookSelected(string categoryName)
    {
        Category.categoryName = categoryName;
        StartCoroutine(loadSceneWithAnimation(GlobalVar.CATEGORY_SCENE));

    }
    IEnumerator loadSceneWithAnimation(string senceName)
    {
 
        GameObject dbook = GameObject.Find("3dbook");
        Vector3 center = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, dbook.transform.position.z));
        iTween.MoveTo(dbook, iTween.Hash("position", center, "time", 0.8f));
        Vector3 scale = dbook.transform.localScale;
        iTween.ScaleTo(dbook, iTween.Hash("scale", scale * 2, "time", 0.8));
        iTween.CameraFadeAdd();
        iTween.CameraFadeTo(0.5f, 2);
        yield return new WaitForSeconds(0.8f);
      //  Application.LoadLevel(senceName);
        SceneManager.LoadScene(senceName);
        yield return null;
    }

     
}
