using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;
public class HomeScript : MonoBehaviour {
    ArrayList listBookInfo = new ArrayList();
	// Use this for initialization
	void Start () {
        if (Debug.isDebugBuild)
            Debug.Log("HomeScript Start...");
        loadAllBook();
    }
    void loadAllBook()
    {
        GameObject animal_book = GameObject.Find("animal_book");
        BookInfo book1 = animal_book.AddComponent<BookInfo>();
        book1.index = 0;
        book1.bookName = "solar_system_book";
        listBookInfo.Add(book1);

        GameObject fairy_book = GameObject.Find("fairy_book");
        BookInfo book2 = fairy_book.AddComponent<BookInfo>();
        book2.index = 1;
        book2.bookName = "solar_system_book";
        listBookInfo.Add(book2);

        GameObject science_book = GameObject.Find("science_book");
        BookInfo book3 = science_book.AddComponent<BookInfo>();
        book3.index = 2;
        book3.bookName = "solar_system_book";
        listBookInfo.Add(book3);

        GameObject fiction_book = GameObject.Find("fiction_book");
        BookInfo book4 = fiction_book.AddComponent<BookInfo>();
        book4.index = 3;
        book4.bookName = "solar_system_book";
        listBookInfo.Add(book4);

    }
    void OnSelectedBook(int index)
    {
        if (Debug.isDebugBuild)
            Debug.Log("OnSelectedBook "+ index);
        BookInfo bookInfo = (BookInfo)listBookInfo[index];
        LoadBookSelected(bookInfo.bookName);



    }
    public void ButtonClick(string assetBundleName)
    {
        Debug.Log("ButtonClick1 ");
        if (Debug.isDebugBuild)
        {

        }
            
    }
    public void LoadBookSelected(string assetBundleName)
    {

        StartCoroutine(loadScene(GlobalConfig.CATEGORY_SCENE));

    }
    IEnumerator loadScene(string senceName)
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
