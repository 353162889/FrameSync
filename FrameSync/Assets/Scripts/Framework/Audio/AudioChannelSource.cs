using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public enum AudioChannelType
    {
        BG = 0,
        UI,
        Scene,
        MaxCount
    }

    public class AudioChannelSource
    {
        public static int MaxPriority = 10000;
        public delegate void AudioChannelHandler(AudioChannelSource audioChannelSource);
        public event AudioChannelHandler OnStartPlay;
        public event AudioChannelHandler OnStopPlay;

        public Transform transform { get { return m_cTransform; } }
        private Transform m_cTransform;
        private AudioSource m_cAudioSource;
        public bool playing { get { return m_bPlaying; } }
        private bool m_bPlaying;
        public int priority { get { return m_nPriority;  } set { m_nPriority = value; } }
        private int m_nPriority;
        public string path { get { return m_sPath; } }
        private string m_sPath;

        public AudioChannelType channelType { get { return m_eChannelType; } }
        private AudioChannelType m_eChannelType;

        public bool loop { get { return m_cAudioSource.loop; } }
        private float m_fTargetVolume;
        private float m_fStartVolume;
        private float m_fCurVolumeTime;
        private float m_fTargetVolumeTime;
        
        public AudioChannelSource()
        {
            GameObject go = new GameObject();
            go.name = "AudioChannelSource";
            m_cAudioSource = go.AddComponent<AudioSource>();
            m_cTransform = go.transform;
            m_cAudioSource.playOnAwake = false;
            m_cTransform.parent = AudioSys.Instance.audioChannelObj.transform;
            m_cTransform.localPosition = Vector3.zero;
            m_cAudioSource.clip = null;
            m_nPriority = 0;
        }

        public void Play(string path, AudioChannelType channelType,bool loop, int priority,Vector3 pos)
        {
            m_cTransform.position = pos;
            Play(path, channelType,loop, priority);
        }

        public void Play(string path, AudioChannelType channelType,bool loop, int priority, Transform trans)
        {
            AudioFollower follower = trans.gameObject.AddComponentOnce<AudioFollower>();
            follower.SetAudioSource(this);
            Play(path, channelType,loop, priority);
        }

        public void SetVolume(float volume,float time = 0)
        {
            m_fTargetVolume = volume;
            m_fTargetVolumeTime = time;
            if (time > 0)
            {
                m_fCurVolumeTime = time;
                m_fStartVolume = m_cAudioSource.volume;
            }
            else
            {
                m_fCurVolumeTime = 0;
                m_fStartVolume = volume;
                m_cAudioSource.volume = volume;
            }
        }
        
        public void Update()
        {
            if(m_bPlaying)
            {
                if(m_fCurVolumeTime > 0)
                {
                    m_fCurVolumeTime -= Time.deltaTime;
                    float percent = 1 - m_fCurVolumeTime / m_fTargetVolumeTime;
                    percent = Mathf.Clamp01(percent);
                    m_cAudioSource.volume = Mathf.Lerp(m_fStartVolume, m_fTargetVolume, percent);
                }
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
                m_bPlaying = false;
                if(null != OnStopPlay)
                {
                    OnStopPlay(this);
                }
                m_nPriority = 0;
                m_cAudioSource.clip = null;
                m_cTransform.parent = AudioSys.Instance.audioChannelObj.transform;
                if (!string.IsNullOrEmpty(m_sPath) && PrefabPool.Instance != null)
                {
                    PrefabPool.Instance.RemoveCallback(m_sPath, OnLoad);
                }
                m_sPath = null;
            }
        }

        public void Clear()
        {
            Stop();
            OnStartPlay = null;
            OnStopPlay = null;
            if(m_cTransform != null)
            {
                GameObject.Destroy(m_cTransform.gameObject);
                m_cTransform = null;
                m_cAudioSource = null;
            }
        }

        private void Play(string path, AudioChannelType channelType, bool loop, int priority)
        {
            if (m_bPlaying) Stop();
            m_sPath = path;
            m_nPriority = priority;
            m_eChannelType = channelType;
            if (priority < 0) priority = 0;
            if (priority > 255) priority = (int)((float)priority / MaxPriority * 255);
            priority = 255 - priority;
            if (priority < 0) priority = 0;
            m_cAudioSource.priority = priority;
            m_cAudioSource.loop = loop;
            m_cAudioSource.spatialize = m_eChannelType == AudioChannelType.Scene;
            m_cAudioSource.spatialBlend = 1f;
            m_cAudioSource.rolloffMode = AudioRolloffMode.Linear;

            m_bPlaying = true;

            if (null != OnStartPlay)
            {
                OnStartPlay(this);
            }

            //音频资源在非editor下必须打bundle
            PrefabPool.Instance.GetObject(m_sPath, OnLoad);
        }

        private void OnLoad(string path, UnityEngine.Object obj)
        {
            var clip = (AudioClip)obj;
            if (clip != null)
            {
                m_cAudioSource.clip = clip;
                m_cAudioSource.Play();
            }
            else
            {
                Stop();
            }
        }

    }
}
