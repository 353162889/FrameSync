using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class UIEffectPool : EffectPool<UIEffectPool>
    {
        public override GameObject CreateEffect(string name, bool autoDestory, Transform parent = null)
        {
            string path = PathTool.GetUIEffectPath(name);
            return base.CreateEffect(path, autoDestory, parent);
        }
    }
}

