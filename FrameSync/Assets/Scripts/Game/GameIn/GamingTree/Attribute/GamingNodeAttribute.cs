﻿using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    /// <summary>
    /// 标识当前类为游戏逻辑节点(节点编辑器中识别)，此类节点游戏逻辑专用
    /// </summary>
    public class GamingNodeAttribute : NENodeAttribute
    {
        public GamingNodeAttribute(Type nodeDataType) : base(nodeDataType)
        {
        }
    }
}
