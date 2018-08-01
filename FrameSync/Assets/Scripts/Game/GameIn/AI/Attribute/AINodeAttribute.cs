using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    /// <summary>
    /// 标识当前类为AI节点(节点编辑器中识别)，此类节点AI专用
    /// </summary>
    public class AINodeAttribute : NENodeAttribute
    {
        public AINodeAttribute(Type nodeDataType) : base(nodeDataType)
        {
        }
    }
}
