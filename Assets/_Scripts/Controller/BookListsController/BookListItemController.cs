using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class BookListItemController : MonoBehaviour {
    public int idx{ get; set; }
    public BookInfo bookInfo;
    public Text txtName;
    public RawImage  img;
    // Use this for initialization
    void Start () {
        string imageUrl = bookInfo.picture_url;
        //Debug.Log("ImageURL="+ imageUrl);
      //  if( imageUrl!=null && imageUrl!="")
        //StartCoroutine(loadImg(imageUrl));
        txtName.text = bookInfo.name;
    }
    public void onClicked()
    {
        Debug.Log("onClicked idx="+idx);

        GlobalVar.shareContext.shareVar.Add("bookInfo",bookInfo);
        SceneManager.LoadScene(GlobalVar.BOOK2DDETAIL_SCENE);
    }


    public IEnumerator loadImg()
    {

        if (bookInfo.picture_url != null && bookInfo.picture_url != "")
        {
            WWW imgLink = new WWW(bookInfo.picture_url);

            yield return imgLink;
            img.texture = imgLink.texture;
        }

    }
}
