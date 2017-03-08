
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Fungus
{
    public class ExecuteHandlerCustom : ExecuteHandler
    {
        protected void Update()
        {
          //  Debug.Log("ExecuteHandlerCustom -Update");
            base.Update();
            if (calculateTouchOnNothing())
            {
                if (IsExecuteMethodSelected(ExecuteMethod.OnTouchNothing) && ShouldExecuteOnFrame())
                {
                    Execute(ExecuteMethod.OnTouchNothing);
                }
            }
            
        }
        private bool calculateTouchOnNothing()
        {
            if (Input.touchCount > 0)
            {
                Touch firstTouch = Input.touches[0];
                if (firstTouch.phase == TouchPhase.Began)
                {
                    //DebugOnScreen.Log("on touch "+Time.deltaTime);
                    //DebugOnScreen.Log("EventSystem.current.currentSelectedGameObject=" + EventSystem.current.IsPointerOverGameObject(0) );
                    if (Camera.main == null || EventSystem.current.IsPointerOverGameObject(0))
                    {
                        Debug.Log("Camera.current =null or touch on some ui");
                        return false;
                    }

                    Ray ray = Camera.main.ScreenPointToRay(firstTouch.position);
                    RaycastHit hit;
                    if (!Physics.Raycast(ray, out hit))
                    {
                       // Debug.Log("fuck all "+hit.collider.gameObject.name);
                        return true;
                    }
                }
            }else  if (Input.GetMouseButton(0))
            {
                if (Camera.main == null || EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log("Camera.current =null or touch on some ui");
                    return false;
                }
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (!Physics.Raycast(ray, out hit))
                {
                    return true;
                }


            }
            return false;
        }
    }

    
}