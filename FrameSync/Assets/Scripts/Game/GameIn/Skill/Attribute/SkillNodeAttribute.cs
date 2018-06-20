using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class SkillNodeAttribute : NENodeAttribute
    {
        public SkillNodeAttribute(Type nodeDataType) : base(nodeDataType)
        {
        }
    }
}
