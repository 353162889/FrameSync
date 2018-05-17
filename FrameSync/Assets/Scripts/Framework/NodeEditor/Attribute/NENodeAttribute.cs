using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeEditor
{
    /// <summary>
    /// 标识当前类是一个节点
    /// </summary>
    public class NENodeAttribute : Attribute
    {
        public Type nodeDataType { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeDataType">关联的节点数据类型</param>
        public NENodeAttribute(Type nodeDataType)
        {
            this.nodeDataType = nodeDataType;
        }
    }
}