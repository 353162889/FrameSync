using Framework;
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
