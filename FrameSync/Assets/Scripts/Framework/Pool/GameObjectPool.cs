using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public delegate void GameObjectPoolHandler(GameObject go);
    /// <summary>
    /// 游戏中全局对象池（上层逻辑池会调用当前池），不允许直接使用
    /// </summary>
    public class GameObjectPool : SingletonMonoBehaviour<GameObjectPool>
    {
        public struct ResourceObjectQueue
        {
            public Resource res;
            public Queue<GameObject> queue;
        }

        public struct CacheCallbackStruct
        {
            public Action<string> callback;
            public int count;
        }

        private Dictionary<string, ResourceObjectQueue> m_dicGO;
        private Dictionary<string, List<GameObjectPoolHandler>> m_dicCallback;
        private Dictionary<string, List<CacheCallbackStruct>> m_dicCacheCallback;

        protected override void Init()
        {
            m_dicGO = new Dictionary<string, ResourceObjectQueue>();
            m_dicCallback = new Dictionary<string, List<GameObjectPoolHandler>>();
            m_dicCacheCallback = new Dictionary<string, List<CacheCallbackStruct>>();
            base.Init();
        }

        public void CacheObject(string path,int count,Action<string> callback)
        {
            ResourceObjectQueue resQueue;
            if (m_dicGO.TryGetValue(path, out resQueue))
            {
                for (int i = 0; i < count; i++)
                {
                    var go = GetGameObject(resQueue.res, path);
                    SaveObject(path, go);
                }
                if (callback != null)
                {
                    callback.Invoke(path);
                }
            }
            else
            {
                if(callback != null)
                {
                    List<CacheCallbackStruct> lstCallbackStruct;
                    if (!m_dicCacheCallback.TryGetValue(path, out lstCallbackStruct))
                    {
                        lstCallbackStruct = new List<CacheCallbackStruct>();
                        m_dicCacheCallback.Add(path,lstCallbackStruct);
                    }
                    bool contain = false;
                    for (int i = 0; i < lstCallbackStruct.Count; i++)
                    {
                        if(lstCallbackStruct[i].callback == callback)
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
                        lstCallbackStruct.Add(callbackStruct);
                    }
                }
                ResourceSys.Instance.GetResource(path, OnResLoadCache);
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

        public GameObject GetObject(string path,GameObjectPoolHandler callback)
        {
            foreach (var item in m_dicGO)
            {
                if(item.Key == path)
                {
                    GameObject go = null;
                    if (item.Value.queue.Count > 0)
                    {
                        go = item.Value.queue.Dequeue();
                    }
                    else
                    {
                        go = GetGameObject(item.Value.res,item.Key);
                    }
                    go.SetActive(true);
                    if (null != callback)
                    {
                        var tempCallback = callback;
                        callback = null;
                        tempCallback.Invoke(go);
                    }
                    return go;
                }
            }
            if (callback != null)
            {
                List<GameObjectPoolHandler> lst = null;
                if (!m_dicCallback.TryGetValue(path, out lst))
                {
                    lst = new List<GameObjectPoolHandler>();
                    m_dicCallback.Add(path, lst);
                }
                if (!lst.Contains(callback))
                {
                    lst.Add(callback);
                }
            }
            ResourceSys.Instance.GetResource(path, OnResLoad);
            return null;
        }

        public void RemoveCallback(string path,GameObjectPoolHandler callback)
        {
            List<GameObjectPoolHandler> lst = null;
            if(m_dicCallback.TryGetValue(path, out lst))
            {
                lst.Remove(callback);
            }
        }

        public void SaveObject(string path, GameObject go)
        {
            ResourceObjectQueue resQueue;
            if(m_dicGO.TryGetValue(path,out resQueue))
            {
                this.gameObject.AddChildToParent(go);
                go.SetActive(false);
                resQueue.queue.Enqueue(go);
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
                while (item.Value.queue.Count > 0)
                {
                    GameObject.Destroy(item.Value.queue.Dequeue());
                }
                item.Value.res.Release();
            }
            m_dicGO.Clear();
        }

        private GameObject GetGameObject(Resource res,string path)
        {
            UnityEngine.Object prefab = res.GetAsset(path);
            GameObject go = (GameObject)GameObject.Instantiate(prefab);
            return go;
        }

        private void OnResLoad(Resource res,string path)
        {
            if (res.isSucc)
            {
                ResourceObjectQueue resQueue;
                if (!m_dicGO.TryGetValue(path, out resQueue))
                {
                    resQueue = new ResourceObjectQueue();
                    resQueue.res = res;
                    res.Retain();
                    resQueue.queue = new Queue<GameObject>();
                    m_dicGO.Add(path, resQueue);
                }
                
                List<GameObjectPoolHandler> lstCallback = null;
                if(m_dicCallback.TryGetValue(res.realPath, out lstCallback))
                {
                    while(lstCallback.Count > 0)
                    {
                        GameObject go = null;
                        if (resQueue.queue.Count > 0)
                        {
                            go = resQueue.queue.Dequeue();
                        }
                        else
                        {
                            go = GetGameObject(res,path);
                        }
                        var callback = lstCallback[0];
                        lstCallback.RemoveAt(0);
                        go.SetActive(true);
                        callback.Invoke(go);
                    }
                }
            }
            else
            {
                CLog.LogError("加载GameObject资源" + path + "失败");
            }
        }

        private void OnResLoadCache(Resource res, string path)
        {
            if (res.isSucc)
            {
                ResourceObjectQueue resQueue;
                if (!m_dicGO.TryGetValue(path, out resQueue))
                {
                    resQueue = new ResourceObjectQueue();
                    resQueue.res = res;
                    res.Retain();
                    resQueue.queue = new Queue<GameObject>();
                    m_dicGO.Add(path, resQueue);
                }

                List<CacheCallbackStruct> lstCallbackStruct = null;
                if (m_dicCacheCallback.TryGetValue(res.realPath, out lstCallbackStruct))
                {
                    while (lstCallbackStruct.Count > 0)
                    {
                        var callbackStruct = lstCallbackStruct[0];
                        lstCallbackStruct.RemoveAt(0);
                        for (int i = 0; i < callbackStruct.count; i++)
                        {
                            var go = GetGameObject(resQueue.res, path);
                            SaveObject(path, go);
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
