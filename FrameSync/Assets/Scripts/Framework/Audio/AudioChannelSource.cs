using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class AudioChannelSource
    {
        public delegate void AudioChannelHandler(AudioSource audioSource);
        public event AudioChannelHandler OnStartPlay;
        public event AudioChannelHandler OnStopPlay;

        private Transform m_cTransform;
        private AudioSource m_cAudioSource;
        private bool m_bPlaying;
        private int m_nPriority;
        private string m_sPath;
        private Resource m_cRes;
        
        public AudioChannelSource()
        {
            GameObject go = new GameObject();
            m_cAudioSource = go.AddComponent<AudioSource>();
            m_cTransform = go.transform;
            m_cAudioSource.playOnAwake = false;
            m_cTransform.parent = AudioSys.Instance.audioChannelObj.transform;
            m_cTransform.localPosition = Vector3.zero;
            m_cAudioSource.clip = null;
            m_cRes = null;
        }

        public void Play(string path,int priority,int maxPriority,Vector3 pos)
        {
            m_cTransform.parent = AudioSys.Instance.audioChannelObj.transform;
            m_cTransform.position = pos;
            Play(path, priority, maxPriority);
        }

        public void Play(string path, int priority, int maxPriority, Transform trans)
        {
            m_cTransform.parent = trans;
            m_cTransform.localPosition = Vector3.zero;
            m_cTransform.localEulerAngles = Vector3.zero;
            m_cTransform.localScale = Vector3.one;
            Play(path, priority, maxPriority);
        }

        private void Play(string path, int priority, int maxPriority)
        {
            if (m_bPlaying) Stop();
            m_sPath = path;

            if (priority < 0) priority = 0;
            if (priority > 255) priority = (int)((float)priority / maxPriority * 255);
            m_nPriority = 255 - priority;
            if (m_nPriority < 0) m_nPriority = 0;

            m_bPlaying = true;

            //音频资源在非editor下必须打bundle
            ResourceSys.Instance.GetResource(m_sPath, OnLoad, OnLoad);
        }

        private void OnLoad(Resource res,string path)
        {
            if(res.isSucc)
            {
                m_cRes = res;
                m_cRes.Retain();
                var clip = (AudioClip)m_cRes.GetAsset(m_sPath);
                if(clip != null)
                {
                    m_cAudioSource.clip = clip;
                    m_cAudioSource.Play();
                }
                else
                {
                    Stop();
                }
            }
            else
            {
                CLog.LogError("播放音频资源"+m_sPath+"失败");
            }
        }

        public void Update()
        {
            if(m_bPlaying)
            {
                if(m_cAudioSource.clip != null && !m_cAudioSource.isPlaying)
                {
                    Stop();
                }
            }
        }

        public void Stop()
        {
            if (m_bPlaying)
            {
                if(m_cRes != null)
                {
                    m_cRes.Release();
                    m_cRes = null;
                }
                m_bPlaying = false;
                m_cAudioSource.clip = null;
                m_cTransform.parent = AudioSys.Instance.audioChannelObj.transform;
                if (!string.IsNullOrEmpty(m_sPath))
                {
                    ResourceSys.Instance.RemoveListener(m_sPath, OnLoad, OnLoad);
                }
                m_sPath = null;
            }
        }
    }
}
