using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{ 
    public class ResourceContainer : MonoBehaviour
    {
        public delegate void ResourceHandler(Resource res);

        private Dictionary<Resource, List<ResourceHandler>> _succCallbacks;
        private Dictionary<Resource, List<ResourceHandler>> _failCallbacks;
        private Dictionary<string, Resource> _mapRes;

        private ResourceLoader _resLoader;
        private AssetBundleFile _assetBundleFile;

        public bool ResourcesLoadMode { get; private set; }
        private string _resRootDir;
        public string ResRootDir { get { return _resRootDir; } }

        public void Init(bool resourceLoadMode,string resRootDir)
        {
            _succCallbacks = new Dictionary<Resource, List<ResourceHandler>>();
            _failCallbacks = new Dictionary<Resource, List<ResourceHandler>>();
            _mapRes = new Dictionary<string, Resource>();
            _resLoader = ResourceLoader.GetResLoader(this.gameObject);
            _resLoader.OnDone += OnResourceDone;
            ResourcesLoadMode = resourceLoadMode;
            ResourceFileUtil resUtil = gameObject.AddComponentOnce<ResourceFileUtil>();
            _resRootDir = resRootDir;
            resUtil.Init(resRootDir);
            _assetBundleFile = gameObject.AddComponentOnce<AssetBundleFile>();
        }

        public void ReleaseUnUseRes()
        {
            List<string> keyList = new List<string>();
            foreach (var item in _mapRes)
            {
                if (item.Value.refCount <= 0)
                {
                    keyList.Add(item.Key);
                }
            }
            for (int i = 0; i < keyList.Count; i++)
            {
                Resource res = _mapRes[keyList[i]];
                RemoveAllListener(res);
                _mapRes.Remove(keyList[i]);
                res.DestroyResource();
            }
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        /// <summary>
        /// 获取到某个资源
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="onSucc">成功时的回调</param>
        /// <param name="onFail">失败时的回调</param>
        /// <param name="resType">资源类型，默认是UnKnow，如果是在bundle模式下，并且传入Unknow类型，会改成AssetBundle类型</param>
        /// <returns>返回当前加载的Resource</returns>
        public Resource GetResource(string path, ResourceHandler onSucc = null, ResourceHandler onFail = null,
            ResourceType resType = ResourceType.UnKnow)
        {
            if (string.IsNullOrEmpty(path))
            {
                CLog.LogError("[GetResource]ResName can not is null!");
                return null;
            }
       
            Resource res;
            _mapRes.TryGetValue(GetCacheResourceKey(path), out res);
            if (res != null)
            {
                if (res.isDone)
                {
                    if (res.isSucc && onSucc != null)
                    {
                        ResourceHandler tempOnSucc = onSucc;
                        onSucc = null;
                        tempOnSucc.Invoke(res);
                        tempOnSucc = null;
                    }
                }
                else
                {
                    AddListener(res, onSucc, onFail);
                }
                return res;
            }
            res = new Resource();
            res.path = path;
            res.resType = (!ResourcesLoadMode && resType == ResourceType.UnKnow) ? ResourceType.AssetBundle : resType;

            //获取到当前资源的依赖资源(可能没法保证顺序，所以拿的时候需要保证所有依赖资源都已经加载好)
            if (!ResourcesLoadMode && res.resType == ResourceType.AssetBundle)
            {
                string[] listDependResPath = GetDependResPath(res);
                if (listDependResPath != null && listDependResPath.Length > 0)
                {
                    List<Resource> listDependRes = new List<Resource>();
                    for (int i = 0; i < listDependResPath.Length; i++)
                    {
                        //加载依赖资源
                        Resource dependRes = GetResource(listDependResPath[i]);
                        listDependRes.Add(dependRes);
                    }
                    res.SetDependsRes(listDependRes);
                }
            }
            //真正加载当前资源
            _mapRes.Add(GetCacheResourceKey(res.path), res);
            res.Retain();
            AddListener(res, onSucc, onFail);
            _resLoader.Load(res);
            return res;
        }

        private void OnResourceDone(Resource res)
        {
            if (res.isSucc)
            {
                List<ResourceHandler> succList;
                _succCallbacks.TryGetValue(res, out succList);
                _succCallbacks.Remove(res);
                if (succList != null)
                {
                    while(succList.Count > 0)
                    {
                        var callback = succList[0];
                        succList.RemoveAt(0);
                        callback.Invoke(res);
                    }
                }
                res.Release();
            }
            else
            {
                //失败的话，将资源先移除掉
                if (res.refCount > 1)
                {
                    CLog.LogError("DestroyResource[resPath=" + res.path + "],RefCount>1.");
                }
                else
                { 
                    _mapRes.Remove(GetCacheResourceKey(res.path));
                }
                List<ResourceHandler> failList;
                _failCallbacks.TryGetValue(res, out failList);
                _failCallbacks.Remove(res);
                if (failList != null)
                {
                    while (failList.Count > 0)
                    {
                        var callback = failList[0];
                        failList.RemoveAt(0);
                        callback.Invoke(res);
                    }
                }
                res.Release();
                if (res.refCount <= 0)
                { 
                    res.DestroyResource();
                }
            }
        }

        private string[] GetDependResPath(Resource res)
        {
            string resName = _assetBundleFile.GetAssetBundleNameByAssetPath(res.path);
            return _assetBundleFile.GetDirectDependencies(resName);
        }

        public string GetCacheResourceKey(string resPath)
        {
            if (string.IsNullOrEmpty(resPath))
            {
                return resPath;
            }
            if (ResourcesLoadMode)
            {
                return resPath;
            }
            return _assetBundleFile.GetAssetBundleNameByAssetPath(resPath);
        }

        private void AddListener(Resource res, ResourceHandler onSucc = null, ResourceHandler onFail = null)
        {

            if (onSucc != null)
            {
                List<ResourceHandler> succList;
                _succCallbacks.TryGetValue(res, out succList);
                if (succList == null)
                {
                    succList = new List<ResourceHandler>();
                    _succCallbacks.Add(res, succList);
                }
                if (!succList.Contains(onSucc))
                {
                    succList.Add(onSucc);
                }
            }
            if (onFail != null)
            {
                List<ResourceHandler> failList;
                _failCallbacks.TryGetValue(res, out failList);
                if (failList == null)
                {
                    failList = new List<ResourceHandler>();
                    _failCallbacks.Add(res, failList);
                }
                if (!failList.Contains(onFail))
                {
                    failList.Add(onFail);
                }
            }
        }

        public void RemoveListener(Resource res, ResourceHandler onSucc = null, ResourceHandler onFail = null)
        {
            if (onSucc != null)
            {
                List<ResourceHandler> succList;
                _succCallbacks.TryGetValue(res, out succList);
                if (succList != null)
                {
                    succList.Remove(onSucc);
                    if(succList.Count == 0)
                    {
                        _succCallbacks.Remove(res);
                    }
                }
            }
            if (onFail != null)
            {
                List<ResourceHandler> failList;
                _failCallbacks.TryGetValue(res, out failList);
                if (failList != null)
                {
                    failList.Remove(onFail);
                    if(failList.Count == 0)
                    {
                        _failCallbacks.Remove(res);
                    }
                }
            }
        }

        public void RemoveListener(string path, ResourceHandler onSucc = null, ResourceHandler onFail = null)
        {
            Resource res;
            _mapRes.TryGetValue(GetCacheResourceKey(path), out res);
            if (res != null)
            {
                RemoveListener(res, onSucc, onFail);
            }
        }

        public void RemoveAllListener(Resource res)
        {
            _succCallbacks.Remove(res);
            _failCallbacks.Remove(res);
        }

        public void RemoveAllListener(string path)
        {
            Resource res;
            _mapRes.TryGetValue(GetCacheResourceKey(path), out res);
            if (res != null)
            {
                RemoveAllListener(res);
            }
        }

        public void RemoveWaitLoadingRes(Resource res)
        {
            if (_resLoader.RemoveWaitingLoadingRes(res))
            {
                RemoveAllListener(res);
                if (_mapRes.Remove(res.path))
                {
                    res.Release();
                }
            }
        }

        protected void OnDestroy()
        {
            _resLoader.OnDone -= OnResourceDone;
            _succCallbacks.Clear();
            _failCallbacks.Clear();
            foreach (var item in _mapRes)
            {
                item.Value.DestroyResource();
            }
            _mapRes.Clear();
        }

    }
}