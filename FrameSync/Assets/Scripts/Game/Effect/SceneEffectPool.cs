using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{ 
    public class SceneEffectPool : EffectPool<SceneEffectPool>
    {
        public override GameObject CreateEffect(string name, bool autoDestory, Transform parent = null)
        {
            string path = PathTool.GetSceneEffectPath(name);
            return base.CreateEffect(path, autoDestory, parent);
        }
    }
}
