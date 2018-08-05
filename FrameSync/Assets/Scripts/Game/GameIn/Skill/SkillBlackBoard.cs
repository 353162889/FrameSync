using BTCore;
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
