using UnityEngine;
using System.Collections;

public interface TouchEventInterface
{
    void OnTouchs();
}
public class TouchEventControler : MonoBehaviour {

    private GameObject currentTouchGameObject;
    private 
	// Update is called once per frame
	void Update() {
        if (Input.touchCount > 0){
            Touch firstTouch= Input.touches[0];
            if( firstTouch.phase== TouchPhase.Began)
            {
                //DebugOnScreen.Log("on touch "+Time.deltaTime);
                if (Camera.current == null)
                {
                    Debug.Log("Camera.current =null ");
                    return;
                }
                   
                Ray ray = Camera.current.ScreenPointToRay(firstTouch.position);
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
