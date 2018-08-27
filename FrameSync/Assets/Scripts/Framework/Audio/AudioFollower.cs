using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class AudioFollower : MonoBehaviour
    {
        public AudioChannelSource m_cAudioSource;

        public void SetAudioSource(AudioChannelSource audioSource)
        {
            ClearAudioSource();
            if (!this.gameObject.activeSelf) return;
            m_cAudioSource = audioSource;
            if (m_cAudioSource != null)
            {
                m_cAudioSource.OnStopPlay += OnStopPlay;
                m_cAudioSource.transform.position = transform.position;
            }
        }

        public void ClearAudioSource()
        {
            if(m_cAudioSource != null)
            {
                m_cAudioSource.OnStopPlay -= OnStopPlay;
                m_cAudioSource = null;
            }
        }

        private void OnStopPlay(AudioChannelSource audioChannelSource)
        {
            ClearAudioSource();
        }

        void OnDisable()
        {
            ClearAudioSource();
        }

        void OnDestory()
        {
            ClearAudioSource();
        }

        void Update()
        {
            if(m_cAudioSource != null)
            {
                m_cAudioSource.transform.position = transform.position;
            }
        }
    }
}
