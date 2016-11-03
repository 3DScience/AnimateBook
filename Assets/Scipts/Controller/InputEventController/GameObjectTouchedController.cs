using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public abstract  class GameObjectTouchedEvent:MonoBehaviour
{
    abstract public void OnMouseDown(string action, string param);
}
public class GameObjectTouchedController : MonoBehaviour {
    public GameObjectTouchedEvent receiver;
    public string action;
    public string param;

    // Use this for initialization
    void Start()
    {
        if (Debug.isDebugBuild)
            Debug.Log("GameObjectTouchedController Start...");
    }
    void OnMouseDown()
    {
       // Event.current.Use();
        if (Debug.isDebugBuild)
            Debug.Log("GameObjectTouchedController-OnMouseDown...");
        if (receiver != null)
        {
            receiver.OnMouseDown(action, param);
        }
    }
}
