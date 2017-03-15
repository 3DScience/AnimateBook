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
    [RequireComponent(typeof(Renderer))]
    public class WeatherMakerBoxFogScript : WeatherMakerFogScript
    {
        [Header("Fog Box")]
        [Tooltip("Percentage of fog box to fill")]
        [Range(0.0f, 1.0f)]
        public float FogBoxPercentage = 0.9f;

        private Renderer _renderer;

        protected override void Awake()
        {
            base.Awake();

            this._renderer = GetComponent<Renderer>();
            this._renderer.sharedMaterial = FogMaterial;
        }

        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();

            Bounds b = _renderer.bounds;
            Vector3 shrinker = b.size * -(1.0f - FogBoxPercentage);
            b.Expand(shrinker);
            FogMaterial.SetVector("_FogBoxMin", b.min);
            FogMaterial.SetVector("_FogBoxMax", b.max);
        }
    }
}
