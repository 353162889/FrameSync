using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    /// <summary>
    /// 标识当前类为技能节点(节点编辑器中识别)，此类节点技能专用
    /// </summary>
    public class SkillNodeAttribute : NENodeAttribute
    {
        public SkillNodeAttribute(Type nodeDataType) : base(nodeDataType)
        {
        }
    }
}
