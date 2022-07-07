using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    public Transform target;
    public float force = 10.0f;

    ParticleSystem ps;


    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
        ps.GetParticles(particles);
        Vector3 particleWorldPos;
        for (int i = 0; i  < particles.Length; ++i)
        {
            ParticleSystem.Particle p = particles[i];
            switch (ps.main.simulationSpace)
            {
                case ParticleSystemSimulationSpace.Local:
                    {
                        particleWorldPos = transform.TransformPoint(p.position);
                        break;
                    }
                case ParticleSystemSimulationSpace.Custom:
                    {
                        particleWorldPos = ps.main.customSimulationSpace.TransformPoint(p.position);
                        break;
                    }
                case ParticleSystemSimulationSpace.World:
                default:
                    {
                        particleWorldPos = p.position;
                        break;
                    }
            }
            Vector3 directionToTarget = (target.position - particleWorldPos).normalized;
            Vector3 seekForce = directionToTarget * force * Time.deltaTime;
            p.velocity += seekForce;
            particles[i] = p;
        }

        ps.SetParticles(particles);
    }
}
