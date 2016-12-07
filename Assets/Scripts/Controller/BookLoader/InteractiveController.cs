﻿using UnityEngine;
using System.Collections;
using Entities;
using System.Collections.Generic;
public class InteractiveController : MonoBehaviour {

    public System.Action<Action> interactiveCallBack;
    public DisplayTextUiController displayTextUiController;
    public MainObject mainObject;
    private bool isITweenPlaying = false;
    private bool isDraging = false;
    private Vector3 screenPoint;
    private Vector3 offset;
    private float startTouchTime;
    private Vector2 fingerStartPos;
    private float minDragDistance = 20.0f;

    protected Dictionary<INTERACTIVE_EVENT, Interactive> listInteractives = new Dictionary<INTERACTIVE_EVENT, Interactive>();
    // Use this for initialization
    void Start () {
        if (Debug.isDebugBuild)
            Debug.Log("[InteractiveController] start with mainObject=" + mainObject + "...");
        if (mainObject.movePath != null)
        {
            if (Debug.isDebugBuild)
                Debug.Log("[InteractiveController] processing for mainObject name=" + mainObject.ObjectName + ", movePath=" + mainObject.movePath);

            iTween.MoveFrom(GameObject.Find(mainObject.ObjectName), iTween.Hash("path", iTweenPath.GetPath(mainObject.movePath), "orienttopath", true,"time", 20, "easetype", iTween.EaseType.linear, "looptype", "loop"));
            if(mainObject.defaultAnimation!=null && !mainObject.defaultAnimation.Equals(""))
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
                DebugOnScreen.Log(exx.ToString());
            }

        }
        if (listInteractives.ContainsKey(INTERACTIVE_EVENT.ONLOAD))
        {
            Interactive interactive = listInteractives[INTERACTIVE_EVENT.ONLOAD];
            doActions(interactive);
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
    

    void OnMouseDown()
    {
        isDraging = false;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, screenPoint.z));
        fingerStartPos = Input.touches[0].position;
        startTouchTime = Time.time;
        if (Debug.isDebugBuild)
            Debug.Log("[InteractiveController] OnMouseDown!.");

    }
    void OnMouseDrag()
    {
        float time = (Time.time - startTouchTime);
        //if (Debug.isDebugBuild)
        //    Debug.Log("[InteractiveController] OnTouchMoved!" + time+"/"+ System.DateTime.Now.Millisecond);
        if (time >=0)
        {
            float gestureDist = (Input.touches[0].position - fingerStartPos).magnitude;
            if (gestureDist > minDragDistance)
            {
                onInteractiveDrag();
                if (isITweenPlaying)
                    pauseMovePath();
                isDraging = true;
            }
        }

    }
    //void OnMouseOver()
    //{

    //}
    void OnMouseUp()
    {
        if (Debug.isDebugBuild)
            Debug.Log("[InteractiveController] OnMouseUp, isDraging=" + isDraging);
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
    protected void onExplorerButtonClick()
    {
        if (Debug.isDebugBuild)
            Debug.Log("[InteractiveController] onExplorerButtonClick");
        if (listInteractives.ContainsKey(INTERACTIVE_EVENT.EXPLORER_BUTTON_CLICK))
        {
            Interactive interactive = listInteractives[INTERACTIVE_EVENT.EXPLORER_BUTTON_CLICK];
            doActions(interactive);
        }
    }
    protected void onMoveCameraEnd()
    {
        if (Debug.isDebugBuild)
            Debug.Log("[InteractiveController] onMoveCameraEnd");
        if (listInteractives.ContainsKey(INTERACTIVE_EVENT.ANIMATE_CAMERA_END))
        {
            Interactive interactive = listInteractives[INTERACTIVE_EVENT.ANIMATE_CAMERA_END];
            doActions(interactive);
        }
    }
    protected void doActions(Interactive interactive)
    {
        foreach (Action action in interactive.actions)
        {
            INTERACTIVE_ACTION action_ = (INTERACTIVE_ACTION)System.Enum.Parse(typeof(INTERACTIVE_ACTION), action.actionName, true);
            switch (action_)
            {
                case INTERACTIVE_ACTION.SCALE:
                    doScale(interactive, action);
                    break;
                case INTERACTIVE_ACTION.ANIMATE:
                    doAnimate(action);
                    break;
                case INTERACTIVE_ACTION.MOVE:
                    doDrag(interactive);
                    break;
                case INTERACTIVE_ACTION.ROTATE:
                    doRotate(interactive);
                    break;
                case INTERACTIVE_ACTION.CHANGE_SCENE:
                    doChangeScene(action);
                    break;
                case INTERACTIVE_ACTION.SHOW_TEXT:
                    doShowtext(action);
                   // interactiveCallBack(action);
                    break;
                case INTERACTIVE_ACTION.ANIMATE_CAMERA:
                    doAnimateCamera(action);
                    // interactiveCallBack(action);
                    break;
                case INTERACTIVE_ACTION.MOVE_CAMERA:
                    doMoveCamera(action);
                    // interactiveCallBack(action);
                    break;
                case INTERACTIVE_ACTION.NONE:
                    break;
                default:
                    break;
            }

        }
    }
    protected void doMoveCamera(Action action)
    {
        //if (Debug.isDebugBuild)
        //    Debug.Log("[InteractiveController] doMoveCamera");
        if (Camera.main == null)
        {
            if (Debug.isDebugBuild)
                Debug.Log("[InteractiveController-doMoveCamera] Got camera is null");
            return;
        }
        TouchEventInterface camControler= Camera.main.GetComponent<TouchEventInterface>();
        camControler.OnTouchs();
        displayTextUiController.hideTextUi();
    }
    protected void doAnimateCamera(Action action)
    {
        if (!mainObject.skipAnimation)
        {
            if (Camera.main == null)
            {
                if (Debug.isDebugBuild)
                    Debug.Log("[InteractiveController-doAnimateCamera] Got camera is null");
                return;
            }
            MovingCam movingCam = Camera.main.gameObject.AddComponent<MovingCam>();
            Transform startMarker = Camera.main.transform.FindChild(action.getDictionaryActionParam()["startMarker"]);
            Transform endMarker = Camera.main.transform.FindChild(action.getDictionaryActionParam()["endMarker"]);
            movingCam.startMarker = startMarker;
            movingCam.endMarker = endMarker;
            movingCam.onMoveCameraEnd = onMoveCameraEnd;
            if (action.getDictionaryActionParam()["playOneTime"].Equals("true") ){
                mainObject.skipAnimation = true;
            }
        }

        if (Debug.isDebugBuild)
            Debug.Log("[InteractiveController] doAnimateCamera.");
    }
    protected void doShowtext(Action action)
    {
        StartCoroutine(displayTextUiController.showTextUi(mainObject, bool.Parse(action.getDictionaryActionParam()["hideOnTouchNothing"]), onExplorerButtonClick));
    }
    protected void doChangeScene(Action action)
    {
        //Debug.Log("doChangeScene .............................");
        if(interactiveCallBack!=null)
        {
            interactiveCallBack(action);
        }
        //SceneHelper.ChangeScene(action.actionParam);
    }
    protected void doAnimate(Action action)
    {
        string animationName;
        if (isITweenPlaying)
        {
            animationName = mainObject.defaultAnimation;
        }else
        {
            animationName = action.actionParams[0].paramValue;
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
    protected void doRotate(Interactive interactive)
    {
        //int rotateSpeed = 20;
        if (Debug.isDebugBuild)
            Debug.Log("[InteractiveController] doRotate.");
        transform.RotateAround(transform.position, new Vector3(0, 1, 0), -Input.touches[0].deltaPosition.x);
        transform.RotateAround(transform.position, new Vector3(1, 0, 0), Input.touches[0].deltaPosition.y);

    }
    protected void doScale(Interactive interactive,Action action)
    {
        if (!interactive.isScaling)
        {
           // Debug.Log("scale!");
            gameObject.transform.localScale = gameObject.transform.localScale * float.Parse(action.actionParams[0].paramValue);
            interactive.isScaling = true;
        }
        else
        {
           // Debug.Log("unscale!");
            gameObject.transform.localScale = gameObject.transform.localScale / float.Parse(action.actionParams[0].paramValue);
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

    void OnDestroy()
    {
        if (Debug.isDebugBuild)
            Debug.Log("[InteractiveController] Script was destroyed <====================");
      
    }
}
