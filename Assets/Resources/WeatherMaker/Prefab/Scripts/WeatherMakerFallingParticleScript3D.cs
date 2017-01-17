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
    public class WeatherMakerFallingParticleScript3D : WeatherMakerFallingParticleScript
    {
        [Header("3D Settings")]
        [Tooltip("The height above the camera that the particles will start falling from")]
        public float Height = 25.0f;

        [Tooltip("How far the particle system is ahead of the player")]
        public float ForwardOffset = -7.0f;

        [Tooltip("The height above the camera that the secondary particles will start falling from")]
        public float SecondaryHeight = 100.0f;

        [Tooltip("How far the secondary particle system is ahead of the player")]
        public float SecondaryForwardOffset = 25.0f;

        [Tooltip("The top y value of the mist particles")]
        public float MistHeight = 3.0f;

        [Header("Particle System Emitters")]
        [Tooltip("ParticleSystem Near Width")]
        [Range(0.0f, 10.0f)]
        public float ParticleSystemNearWidth = 5.0f;

        [Tooltip("ParticleSystem Far Width")]
        [Range(0.0f, 2000.0f)]
        public float ParticleSystemFarWidth = 70.0f;

        [Tooltip("ParticleSystem Near Depth")]
        [Range(0.0f, 100.0f)]
        public float ParticleSystemNearDepth = 0.25f;

        [Tooltip("ParticleSystem Far Depth")]
        [Range(0.0f, 500.0f)]
        public float ParticleSystemFarDepth = 50.0f;

        [Tooltip("ParticleSystemSecondary Near Width")]
        [Range(0.0f, 10.0f)]
        public float ParticleSystemSecondaryNearWidth = 5.0f;

        [Tooltip("ParticleSystemSecondary Far Width")]
        [Range(0.0f, 2000.0f)]
        public float ParticleSystemSecondaryFarWidth = 500.0f;

        [Tooltip("ParticleSystemSecondary Near Depth")]
        [Range(0.0f, 100.0f)]
        public float ParticleSystemSecondaryNearDepth = 0.25f;

        [Tooltip("ParticleSystemSecondary Far Depth")]
        [Range(0.0f, 500.0f)]
        public float ParticleSystemSecondaryFarDepth = 50.0f;

        [Tooltip("ParticleSystemMist Near Width")]
        [Range(0.0f, 10.0f)]
        public float ParticleSystemMistNearWidth = 5.0f;

        [Tooltip("ParticleSystemMist Far Width")]
        [Range(0.0f, 2000.0f)]
        public float ParticleSystemMistFarWidth = 70.0f;

        [Tooltip("ParticleSystemMist Near Depth")]
        [Range(0.0f, 100.0f)]
        public float ParticleSystemMistNearDepth = 0.25f;

        [Tooltip("ParticleSystemMist Far Depth")]
        [Range(0.0f, 500.0f)]
        public float ParticleSystemMistFarDepth = 50.0f;

        private void CreateMeshEmitter(ParticleSystem p, float nearWidth, float farWidth, float nearDepth, float farDepth)
        {
            if (p == null || p.shape.shapeType != ParticleSystemShapeType.Mesh)
            {
                return;
            }

            Mesh emitter = new Mesh();
            emitter.vertices = new Vector3[]
            {
                new Vector3(-nearWidth, 0.0f, nearDepth),
                new Vector3(nearWidth, 0.0f, nearDepth),
                new Vector3(-farWidth, 0.0f, farDepth),
                new Vector3(farWidth, 0.0f, farDepth)
            };
            emitter.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
            var s = p.shape;
            s.mesh = emitter;
            s.meshShapeType = ParticleSystemMeshShapeType.Triangle;
        }

        private void TransformParticleSystem(ParticleSystem p, float forward, float height, float rotationYModifier)
        {
            if (p == null)
            {
                return;
            }

            Vector3 pos = Camera.transform.position;
            Vector3 anchorForward = Camera.transform.forward;
            pos.x += anchorForward.x * forward;
            pos.y += height;
            pos.z += anchorForward.z * forward;
            p.transform.position = pos;
            Vector3 angles = p.transform.rotation.eulerAngles;
            p.transform.rotation = Quaternion.Euler(angles.x, Camera.transform.rotation.eulerAngles.y * rotationYModifier, angles.z);
        }

        protected virtual void UpdatePositions()
        {
            // keep particles and mist above the player
            TransformParticleSystem(ParticleSystem, ForwardOffset, Height, 1.0f);
            TransformParticleSystem(ParticleSystemSecondary, SecondaryForwardOffset, SecondaryHeight, 1.0f);
            TransformParticleSystem(MistParticleSystem, 0.0f, MistHeight, 0.0f);
        }

        protected override void Start()
        {
            base.Start();

            CreateMeshEmitter(ParticleSystem, ParticleSystemNearWidth, ParticleSystemFarWidth, ParticleSystemNearDepth, ParticleSystemFarDepth);
            CreateMeshEmitter(ParticleSystemSecondary, ParticleSystemSecondaryNearWidth, ParticleSystemSecondaryFarWidth, ParticleSystemSecondaryNearDepth, ParticleSystemSecondaryFarDepth);
            CreateMeshEmitter(MistParticleSystem, ParticleSystemMistNearDepth, ParticleSystemMistFarDepth, ParticleSystemMistNearDepth, ParticleSystemMistFarDepth);
        }

        protected override void Update()
        {
            base.Update();

            UpdatePositions();
        }
    }
}