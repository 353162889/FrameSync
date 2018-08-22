using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// UI特效池（根据需求清理）
    /// </summary>
    public class UIEffectPool : EffectPool<UIEffectPool>
    {

        public void CacheObject(string name, int count, Action<string> callback)
        {
            string path = PathTool.GetUIEffectPath(name);
            base._CacheObject(path, true, count, callback);
        }

        public void RemoveCacheObject(string name, Action<string> callback)
        {
            string path = PathTool.GetUIEffectPath(name);
            base._RemoveCacheObject(path, callback);
        }

        public GameObject CreateEffect(string name, bool autoDestory, Transform parent = null)
        {
            string path = PathTool.GetUIEffectPath(name);
            return base._CreateEffect(path, autoDestory, parent);
        }
    }
}

