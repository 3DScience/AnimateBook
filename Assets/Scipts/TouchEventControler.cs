using UnityEngine;
using System.Collections;

public class TouchEventControler : MonoBehaviour {

    private GameObject currentTouchGameObject;
    private 
	// Update is called once per frame
	void Update() {
        if (Input.touchCount > 0){
            Touch firstTouch= Input.touches[0];
            Ray ray = Camera.main.ScreenPointToRay(firstTouch.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                
                GameObject touchedGameObject = hit.transform.gameObject;
                if (Debug.isDebugBuild)
                    Debug.Log("rx touch event on object: " + touchedGameObject.name);
                if( touchedGameObject.CompareTag("BackGround" ))
                {
                    if (Debug.isDebugBuild)
                        Debug.Log("rx touch obj is BackGround: " );
                    gameObject.SendMessage("OnTouchs", SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    if (firstTouch.phase == TouchPhase.Began)
                    {
                        touchedGameObject.SendMessage("OnTouchDown", SendMessageOptions.DontRequireReceiver);
                    }
                    else if (firstTouch.phase == TouchPhase.Moved)
                    {
                        touchedGameObject.SendMessage("OnTouchMoved", SendMessageOptions.DontRequireReceiver);
                    }
                    else if (firstTouch.phase == TouchPhase.Stationary)
                    {
                        touchedGameObject.SendMessage("OnTouchStay", SendMessageOptions.DontRequireReceiver);
                    }
                    else if (firstTouch.phase == TouchPhase.Ended)
                    {
                        touchedGameObject.SendMessage("OnTouchUp", SendMessageOptions.DontRequireReceiver);
                    }
                    else if (firstTouch.phase == TouchPhase.Canceled)
                    {
                        touchedGameObject.SendMessage("OnTouchExit", SendMessageOptions.DontRequireReceiver);
                    }
                
                }
                if (currentTouchGameObject != touchedGameObject)
                {
                    if (currentTouchGameObject != null)
                        currentTouchGameObject.SendMessage("OnTouchExit", SendMessageOptions.DontRequireReceiver);
                    currentTouchGameObject = touchedGameObject;
                }


            }
            else
            {
                //if (Debug.isDebugBuild)
                //    Debug.Log("nothing " );
            }
        }else
        {
            if(currentTouchGameObject != null)
            {
                gameObject.SendMessage("OnTouchs", SendMessageOptions.DontRequireReceiver);
                currentTouchGameObject = null;
            }
        }
	}
}
