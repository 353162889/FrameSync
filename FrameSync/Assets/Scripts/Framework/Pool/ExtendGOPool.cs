using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class ExtendGOPool<T> : SingletonMonoBehaviour<ExtendGOPool<T>>
    {
        private List<string> m_lstPath = new List<string>();

        public void CacheObject(Resource res, int count)
        {
            if (!m_lstPath.Contains(res.path)) m_lstPath.Add(res.path);
            GameObjectPool.Instance.CacheObject(res, count);
        }

        public virtual GameObject GetObject(string path, GameObjectPoolHandler callback)
        {
            if (!m_lstPath.Contains(path)) m_lstPath.Add(path);
            return GameObjectPool.Instance.GetObject(path, callback);
        }

        public virtual void RemoveCallback(string path, GameObjectPoolHandler callback)
        {
            GameObjectPool.Instance.RemoveCallback(path, callback);
        }

        public virtual void SaveObject(string path, GameObject go)
        {
            GameObjectPool.Instance.SaveObject(path, go);
        }

        public virtual void Clear(string path)
        {
            if (m_lstPath.Remove(path))
            {
                GameObjectPool.Instance.Clear(path);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < m_lstPath.Count; i++)
            {
                GameObjectPool.Instance.Clear(m_lstPath[i]);
            }
            m_lstPath.Clear();
        }
    }
}
