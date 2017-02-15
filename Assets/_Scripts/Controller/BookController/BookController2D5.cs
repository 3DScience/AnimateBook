using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BookController2D5 : MonoBehaviour {
	private const string OPEN_BOOK = "openBook";
	private const string NEXT_PAGE = "nextPage";
	private const string BACK_PAGE = "backPage";

	[HideInInspector]public int current_page = 0;
    //Maximum content page
    public int max_page = 8;

    //Scence Controller
    private GameObject Scence;

    //BackGround Plane
    private GameObject BGPlane;

    //Book Static Page
    private GameObject topPageLeft, topPageRight;

    //Book Flip Page
    private GameObject page1, page2, page3, page4, page5, page6, page7, page8;

    //Flip Page Bone
    private GameObject left_pageske2, left_pageske3, left_pageske4, left_pageske5, left_pageske6, left_pageske7, left_pageske8;
    private GameObject right_pageske1, right_pageske2, right_pageske3, right_pageske4, right_pageske5, right_pageske6, right_pageske7;

    //Page Effect
    private GameObject Page1_Effect, Page2_Effect;

	bool activityEnabled;

    //Book Material
    public Material pageBlank_mat;
    public Material page1Left_mat, page1Right_mat, 
                    page2Left_mat, page2Right_mat, 
                    page3Left_mat, page3Right_mat, 
                    page4Left_mat, page4Right_mat, 
                    page5Left_mat, page5Right_mat, 
                    page6Left_mat, page6Right_mat, 
                    page7Left_mat, page7Right_mat, 
                    page8Left_mat, page8Right_mat;
    public Material flipPage1_mat, flipPage2_mat, flipPage3_mat, flipPage4_mat, flipPage5_mat, flipPage6_mat, flipPage7_mat, flipPage8_mat;
    public Material BG1_mat, BG2_mat, BG3_mat, BG4_mat, BG5_mat, BG6_mat, BG7_mat, BG8_mat;

    // Use this for initialization
    void Start () {
		activityEnabled = false;
        Scence = GameObject.Find("Scence");
        BGPlane = GameObject.Find("BGPlane");

        topPageLeft = GameObject.Find("Open_Book/top_page_left");
        topPageRight = GameObject.Find("Open_Book/top_page_right");

        page1 = transform.FindChild("Flip_Page/page1").gameObject;
        page2 = transform.FindChild("Flip_Page/page2").gameObject;
        page3 = transform.FindChild("Flip_Page/page3").gameObject;
        page4 = transform.FindChild("Flip_Page/page4").gameObject;
        page5 = transform.FindChild("Flip_Page/page5").gameObject;
        page6 = transform.FindChild("Flip_Page/page6").gameObject;
        page7 = transform.FindChild("Flip_Page/page7").gameObject;
        page8 = transform.FindChild("Flip_Page/page8").gameObject;

        left_pageske2 = GameObject.Find("Book_Ske/page_ske2/left_pageske2");
        left_pageske3 = GameObject.Find("Book_Ske/page_ske3/left_pageske3");
        left_pageske4 = GameObject.Find("Book_Ske/page_ske4/left_pageske4");
        left_pageske5 = GameObject.Find("Book_Ske/page_ske5/left_pageske5");
        left_pageske6 = GameObject.Find("Book_Ske/page_ske6/left_pageske6");
        left_pageske7 = GameObject.Find("Book_Ske/page_ske7/left_pageske7");
        left_pageske8 = GameObject.Find("Book_Ske/page_ske8/left_pageske8");

        right_pageske1 = GameObject.Find("Book_Ske/page_ske1/right_pageske1");
        right_pageske2 = GameObject.Find("Book_Ske/page_ske2/right_pageske2");
        right_pageske3 = GameObject.Find("Book_Ske/page_ske3/right_pageske3");
        right_pageske4 = GameObject.Find("Book_Ske/page_ske4/right_pageske4");
        right_pageske5 = GameObject.Find("Book_Ske/page_ske5/right_pageske5");
        right_pageske6 = GameObject.Find("Book_Ske/page_ske6/right_pageske6");
        right_pageske7 = GameObject.Find("Book_Ske/page_ske7/right_pageske7");

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
	}

    public void onBookClick()
    {
		if (activityEnabled == true) {
	        //Open next page
			if (max_page > 0 && current_page <= max_page && current_page >= 0) {
	            StartCoroutine(_nextPage());
			}

	        //Start Story Tell Scence
			if (current_page == 1) {
	            Scence.GetComponent<ScenceController>().StartStoryTellScence();
			}
		}
    }

	public void test() {
		Debug.Log("test");
	}

    GameObject getFlipPage(int current_page)
    {
        switch (current_page)
        {
            case 1:
                return page1;
            case 2:
                return page2;
            case 3:
                return page3;
            case 4:
                return page4;
            case 5:
                return page5;
            case 6:
                return page6;
            case 7:
                return page7;
            case 8:
                return page8;
            default:
                return null;
        }
    }

    GameObject[] getPageSke(int current_page)
    {
        GameObject[] pageSke = new GameObject[4];
        GameObject left_prePageSke = null, right_prePageSke = null, left_curPageSke = null, right_curPageSke = null;
        switch (current_page)
        {
            case 1:
                left_curPageSke = right_pageske1;
                right_curPageSke = left_pageske2;
                break;
            case 2:
                left_prePageSke = right_pageske1;
                right_prePageSke = left_pageske2;
                left_curPageSke = right_pageske2;
                right_curPageSke = left_pageske3;
                break;
            case 3:
                left_prePageSke = right_pageske2;
                right_prePageSke = left_pageske3;
                left_curPageSke = right_pageske3;
                right_curPageSke = left_pageske4;
                break;
            case 4:
                left_prePageSke = right_pageske3;
                right_prePageSke = left_pageske4;
                left_curPageSke = right_pageske4;
                right_curPageSke = left_pageske5;
                break;
            case 5:
                left_prePageSke = right_pageske4;
                right_prePageSke = left_pageske5;
                left_curPageSke = right_pageske5;
                right_curPageSke = left_pageske6;
                break;
            case 6:
                left_prePageSke = right_pageske5;
                right_prePageSke = left_pageske6;
                left_curPageSke = right_pageske6;
                right_curPageSke = left_pageske7;
                break;
            case 7:
                left_prePageSke = right_pageske6;
                right_prePageSke = left_pageske7;
                left_curPageSke = right_pageske7;
                right_curPageSke = left_pageske8;
                break;
            case 8:
                left_prePageSke = right_pageske7;
                right_prePageSke = left_pageske8;
                break;
            default:
                break;
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
        switch (current_page)
        {
            case 1:
                if(page1Left_mat != null)
                    bookLeftMat = page1Left_mat;
                if (page1Right_mat != null)
                    bookRightMat = page1Right_mat;
                if (flipPage1_mat != null)
                    bookFlipMat = flipPage1_mat;
                if (BG1_mat != null)
                    BGPlaneMat = BG1_mat;
                break;
            case 2:
                if (page2Left_mat != null)
                    bookLeftMat = page2Left_mat;
                if (page2Right_mat != null)
                    bookRightMat = page2Right_mat;
                if (flipPage2_mat != null)
                    bookFlipMat = flipPage2_mat;
                if (BG2_mat != null)
                    BGPlaneMat = BG2_mat;
                break;
            case 3:
                if (page3Left_mat != null)
                    bookLeftMat = page3Left_mat;
                if (page3Right_mat != null)
                    bookRightMat = page3Right_mat;
                if (flipPage3_mat != null)
                    bookFlipMat = flipPage3_mat;
                if (BG3_mat != null)
                    BGPlaneMat = BG3_mat;
                break;
            case 4:
                if (page4Left_mat != null)
                    bookLeftMat = page4Left_mat;
                if (page4Right_mat != null)
                    bookRightMat = page4Right_mat;
                if (flipPage4_mat != null)
                    bookFlipMat = flipPage4_mat;
                if (BG4_mat != null)
                    BGPlaneMat = BG4_mat;
                break;
            case 5:
                if (page5Left_mat != null)
                    bookLeftMat = page5Left_mat;
                if (page5Right_mat != null)
                    bookRightMat = page5Right_mat;
                if (flipPage5_mat != null)
                    bookFlipMat = flipPage5_mat;
                if (BG5_mat != null)
                    BGPlaneMat = BG5_mat;
                break;
            case 6:
                if (page6Left_mat != null)
                    bookLeftMat = page6Left_mat;
                if (page6Right_mat != null)
                    bookRightMat = page6Right_mat;
                if (flipPage6_mat != null)
                    bookFlipMat = flipPage6_mat;
                if (BG6_mat != null)
                    BGPlaneMat = BG6_mat;
                break;
            case 7:
                if (page7Left_mat != null)
                    bookLeftMat = page7Left_mat;
                if (page7Right_mat != null)
                    bookRightMat = page7Right_mat;
                if (flipPage7_mat != null)
                    bookFlipMat = flipPage7_mat;
                if (BG7_mat != null)
                    BGPlaneMat = BG7_mat;
                break;
            case 8:
                if (page8Left_mat != null)
                    bookLeftMat = page8Left_mat;
                if (page8Right_mat != null)
                    bookRightMat = page8Right_mat;
                if (flipPage8_mat != null)
                    bookFlipMat = flipPage8_mat;
                if (BG8_mat != null)
                    BGPlaneMat = BG8_mat;
                break;
            default:
                break;
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
			//Open next page
			if (max_page > 0 && current_page <= max_page && current_page >= 0) {
				StartCoroutine(_backPage());
			}
		}
	}

	private IEnumerator _backPage()
	{
		

		//Play animation open page
		activityEnabled = false;
		Debug.Log("_backPage :: " + current_page);
		Animation animation = GetComponent<Animation>();
		animation.PlayQueued(BACK_PAGE + (current_page));



		//Disable all Effect
		disableAllEffect();

		//Hide current Background texture
		InvokeRepeating("HideBGPlaneTexture", 0f, 0.03F);

		//Set active current page
		getFlipPage(current_page).SetActive(true);
		getFlipPage(current_page).GetComponent<Renderer>().material = getMaterial(current_page)[0];

		current_page--;

		//Set blank_page material to Book Right
		topPageRight.GetComponent<Renderer>().material = pageBlank_mat;  

		do
		{
			ScaleUpObject(getPageSke(current_page + 1)[0]);
			ScaleUpObject(getPageSke(current_page + 1)[1]);
			ScaleDownObject(getPageSke(current_page + 1)[2]);
			ScaleDownObject(getPageSke(current_page +1)[3]);

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
