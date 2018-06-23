﻿using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class EffectPool<T> : SingletonMonoBehaviour<EffectPool<T>>
    {
        private List<EffectCtrl> m_lstEffect = new List<EffectCtrl>();
        private Queue<EffectCtrl> m_queuePool = new Queue<EffectCtrl>();
        private List<string> m_lstPath = new List<string>();

        public virtual GameObject CreateEffect(string path,bool autoDestory, Transform parent = null)
        {
            if (!m_lstPath.Contains(path)) m_lstPath.Add(path);
            EffectCtrl effectCtrl = GetEffectCtrl();
            if(parent != null)
            {
                parent.gameObject.AddChildToParent(effectCtrl.gameObject);
            }
            m_lstEffect.Add(effectCtrl);
            effectCtrl.Begin(path, autoDestory);
            return effectCtrl.gameObject;
        }

        private void DestroyEffect(EffectCtrl effect)
        {
            for (int i = m_lstEffect.Count - 1; i > -1; i--)
            {
                if (m_lstEffect[i] == effect)
                {
                    m_lstEffect.RemoveAt(i);
                    SaveEffectCtrl(effect);
                }
            }
        }

        public void DestroyEffectGO(GameObject go)
        {
            EffectCtrl effectCtrl = go.GetComponent<EffectCtrl>();
            if(effectCtrl != null)
            {
                DestroyEffect(effectCtrl);
            }
            else
            {
                CLog.LogError("销毁特效的对象上必须有EffectCtrl脚本");
            }
        }


        public void Clear()
        {
            for (int i = m_lstEffect.Count - 1; i > -1; i--)
            {
                EffectCtrl effectCtrl = m_lstEffect[i];
                m_lstEffect.RemoveAt(i);
                SaveEffectCtrl(effectCtrl);
            }
            m_lstEffect.Clear();
            for (int i = 0; i < m_lstPath.Count; i++)
            {
                GameObjectPool.Instance.Clear(m_lstPath[i]);
            }
            m_lstPath.Clear();
        }

        void Update()
        {
            for (int i = m_lstEffect.Count - 1; i > -1 ; i--)
            {
                if(!m_lstEffect[i].OnUpdate(Time.deltaTime))
                {
                    EffectCtrl effectCtrl = m_lstEffect[i];
                    m_lstEffect.RemoveAt(i);
                    SaveEffectCtrl(effectCtrl);
                }
            }
        }

        private EffectCtrl GetEffectCtrl()
        {
            EffectCtrl effectCtrl = null;
            if (m_queuePool.Count > 0)
            {
                effectCtrl = m_queuePool.Dequeue();
            }
            else
            {
                GameObject go = new GameObject("effect");
                this.gameObject.AddChildToParent(go);
                effectCtrl = go.AddComponentOnce<EffectCtrl>();
            }
            effectCtrl.gameObject.SetActive(true);
            return effectCtrl;
        }

        private void SaveEffectCtrl(EffectCtrl effectCtrl)
        {
            effectCtrl.End();
            this.gameObject.AddChildToParent(effectCtrl.gameObject);
            effectCtrl.gameObject.SetActive(false);
            m_queuePool.Enqueue(effectCtrl);
        }
    }
}
