using BTCore;
using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public enum SkillTargetType
    {
        Target,
        TargetPosition,
        TargetForward,
    }
    public class SkillData
    {
        [NEPropertyKey]
        [NEProperty("技能ID")]
        public int skillId;
        [NEProperty("技能描述")]
        public string skillDesc;
        [NEProperty("技能目标类型")]
        public SkillTargetType skillTarget;
        [NEProperty("技能CD")]
        public FP skillCD;
    }

    [BTNode(typeof(SkillData))]
    [NENodeDisplay(false, true, false)]
    [NENodeName("SkillRoot")]
    public class SkillTree : BTNode
    {
        private BTNode m_cChild;
        public override void AddChild(BTNode child)
        {
            if (m_cChild != null)
            {
                CLog.LogError("SkillTree has exist child node! add has override it");
            }
            m_cChild = child;
        }
      

        public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            if(m_cChild != null)
            {
                return m_cChild.OnTick(blackBoard);
            }
            return base.OnTick(blackBoard);
        }

        public override void Clear()
        {
            if(m_cChild != null)
            {
                m_cChild.Clear();
            }
        }
    }
}
