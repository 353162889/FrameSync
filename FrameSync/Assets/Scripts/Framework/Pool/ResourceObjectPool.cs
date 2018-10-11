using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public delegate void ResourceObjectPoolHandler(string path,UnityEngine.Object go);
    /// <summary>
    /// 只允许给框架内部使用
    /// 游戏中全局对象池（上层逻辑池会调用当前池），不允许直接使用，直接使用会造成一些对象清除不了
    /// </summary>
    public class ResourceObjectPool : SingletonMonoBehaviour<ResourceObjectPool>
    {
        public class ResourceObjectQueue : IPoolable
        {
            public UnityEngine.Object prefab;
            public Resource res;
            public Queue<UnityEngine.Object> queue;
            public bool isDone;

            public void Reset()
            {
                prefab = null;
                res = null;
                isDone = false;
                if (queue != null)
                {
                    queue.Clear();
                }
            }
        }

        public struct CacheCallbackStruct
        {
            public Action<string> callback;
            public int count;
            public bool isPrefab;
        }

        public struct GameObjectPoolCallbackStruct
        {
            public ResourceObjectPoolHandler callback;
            public bool isPrefab;
        }

        private Dictionary<string, ResourceObjectQueue> m_dicGO;
        private Dictionary<string, List<GameObjectPoolCallbackStruct>> m_dicCallback;
        private Dictionary<string, List<CacheCallbackStruct>> m_dicCacheCallback;
        private LinkedList<IEnumerator> m_lstAsyncQueue;

        protected override void Init()
        {
            ObjectPool<ResourceObjectQueue>.Instance.Init(100,true);
            m_dicGO = new Dictionary<string, ResourceObjectQueue>();
            m_dicCallback = new Dictionary<string, List<GameObjectPoolCallbackStruct>>();
            m_dicCacheCallback = new Dictionary<string, List<CacheCallbackStruct>>();
            m_lstAsyncQueue = new LinkedList<IEnumerator>();
            base.Init();
        }

        public void CacheObject(string path,bool isPrefab, int count,Action<string> callback)
        {
            ResourceObjectQueue resQueue;
            if (m_dicGO.TryGetValue(path, out resQueue))
            {
                if (resQueue.isDone)
                {
                    if (!isPrefab)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var go = GetGameObject(resQueue.prefab, path);
                            SaveObject(path, go);
                        }
                    }
                    if (callback != null)
                    {
                        callback.Invoke(path);
                    }
                    return;
                }
            }
            if (callback != null)
            {
                List<CacheCallbackStruct> lstCallbackStruct;
                if (!m_dicCacheCallback.TryGetValue(path, out lstCallbackStruct))
                {
                    lstCallbackStruct = new List<CacheCallbackStruct>();
                    m_dicCacheCallback.Add(path, lstCallbackStruct);
                }
                bool contain = false;
                for (int i = 0; i < lstCallbackStruct.Count; i++)
                {
                    if (lstCallbackStruct[i].callback == callback)
                    {
                        contain = true;
                        break;
                    }
                }
                if (!contain)
                {
                    CacheCallbackStruct callbackStruct = new CacheCallbackStruct();
                    callbackStruct.callback = callback;
                    callbackStruct.count = count;
                    callbackStruct.isPrefab = isPrefab;
                    lstCallbackStruct.Add(callbackStruct);
                }
            }
            if (resQueue == null)
            {
                resQueue = ObjectPool<ResourceObjectQueue>.Instance.GetObject();
                resQueue.isDone = false;
                resQueue.queue = new Queue<UnityEngine.Object>();
                resQueue.prefab = null;
                m_dicGO.Add(path, resQueue);
                Resource res = ResourceSys.Instance.GetResource(path, OnResLoadCache);
                resQueue.res = res;
                res.Retain();
            }
        }

        public void RemoveCacheObject(string path,Action<string> callback)
        {
            List<CacheCallbackStruct> lst = null;
            if (m_dicCacheCallback.TryGetValue(path, out lst))
            {
                for (int i = lst.Count - 1; i > -1; i--)
                {
                    if(lst[i].callback == callback)
                    {
                        lst.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public UnityEngine.Object GetObject(string path, bool isPrefab,ResourceObjectPoolHandler callback)
        {
            ResourceObjectQueue resQueue;
            if(m_dicGO.TryGetValue(path,out resQueue))
            {
                if (resQueue.isDone)
                {
                    UnityEngine.Object go = null;
                    if (isPrefab)
                    {
                        go = resQueue.prefab;
                    }
                    else
                    {
                        if (resQueue.queue.Count > 0)
                        {
                            go = resQueue.queue.Dequeue();
                        }
                        else
                        {
                            go = GetGameObject(resQueue.prefab, path);
                        }
                        ((GameObject)go).SetActive(true);
                    }

                    if (null != callback)
                    {
                        var tempCallback = callback;
                        callback = null;
                        tempCallback.Invoke(path, go);
                    }
                    return go;
                }
            }
            if (callback != null)
            {
                List<GameObjectPoolCallbackStruct> lst = null;
                if (!m_dicCallback.TryGetValue(path, out lst))
                {
                    lst = new List<GameObjectPoolCallbackStruct>();
                    m_dicCallback.Add(path, lst);
                }
                bool contain = false;
                for (int i = 0; i < lst.Count; i++)
                {
                    if (lst[i].callback == callback)
                    {
                        contain = true;
                        break;
                    }
                }
                if (!contain)
                {
                    GameObjectPoolCallbackStruct callbackStruct = new GameObjectPoolCallbackStruct();
                    callbackStruct.callback = callback;
                    callbackStruct.isPrefab = isPrefab;
                    lst.Add(callbackStruct);
                }
            }
            if (resQueue == null)
            {
                resQueue = ObjectPool<ResourceObjectQueue>.Instance.GetObject();
                resQueue.isDone = false;
                resQueue.queue = new Queue<UnityEngine.Object>();
                resQueue.prefab = null;
                m_dicGO.Add(path, resQueue);
                Resource res = ResourceSys.Instance.GetResource(path, OnResLoad);
                resQueue.res = res;
                res.Retain();
            }
            return null;
        }

        public void RemoveCallback(string path,ResourceObjectPoolHandler callback)
        {
            List<GameObjectPoolCallbackStruct> lst = null;
            if(m_dicCallback.TryGetValue(path, out lst))
            {
                for (int i = lst.Count - 1; i > -1; i--)
                {
                    if (lst[i].callback == callback)
                    {
                        lst.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void SaveObject(string path, UnityEngine.Object go)
        {
            ResourceObjectQueue resQueue;
            if(m_dicGO.TryGetValue(path,out resQueue))
            {
                if (go != resQueue.prefab)
                {
                    this.gameObject.AddChildToParent((GameObject)go);
                    ((GameObject)go).SetActive(false);
                    resQueue.queue.Enqueue(go);
                }
            }
            else
            {
                GameObject.Destroy(go);
            }
        }

        public void Clear(string path)
        {
            ResourceSys.Instance.RemoveListener(path, OnResLoad);
            m_dicCallback.Remove(path);

            ResourceObjectQueue resQueue;
            if (m_dicGO.TryGetValue(path, out resQueue))
            {
                while (resQueue.queue.Count > 0)
                {
                    GameObject.Destroy(resQueue.queue.Dequeue());
                }
                resQueue.res.Release();
                resQueue.res = null;
                resQueue.prefab = null;
                ObjectPool<ResourceObjectQueue>.Instance.SaveObject(resQueue);
            }
            m_dicGO.Remove(path);
        }

        public void Clear()
        {
            foreach (var item in m_dicCallback)
            {
                ResourceSys.Instance.RemoveListener(item.Key, OnResLoad);
            }
            m_dicCallback.Clear();
            foreach (var item in m_dicGO)
            {
                var resQueue = item.Value;
                while (resQueue.queue.Count > 0)
                {
                    GameObject.Destroy(resQueue.queue.Dequeue());
                }
                resQueue.res.Release();
                resQueue.res = null;
                resQueue.prefab = null;
                ObjectPool<ResourceObjectQueue>.Instance.SaveObject(resQueue);
            }
            m_dicGO.Clear();
        }

        private GameObject GetGameObject(UnityEngine.Object prefab,string path)
        {
            try
            {
                GameObject go = (GameObject)GameObject.Instantiate(prefab);
                return go;
            }
            catch(Exception e)
            {
                CLog.LogError("路径为"+ path +"传入预设为空");
            }
            return null;
        }

        private void OnResLoad(Resource res,string path)
        {
            if (res.isSucc)
            {
                m_lstAsyncQueue.AddLast(LoadAssetAndCallback(res, path));
            }
            else
            {
                CLog.LogError("加载GameObject资源" + path + "失败");
            }
        }

        private void Update()
        {
            if (m_lstAsyncQueue.Count == 0) return;
            var node = m_lstAsyncQueue.First;
            while (node != null)
            {
                if(!node.Value.MoveNext() && node == m_lstAsyncQueue.First)
                {
                    m_lstAsyncQueue.RemoveFirst();
                    node = m_lstAsyncQueue.First;
                }
                else
                {
                    node = node.Next;
                }
            }
        }

        private IEnumerator LoadAssetAndCallback(Resource res, string path)
        {
            ResourceObjectQueue resQueue = m_dicGO[path];
            if (resQueue.prefab == null)
            {
                IEnumerator enumerator = resQueue.res.GetAssetAsync(path, OnLoadAsset);
                while (enumerator != null && enumerator.MoveNext())
                {
                    yield return null;
                }
                //yield return resQueue.res.GetAssetAsync(path, OnLoadAsset);
            }
            resQueue.isDone = true;
            List<GameObjectPoolCallbackStruct> lstCallback = null;
            if (m_dicCallback.TryGetValue(path, out lstCallback))
            {
                while (lstCallback.Count > 0)
                {
                    var callback = lstCallback[0];
                    lstCallback.RemoveAt(0);
                    UnityEngine.Object go = null;
                    if(callback.isPrefab)
                    {
                        go = resQueue.prefab;
                    }
                    else
                    {
                        if (resQueue.queue.Count > 0)
                        {
                            go = resQueue.queue.Dequeue();
                        }
                        else
                        {
                            go = GetGameObject(resQueue.prefab, path);
                        }
                        ((GameObject)go).SetActive(true);
                    }
                    callback.callback.Invoke(path,go);
                }
            }
        }

        private void OnLoadAsset(string path,UnityEngine.Object prefab)
        {
            if(prefab == null)
            {
                CLog.LogError("加载资源path="+path+"的预制为空");
            }
            ResourceObjectQueue resQueue;
            if (m_dicGO.TryGetValue(path, out resQueue))
            {
                resQueue.prefab = prefab;
            }
        }

        private void OnResLoadCache(Resource res, string path)
        {
            if (res.isSucc)
            {
                ResourceObjectQueue resQueue = m_dicGO[path];
                resQueue.prefab = res.GetAsset(path);
                resQueue.isDone = true;
                List<CacheCallbackStruct> lstCallbackStruct = null;
                if (m_dicCacheCallback.TryGetValue(path, out lstCallbackStruct))
                {
                    while (lstCallbackStruct.Count > 0)
                    {
                        var callbackStruct = lstCallbackStruct[0];
                        lstCallbackStruct.RemoveAt(0);
                        if(!callbackStruct.isPrefab)
                        {
                            for (int i = 0; i < callbackStruct.count; i++)
                            {
                                var go = GetGameObject(resQueue.prefab, path);
                                SaveObject(path, go);
                            }
                        }
                        callbackStruct.callback.Invoke(path);
                    }
                }
            }
            else
            {
                CLog.LogError("加载GameObject资源" + path + "失败");
            }
        }
    }
}
