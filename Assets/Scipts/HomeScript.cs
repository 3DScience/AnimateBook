using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
public class HomeScript : MonoBehaviour {
    ArrayList listBookInfo = new ArrayList();
	// Use this for initialization
	void Start () {
        if (Debug.isDebugBuild)
            Debug.Log("MainScript Start...");
        loadAllBook();
    }
    void loadAllBook()
    {
        GameObject animal_book = GameObject.Find("animal_book");
        BookInfo book1 = animal_book.AddComponent<BookInfo>();
        book1.index = 0;
        book1.bookName = "book1";
        listBookInfo.Add(book1);

        GameObject fairy_book = GameObject.Find("fairy_book");
        BookInfo book2 = fairy_book.AddComponent<BookInfo>();
        book2.index = 1;
        book2.bookName = "book1";
        listBookInfo.Add(book2);

        GameObject science_book = GameObject.Find("science_book");
        BookInfo book3 = science_book.AddComponent<BookInfo>();
        book3.index = 2;
        book3.bookName = "book1";
        listBookInfo.Add(book3);

        GameObject fiction_book = GameObject.Find("fiction_book");
        BookInfo book4 = fiction_book.AddComponent<BookInfo>();
        book4.index = 3;
        book4.bookName = "book1";
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

     
        if(checkIsDownloadedAsset(assetBundleName))
        {
            BookLoaderScript.assetBundleName = assetBundleName;
            Application.LoadLevel(GlobalConfig.BOOK_LOADER_SCENE);
            //test 2
        }
        else
        {
            Application.LoadLevel(GlobalConfig.DOWNLOAD_ASSET_SCENE);
        }
    }

    private bool checkIsDownloadedAsset(string assetBundleName)
    {
        string assetDataFolder = GlobalConfig.DATA_PATH + "/" + assetBundleName;
        if (Directory.Exists(assetDataFolder))
        {
            return true;
        }
        return false;
    }
     
}
