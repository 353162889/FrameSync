using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    /// <summary>
    /// 标识当前类为远程节点(节点编辑器中识别)，此类节点远程专用
    /// </summary>
    public class RemoteNodeAttribute : NENodeAttribute
    {
        public RemoteNodeAttribute(Type nodeDataType) : base(nodeDataType)
        {
        }
    }
}
