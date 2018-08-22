using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class MultiResourceObjectLoader
    {
        private Dictionary<string,bool> m_dicLoad;
        private Action<MultiResourceObjectLoader> m_cOnComplete;
        private Action<UnityEngine.Object, string> m_cOnProgress;

        public MultiResourceObjectLoader()
        {
            m_dicLoad = new Dictionary<string, bool>();
        }

        public void LoadList(List<string> names,bool isPrefab, Action<MultiResourceObjectLoader> OnComplete = null, Action<UnityEngine.Object, string> OnProgress = null)
        {
            if (names == null || names.Count == 0)
                return;
            for (int i = 0; i < names.Count; i++)
            {
                if(m_dicLoad.ContainsKey(names[i]))
                {
                    CLog.LogError("Can not has same name in one MultiResourceLoader");
                    continue;
                }
                else
                {
                    m_dicLoad.Add(names[i],false);
                }
            }
            if (LoadedFinish())
            {
                if(OnComplete != null)
                {
                    var temp = OnComplete;
                    temp.Invoke(this);
                }
            }
            else
            {
                this.m_cOnComplete = OnComplete;
                this.m_cOnProgress = OnProgress;
                foreach (var item in m_dicLoad)
                {
                    if (!item.Value)
                    {
                        ResourceObjectPool.Instance.GetObject(item.Key, isPrefab, OnLoad);
                    }
                }
            }
        }

        private void OnLoad(string path, UnityEngine.Object go)
        {
            if(m_dicLoad.ContainsKey(path))
            {
                m_dicLoad[path] = true;
            }

            if (m_cOnProgress != null)
            {
                Action< UnityEngine.Object, string> tempAction = m_cOnProgress;
                tempAction.Invoke(go, path);
            }
           
            if (LoadedFinish())
            {
                if (m_cOnComplete != null)
                {
                    Action<MultiResourceObjectLoader> tempAction = m_cOnComplete;
                    m_cOnComplete = null;
                    tempAction.Invoke(this);
                }
            }
        }

        private bool LoadedFinish()
        {
            bool finish = true;
            foreach (var item in m_dicLoad)
            {
                if (!item.Value) { finish = false; break; }
            }
            return finish;
        }

        public void Clear()
        {
            foreach (var item in m_dicLoad)
            {
                ResourceObjectPool.Instance.Clear(item.Key);
            }
            this.m_cOnComplete = null;
            this.m_cOnProgress = null;
        }
    }
}
