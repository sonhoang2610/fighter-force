using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHomissile : MonoBehaviour
{
    public Transform Target;
    public float Force;
    public float delay;

    private ParticleSystem _ps;
    private ParticleSystem.MainModule _psMain;
    private ParticleSystem.Particle[] _particles;

    public bool IsRunning { get; private set; }

    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        _psMain = _ps.main;
        _particles = new ParticleSystem.Particle[_psMain.maxParticles];
        IsRunning = false;
    }
    private void OnEnable()
    {
        IsRunning = false;
        Invoke( nameof(RunStart),delay);
    }

    public void RunStart()
    {
        IsRunning = true;
    }
    private void LateUpdate()
    {
        if (!IsRunning)
            return;

        if (Target == null)
            return;

        _ps.GetParticles(_particles);

        var originalTargetPos = Target.position;

        //if (FollowColliderCenter)
        //{
        //    var coll = Target.GetComponent<Collider>();
        //    if (coll != null)
        //        originalTargetPos = coll.bounds.center;
        //}
        //else originalTargetPos = Target.position;

        Vector3 resultTargetPos;

        switch (_psMain.simulationSpace)
        {
            case ParticleSystemSimulationSpace.Local:
                {
                    resultTargetPos = transform.InverseTransformPoint(originalTargetPos);
                    break;
                }
            case ParticleSystemSimulationSpace.Custom:
                {
                    resultTargetPos = _psMain.customSimulationSpace.InverseTransformPoint(originalTargetPos);
                    break;
                }
            case ParticleSystemSimulationSpace.World:
                {
                    resultTargetPos = originalTargetPos;
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException();
        }

        int particleCount = _ps.particleCount;

        for (int i = 0; i < particleCount; i++)
        {
            var dir = Vector3.Normalize(resultTargetPos - _particles[i].position);
            var force = dir * Force;
            if(Vector2.Distance(resultTargetPos,_particles[i].position) <= 0.1f)
            {
                _particles[i].remainingLifetime = 0;
            }
          
            _particles[i].velocity += force;
        }

        _ps.SetParticles(_particles, particleCount);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
