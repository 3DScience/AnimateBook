using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CameraController_1 : MonoBehaviour {

	public float zoomSpeed = 50.0f;
	private float currDist = 0.0f,
	lastDist = 0.0f,
	zoomFactor = 0.0f;

	public Vector2 ObjectPostionSart = Vector2.zero;
	public Vector2 currDistVector = Vector2.zero;

	private GameObject _plane;
	private Transform cachedTransform;
	private Vector2 currTouch1 = Vector2.zero,
	lastTouch1 = Vector2.zero,
	currTouch2 = Vector2.zero,
	lastTouch2 = Vector2.zero;

	private float flagCameraExitZoomOut = 0;
	private float flagCameraExitZoomIn = 1;

	public int currTouch = 0;

	// rotation camera
	public Transform lookAt;
	private const float Y_ANGLE_MIN = -80;
	private const float Y_ANGLE_MAX = 80;

	private float distance = 1000.0f;
	private float currentX = 0.0f;
	private float currentY = 0.0f;

	private float sensitiveX = 0.2f;
	private float sensitiveY = 0.2f;

	private bool flagShowtxt = false;
    private Vector3 lastMouseCoordinate;
    private Vector3 originMouseCoordinate;
    void Start() {
		cachedTransform = transform;
	}
	public void OnTouchs() {
		Update ();
	}
    private bool chekcIsPoitOverGo(Vector3 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;
        bool b = Physics.Raycast(ray, out hit);
      //  Debug.Log("b=" + b);
        if (b)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool rxTouchEvent = false;
	public void Update()
	{
        if (Input.touchCount > 0)
        {
            Touch firstTouch = Input.touches[0];
            if (firstTouch.phase == TouchPhase.Began)
            {
                
               // DebugOnScreen.Log("CameraController_1 EventSystem.current.currentSelectedGameObject=" + EventSystem.current.IsPointerOverGameObject(firstTouch.fingerId));
                if (Camera.main == null || EventSystem.current.IsPointerOverGameObject(firstTouch.fingerId))
                {

                    if (!chekcIsPoitOverGo(firstTouch.position))
                    {
                        rxTouchEvent = false;
                        return;
                    }else
                    {
                        rxTouchEvent = true;
                    }

                    //Ray ray = Camera.main.ScreenPointToRay(firstTouch.position);
                    //RaycastHit hit;
                    //         bool b = Physics.Raycast(ray, out hit);
                    //         Debug.Log("b="+b);
                    //         if (b){

                    //} else {
                    //             return;
                    //}

                }
                else
                {
                    rxTouchEvent = true;
                }
            }
            if (!rxTouchEvent)
                return;



            for (int i = 0; i < Input.touchCount; i++)
            {

                currTouch = i;
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {   // khi bat dau cham vao man hinh
                    OnTouchBeganAnyWhere();
                }
                if (Input.GetTouch(i).phase == TouchPhase.Ended)
                {   // khi ket thuc cham vao man hinh
                    OnTouchEndedAnyWhere();
                }
                if (Input.GetTouch(i).phase == TouchPhase.Moved)
                {   // khi cham vao man hinh va di chuyen
                    OnTouchMoveAnyWhere();
                }
                if (Input.GetTouch(i).phase == TouchPhase.Stationary)
                {   // khi cham vao man hinh am ko di chuyen
                    OnTouchStayAnyWhere();
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                originMouseCoordinate = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                Debug.Log("GetMouseButtonDown");
                if (Camera.main == null || EventSystem.current.IsPointerOverGameObject())
                {
                    if (!chekcIsPoitOverGo(Input.mousePosition))
                    {
                        return;
                    }

                }
                OnTouchMoveAnyWhere();

            }
        }
	}
		
	void OnTouchBeganAnyWhere() {
	}

	void OnTouchMoveAnyWhere()
	{
		
        if (Input.GetMouseButton(0))
        {
            RotationCamera();
        }
        if (Input.touchCount == 1) {
			RotationCamera ();
		} else if (Input.touchCount == 2) {
			BoxCollider[] myColliders = gameObject.GetComponents<BoxCollider>();
			foreach(BoxCollider bc in myColliders) bc.enabled = true;
			ZoomCamera ();
		}
	}

	void OnTouchStayAnyWhere()
	{
	}

	void OnTouchEndedAnyWhere () {
	}
		
	void RotationCamera ()
	{	
		Vector3 offset = transform.position - lookAt.position;
		distance = offset.magnitude;



		if  (Input.touchCount == 1) {
            Touch touch = Input.GetTouch(0);
			if (Input.GetTouch(0).phase == TouchPhase.Moved) {
				//Debug.Log("Start begin" + gameObject.transform.position);
				currentX += touch.deltaPosition.x * sensitiveX;
				currentY -= touch.deltaPosition.y * sensitiveY;
                currentY = Mathf.Clamp (currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);

				Vector3 dir = new Vector3 (0,0, -distance);
				Quaternion rotation = Quaternion.Euler (currentY, currentX, 0);
				cachedTransform.position = lookAt.position + rotation*dir;
				cachedTransform.LookAt (lookAt.position);
				//cachedTransform.LookAt (Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/3,Screen.height/2,lookAt.position.z)));
			}
		}else  if (Input.GetMouseButton(0) )
        {
            Vector3 mousePosition= Input.mousePosition;
            Vector3 mouseDelta = mousePosition - lastMouseCoordinate;
            Vector3 mouseOriginDelta = mousePosition - originMouseCoordinate;
            lastMouseCoordinate = mousePosition;
            if (mouseDelta.x !=0 || mouseDelta.y != 0)
            {
                currentX += mouseOriginDelta.x * sensitiveX;
                currentY -= mouseOriginDelta.y * sensitiveY;

                currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);

                Vector3 dir = new Vector3(0, 0, -distance);
                Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
                cachedTransform.position = lookAt.position + rotation * dir;
                cachedTransform.LookAt(lookAt.position);
            }
            
        }
    }
    
		
	void ZoomCamera ()
	{
		switch (currTouch) 
		{
		case 0:	// firt touch
			currTouch1 = Input.GetTouch (0).position;
			lastTouch1 = currTouch1 - Input.GetTouch (0).deltaPosition;	// deltaPosition khoang cvi tri gan day nhat va vi tri truoc do
			break;
		case 1:	// second touch
			currTouch2 = Input.GetTouch (1).position;
			lastTouch2 = currTouch2 - Input.GetTouch (1).deltaPosition;
			break;
		}
		// we dont want to find the distance between 1 finger and nothing
		if (currTouch >= 1) {
			currDist = Vector2.Distance (currTouch1, currTouch2);		// Distance tra ve khoang cach giau a va b
			lastDist = Vector2.Distance (lastTouch1, lastTouch2);
		} else {
			currDist = 0.0f;
			lastDist = 0.0f;
		}
			
		zoomFactor =  Mathf.Clamp(lastDist - currDist, -30f, 30f);

		Debug.Log ("flagCameraExitZoomOut : " + flagCameraExitZoomOut);
		Debug.Log ("flagCameraExitZoomIn : " + flagCameraExitZoomIn);
	
		if (flagCameraExitZoomOut == 1 & zoomFactor >= 0) {
		} else if (zoomFactor <= 0 & flagCameraExitZoomIn == 0) {
		} else {
			Camera.main.transform.Translate(Vector3.back * zoomFactor * zoomSpeed * Time.deltaTime);
		}

	}
		

	void OnTriggerExit(Collider other) {
		if(other.gameObject.CompareTag("PlaneMax")) {
			flagCameraExitZoomOut = 1;
		}
		if(other.gameObject.CompareTag("PlaneMin")) {
			flagCameraExitZoomIn = 1;
		}
	}

	void OnTriggerStay(Collider other) {
		if(other.gameObject.CompareTag("PlaneMax")) {
			flagCameraExitZoomOut = 0;
		}
		if(other.gameObject.CompareTag("PlaneMin")) {
			flagCameraExitZoomIn = 0;
		}
	}

	public void OnShowUi() {
		if (flagShowtxt == true)
			return;
		flagShowtxt = true;
		float x = cachedTransform.eulerAngles.x;
		float y = cachedTransform.eulerAngles.y;
		float z = cachedTransform.eulerAngles.z;
		//Vector3 eulerAngles;

		if (x >= 69 & x < 100) {
			cachedTransform.eulerAngles  = new Vector3 (x + 5, y + 70, z);

		} else if (x >= 50 & x < 69) {
			cachedTransform.eulerAngles  = new Vector3 (x + 1, y + 20, z);

		} else if (x >= 40 & x < 50) {
			cachedTransform.eulerAngles  = new Vector3 (x, y + 17, z);

		} else if (x > 260 & x <= 284) {
			cachedTransform.eulerAngles  = new Vector3 (x - 7, y + 70, z);

		} else if (x > 284 & x <= 290) {
			cachedTransform.eulerAngles  = new Vector3 (x - 7, y + 50, z);

		} else if (x > 290 & x <= 297) { // < - 69
			cachedTransform.eulerAngles  = new Vector3 (x - 3, y + 30, z);

		} else if (x > 297 & x <= 310) { // -69 -> -50
			cachedTransform.eulerAngles  = new Vector3 (x - 3, y + 20, z);

		} else if (x > 310 & x <= 320) {
			cachedTransform.eulerAngles  = new Vector3 (x - 2, y + 17, z);

		} else {
			cachedTransform.eulerAngles = new Vector3 (x, y + 12, z);
//			eulerAngles = new Vector3 (x, y + 12, z);
		}
//		Debug.Log ("Test");
//		cachedTransform.eulerAngles = Vector3.Lerp (cachedTransform.eulerAngles, eulerAngles, 500*Time.deltaTime);
//		Quaternion newRotation = Quaternion.Euler (eulerAngles);
//		cachedTransform.rotation = Quaternion.Lerp (cachedTransform.rotation, newRotation, 20*Time.deltaTime);
	}

	public void OnHideUi() {
		flagShowtxt = false;
		cachedTransform.LookAt (lookAt.position);
	}
		
}
