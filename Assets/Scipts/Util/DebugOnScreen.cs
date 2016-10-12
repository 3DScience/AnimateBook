using UnityEngine;
using System.Collections;

public class DebugOnScreen : MonoBehaviour {
    bool msg = true;
    string msgStr = "";
    void OnGUI()
    {
        if (msg)
        {
            GUIStyle gui = new GUIStyle();
            gui.fontSize = 30;
            gui.wordWrap = true;
            // string text = System.IO.File.ReadAllText("D:/ping.bat");
            GUI.Label(new Rect(10, Screen.height / 10, Screen.width, Screen.height), msgStr, gui);
        }
    }
    public static DebugOnScreen addToGameObject(GameObject gameObject) { 

        return gameObject.AddComponent<DebugOnScreen>();


    }
    public void Log(string str)
    {
        msg = true;
        msgStr = str;
    }
    public void UnLog()
    {
        msg = false;
    }

}
