using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{ 
    public class ResourceContainer : MonoBehaviour
    {
        public struct ResourceHandlerStruct
        {
            public string path;
            public ResourceHandler callback;
        }

        public delegate void ResourceHandler(Resource res,string path);

        private Dictionary<Resource, List<ResourceHandlerStruct>> _succCallbacks;
        private Dictionary<Resource, List<ResourceHandlerStruct>> _failCallbacks;
        private Dictionary<string, Resource> _mapRes;

        private ResourceLoader _resLoader;
        public AssetBundleFile assetBundleFile { get { return _assetBundleFile; } }
        private AssetBundleFile _assetBundleFile;

        //资源直接加载模式
        public bool DirectLoadMode { get; private set; }
        private string _resRootDir;
        public string ResRootDir { get { return _resRootDir; } }
        private bool _dirInResources;
        public bool DirInResources { get { return _dirInResources; } }

        public void Init(bool directLoadMode,string resRootDir)
        {
            _resRootDir = resRootDir;
            Resource.BundlePreRootDir = _resRootDir.EndsWith("/") ? _resRootDir.ToLower() : (_resRootDir + "/").ToLower();
            //根据传入的目录判断是直接用Resources.Load还是外部加载
            _dirInResources = _resRootDir == "Assets/Resources";
            _succCallbacks = new Dictionary<Resource, List<ResourceHandlerStruct>>();
            _failCallbacks = new Dictionary<Resource, List<ResourceHandlerStruct>>();
            _mapRes = new Dictionary<string, Resource>();
            _resLoader = ResourceLoader.GetResLoader(this.gameObject);
            _resLoader.OnDone += OnResourceDone;
            DirectLoadMode = directLoadMode;
            ResourceFileUtil resUtil = gameObject.AddComponentOnce<ResourceFileUtil>();
            resUtil.Init(resRootDir);
            _assetBundleFile = gameObject.AddComponentOnce<AssetBundleFile>();
        }

        private HashSet<string> m_setSign = new HashSet<string>();
        private List<string> m_lstRemoveKeyList = new List<string>();
        public void ReleaseUnUseRes()
        {
            //释放资源使用垃圾回收的机制（标记清除法）
            foreach (var item in _mapRes)
            {
                if(!m_setSign.Contains(item.Key) && item.Value.refCount > 0)
                {
                    m_setSign.Add(item.Key);
                    //收集依赖资源的标记
                    var lstDepend = item.Value.dependRes;
                    if(lstDepend != null)
                    {
                        for (int i = 0; i < lstDepend.Count; i++)
                        {
                            if(!m_setSign.Contains(lstDepend[i].realPath))
                            {
                                m_setSign.Add(lstDepend[i].realPath);
                            }
                        }
                    }
                }
                
            }
            foreach (var item in _mapRes)
            {
                if (!m_setSign.Contains(item.Key))
                {
                    m_lstRemoveKeyList.Add(item.Key);
                }
            }

            for (int i = 0; i < m_lstRemoveKeyList.Count; i++)
            {
                Resource res = _mapRes[m_lstRemoveKeyList[i]];
                RemoveAllListener(res);
                _mapRes.Remove(m_lstRemoveKeyList[i]);
                res.DestroyResource();
            }
            m_setSign.Clear();
            m_lstRemoveKeyList.Clear();
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        /// <summary>
        /// 获取到某个资源
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="onSucc">成功时的回调</param>
        /// <param name="onFail">失败时的回调</param>
        /// <param name="resType">资源类型，默认是UnKnow，如果是在bundle模式下，并且传入Unknow类型，会改成AssetBundle类型，如果text或AudioClip不打成bungle，必须传入类型，获取资源使用特定的方法</param>
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
            _mapRes.TryGetValue(GetRealResourcePath(path), out res);
            if (res != null)
            {
                if (res.isDone)
                {
                    if (res.isSucc && onSucc != null)
                    {
                        ResourceHandler tempOnSucc = onSucc;
                        onSucc = null;
                        tempOnSucc.Invoke(res,path);
                        tempOnSucc = null;
                    }
                }
                else
                {
                    AddListener(res,path, onSucc, onFail);
                }
                return res;
            }
            res = new Resource();
            res.realPath = GetRealResourcePath(path);
            res.resType = (!DirectLoadMode && resType == ResourceType.UnKnow) ? ResourceType.AssetBundle : resType;

            //这个需要在加载依赖之前放入字典中（如果出现循环引用时需要直接获取）
            _mapRes.Add(res.realPath, res);
            res.Retain();

            //获取到当前资源的依赖资源(可能没法保证顺序，所以拿的时候需要保证所有依赖资源都已经加载好)
            if (!DirectLoadMode && res.resType == ResourceType.AssetBundle)
            {
                string[] listDependResPath = GetDependResPath(path);
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
            //_mapRes.Add(res.realPath, res);
            //res.Retain();
            AddListener(res,path, onSucc, onFail);
            _resLoader.Load(res);
            return res;
        }

        private void OnResourceDone(Resource res)
        {
            if (res.isSucc)
            {
                List<ResourceHandlerStruct> succList;
                _succCallbacks.TryGetValue(res, out succList);
                _succCallbacks.Remove(res);
                if (succList != null)
                {
                    while(succList.Count > 0)
                    {
                        var callbackStruct = succList[0];
                        succList.RemoveAt(0);
                        callbackStruct.callback.Invoke(res, callbackStruct.path);
                    }
                }
                res.Release();
            }
            else
            {
                //失败的话，将资源先移除掉
                if (res.refCount > 1)
                {
                    CLog.LogError("DestroyResource[resPath=" + res.realPath + "],RefCount>1.");
                }
                else
                { 
                    _mapRes.Remove(res.realPath);
                }
                List<ResourceHandlerStruct> failList;
                _failCallbacks.TryGetValue(res, out failList);
                _failCallbacks.Remove(res);
                if (failList != null)
                {
                    while (failList.Count > 0)
                    {
                        var callbackStruct = failList[0];
                        failList.RemoveAt(0);
                        callbackStruct.callback.Invoke(res,callbackStruct.path);
                    }
                }
                res.Release();
                if (res.refCount <= 0)
                { 
                    res.DestroyResource();
                }
            }
        }

        private string[] GetDependResPath(string path)
        {
            return _assetBundleFile.GetDirectDependencies(path);
        }

        private string GetRealResourcePath(string resPath)
        {
            if (string.IsNullOrEmpty(resPath))
            {
                return resPath;
            }
            if (DirectLoadMode)
            {
                return resPath;
            }
            return _assetBundleFile.GetAssetBundleNameByAssetPath(resPath);
        }

        private void AddListener(Resource res,string path, ResourceHandler onSucc = null, ResourceHandler onFail = null)
        {
            if (onSucc != null)
            {
                List<ResourceHandlerStruct> succList;
                _succCallbacks.TryGetValue(res, out succList);
                if (succList == null)
                {
                    succList = new List<ResourceHandlerStruct>();
                    _succCallbacks.Add(res, succList);
                }
                bool contain = false;
                for (int i = 0; i < succList.Count; i++)
                {
                    if(succList[i].callback == onSucc && succList[i].path == path)
                    {
                        contain = true;
                        break;
                    }
                }
                if (!contain)
                {
                    var succCallback = new ResourceHandlerStruct();
                    succCallback.callback = onSucc;
                    succCallback.path = path;
                    succList.Add(succCallback);
                }
            }
            if (onFail != null)
            {
                List<ResourceHandlerStruct> failList;
                _failCallbacks.TryGetValue(res, out failList);
                if (failList == null)
                {
                    failList = new List<ResourceHandlerStruct>();
                    _failCallbacks.Add(res, failList);
                }

                bool contain = false;
                for (int i = 0; i < failList.Count; i++)
                {
                    if (failList[i].callback == onSucc && failList[i].path == path)
                    {
                        contain = true;
                        break;
                    }
                }
                if (!contain)
                {
                    var failCallback = new ResourceHandlerStruct();
                    failCallback.callback = onFail;
                    failCallback.path = path;
                    failList.Add(failCallback);
                }
            }
        }

        public void RemoveListener(Resource res,string path, ResourceHandler onSucc = null, ResourceHandler onFail = null)
        {
            if (onSucc != null)
            {
                List<ResourceHandlerStruct> succList;
                _succCallbacks.TryGetValue(res, out succList);
                if (succList != null)
                {
                    for (int i = succList.Count - 1; i > -1; i--)
                    {
                        if(succList[i].path == path && succList[i].callback == onSucc)
                        {
                            succList.RemoveAt(i);
                            break;
                        }
                    }
                    if(succList.Count == 0)
                    {
                        _succCallbacks.Remove(res);
                    }
                }
            }
            if (onFail != null)
            {
                List<ResourceHandlerStruct> failList;
                _failCallbacks.TryGetValue(res, out failList);
                if (failList != null)
                {
                    for (int i = failList.Count - 1; i > -1; i--)
                    {
                        if (failList[i].path == path && failList[i].callback == onSucc)
                        {
                            failList.RemoveAt(i);
                            break;
                        }
                    }
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
            _mapRes.TryGetValue(GetRealResourcePath(path), out res);
            if (res != null)
            {
                RemoveListener(res,path, onSucc, onFail);
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
            _mapRes.TryGetValue(GetRealResourcePath(path), out res);
            if (res != null)
            {
                RemoveAllListener(res);
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