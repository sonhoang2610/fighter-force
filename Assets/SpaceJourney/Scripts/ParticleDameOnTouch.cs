using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDameOnTouch : MonoBehaviour
{
    List<ParticleSystem.Particle> enters = new List<ParticleSystem.Particle>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other);
    }
}
