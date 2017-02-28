
using System.Collections.Generic;
#if !UNITY_WEBGL
using Firebase.Database;
using UnityEngine;
using System.IO;
public class BooksFireBaseDb {
    string fileSave = GlobalVar.DB_PATH + "/books.text";
    private DatabaseReference dbf;
    private static BooksFireBaseDb _instance;
    private ListBookInfos listAllBooks;
    public static BooksFireBaseDb getInstance()
    {
        if(_instance == null)
        {
            _instance = new BooksFireBaseDb();
        }
        return _instance;
    }
    public BooksFireBaseDb()
    {
        FirebaseHelper.getInstance().initFirebase( () => {
            dbf = FirebaseDatabase.DefaultInstance.RootReference;
        });
    }
    public void reSaveBooksToLocal(System.Action callbackWhenDone)
    {
        listAllBooks = null;
        saveBooksToLocal(callbackWhenDone);
    }
    public void saveBooksToLocal(System.Action callbackWhenDone)
    {
        GlobalVar.shareContext.loadingIndicator.SetActive(true);
        dbf.Child("public").Child("vn").Child("books").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<BookInfo> lsbooks = new List<BookInfo>();
                foreach (var item in snapshot.Children)
                {
                    //  Debug.Log("b id="+item.Key+"/ v="+item.GetRawJsonValue());
                    BookInfo b = JsonUtility.FromJson<BookInfo>(item.GetRawJsonValue());
                    b.id = int.Parse(item.Key);
                    // Debug.Log("b id="+item.Key+"/ v="+b.name);
                    lsbooks.Add(b);
                }
                ListBookInfos bs = new ListBookInfos();
                bs.books = lsbooks.ToArray();
                bs.db_version = 1;
                string jsonBooks = JsonUtility.ToJson(bs,true);

                Debug.Log(jsonBooks);
        
                Debug.Log(" saving textAsset "+ fileSave);
                File.WriteAllText(fileSave, jsonBooks);
                Debug.Log(" saved textAsset");

                string loadedText = File.ReadAllText(fileSave);
                Debug.Log("textAsset loaded=" + loadedText);
                callbackWhenDone();
                GlobalVar.shareContext.loadingIndicator.SetActive(false);
            }
        });
    }
    public void getBooksFromLocal(System.Action<List<BookInfo>> callbackWhenDone, string catName)
    {
        if (listAllBooks != null)
        {
            callbackWhenDone(getListBookByCatName(catName));
            return;
        }
        if (File.Exists(fileSave))
        {
            _getListAllBook();
            callbackWhenDone(getListBookByCatName(catName));
            return;
        }
        else
        {
            saveBooksToLocal( ()=>{
                _getListAllBook();
                callbackWhenDone(getListBookByCatName(catName));
            });
        }
    }
    private List<BookInfo> getListBookByCatName(string catName)
    {
        List<BookInfo> listBookBycatName = new List<BookInfo>();
        foreach (var bookInfo in listAllBooks.books)
        {
            if (bookInfo.categories.Contains(catName))
            {
                listBookBycatName.Add(bookInfo);
            }
        }
        return listBookBycatName;
    }
    private void _getListAllBook()
    {
        Debug.Log(" saving textAsset " + fileSave);
        string loadedText = File.ReadAllText(fileSave);
        ///Debug.Log("textAsset loaded=" + loadedText);
        listAllBooks = JsonUtility.FromJson<ListBookInfos>(loadedText);
    }
}
#endif