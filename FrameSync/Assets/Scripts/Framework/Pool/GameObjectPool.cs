using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class GameObjectPool : SingletonMonoBehaviour<GameObjectPool>
    {
        public delegate void GameObjectPoolHandler(GameObject go);
        private Dictionary<Resource, Queue<GameObject>> m_dicGO;
        private Dictionary<string, List<GameObjectPoolHandler>> m_dicCallback;

        protected override void Init()
        {
            m_dicGO = new Dictionary<Resource, Queue<GameObject>>();
            m_dicCallback = new Dictionary<string, List<GameObjectPoolHandler>>();
            base.Init();
        }
        public void GetObject(string path,GameObjectPoolHandler callback)
        {
            if(callback == null)
            {
                CLog.LogError("GameObjectPool callback can not be null!");
                return;
            }
            foreach (var item in m_dicGO)
            {
                if(item.Key.path == path)
                {
                    GameObject go = null;
                    if (item.Value.Count > 0)
                    {
                        go = item.Value.Dequeue();
                    }
                    else
                    {
                        go = GetGameObject(item.Key);
                    }
                    go.SetActive(true);
                    callback.Invoke(go);
                    return;
                }
            }
            List<GameObjectPoolHandler> lst = null;
            if(!m_dicCallback.TryGetValue(path, out lst))
            {
                lst = new List<GameObjectPoolHandler>();
                m_dicCallback.Add(path, lst);
            }
            if (!lst.Contains(callback))
            {
                lst.Add(callback);
            }
            ResourceSys.Instance.GetResource(path, OnResLoad);
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
            foreach (var item in m_dicGO)
            {
                if (item.Key.path == path)
                {
                    this.gameObject.AddChildToParent(go);
                    go.SetActive(false);
                    item.Value.Enqueue(go);
                    return;
                }
            }
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
                while (item.Value.Count > 0)
                {
                    GameObject.Destroy(item.Value.Dequeue());
                }
                item.Key.Retain();
            }
            m_dicGO.Clear();
        }

        private GameObject GetGameObject(Resource res)
        {
            UnityEngine.Object prefab = res.GetAsset(res.path);
            GameObject go = (GameObject)GameObject.Instantiate(prefab);
            return go;
        }

        private void OnResLoad(Resource res)
        {
            if (res.isSucc)
            {
                Queue<GameObject> queueGO = null;
                if(!m_dicGO.TryGetValue(res, out queueGO))
                {
                    queueGO = new Queue<GameObject>();
                    m_dicGO.Add(res, queueGO);
                    res.Retain();
                }
                List<GameObjectPoolHandler> lstCallback = null;
                if(m_dicCallback.TryGetValue(res.path, out lstCallback))
                {
                    while(lstCallback.Count > 0)
                    {
                        GameObject go = null;
                        if (queueGO.Count > 0)
                        {
                            go = queueGO.Dequeue();
                        }
                        else
                        {
                            go = GetGameObject(res);
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
                CLog.LogError("加载unit资源" + res.path + "失败");
            }
        }
    }
}
