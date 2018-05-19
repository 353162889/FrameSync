using BTCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class SkillBlackBoard : BTBlackBoard
    {
        private Skill m_cSkill;
        public Skill skill { get { return m_cSkill; } }
        public SkillBlackBoard(Skill skill)
        {
            m_cSkill = skill;
        }
    }
}
