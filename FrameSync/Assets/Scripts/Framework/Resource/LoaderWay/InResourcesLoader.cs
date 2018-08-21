using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class InResourcesLoader : BaseResLoader
    {

        private Dictionary<ResourceRequest, Resource> m_dicLoadingQueue = new Dictionary<ResourceRequest, Resource>();
        private Dictionary<ResourceRequest, Resource> m_dicLoadedQueue = new Dictionary<ResourceRequest, Resource>();
        public override void Load(Resource res)
        {
            string loadPath = GetInResPath(res);
            ResourceRequest request = Resources.LoadAsync(loadPath);
            m_dicLoadingQueue.Add(request, res);
        }

        private void Update()
        {
            if (m_dicLoadingQueue.Count == 0) return;
            foreach (var item in m_dicLoadingQueue)
            {
                if(item.Key.isDone)
                {
                    m_dicLoadedQueue.Add(item.Key, item.Value);
                }
            }
            foreach (var item in m_dicLoadedQueue)
            {
                item.Value.isDone = true;
                if(item.Key.asset == null)
                {
                    item.Value.errorTxt = "Load resource [" + item.Value.realPath + "] fail!";
                    CLog.LogError(item.Value.errorTxt);
                }
                else
                {
                    item.Value.SetDirectObject(item.Key.asset);
                }
                m_dicLoadingQueue.Remove(item.Key);
                OnDone(item.Value);
            }
            m_dicLoadedQueue.Clear();
        }

        protected override string GetInResPath(Resource res)
        {
            return _resUtil.FullPathForFile(res.realPath, res.resType);
        }

        protected override void OnDestroy()
        {
            m_dicLoadingQueue.Clear();
            m_dicLoadedQueue.Clear();
            base.OnDestroy();
        }
    }

}