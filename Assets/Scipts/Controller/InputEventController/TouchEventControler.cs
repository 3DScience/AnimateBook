using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public interface TouchEventInterface
{
    void OnTouchs();
}
public delegate void onTouchNothing();
public class TouchEventControler : MonoBehaviour {

    public onTouchNothing deledateOnTouchNothing;
    private GameObject currentTouchGameObject;
    private 
	// Update is called once per frame
	void Update() {
        if (Input.touchCount > 0){
    
            Touch firstTouch= Input.touches[0];
            if( firstTouch.phase== TouchPhase.Began)
            {
                //DebugOnScreen.Log("on touch "+Time.deltaTime);
                Debug.Log("EventSystem.current.currentSelectedGameObject="+ EventSystem.current.IsPointerOverGameObject(0)+ "Camera.current ="+ Camera.current);
                if (Camera.current == null|| EventSystem.current.IsPointerOverGameObject(0))
                {
                    currentTouchGameObject = null;
                    Debug.Log("Camera.current =null or touch on some ui");
                    return;
                }
                   
				Ray ray = Camera.main.ScreenPointToRay(firstTouch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject touchedGameObject = hit.transform.gameObject;
                    if (Debug.isDebugBuild)
                        Debug.Log("rx touch event on object ff: " + touchedGameObject.name);
                    if (touchedGameObject.CompareTag("BackGround"))
                    {
                        if (Debug.isDebugBuild)
                            Debug.Log("rx touch obj is BackGround: ");
                        currentTouchGameObject = Camera.main.gameObject;

                    }
                    else 
                    {
                        currentTouchGameObject = touchedGameObject;
                    }
                }
                else
                {
					currentTouchGameObject = Camera.main.gameObject;
                    if (Debug.isDebugBuild)
                        Debug.Log("nothing " );
                    if(deledateOnTouchNothing != null)
                    {
                        deledateOnTouchNothing();
                    }
                }
                //currentTouchGameObject.SendMessage("OnTouchs", SendMessageOptions.DontRequireReceiver);
                TouchEventInterface touchEventInterface = currentTouchGameObject.GetComponent<TouchEventInterface>();
                if (touchEventInterface != null)
                {
                    touchEventInterface.OnTouchs();
                }
                
            }
            else if(firstTouch.phase == TouchPhase.Ended || firstTouch.phase == TouchPhase.Canceled)
            {
                if (currentTouchGameObject != null)
                {
                    //currentTouchGameObject.SendMessage("OnTouchs", SendMessageOptions.DontRequireReceiver);
                    TouchEventInterface touchEventInterface = currentTouchGameObject.GetComponent<TouchEventInterface>();
                    if (touchEventInterface != null)
                    {
                        touchEventInterface.OnTouchs();
                    }
                    currentTouchGameObject = null;
                }
            }else
            {
                if (currentTouchGameObject != null)
                {
                    //currentTouchGameObject.SendMessage("OnTouchs", SendMessageOptions.DontRequireReceiver);
                    TouchEventInterface touchEventInterface = currentTouchGameObject.GetComponent<TouchEventInterface>();
                    if (touchEventInterface != null)
                    {
                        touchEventInterface.OnTouchs();
                    }
                
                }
            }
        }
	}
}
