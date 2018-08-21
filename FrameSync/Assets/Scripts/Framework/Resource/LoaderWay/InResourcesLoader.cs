﻿using System;
using System.Collections;
using UnityEngine;

namespace Framework
{
    public class InResourcesLoader : BaseResLoader
    {

        public override void Load(Resource res)
        {
            StartCoroutine(LoadDirectResource(res));
        }

        IEnumerator LoadDirectResource(Resource res)
        {
            string loadPath = GetInResPath(res);
            ResourceRequest request = Resources.LoadAsync(loadPath);
            yield return request;
            res.isDone = true;
            if (request.asset == null)
            {
                res.errorTxt = "Load resource [" + loadPath + "] fail!";
                CLog.LogError(res.errorTxt);
            }
            else
            {
                res.SetDirectObject(request.asset);
            }
            OnDone(res);
        }

        protected override string GetInResPath(Resource res)
        {
            return _resUtil.FullPathForFile(res.realPath, res.resType);
        }

    }

}