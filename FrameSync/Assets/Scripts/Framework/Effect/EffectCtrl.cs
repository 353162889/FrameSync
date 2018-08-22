using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class EffectCtrl : MonoBehaviour
    {
        private float m_fDuration;
        public float duration { get { return m_fDuration; } }

        private bool m_bAutoDestory;
        public bool autoDestory { get { return m_bAutoDestory; } }
        private string m_sPath;
        public string path { get { return m_sPath; } }
        private bool m_bDestory;
        public bool destory { get { return m_bDestory; } }

        private float m_fDestoryTime;

        private EffectInfo m_cEffectInfo;

        private void OnLoadResource(string path, UnityEngine.Object obj)
        {
            GameObject go = (GameObject)obj;
            this.gameObject.AddChildToParent(go);
            m_cEffectInfo = go.AddComponentOnce<EffectInfo>();
            m_fDuration = m_cEffectInfo.duration;
            if (m_bAutoDestory)
            {
                m_fDestoryTime = m_fDuration;
            }
            else
            {
                m_fDestoryTime = 0;
            }
        }

        public void Begin(string path,bool autoDestory)
        {
            m_bAutoDestory = autoDestory;
            m_sPath = path;
            this.name = path;
            m_fDuration = 0;
            m_bDestory = false;
            m_cEffectInfo = null;
            ResourceObjectPool.Instance.GetObject(m_sPath,false, OnLoadResource);
        }

        public void End()
        {
            m_fDestoryTime = 0;
            m_bDestory = true;
            if (!string.IsNullOrEmpty(m_sPath))
            {
                ResourceObjectPool.Instance.RemoveCallback(m_sPath, OnLoadResource);
            }
            if (m_cEffectInfo != null)
            {
                ResourceObjectPool.Instance.SaveObject(m_sPath, m_cEffectInfo.gameObject);
                m_cEffectInfo = null;
            }
            m_sPath = null;
        }

        public bool OnUpdate(float deltaTime)
        {
            if (!m_bDestory)
            {
                if (m_bAutoDestory && m_cEffectInfo != null)
                {
                    m_fDestoryTime -= deltaTime;
                    if (m_fDestoryTime <= 0)
                    {
                        m_bDestory = true;
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
