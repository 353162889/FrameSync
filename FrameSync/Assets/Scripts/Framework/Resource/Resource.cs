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
        private static string BundleRootName = "Assets/Resources/".ToLower();
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
                            string realName = BundleRootName + name.ToLower();
                            asset = _assetBundle.LoadAsset(realName);
                        }
                        if (asset == null)
                        {
                            asset = _assetBundle.LoadAsset(arr[0]);
                        }
                    }
                    else
                    {
                        string realName = BundleRootName + name.ToLower();
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
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerator GetAssetAsync(string name, Action<UnityEngine.Object> callback)
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
                            string realName = BundleRootName + name.ToLower();
                            AssetBundleRequest request = _assetBundle.LoadAssetAsync(name);
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
                        string realName = BundleRootName + name.ToLower();
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
            callback.Invoke(asset);
        }

        public void SetDependsRes(List<Resource> list)
        {
            this._dependRes = list;
            if (_dependRes != null)
            {
                int count = _dependRes.Count;
                for (int i = 0; i < count; i++)
                {
                    if(_dependRes[i] != null)
                    { 
                        _dependRes[i].Retain();
                    }
                }
            }
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