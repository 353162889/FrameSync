using System;
using System.Collections;
using UnityEngine;

namespace Framework
{
    public class WWWResLoader : BaseResLoader
    {
        

        public override void Load(Resource res)
        {
            StartCoroutine(LoadWWWResource(res));
        }

        IEnumerator LoadWWWResource(Resource res)
        {
            string url = GetInResPath(res);
            using (WWW www = new WWW(url))
            {
                yield return www;
                res.isDone = true;
                if (string.IsNullOrEmpty(www.error))
                {
                    res.SetWWWObject(www);
                }
                else
                {
                    res.errorTxt = www.error;
                    CLog.LogError("Load resource [" + url + "] fail!");
                }
            }
            OnDone(res);
        }

        protected override string GetInResPath(Resource res)
        {
            return _resUtil.FullPathForFile(res.realPath, res.resType);
        }
    }
}