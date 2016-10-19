using UnityEngine;
using System.Collections;

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
                if (Camera.main == null)
                    return;
                Ray ray = Camera.main.ScreenPointToRay(firstTouch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject touchedGameObject = hit.transform.gameObject;
                    if (Debug.isDebugBuild)
                        Debug.Log("rx touch event on object: " + touchedGameObject.name);
                    if (touchedGameObject.CompareTag("BackGround"))
                    {
                        if (Debug.isDebugBuild)
                            Debug.Log("rx touch obj is BackGround: ");
                        //Camera.main.SendMessage("OnTouchs", SendMessageOptions.DontRequireReceiver);
                        currentTouchGameObject = Camera.main.gameObject;

                    }
                    else 
                    {
                        currentTouchGameObject = touchedGameObject;
                    }
                    currentTouchGameObject.SendMessage("OnTouchs", SendMessageOptions.DontRequireReceiver);

                }
                else
                {

                   
                    currentTouchGameObject = Camera.main.gameObject;
                    currentTouchGameObject.SendMessage("OnTouchs", SendMessageOptions.DontRequireReceiver);
                    //if (Debug.isDebugBuild)
                    //    Debug.Log("nothing " );
                }
            }
            else if(firstTouch.phase == TouchPhase.Ended || firstTouch.phase == TouchPhase.Canceled)
            {
                if (currentTouchGameObject != null)
                { 
                    currentTouchGameObject.SendMessage("OnTouchs", SendMessageOptions.DontRequireReceiver);
                    currentTouchGameObject = null;
                }
            }else
            {
                if (currentTouchGameObject != null)
                {
                    currentTouchGameObject.SendMessage("OnTouchs", SendMessageOptions.DontRequireReceiver);
                }
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
