using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
namespace Fungus
{
    [CommandInfo("custom",
          "AmimateCamera",
          "do moving camera from far to near")]
    [AddComponentMenu("")]
    public class AnimateCamera : Command
    {
        [Tooltip("start pos")]
        [SerializeField]
        protected Transform startMarker;
        [Tooltip("target pos")]
        [SerializeField]
        protected Transform endMarker;
        public override void OnEnter()
        {
            if(Camera.main != null)
            {
                MovingCam movingCam=Camera.main.gameObject.AddComponent<MovingCam>();
                movingCam.startMarker = startMarker;
                movingCam.endMarker = endMarker;
            }
            Continue();
        }
    }
}
