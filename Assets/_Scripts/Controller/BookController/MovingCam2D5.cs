using UnityEngine;
using System.Collections;

public class MovingCam2D5 : MonoBehaviour {
	
	public Transform startScenceMarker;
	public Transform endScenceMarker;

    public Transform startStoryTellScenceMarker;
    public Transform endStoryTellScenceMarker;

    public float move_speed = 0.6F;
    public float rotate_speed = 0.6F;

    private float startTime;
	private float journeyLength;

    private bool isStartScence = false;
    private bool isStoryTellScence = false;

    void Start() {
      
	}

	void Update() {
        if(isStartScence)   
            Move(startScenceMarker, endScenceMarker);

        if (isStoryTellScence) {
            Move(startStoryTellScenceMarker, endStoryTellScenceMarker);
            Rotate();
            float distance = Vector3.Distance(transform.position, endStoryTellScenceMarker.position);
            if (distance < 1f) { 
                rotate_speed = 1.8F;
                //Debug.Log("Rotate Now!");
            }
        }     
    }

    public void StartScence()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(startScenceMarker.position, endScenceMarker.position);
        isStartScence = true;
    }

    public void StoryTellScence()
    {       
        startTime = Time.time;
        journeyLength = Vector3.Distance(startStoryTellScenceMarker.position, endStoryTellScenceMarker.position);
        isStartScence = false;
        isStoryTellScence = true;
    }

    void Move(Transform startMarker, Transform endMarker)
    {
        if(startMarker != null && endMarker != null) { 
            float distCovered = (Time.time - startTime) * move_speed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
        }
    }

    void Rotate() {
        Quaternion target = Quaternion.Euler(0, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotate_speed);
    }
}
