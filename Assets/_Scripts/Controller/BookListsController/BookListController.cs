using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class BookListController : MonoBehaviour {
    public static string catName;
    public Text txtCategotyName;
    public GameObject listUi;
    public GameObject bookListItemPref;
    private RectTransform listUiRectTransform;
    private ScrollRect listUiScrollRect;
    private GridLayoutGroup contentGridLayout;

    List<GameObject> listGameObjectBookAdded = new List<GameObject>();
    // Use this for initialization
    void Start () {
        if (catName == null)
        {
            catName = "science";
        }
        txtCategotyName.text = catName;
        listUiRectTransform = listUi.GetComponent<RectTransform>();
        listUiScrollRect = listUi.GetComponent<ScrollRect>();
        contentGridLayout = listUi.GetComponentInChildren<GridLayoutGroup>();
        Debug.Log(listUiScrollRect);
        float cellWidth= (listUiRectTransform.rect.width - contentGridLayout.padding.left) / 3 - (contentGridLayout.spacing.x);
        contentGridLayout.cellSize = new Vector2(cellWidth, cellWidth*1.3f);

        loadListBook();

        //for( int i=0; i<20; i ++)
        //{
        //    GameObject g=GameObject.Instantiate(bookListItemPref);
        //    g.transform.SetParent(contentGridLayout.transform);
        //    g.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        //    BookListItemController bookListItemController = g.GetComponentInChildren<BookListItemController>();
        //    bookListItemController.idx = i;
        //    //Debug.Log("added " + bookListItemController.idx);
        //}


    }
    private void loadListBook()
    {

        foreach (var go in listGameObjectBookAdded)
        {
            Destroy(go);
            Debug.Log("remove gox"+ listGameObjectBookAdded.Count);
        }
        listGameObjectBookAdded.Clear();
        BooksFireBaseDb.getInstance().getBooksFromLocal(books => {

            foreach (var book in books)
            {
                GameObject g = GameObject.Instantiate(bookListItemPref);
                g.transform.SetParent(contentGridLayout.transform);
                g.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                listGameObjectBookAdded.Add(g);
                BookListItemController bookListItemController = g.GetComponentInChildren<BookListItemController>();
                bookListItemController.bookInfo = book;
                Debug.Log("add gox"+ listGameObjectBookAdded.Count);
            }

        }, catName);
        listUiScrollRect.normalizedPosition = new Vector2(0, 1);
    }
    public void refressButtonClicked()
    {
        BooksFireBaseDb.getInstance().reSaveBooksToLocal(()=> { loadListBook(); });
        
    }
    public void onHomeButtonClick()
    {
        SceneManager.LoadScene(GlobalVar.MAINSCENE);
    }
}
