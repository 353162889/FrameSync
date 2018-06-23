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
            return SceneEffectDir + name + ".prefab";
        }

        public static string UIEffectDir = "Prefab/UIEffect/";
        public static string GetUIEffectPath(string name)
        {
            return UIEffectDir + name + ".prefab";
        }
    }
}
