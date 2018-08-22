using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public static class PathTool
    {
        public static string SceneEffectDir = "Prefab/Effect/";
        public static string GetSceneEffectPath(string name)
        {
            return GetBasePrefabPath(SceneEffectDir + name);
        }

        public static string UIEffectDir = "Prefab/UIEffect/";
        public static string GetUIEffectPath(string name)
        {
            return GetBasePrefabPath(UIEffectDir + name);
        }

        public static string GetBasePrefabPath(string name)
        {
            if (!name.EndsWith(".prefab"))
            {
                return name + ".prefab";
            }
            return name;
        }
    }
}
