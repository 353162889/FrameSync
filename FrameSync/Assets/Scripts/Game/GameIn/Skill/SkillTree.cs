using BTCore;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class SkillData
    {
    }

    [NENode(typeof(SkillData))]
    [NENodeDisplay(false, true, false)]
    [NENodeName("SkillRoot")]
    public class SkillTree : BTNode
    {

    }
}
