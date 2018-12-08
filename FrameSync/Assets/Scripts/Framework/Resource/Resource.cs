using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Framework
{ 
    public enum ResourceType
    {
        AssetBundle = 1,
        Bytes = 2,
        AudioClip = 3,
        Movie = 4,
        Text = 5,
        Texture = 6,
        UnKnow = 7
    }

    public class Resource
    {
        //bundle内部资源路径的前缀
        public static string BundlePreRootDir = "";
        /// <summary>
        /// 当前资源的真正路径（如果是bundle模式加载的，那么它是bundle的路径）
        /// </summary>
        public string realPath;
        public ResourceType resType;

        public bool isDone { get; set; }
        public string errorTxt { get; set; }
        public bool isSucc { get { return isDone && string.IsNullOrEmpty(errorTxt); } }

        private UnityEngine.Object _directObj;
        private UnityEngine.Object _wwwAssetObj;
        private string _txt;
        private byte[] _bytes;
        private AssetBundle _assetBundle;

        private List<Resource> _dependRes;
        public List<Resource> dependRes { get { return _dependRes; } }

        protected bool _bytesUnloaded = false;
        public bool bytesUnloaded
        {
            get
            {
                if (resType != ResourceType.AssetBundle)
                {
                    return true;
                }

                return _bytesUnloaded;
            }
        }
        //引用计数，标识游戏内部是否有系统引用该资源（不包括依赖，因为用的是垃圾回收的标记清除方法），
        //引用计数大于0的作为资源节点的入口，通过为每个资源（或其依赖）添加标识来删除没有标识的资源
        public int refCount { get; private set; }
        public bool IsDestroy { get; private set; }

        public Resource()
        {
            isDone = false;
            _directObj = null;
            _txt = null;
            _bytes = null;
            _assetBundle = null;
            refCount = 0;
            IsDestroy = false;
            _bytesUnloaded = false;
        }

        public void SetDirectObject(UnityEngine.Object obj)
        {
            this._directObj = obj;
        }

        public void SetWWWObject(WWW www)
        {
            if (resType == ResourceType.AssetBundle)
            {
                _assetBundle = www.assetBundle;
            }
            else if (resType == ResourceType.Text)
            {
                _txt = www.text;
            }
            else if (resType == ResourceType.Bytes)
            {
                _bytes = www.bytes;
            }
            else if (resType == ResourceType.AudioClip)
            {
                _wwwAssetObj = www.GetAudioClip();
            }
            else if (resType == ResourceType.Movie)
            {
                //_wwwAssetObj = www.movie;
            }
            else if (resType == ResourceType.Texture)
            {
                _wwwAssetObj = www.texture;
            }
        }

        public void SetBundle(AssetBundle assetBundle)
        {
            _assetBundle = assetBundle;
        }

        public string GetText()
        {
            if (resType == ResourceType.Text && !string.IsNullOrEmpty(_txt))
            {
                return _txt;
            }
            else
            {
                if (_directObj != null && _directObj is TextAsset)
                {
                    return ((TextAsset)_directObj).text;
                }
                return null;
            }
        }

        public byte[] GetBytes()
        {
            if (resType == ResourceType.Bytes && _bytes != null)
            {
                return _bytes;
            }
            else
            {
                if (_directObj != null && _directObj is TextAsset)
                {
                    return ((TextAsset)_directObj).bytes;
                }
                return null;
            }
        }

        //public static GameObject GetGameObject(Resource res, string name)
        //{
        //    var prefab = (GameObject)res.GetAsset(name);
        //    if(prefab != null)
        //    {
        //        return GameObject.Instantiate(prefab);
        //    }
        //    return null;
        //}

        //同步加载资源,如果是bundle模式，要注意，该bundle可能打入多个资源，需要传入资源名称
        public UnityEngine.Object GetAsset(string name)
        {
            UnityEngine.Object asset = null;
            if (resType == ResourceType.AssetBundle)
            {
                if (_assetBundle != null)
                {
                    string[] arr = _assetBundle.GetAllAssetNames();
                    if (string.IsNullOrEmpty(name))
                    {
                        if (arr.Length > 1)
                        {
                            string realName = BundlePreRootDir + name.ToLower();
                            asset = _assetBundle.LoadAsset(realName);
                        }
                        if (asset == null)
                        {
                            asset = _assetBundle.LoadAsset(arr[0]);
                        }
                    }
                    else
                    {
                        string realName = BundlePreRootDir + name.ToLower();
                        asset = _assetBundle.LoadAsset(realName);
                    }
                }
                else
                {
                    asset = _wwwAssetObj;
                }
            }
            else
            {
                if (_wwwAssetObj != null)
                {
                    asset = _wwwAssetObj;
                }
                else
                {
                    asset = _directObj;
                }
            }
            return asset;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerator GetAssetAsync(string path, Action<string,UnityEngine.Object> callback)
        {
            UnityEngine.Object asset = null;
            if (resType == ResourceType.AssetBundle)
            {
                if (_assetBundle != null)
                {
                    string[] arr = _assetBundle.GetAllAssetNames();
                    if (string.IsNullOrEmpty(path))
                    {
                        if (arr.Length > 1)
                        {
                            string realName = BundlePreRootDir + path.ToLower();
                            AssetBundleRequest request = _assetBundle.LoadAssetAsync(realName);
                            yield return request;
                            asset = request.asset;
                        }
                        if (asset == null)
                        {
                            AssetBundleRequest request = _assetBundle.LoadAssetAsync(arr[0]);
                            yield return request;
                            asset = request.asset;
                        }
                    }
                    else
                    {
                        string realName = BundlePreRootDir + path.ToLower();
                        AssetBundleRequest request = _assetBundle.LoadAssetAsync(realName);
                        yield return request;
                        asset = request.asset;
                    }
                }
                else
                {
                    asset = _wwwAssetObj;
                }
            }
            else
            {
                if (_wwwAssetObj != null)
                {
                    asset = _wwwAssetObj;
                }
                else
                {
                    asset = _directObj;
                }
            }
            callback.Invoke(path,asset);
        }

        public void SetDependsRes(List<Resource> list)
        {
            this._dependRes = list;
            //这里不需要依赖文件引用
            //if (_dependRes != null)
            //{
            //    int count = _dependRes.Count;
            //    for (int i = 0; i < count; i++)
            //    {
            //        if (_dependRes[i] != null)
            //        {
            //            _dependRes[i].Retain();
            //        }
            //    }
            //}
        }

        public void Retain()
        {
            ++refCount;
        }

        public void Release()
        {
            if (refCount > 0)
            {
                --refCount;
            }
        }

        public void UnloadBinaryBytes()
        {
            if (_assetBundle != null && !_bytesUnloaded)
            {
                _assetBundle.Unload(false);
                _bytesUnloaded = true;
            }
        }

        public void DestroyResource()
        {
            _txt = null;
            _bytes = null;
            if (_assetBundle != null)
            {
                _assetBundle.Unload(false);
                _assetBundle = null;
            }
            _bytesUnloaded = true;
            if (_wwwAssetObj != null)
            {
                if (resType != ResourceType.AssetBundle)
                {
                    GameObject.Destroy(_wwwAssetObj);
                }
                _wwwAssetObj = null;
            }
            if (_directObj != null)
            {
                _directObj = null;
            }
            _dependRes = null;
            IsDestroy = true;
        }
    }
}