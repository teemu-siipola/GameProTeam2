using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumVfx : MonoBehaviour
{
    public float speed;

    ParticleSystem _system;
    ParticleSystem.MainModule _mainSystem;
    ParticleSystem.Particle[] _particles;
    ParticleSystem.ShapeModule _shape;
    PlayerVacuum _control;

    float _distance;
    float _radius;

    void Awake()
    {
        _control = transform.root.GetComponent<PlayerVacuum>();
        _system = GetComponent<ParticleSystem>();
        _shape = _system.shape;
        _mainSystem = _system.main;
        _mainSystem.simulationSpace = ParticleSystemSimulationSpace.Local;
        _particles = new ParticleSystem.Particle[_mainSystem.maxParticles];
        _system.Stop();
    }

    void Start()
    {
        _distance = _control.vacuumRadius;
        _radius = 2 * Mathf.Tan( Mathf.Deg2Rad * (_control.vacuumingAngle * 0.5f)) * _distance;
    }

    void LateUpdate()
    {
        if (_distance != _shape.position.z || _radius != _shape.radius)
        {
            _shape.position = Vector3.forward * _distance;
            _shape.radius = _radius;
        }

        _mainSystem.startLifetime = _distance / speed;

        int particles = _system.GetParticles(_particles);

        Vector3 delta, velocity;
        for (int i = 0; i < particles; i++)
        {
            if (_particles[i].position.z <= 0)
            {
                _particles[i].remainingLifetime = 0;
                continue;
            }

            delta = _particles[i].position;
            velocity = -delta.normalized;
            velocity *= -speed / velocity.z;

            _particles[i].velocity = velocity;
        }
        _system.SetParticles(_particles, particles);
    }

    void Test()
    {

    }
}
