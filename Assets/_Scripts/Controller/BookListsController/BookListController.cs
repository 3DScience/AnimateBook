using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BookListController : MonoBehaviour {
    public static string catName;
    public GameObject listUi;
    public GameObject bookListItemPref;
    private RectTransform listUiRectTransform;
    private ScrollRect listUiScrollRect;
    private GridLayoutGroup contentGridLayout;
    // Use this for initialization
    void Start () {
        listUiRectTransform = listUi.GetComponent<RectTransform>();
        listUiScrollRect = listUi.GetComponent<ScrollRect>();
        contentGridLayout = listUi.GetComponentInChildren<GridLayoutGroup>();
        Debug.Log(listUiScrollRect);
        float cellWidth= (listUiRectTransform.rect.width - contentGridLayout.padding.left) / 3 - (contentGridLayout.spacing.x);
        contentGridLayout.cellSize = new Vector2(cellWidth, cellWidth*1.3f);


        BooksFireBaseDb.getInstance().getBooksFromLocal(books => {

            foreach (var book in books)
            {
                GameObject g = GameObject.Instantiate(bookListItemPref);
                g.transform.SetParent(contentGridLayout.transform);
                g.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                BookListItemController bookListItemController = g.GetComponentInChildren<BookListItemController>();
                bookListItemController.bookInfo = book;
            }
                
        }, catName);
        //for( int i=0; i<20; i ++)
        //{
        //    GameObject g=GameObject.Instantiate(bookListItemPref);
        //    g.transform.SetParent(contentGridLayout.transform);
        //    g.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        //    BookListItemController bookListItemController = g.GetComponentInChildren<BookListItemController>();
        //    bookListItemController.idx = i;
        //    //Debug.Log("added " + bookListItemController.idx);
        //}
        listUiScrollRect.normalizedPosition = new Vector2(0, 1);
      
    }



}
