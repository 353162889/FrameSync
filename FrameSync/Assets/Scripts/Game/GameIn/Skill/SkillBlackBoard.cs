using BTCore;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class SkillBlackBoard : AgentObjectBlackBoard
    {
        private Skill m_cSkill;
        public Skill skill { get { return m_cSkill; } }

        public override AgentObject host
        {
            get
            {
                return m_cSkill.host;
            }
        }

        public override FP GetHostAttr(AttrType attrType)
        {
            if (attrType == AttrType.Attack)
            {
                return base.GetHostAttr(attrType) + m_cSkill.resInfo.add_damage;
            }
            else
            {
                return base.GetHostAttr(attrType);
            }
        }

        public void Init(Skill skill)
        {
            m_cSkill = skill;
        }

        public override void Clear()
        {
            m_cSkill = null;
            base.Clear();
        }
    }
}
