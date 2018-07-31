using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{ 
    /// <summary>
    /// 场景特效池（切换场景时清理）
    /// </summary>
    public class SceneEffectPool : EffectPool<SceneEffectPool>
    {
        public override GameObject CreateEffect(string name, bool autoDestory, Transform parent = null)
        {
            string path = PathTool.GetSceneEffectPath(name);
            return base.CreateEffect(path, autoDestory, parent);
        }
    }
}
