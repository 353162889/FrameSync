using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 缓存当前游戏基础对象（整个游戏中都不会释放）
    /// </summary>
    public class CoreGOPool : ExtendGOPool<CoreGOPool>
    {
        public override GameObject GetObject(string path, GameObjectPoolHandler callback)
        {
            return base.GetObject(PathTool.GetBasePrefabPath(path), callback);
        }

        public override void RemoveCallback(string path, GameObjectPoolHandler callback)
        {
            base.RemoveCallback(PathTool.GetBasePrefabPath(path), callback);
        }

        public override void SaveObject(string path, GameObject go)
        {
            base.SaveObject(PathTool.GetBasePrefabPath(path), go);
        }

        public override void Clear(string path)
        {
            base.Clear(PathTool.GetBasePrefabPath(path));
        }
    }
}
