using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class AudioSys : Singleton<AudioSys>
    {

        private GameObject m_cAudioListenerObj;
        
        public GameObject audioListenerObj
        {
            get
            {
                if (m_cAudioListenerObj == null)
                {
                    m_cAudioListenerObj = new GameObject("AudioListen");
                    m_cAudioListenerObj.AddComponent<AudioListener>();
                    GameObject.DontDestroyOnLoad(m_cAudioListenerObj);
                }
                return m_cAudioListenerObj;
            }
        }

        private GameObject m_cAudioChannelObj;
        public GameObject audioChannelObj
        {
            get
            {
                if (m_cAudioChannelObj == null)
                {
                    m_cAudioChannelObj = new GameObject("AudioChannels");
                    m_cAudioChannelObj.transform.position = Vector3.zero;
                    GameObject.DontDestroyOnLoad(m_cAudioChannelObj);
                }
                return m_cAudioChannelObj;
            }
        }

        public void Play(string path,bool loop, int priority,Vector3 pos)
        {

        }

        public void Stop()
        {

        }
    }
}
