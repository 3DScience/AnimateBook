#pragma strict

private var hSliderValue : float = 0.0;
 private var myAnimation : AnimationState; 
 
 
 function Start(){
     myAnimation = GetComponent.<Animation>()["AllPageTurnReal"];
 }
  
 function LateUpdate() {     
     myAnimation.time = hSliderValue;
     myAnimation.enabled = true;
      
     GetComponent.<Animation>().Sample();
     myAnimation.enabled = false;  
 }
  
 function OnGUI() {
     // Horizontal slider
     GUILayout.BeginArea (Rect (60,Screen.height - 30,1220,60));
     hSliderValue = GUILayout.HorizontalSlider (hSliderValue, 0.0, myAnimation.length, GUILayout.Width(1200.0f));
     GUILayout.EndArea ();
     
 }