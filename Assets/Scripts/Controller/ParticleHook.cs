using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public class ParticleHook : MonoBehaviour
    {
        public ParticleSystem[] particles; 
	    public void Init ()
	    {
            particles = GetComponentsInChildren<ParticleSystem>();
	    }

        public void Emit(int v = 1)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Emit(v);
            }
        }
	
    }

}

