//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;

namespace DigitalRuby.WeatherMaker
{
    [RequireComponent(typeof(MeshRenderer))]
    public class WeatherMakerFullScreenQuadScript : MonoBehaviour
    {
        [Tooltip("Camera the full screen quad will render in front of.")]
        public Camera Camera;

        [Tooltip("Offset away from camera.")]
        public float Offset = 0.01f;

        protected virtual void Awake()
        {
            Camera = (Camera == null ? (Camera.main == null ? Camera.current : Camera.main) : Camera);
            MeshRenderer = GetComponent<MeshRenderer>();

            // ensure depth texture
            Camera.depthTextureMode |= DepthTextureMode.Depth;
        }

        protected virtual void Update()
        {
        }

        protected virtual void LateUpdate()
        {
            float pos = Camera.nearClipPlane + Offset;
            float h = Mathf.Tan(Camera.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2.0f;
            transform.position = Camera.transform.position + (Camera.transform.forward * pos);
            transform.localScale = new Vector3((h * Camera.aspect), h, 1.0f);
            transform.rotation = Camera.transform.rotation;
        }

        public MeshRenderer MeshRenderer { get; private set; }
    }
}