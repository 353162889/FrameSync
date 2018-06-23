using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    /// <summary>
    /// 表示会当前类为通用行为树节点(编辑器中识别)此类节点，技能、远程通用
    /// </summary>
    public class BTGameNodeAttribute : NENodeAttribute
    {
        public BTGameNodeAttribute(Type nodeDataType) : base(nodeDataType)
        {
        }
    }
}
