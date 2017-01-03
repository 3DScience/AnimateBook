#pragma strict
 
 var Trigger : AudioClip;
 var isPlayingAudio : boolean = false;
 var DonePlaying : boolean = false;
 
 function OnMouseDown()
 {
     if (!isPlayingAudio)
     {
         isPlayingAudio = true;
         GetComponent.<AudioSource>().clip = Trigger;
         GetComponent.<AudioSource>().Play();
         Debug.Log("audio is Playing");
         Invoke( "AudioIsFinished", Trigger.length );
     }
 }
 
 function AudioIsFinished() 
 {
     Debug.Log("audio is Stopped");
     isPlayingAudio = false;
     DonePlaying = true;
     
     if(DonePlaying)
     {
     Application.Quit();
     
     }
     
     
  
 }
 

