using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{ 
    /// <summary>
    /// 不再使用该方式
    /// </summary>
    //public class MultiResourceLoader
    //{
    //    private int _finishCount;
    //    private List<string> _loadList;
    //    private Action<MultiResourceLoader> _OnComplete;
    //    private Action<Resource,string> _OnProgress;

    //    private Dictionary<string, Resource> _mapRes;

    //    private Dictionary<string, List<Action<Resource,string>>> _mapTryGetRes;
    //    private ResourceContainer _resourceContainer;

    //    public MultiResourceLoader(ResourceContainer resourceContainer)
    //    {
    //        _resourceContainer = resourceContainer;
    //        _loadList = new List<string>();
    //        _mapRes = new Dictionary<string, Resource>();
    //        _mapTryGetRes = new Dictionary<string, List<Action<Resource,string>>>();
    //        _finishCount = 0;
    //    }

    //    public void LoadList(List<string> names, Action<MultiResourceLoader> OnComplete = null, Action<Resource,string> OnProgress = null, ResourceType resType = ResourceType.UnKnow)
    //    {
    //        if (names == null || names.Count == 0)
    //            return;
    //        for (int i = 0; i < names.Count; i++)
    //        {
    //            if (_loadList.Contains(names[i]))
    //            {
    //                CLog.LogError("Can not has same name in one MultiResourceLoader");
    //                return;
    //            }
    //            else
    //            {
    //                _loadList.Add(names[i]);
    //            }
    //        }
    //        this._OnComplete = OnComplete;
    //        this._OnProgress = OnProgress;
    //        for (int i = 0; i < names.Count; i++)
    //        {
    //            _resourceContainer.GetResource(names[i], OnFinish, OnFinish, resType);
    //        }
    //    }

    //    public Resource TryGetRes(string path, Action<Resource,string> callback = null)
    //    {
    //        Resource res;
    //        _mapRes.TryGetValue(path, out res);
    //        if (res != null)
    //        {
    //            if (callback != null)
    //            {
    //                callback.Invoke(res,path);
    //            }
    //            return res;
    //        }
    //        if (callback != null)
    //        {
    //            List<Action<Resource,string>> list;
    //            _mapTryGetRes.TryGetValue(path, out list);
    //            if (list == null)
    //            {
    //                list = new List<Action<Resource,string>>();
    //                _mapTryGetRes.Add(path, list);
    //            }
    //            if (!list.Contains(callback))
    //            {
    //                list.Add(callback);
    //            }
    //        }
    //        return null;
    //    }

    //    private void OnFinish(Resource res,string path)
    //    {
    //        if (res.isSucc)
    //        {
    //            res.Retain();
    //            _mapRes.Add(path, res);
    //        }
    //        else
    //        {
    //            CLog.LogError("[MultiResourceLoader] load " + path + " fail!");
    //        }
    //        _finishCount++;
    //        List<Action<Resource,string>> list;
    //        _mapTryGetRes.TryGetValue(path, out list);
    //        if (list != null)
    //        {
    //            _mapTryGetRes.Remove(path);
    //            for (int i = 0; i < list.Count; i++)
    //            {
    //                list[i].Invoke(res,path);
    //            }
    //        }
    //        if (_OnProgress != null)
    //        {
    //            Action<Resource,string> tempAction = _OnProgress;
    //            tempAction.Invoke(res,path);
    //        }
    //        if (_finishCount == _loadList.Count)
    //        {
    //            foreach (var item in _mapTryGetRes)
    //            {
    //                Resource tempRes;
    //                _mapRes.TryGetValue(item.Key, out tempRes);
    //                if (tempRes == null)
    //                    continue;
    //                for (int i = 0; i < item.Value.Count; i++)
    //                {
    //                    item.Value[i].Invoke(tempRes,path);
    //                }
    //            }
    //            _mapTryGetRes.Clear();
    //            if (_OnComplete != null)
    //            {
    //                Action<MultiResourceLoader> tempAction = _OnComplete;
    //                _OnComplete = null;
    //                tempAction.Invoke(this);
    //            }
    //        }
    //    }

    //    public void Clear()
    //    {
    //        if (_finishCount < _loadList.Count)
    //        {
    //            for (int i = 0; i < _loadList.Count; i++)
    //            {
    //                _resourceContainer.RemoveListener(_loadList[i], OnFinish, OnFinish);
    //            }
    //        }
    //        _mapTryGetRes.Clear();
    //        _finishCount = 0;
    //        _loadList.Clear();
    //        foreach (var item in _mapRes)
    //        {
    //            item.Value.Release();
    //        }
    //        _mapRes.Clear();
    //        this._OnComplete = null;
    //        this._OnProgress = null;
    //        _resourceContainer = null;
    //    }
    //}

}