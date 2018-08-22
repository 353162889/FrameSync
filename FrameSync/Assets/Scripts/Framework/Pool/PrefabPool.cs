using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class PrefabPool : SingletonMonoBehaviour<PrefabPool>
    {
        public void CacheObject(string path, Action<string> callback)
        {
            ResourceObjectPool.Instance.CacheObject(path, true, 1, callback);
        }

        public void RemoveCacheObject(string path, Action<string> callback)
        {
            ResourceObjectPool.Instance.RemoveCacheObject(path, callback);
        }

        public UnityEngine.Object GetObject(string path, ResourceObjectPoolHandler callback)
        {
            return ResourceObjectPool.Instance.GetObject(path, true, callback);
        }

        public void RemoveCallback(string path, ResourceObjectPoolHandler callback)
        {
            ResourceObjectPool.Instance.RemoveCallback(path, callback);
        }

        public void Clear(string path)
        {
            ResourceObjectPool.Instance.Clear(path);
        }
    }
}
