﻿using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 战斗场景用到的对象池（退出战斗清理）
    /// </summary>
    public class SceneGOPool : ExtendGOPool<SceneGOPool>
    {
        public void CacheObject(string path, bool isPrefab, int count, Action<string> callback)
        {
            base._CacheObject(PathTool.GetBasePrefabPath(path), isPrefab, count, callback);
        }

        public void RemoveCacheObject(string path, Action<string> callback)
        {
            base._RemoveCacheObject(PathTool.GetBasePrefabPath(path), callback);
        }

        public UnityEngine.Object GetObject(string path,bool isPrefab, ResourceObjectPoolHandler callback)
        {
            return base._GetObject(PathTool.GetBasePrefabPath(path), isPrefab, callback);
        }

        public void RemoveCallback(string path, ResourceObjectPoolHandler callback)
        {
            base._RemoveCallback(PathTool.GetBasePrefabPath(path), callback);
        }

        public void SaveObject(string path, GameObject go)
        {
            base._SaveObject(PathTool.GetBasePrefabPath(path), go);
        }

        public void Clear(string path)
        {
            base._Clear(PathTool.GetBasePrefabPath(path));
        }
    }
}
