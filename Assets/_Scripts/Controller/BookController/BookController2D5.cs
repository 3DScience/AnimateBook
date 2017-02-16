using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BookController2D5 : MonoBehaviour {
	private const string OPEN_BOOK = "openBook";
	private const string CLOSE_BOOK = "closeBook";
	private const string NEXT_PAGE = "nextPage";
	private const string BACK_PAGE = "backPage";

	[HideInInspector]public int current_page = 0;	//current_page is counted from 1 :: page1 is 1
    //Maximum content page
    public int max_page = 0;

    //Scence Controller
    private GameObject Scence;

    //BackGround Plane
    private GameObject BGPlane;

    //Book Static Page
    private GameObject topPageLeft, topPageRight;

    //Book Flip Page
	private List<GameObject> pages = new List<GameObject>();

    //Flip Page Bone
	private List<GameObject> left_pageskes = new List<GameObject>();
	private List<GameObject> right_pageskes = new List<GameObject>();


    //Page Effect
    private GameObject Page1_Effect, Page2_Effect;

	bool activityEnabled;	//true after animations are finished
	bool allowMovingCam;	//to prevent moving camera after back to the first page then open next page, only move camera at the first time open book
	bool closedBook;

    //Book Material
    public Material pageBlank_mat;

	public Material[] pageLeft_mats;
	public Material[] pageRight_mats;
	public Material[] flipPage_mats;
	public Material[] BG_mats;

    // Use this for initialization
    void Start () {
		activityEnabled = false;
		allowMovingCam = false;

        Scence = GameObject.Find("Scence");
        BGPlane = GameObject.Find("BGPlane");

        topPageLeft = GameObject.Find("Open_Book/top_page_left");
        topPageRight = GameObject.Find("Open_Book/top_page_right");

		string pg = "";
		string left_pgske = "";
		string right_pgske = "";

		for (int i = 1; i <= max_page; i++) {
			pg = "Flip_Page/page";
			pg = pg + i;

			pages.Add(transform.FindChild(pg).gameObject);

			//left pageske
			left_pgske = "Book_Ske/page_ske{0}/left_pageske{1}";
			left_pgske = string.Format(left_pgske, i, i);

			left_pageskes.Add(GameObject.Find(left_pgske));

			//right pageske
			right_pgske = "Book_Ske/page_ske{0}/right_pageske{1}";
			right_pgske = string.Format(right_pgske, i, i);

			right_pageskes.Add(GameObject.Find(right_pgske));
		}

        Page1_Effect = GameObject.Find("Book_Ske/page_ske2/left_pageske2/Page1_Effect");
        Page2_Effect = GameObject.Find("Book_Ske/page_ske3/left_pageske3/Page2_Effect");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void open()
    {
		StartCoroutine(openBook());
    }

	IEnumerator openBook () {
		activityEnabled = false;
		GetComponent<Animation>().Play(OPEN_BOOK);

		yield return new WaitForSeconds(3);
		activityEnabled = true;
		closedBook = false;
	}

	IEnumerator closeBook () {
		current_page = 0;
		activityEnabled = false;
		GetComponent<Animation>().Play(CLOSE_BOOK);

		yield return new WaitForSeconds(2);
		activityEnabled = true;
		closedBook = true;
	}

    public void onBookClick()
    {
		if (activityEnabled == true) {

			if (closedBook == false) {	//book is opening
		        //Open next page
				if (max_page > 0 && current_page < max_page && current_page >= 0) {
		            StartCoroutine(_nextPage());

				} else {
					StartCoroutine(closeBook());
				}

		        //Start Story Tell Scence
				if (current_page == 1 && allowMovingCam == false) {
		            Scence.GetComponent<ScenceController>().StartStoryTellScence();
					allowMovingCam = true;
				}

			} else {
				open();
			}
		}
    }

	public void test() {
		Debug.Log("test");
	}

    GameObject getFlipPage(int current_page)
    {
		if (max_page > 0 && current_page <= max_page && current_page > 0) { 	//current_page is counted from 1
			return pages[current_page - 1];

		}

		return null;
    }

    GameObject[] getPageSke(int current_page)
    {
        GameObject[] pageSke = new GameObject[4];
        GameObject left_prePageSke = null, right_prePageSke = null, left_curPageSke = null, right_curPageSke = null;
 
		if (max_page > 0 && current_page <= max_page && current_page > 0) {
			if (current_page == 1) {
				left_curPageSke = right_pageskes[current_page - 1];
				right_curPageSke = left_pageskes[current_page];

			} else if (current_page == max_page) {
				left_prePageSke = right_pageskes[current_page - 2];
				right_prePageSke = left_pageskes[current_page - 1];

			} else {

				left_prePageSke = right_pageskes[current_page - 2];
				right_prePageSke = left_pageskes[current_page - 1];
				left_curPageSke = right_pageskes[current_page - 1];
				right_curPageSke = left_pageskes[current_page];
			}
		}

        pageSke[0] = left_prePageSke;
        pageSke[1] = right_prePageSke;
        pageSke[2] = left_curPageSke;
        pageSke[3] = right_curPageSke;
        return pageSke;
    }

    Material[] getMaterial(int current_page)
    {
        Material[] book_mat = new Material[4];
        Material bookLeftMat = pageBlank_mat;
        Material bookRightMat = pageBlank_mat;
		Material bookFlipMat = pageBlank_mat;
		Material BGPlaneMat = pageBlank_mat;
        
		if (max_page > 0 && current_page <= max_page && current_page > 0) {
			if (pageLeft_mats.Length >= current_page && pageLeft_mats[current_page - 1] != null) {
				bookLeftMat = pageLeft_mats[current_page - 1];
			}

			if (pageRight_mats.Length >= current_page && pageRight_mats[current_page - 1] != null) {
				bookRightMat = pageRight_mats[current_page - 1];
			}

			if (flipPage_mats.Length >= current_page && flipPage_mats[current_page - 1] != null) {
				bookLeftMat = flipPage_mats[current_page - 1];
			}

			if (BG_mats.Length >= current_page && BG_mats[current_page - 1] != null) {
				BGPlaneMat = BG_mats[current_page - 1];
			}
		}

        book_mat[0] = bookLeftMat;
        book_mat[1] = bookRightMat;
        book_mat[2] = bookFlipMat;
        book_mat[3] = BGPlaneMat;
        return book_mat;
    }

    private void enableEffect()
    {
        switch (current_page)
        {
            case 1:
				if (Page1_Effect != null) {
	                Page1_Effect.SetActive(true);
				}
                break;
            case 2:
				if (Page1_Effect != null) {
	                Page2_Effect.SetActive(true);
				}
                break;
            case 3:              
                break;
            case 4:              
                break;
            case 5:              
                break;
            case 6:               
                break;
            case 7:               
                break;
            case 8:                
                break;
            default:
                break;
        }
    }

    private void disableAllEffect()
    {
		if (Page1_Effect != null) {
	        Page1_Effect.SetActive(false);
		}

		if (Page2_Effect != null) {
        	Page2_Effect.SetActive(false);
		}
    }

	private IEnumerator _nextPage()
    {
		current_page++;
        
        //Play animation open page
		activityEnabled = false;
		Debug.Log("_nextPage :: " + current_page);
        Animation animation = GetComponent<Animation>();
        animation.PlayQueued(NEXT_PAGE + current_page);

        //Disable all Effect
        disableAllEffect();

        //Hide current Background texture
        InvokeRepeating("HideBGPlaneTexture", 0f, 0.03F);

        //Set active current page
        getFlipPage(current_page).SetActive(true);
        getFlipPage(current_page).GetComponent<Renderer>().material = getMaterial(current_page)[2];

        //Set blank_page material to Book Right
        topPageRight.GetComponent<Renderer>().material = pageBlank_mat;  

        do
		{
            ScaleDownObject(getPageSke(current_page)[0]);
            ScaleDownObject(getPageSke(current_page)[1]);
            ScaleUpObject(getPageSke(current_page)[2]);
            ScaleUpObject(getPageSke(current_page)[3]);

            yield return null;
        } while (animation.isPlaying);

		activityEnabled = true;

        //Add new background texture
        //BGPlane.GetComponent<Renderer>().material = getMaterial(current_page)[3];
        //BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
        //InvokeRepeating("DissolveBackGroundTexture", 0f, 0.03F);

        //Add Book materials when open page animation done
        topPageLeft.GetComponent<Renderer>().material = getMaterial(current_page)[0];
        topPageRight.GetComponent<Renderer>().material = getMaterial(current_page)[1];
        topPageLeft.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
        topPageRight.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
        InvokeRepeating("DissolveTopBookTexture", 0f, 0.03F);

        //Enable Page Deco_Effect
        enableEffect();

        //Deactive current page
        getFlipPage(current_page).SetActive(false);
    }

	public void backPage() {
		if (activityEnabled == true) {
			if (closedBook == false) {	//book is opening
				//Open next page
				if (max_page > 0 && current_page <= max_page && current_page > 0) {
					StartCoroutine(_backPage());

				} else {
					StartCoroutine(closeBook());
				}
			}
		}
	}

	private IEnumerator _backPage()
	{
		int old_page = current_page;
		current_page--;
		//Play animation open page
		activityEnabled = false;
		Debug.Log("_backPage :: " + old_page);
		Animation animation = GetComponent<Animation>();
		animation.PlayQueued(BACK_PAGE + (old_page));

		//Disable all Effect
		disableAllEffect();

		//Hide current Background texture
		InvokeRepeating("HideBGPlaneTexture", 0f, 0.03F);

		//Set active old_page, to make flip_page to be visible
		getFlipPage(old_page).SetActive(true);
		getFlipPage(old_page).GetComponent<Renderer>().material = getMaterial(old_page)[2];

		//Set blank_page material to Book left
		topPageLeft.GetComponent<Renderer>().material = pageBlank_mat;  

		do
		{
			ScaleUpObject(getPageSke(old_page)[0]);
			ScaleUpObject(getPageSke(old_page)[1]);
			ScaleDownObject(getPageSke(old_page)[2]);
			ScaleDownObject(getPageSke(old_page)[3]);

			yield return null;
		} while (animation.isPlaying);

		activityEnabled = true;

		//Add new background texture
		//BGPlane.GetComponent<Renderer>().material = getMaterial(current_page)[3];
		//BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
		//InvokeRepeating("DissolveBackGroundTexture", 0f, 0.03F);

		//Add Book materials when open page animation done
		topPageLeft.GetComponent<Renderer>().material = getMaterial(current_page)[0];
		topPageRight.GetComponent<Renderer>().material = getMaterial(current_page)[1];
		topPageLeft.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
		topPageRight.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
		InvokeRepeating("DissolveTopBookTexture", 0f, 0.03F);

		//Enable Page Deco_Effect
		enableEffect();

		//Deactive current page
		getFlipPage(old_page).SetActive(false);
	}

    private void HideBGPlaneTexture()
    {
        float dissolveLevel = BGPlane.GetComponent<Renderer>().material.GetFloat("_Level");
        if(dissolveLevel < 1)        
            BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", dissolveLevel + 0.02F);
        else { 
            BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
            CancelInvoke("HideBGPlaneTexture");
            BGPlane.GetComponent<Renderer>().material = getMaterial(current_page)[3];
            BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
            InvokeRepeating("DissolveBackGroundTexture", 0f, 0.03F);
        }
    }

    private void DissolveBackGroundTexture()
    {
        float dissolveLevel = BGPlane.GetComponent<Renderer>().material.GetFloat("_Level");
        if (dissolveLevel > 0)
        {
            BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", dissolveLevel - 0.02F);
        }
        else
        {
            BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", 0F);
            CancelInvoke("DissolveBackGroundTexture");
        }
    }

    private void DissolveTopBookTexture()
    {
        float dissolveLevel = topPageLeft.GetComponent<Renderer>().material.GetFloat("_Level");
        if (dissolveLevel > 0)
        {
            topPageLeft.GetComponent<Renderer>().material.SetFloat("_Level", dissolveLevel - 0.02F);
            topPageRight.GetComponent<Renderer>().material.SetFloat("_Level", dissolveLevel - 0.02F);
        }   
        else
        {
            topPageLeft.GetComponent<Renderer>().material.SetFloat("_Level", 0F);
            topPageRight.GetComponent<Renderer>().material.SetFloat("_Level", 0F);
            CancelInvoke("DissolveTopBookTexture");
        }
    }

    private void ScaleUpObject(GameObject obj)
    {
        if(obj != null)
        { 
            float x = 0.01f;

            if (obj.transform.localScale.y < 1)
                obj.transform.localScale = obj.transform.localScale + new Vector3(x, x, 0);

            if (obj.transform.localScale.y > 1)
                obj.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void ScaleDownObject(GameObject obj)
    {
        if (obj != null)
        {
            float x = 0.01f;

            if (obj.transform.localScale.y > 0)
                obj.transform.localScale = obj.transform.localScale - new Vector3(x, x, 0);

            if (obj.transform.localScale.y < 0)
                obj.transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
