using UnityEngine;
using System.Collections;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherMakerBillboardScript : MonoBehaviour
    {
        [Tooltip("Camera to look at")]
        public Camera Camera;

        private void DoBillboard()
        {
            transform.LookAt(transform.position + (Camera.transform.rotation * Vector3.forward), Camera.transform.rotation * Vector3.up);
        }

        private void Start()
        {
            Camera = (Camera == null ? (Camera.main == null ? Camera.current : Camera.main) : Camera);

            DoBillboard();
        }

        private void Update()
        {
            DoBillboard();
        }
    }
}