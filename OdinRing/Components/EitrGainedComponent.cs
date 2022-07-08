using Jotunn.Managers;
using OdinRing.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OdinRing.Components
{
    public class EitrGainedComponent : MonoBehaviour
    {
        ParticleSystem main;
        ParticleSystem sub;
        float startTime;
        Rigidbody rb;

        public Player Player { get; set; }

        private void Awake()
        {
            //var systems = GetComponentsInChildren<ParticleSystem>();
            startTime = Time.time;
            //main = systems[0];
            //sub = systems[1];
            rb = GetComponent<Rigidbody>();
        }

        private void UpdateParticles(ParticleSystem system, float force)
        {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[system.particleCount];
            system.GetParticles(particles);
            Vector3 particleWorldPos;
            for (int i = 0; i < particles.Length; ++i)
            {
                ParticleSystem.Particle p = particles[i];
                switch (system.main.simulationSpace)
                {
                    case ParticleSystemSimulationSpace.Local:
                        {
                            particleWorldPos = transform.TransformPoint(p.position);
                            break;
                        }
                    case ParticleSystemSimulationSpace.Custom:
                        {
                            particleWorldPos = system.main.customSimulationSpace.TransformPoint(p.position);
                            break;
                        }
                    case ParticleSystemSimulationSpace.World:
                    default:
                        {
                            particleWorldPos = p.position;
                            break;
                        }
                }
                Vector3 directionToTarget = (Player.transform.position - particleWorldPos).normalized;
                Vector3 seekForce = directionToTarget * (Time.time - startTime) * Time.deltaTime;
                p.velocity += seekForce;
                particles[i] = p;
            }

            system.SetParticles(particles);
        }

        private void FixedUpdate()
        {
            if (Player == null)
            {
                return;
            }

            float duration = Time.time - startTime;
            if (duration > 3f)
            {
                ZNetScene.instance.Destroy(gameObject);
                return;
            }

            transform.position = Player.transform.position + Vector3.up;
        }
    }
}
