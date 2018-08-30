using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class AudioSys : SingletonMonoBehaviour<AudioSys>
    {
        private static int MaxAudioCount = 30;
        private GameObject m_cAudioListenerObj;
        private float[] m_arrVolume;
        
        public GameObject audioListenerObj
        {
            get
            {
                return m_cAudioListenerObj;
            }
        }

        private GameObject m_cAudioChannelObj;
        public GameObject audioChannelObj
        {
            get
            {
                return m_cAudioChannelObj;
            }
        }

        private LinkedList<AudioChannelSource> m_lstQueue;
        protected override void Init()
        {
            m_cAudioListenerObj = new GameObject();
            m_cAudioListenerObj.name = "AudioListen";
            m_cAudioListenerObj.transform.parent = this.transform;
            m_cAudioListenerObj.transform.position = Vector3.zero;
            m_cAudioListenerObj.AddComponent<AudioListener>();

            m_cAudioChannelObj = new GameObject("AudioChannels");
            m_cAudioChannelObj.name = "AudioChannels";
            m_cAudioChannelObj.transform.parent = this.transform;
            m_cAudioChannelObj.transform.position = Vector3.zero;

            m_lstQueue = new LinkedList<AudioChannelSource>();
            for (int i = 0; i < MaxAudioCount; i++)
            {
                AudioChannelSource audioChannelSource = new AudioChannelSource(i);
                audioChannelSource.OnStartPlay += OnStartPlay;
                audioChannelSource.OnStopPlay += OnStopPlay;
                m_lstQueue.AddLast(audioChannelSource);
            }
            m_arrVolume = new float[(int)AudioChannelType.MaxCount];
            for (int i = 0; i < m_arrVolume.Length; i++)
            {
                m_arrVolume[i] = 1f;
            }
        }

        protected override void OnDestroy()
        {
            while (m_lstQueue.Count > 0)
            {
                var source = m_lstQueue.First.Value;
                source.OnStartPlay -= OnStartPlay;
                source.OnStopPlay -= OnStopPlay;
                m_lstQueue.RemoveFirst();
                source.Clear();
            }
            base.OnDestroy();
        }

       

        public int Play(string path, AudioChannelType channelType, bool loop, int priority)
        {
            return Play(path, channelType, loop, priority, Vector3.zero);
        }

        public int Play(string path, AudioChannelType channelType, bool loop, int priority,Vector3 pos)
        {
            AudioChannelSource audioSource = GetChannelSource();
            if(audioSource != null)
            {
                audioSource.SetVolume(m_arrVolume[(int)channelType]);
                audioSource.Play(path, channelType, loop, priority, pos);
                return audioSource.key;
            }
            return -1;
        }

        public int Play(string path, AudioChannelType channelType, bool loop, int priority, Transform trans)
        {
            AudioChannelSource audioSource = GetChannelSource();
            if (audioSource != null)
            {
                audioSource.SetVolume(m_arrVolume[(int)channelType]);
                audioSource.Play(path, channelType, loop, priority, trans);
                return audioSource.key;
            }
            return -1;
        }

        public void SetVolume(float volume)
        {
            for (int i = 0; i < m_arrVolume.Length; i++)
            {
                m_arrVolume[i] = volume;
            }
            var node = m_lstQueue.First;
            while (node != null)
            {
                node.Value.SetVolume(volume);
                node = node.Next;
            }
        }

        public void SetVolume(AudioChannelType channelType,float volume)
        {
            m_arrVolume[(int)channelType] = volume;
            var node = m_lstQueue.First;
            while (node != null)
            {
                if (node.Value.channelType == channelType)
                {
                    node.Value.SetVolume(volume);
                }
                node = node.Next;
            }
        }

        public void Stop(string path,int key = -1)
        {
            var node = m_lstQueue.First;
            while (node != null)
            {
                if (key > -1)
                {
                    if(node.Value.key == key &&node.Value.path == path)
                    {
                        node.Value.Stop();
                    }
                }
                else
                {
                    if (node.Value.path == path)
                    {
                        node.Value.Stop();
                    }
                }
                node = node.Next;
            }
        }

        public void Stop(AudioChannelType channelType)
        {
            var node = m_lstQueue.First;
            while (node != null)
            {
                if (node.Value.channelType == channelType)
                {
                    node.Value.Stop();
                }
                node = node.Next;
            }
        }

        public void StopAll()
        {
            var node = m_lstQueue.First;
            while (node != null)
            {
                node.Value.Stop();
                node = node.Next;
            }
        }

        private void OnStartPlay(AudioChannelSource audioChannelSource)
        {
            //此时audioChannelSource还在lstQueue中
            var node = m_lstQueue.Find(audioChannelSource);
            m_lstQueue.Remove(node);
            var insertNode = m_lstQueue.First;
            while (insertNode != null)
            {
                if (node.Value.priority >= insertNode.Value.priority)
                {
                    break;
                }
                else
                {
                    insertNode = insertNode.Next;
                }
            }
            if (insertNode == null)
            {
                m_lstQueue.AddLast(node);
            }
            else
            {
                m_lstQueue.AddBefore(insertNode, node);
            }
        }

        private void OnStopPlay(AudioChannelSource audioChannelSource)
        {
            //此时audioChannelSource还在lstQueue中
            //重置优先级
            audioChannelSource.priority = 0;
            var node = m_lstQueue.Find(audioChannelSource);
            m_lstQueue.Remove(node);
            var insertNode = m_lstQueue.Last;
            while (insertNode != null)
            {
                if (insertNode.Value.priority > node.Value.priority)
                {
                    break;
                }
                else
                {
                    insertNode = insertNode.Previous;
                }
            }
            if (insertNode == null)
            {
                m_lstQueue.AddFirst(node);
            }
            else
            {
                m_lstQueue.AddAfter(insertNode, node);
            }
        }

        private AudioChannelSource GetChannelSource()
        {
            var last = m_lstQueue.Last;
            if (last != null) return last.Value;
            return null;
        }
    }
}
