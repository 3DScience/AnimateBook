using UnityEngine;
using System.Collections;

public class DebugOnScreen : MonoBehaviour {
    static bool msg = true;
    static string msgStr = "";
    Vector2 scrollPos;
    static int i;
    void OnGUI()
    {
        if (msg)
        {
            
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width-50), GUILayout.Height(Screen.height-50));
            GUIStyle gui = new GUIStyle();
            gui.fontSize = 30;
            gui.wordWrap = true;
            
            // string text = System.IO.File.ReadAllText("D:/ping.bat");
            GUILayout.Label( msgStr, gui);
            GUILayout.EndScrollView();
            scrollPos.y = Mathf.Infinity;
        }
    }
    public static void Log(string str)
    {
        if(Camera.main.gameObject.GetComponent<DebugOnScreen>()==null)
        {
            Camera.main.gameObject.AddComponent<DebugOnScreen>();
        }
        msg = true;
        msgStr =msgStr+"\n"+i+".    "+str ;
        i++;
    }
    public void UnLog()
    {

        msg = false;
    }

}
