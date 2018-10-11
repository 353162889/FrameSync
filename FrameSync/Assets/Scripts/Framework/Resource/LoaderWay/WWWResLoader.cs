using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class WWWResLoader : BaseResLoader
    {
        private Dictionary<WWW, Resource> m_dicLoadingQueue = new Dictionary<WWW, Resource>();
        private Dictionary<WWW, Resource> m_dicLoadedQueue = new Dictionary<WWW, Resource>();

        public override void Load(Resource res)
        {
            string url = GetInResPath(res);
            WWW www = new WWW(url);
            m_dicLoadingQueue.Add(www, res);
        }

        private void Update()
        {
            if (m_dicLoadingQueue.Count == 0) return;
            foreach (var item in m_dicLoadingQueue)
            {
                if (item.Key.isDone)
                {
                    m_dicLoadedQueue.Add(item.Key, item.Value);
                }
            }
            foreach (var item in m_dicLoadedQueue)
            {
                item.Value.isDone = true;
                if (string.IsNullOrEmpty(item.Key.error))
                {
                    item.Value.SetWWWObject(item.Key);
                }
                else
                {
                    item.Value.errorTxt = item.Key.error;
                    
                    CLog.LogError("Load resource [" + item.Key.url + "] fail!");
                }
                m_dicLoadingQueue.Remove(item.Key);
                item.Key.Dispose();
                OnDone(item.Value);
            }
            m_dicLoadedQueue.Clear();
        }

        protected override string GetInResPath(Resource res)
        {
            return _resUtil.FullPathForFile(res.realPath, res.resType,true);
        }
    }
}