using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class ExtendGOPool<T> : MonoBehaviour where T : ExtendGOPool<T>
    {

        private static T uniqueInstance;

        public static T Instance
        {
            get
            {
                return uniqueInstance;
            }
        }

        protected virtual void Awake()
        {
            if (uniqueInstance == null)
            {
                uniqueInstance = (T)this;
                GameObject.DontDestroyOnLoad(this);
                uniqueInstance.Init();
            }
            else if (uniqueInstance != this)
            {
                throw new InvalidOperationException("Cannot have two instances of a SingletonMonoBehaviour : " + typeof(T).ToString() + ".");
            }
        }

        protected virtual void Init()
        {
        }

        protected virtual void OnDestroy()
        {
            if (uniqueInstance == this)
            {
                uniqueInstance = null;
            }
        }

        private List<string> m_lstPath = new List<string>();

        protected void _CacheObject(string path,bool isPrefab, int count, Action<string> callback)
        {
            if (!m_lstPath.Contains(path)) m_lstPath.Add(path);
            ResourceObjectPool.Instance.CacheObject(path,isPrefab, count,callback);
        }

        protected void _RemoveCacheObject(string path, Action<string> callback)
        {
            ResourceObjectPool.Instance.RemoveCacheObject(path, callback);
        }

        protected UnityEngine.Object _GetObject(string path,bool isPrefab, ResourceObjectPoolHandler callback)
        {
            if (!m_lstPath.Contains(path)) m_lstPath.Add(path);
            return ResourceObjectPool.Instance.GetObject(path, isPrefab, callback);
        }

        protected void _RemoveCallback(string path, ResourceObjectPoolHandler callback)
        {
            ResourceObjectPool.Instance.RemoveCallback(path, callback);
        }

        protected void _SaveObject(string path, GameObject go)
        {
            ResourceObjectPool.Instance.SaveObject(path, go);
        }

        protected void _Clear(string path)
        {
            if (m_lstPath.Remove(path))
            {
                ResourceObjectPool.Instance.Clear(path);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < m_lstPath.Count; i++)
            {
                ResourceObjectPool.Instance.Clear(m_lstPath[i]);
            }
            m_lstPath.Clear();
        }
    }
}
