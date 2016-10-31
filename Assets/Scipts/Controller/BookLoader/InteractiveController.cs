using UnityEngine;
using System.Collections;
using Entities;
using System.Collections.Generic;
public class InteractiveController : MonoBehaviour,TouchEventInterface {
 

    public MainObject mainObject;
    private bool isITweenPlaying = false;
    private bool isDraging = false;
    protected Dictionary<INTERACTIVE_EVENT, Interactive> listInteractives = new Dictionary<INTERACTIVE_EVENT, Interactive>();
    // Use this for initialization
    void Start () {
        if (Debug.isDebugBuild)
            Debug.Log("InteractiveController start with mainObject=" + mainObject + "...");
        if (mainObject.movePath != null)
        {
            if (Debug.isDebugBuild)
                Debug.Log("processing for mainObject name=" + mainObject.ObjectName + ", movePath=" + mainObject.movePath);

            iTween.MoveFrom(GameObject.Find(mainObject.ObjectName), iTween.Hash("path", iTweenPath.GetPath(mainObject.movePath), "orienttopath", true,"time", 20, "easetype", iTween.EaseType.linear, "looptype", "loop"));
            gameObject.GetComponent<Animation>().Play(mainObject.defaultAnimation); 
            isITweenPlaying = true;
        }

        foreach (Interactive interactive in mainObject.interactives)
        {
           // Debug.Log(" interactive+"+ interactive.eventName);
            try
            {
                INTERACTIVE_EVENT event_ = (INTERACTIVE_EVENT)System.Enum.Parse(typeof(INTERACTIVE_EVENT),interactive.eventName,true);
        
                listInteractives.Add(event_, interactive);

              // Debug.Log("INTERACTIVE_EVENT: " + event_.ToString() + " , INTERACTIVE_ACTION: " + interactive.actions[0].actionName);
            }
            catch (System.Exception exx)
            {

                throw new System.Exception("Outer", exx);
            }

        }
    }
	
    // -http://forum.unity3d.com/threads/add-component-to-3d-object-with-parameters-to-constructor.334757/
    public static InteractiveController addToGameObject(MainObject mainObject)
    {
        GameObject gameObject = GameObject.Find(mainObject.ObjectName);
        InteractiveController interactiveController = gameObject.AddComponent<InteractiveController>();
        interactiveController.mainObject = mainObject;
        return interactiveController;
    }
    

	private Vector3 screenPoint;
	private Vector3 offset;
    private float startTouchTime;

    public void OnTouchs()
    {
        Touch firstTouch = Input.touches[0];
        if (firstTouch.phase == TouchPhase.Began)
        {
            OnTouchDown();
        }
        else if (firstTouch.phase == TouchPhase.Moved)
        {
            OnTouchMoved();
        }
        else if (firstTouch.phase == TouchPhase.Stationary)
        {
            OnTouchStay();
        }
        else if (firstTouch.phase == TouchPhase.Ended)
        {
            OnTouchUp();
        }
        else if (firstTouch.phase == TouchPhase.Canceled)
        {
            //OnTouchExit();
        }
    }
    void OnTouchDown()
    {
        isDraging = false;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, screenPoint.z));

        startTouchTime = Time.time;
        if (Debug.isDebugBuild)
            Debug.Log("OnTouchDown!"+ System.DateTime.Now.Millisecond);

    }
    void OnTouchMoved()
    {
        float time = (Time.time - startTouchTime);
        if (Debug.isDebugBuild)
            Debug.Log("OnTouchMoved!"+ time+"/"+ System.DateTime.Now.Millisecond);
        if (time >=0)
        {
            onInteractiveDrag();
            if (isITweenPlaying)
                pauseMovePath();
            isDraging = true;
        }

    }
    void OnTouchStay()
    {

        float time = (Time.time - startTouchTime);
        //Debug.Log("OnTouchStay!" + time);
    }
    void OnTouchUp()
    {
        if (Debug.isDebugBuild)
            Debug.Log("OnTouchUp!");
        // iTween.Resume(gameObject);
        if (!isDraging)
        {
            playMovePath();
            onInteractiveTouch();
        }
        else
        {
            onInteractiveDrop();
        }
            
    }
    void Canceled()
    {

    }
    protected void onInteractiveTouch()
    {
        if (listInteractives.ContainsKey(INTERACTIVE_EVENT.TOUCH))
        {
            Interactive interactive = listInteractives[INTERACTIVE_EVENT.TOUCH];
            doActions(interactive);
        }
    }
    protected void onInteractiveDrag()
    {
        if (listInteractives.ContainsKey(INTERACTIVE_EVENT.DRAG))
        {
            Interactive interactive = listInteractives[INTERACTIVE_EVENT.DRAG];
            doActions(interactive);
        }
    }
    protected void onInteractiveDrop()
    {
        if (listInteractives.ContainsKey(INTERACTIVE_EVENT.DROP))
        {
            Interactive interactive = listInteractives[INTERACTIVE_EVENT.DROP];
            doActions(interactive);
        }
    }
    protected void doActions(Interactive interactive)
    {
        foreach (Action action in interactive.actions)
        {
            INTERACTIVE_ACTION action_ = (INTERACTIVE_ACTION)System.Enum.Parse(typeof(INTERACTIVE_ACTION), action.actionName, true);
            if (action_ == INTERACTIVE_ACTION.SCALE)
            {
                doScale(interactive,action);
            }
            else if (action_ == INTERACTIVE_ACTION.ANIMATION)
            {
                doAnimation(action);

            }
            else if (action_ == INTERACTIVE_ACTION.MOVE)
            {
                doDrag(interactive);
            }
        }
    }
    protected void doAnimation(Action action)
    {
        string animationName;
        if (isITweenPlaying)
        {
            animationName = mainObject.defaultAnimation;
        }else
        {
            animationName = action.actionParam;
        }
        gameObject.GetComponent<Animation>().Play(animationName);
    }
    protected void doDrag(Interactive interactive)
    {
        UnityEngine.Cursor.visible = true;

        Vector3 cursorPoint = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, screenPoint.z);
        
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
        Debug.Log("offset=" + cursorPosition);
        cursorPosition.y = gameObject.transform.position.y;
        gameObject.transform.position = cursorPosition;

    }
    protected void doScale(Interactive interactive,Action action)
    {
        if (!interactive.isScaling)
        {
           // Debug.Log("scale!");
            gameObject.transform.localScale = gameObject.transform.localScale * float.Parse(action.actionParam);
            interactive.isScaling = true;
        }
        else
        {
           // Debug.Log("unscale!");
            gameObject.transform.localScale = gameObject.transform.localScale / float.Parse(action.actionParam);
            interactive.isScaling = false;
        }
    }
    protected void playMovePath()
    {
        if (isITweenPlaying)
        {
            pauseMovePath();
        }
        else
        {
            resumeMovePath();
        }
    }
    private void pauseMovePath()
    {
        iTween.Pause(gameObject);
        isITweenPlaying = false;
    }
    private void resumeMovePath()
    {
        iTween.Resume(gameObject);
        isITweenPlaying = true;
    }

 
}
