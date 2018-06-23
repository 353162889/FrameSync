using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class EffectInfo : MonoBehaviour
    {
        [SerializeField]
        private float m_fDuration;
        public float duration { get { return m_fDuration; } }

        void Awake()
        {
            m_fDuration = 0;
            ParticleSystem[] arrParticlesystems = gameObject.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < arrParticlesystems.Length; i++)
            {
                var particle = arrParticlesystems[i];
                float time = particle.main.duration + particle.main.startDelay.constantMax + particle.main.startLifetime.constantMax;
                if (time > m_fDuration)
                {
                    m_fDuration = time;
                }
            }
        }
    }
}
