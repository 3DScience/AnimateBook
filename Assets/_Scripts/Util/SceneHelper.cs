using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class SceneHelper {
    public static void ChangeScene(string sceen)
    {
        SceneManager.LoadScene(sceen);
    }
}
