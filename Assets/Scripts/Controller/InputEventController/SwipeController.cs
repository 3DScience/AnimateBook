using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public delegate void OnSwipe(SwipeController.SwipeType type);



public class SwipeController : MonoBehaviour
{
    public enum SwipeType
    {
        LEFT,
        RIGT,
        UP,
        DOWN,
        TOUCH
    }

    public OnSwipe onSwipeCallBack;

    private float fingerStartTime = 0.0f;
    private Vector2 fingerStartPos = Vector2.zero;

    private bool isSwipe = false;
    private float minSwipeDist = 50.0f;
    private float maxSwipeTime = 5f;

    // Update is called once per frame
    void Update()
    {

        if (Input.touchCount > 0)
        {
           
            Touch firstTouch = Input.touches[0];

                // Debug.Log("touch");
            switch (firstTouch.phase)
            {
                case TouchPhase.Began:
                    /* this is a new touch */
                    startDetectSwipe(firstTouch.position);
                    break;

                case TouchPhase.Canceled:
                    /* The touch is being canceled */
                    isSwipe = false;
                    break;

                case TouchPhase.Ended:
                    handleSwipe(firstTouch.position);
                    break;
            }
            
        }else
        {
            if (Input.GetMouseButtonDown(0))
            {
                startDetectSwipe(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0))
            {
                handleSwipe(Input.mousePosition);
            }
        }

    }
    void startDetectSwipe(Vector2 position)
    {
        if (Camera.main == null)
            return;
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, position);
        if (!hit) // touch on none colider
        {
            isSwipe = true;
            fingerStartTime = Time.time;
            fingerStartPos = position;
            //Debug.Log("touch on none colider");
        }
        else
        {
            //Debug.Log("touch colider " + hit.transform.gameObject);
        }
    }
    void handleSwipe(Vector2 secondSosition)
    {
        float gestureTime = Time.time - fingerStartTime;
        float gestureDist = (secondSosition - fingerStartPos).magnitude;
        if (isSwipe )
        {
            if (gestureTime < maxSwipeTime && gestureDist > minSwipeDist)
            {
                Vector2 direction = secondSosition - fingerStartPos;
                Vector2 swipeType = Vector2.zero;

                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    // the swipe is horizontal:
                    swipeType = Vector2.right * Mathf.Sign(direction.x);
                }
                else
                {
                    // the swipe is vertical:
                    swipeType = Vector2.up * Mathf.Sign(direction.y);
                }

                if (swipeType.x != 0.0f)
                {
                    if (swipeType.x > 0.0f)
                    {
                        // MOVE RIGHT
                        if (onSwipeCallBack != null)
                            onSwipeCallBack(SwipeType.RIGT);
                        //Debug.Log("MOVE RIGHT");
                    }
                    else
                    {
                        // MOVE LEFT
                        if (onSwipeCallBack != null)
                            onSwipeCallBack(SwipeType.LEFT);
                        //Debug.Log("MOVE LEFT");
                    }
                }

                if (swipeType.y != 0.0f)
                {
                    if (swipeType.y > 0.0f)
                    {
                        // MOVE UP
                        if (onSwipeCallBack != null)
                            onSwipeCallBack(SwipeType.UP);
                       // Debug.Log("MOVE UP");
                    }
                    else
                    {
                        // MOVE DOWN
                        if (onSwipeCallBack != null)
                            onSwipeCallBack(SwipeType.DOWN);
                        //Debug.Log("MOVE DOWN");
                    }
                }
            }else
            {
                if (onSwipeCallBack != null)
                    onSwipeCallBack(SwipeType.TOUCH);
                //Debug.Log("TOUCH ONLY");
            }
        }
        isSwipe = false;
   
    }


}