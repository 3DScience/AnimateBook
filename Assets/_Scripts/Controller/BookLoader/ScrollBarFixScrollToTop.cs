using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ScrollBarFixScrollToTop : MonoBehaviour {
    private string NAME_SCROLL_BAR = "infomationBox/Scrollbar";
	void Update () {
        GameObject panel = gameObject.transform.Find(NAME_SCROLL_BAR).gameObject;
        panel.GetComponent<Scrollbar>().value = 1;
        Canvas.ForceUpdateCanvases();
        Destroy(this);
    }
}
