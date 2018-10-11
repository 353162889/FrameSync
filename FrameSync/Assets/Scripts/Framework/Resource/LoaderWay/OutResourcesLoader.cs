using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class OutResourcesLoader : BaseResLoader
    {
        public override void Load(Resource res)
        {
            string loadPath = GetInResPath(res);
            UnityEngine.Object go = null;
#if UNITY_EDITOR
            go = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(loadPath);
#endif
            res.isDone = true;
            if (go == null)
            {
                res.errorTxt = "Load resource [" + loadPath + "] fail!";
                CLog.LogError(res.errorTxt);
            }
            else
            {
                res.SetDirectObject(go);
            }
            OnDone(res);
        }

        protected override string GetInResPath(Resource res)
        {
            return _resUtil.FullPathForFile(res.realPath, res.resType,false);
        }
    }
}