using UnityEngine;
using System.Collections.Generic;

namespace Framework
{
    public class AssetBundleResourcesLoader : BaseResLoader
    {
        private Dictionary<AssetBundleCreateRequest, Resource> m_dicLoadingQueue = new Dictionary<AssetBundleCreateRequest, Resource>();
        private Dictionary<AssetBundleCreateRequest, Resource> m_dicLoadedQueue = new Dictionary<AssetBundleCreateRequest, Resource>();

        public override void Load(Resource res)
        {
            string url = GetInResPath(res);
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(url);
            m_dicLoadingQueue.Add(request, res);
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
                if (item.Key.assetBundle != null)
                {
                    item.Value.SetBundle(item.Key.assetBundle);
                }
                else
                {
                    string errorTxt = "Load resource [" + GetInResPath(item.Value) + "] fail!";
                    item.Value.errorTxt = errorTxt;
                    CLog.LogError(errorTxt);
                }
                m_dicLoadingQueue.Remove(item.Key);
                OnDone(item.Value);
            }
            m_dicLoadedQueue.Clear();
        }

        protected override string GetInResPath(Resource res)
        {
            return _resUtil.FullPathForFile(res.realPath, res.resType,false);
        }
    }
}