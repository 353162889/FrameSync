using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class SkillExecutor
    {
        private AgentObject m_cAgentObject;
        private List<Skill> m_lstSkill;
        private List<Skill> m_lstCurSkill;
        public SkillExecutor(AgentObject agentObject)
        {
            m_cAgentObject = agentObject;
            m_lstSkill = new List<Skill>();
            m_lstCurSkill = new List<Skill>();
        }

        public void AddSkill(int skillId)
        {
            if (GetSkill(skillId) != null)
            {
                CLog.LogError("skillId = "+ skillId +" is exist!");
                return;
            }
            NEData neData = SkillCfgSys.Instance.GetSkillData(skillId);
            if (neData == null) return;
            Skill skill = new Skill(m_cAgentObject, neData);
            m_lstSkill.Add(skill);
        }

        public void RemoveSkill(int skillId)
        {
            Break(skillId);
            for (int i = m_lstSkill.Count - 1; i > -1; i--)
            {
                if(m_lstSkill[i].skillId == skillId)
                {
                    m_lstSkill.RemoveAt(i);
                    break;
                }
            }
        }

        public Skill GetSkill(int skillId)
        {
            for (int i = 0; i < m_lstSkill.Count; i++)
            {
                if (m_lstSkill[i].skillId == skillId) return m_lstSkill[i];
            }
            return null;
        }

        public bool CanDo(int skillId)
        {
            return CanDo(GetSkill(skillId));
        }

        public bool CanDo(Skill skill)
        {
            if (skill == null) return false;
            if (IsDoing(skill)) return false;
            return skill.CanDo();
        }

        public void Do(int skillId,uint targetAgentId,AgentObjectType targetAgentType,TSVector position,TSVector forward)
        {
            Do(GetSkill(skillId), targetAgentId, targetAgentType, position, forward);
        }

        public void Do(Skill skill, uint targetAgentId, AgentObjectType targetAgentType, TSVector position, TSVector forward)
        {
            if (skill == null) return;
            if (!CanDo(skill)) return;
            m_lstCurSkill.Add(skill);
            skill.Do(targetAgentId, targetAgentType, position, forward);
        }

        public void Break(int skillId)
        {
            Skill skill = GetSkill(skillId);
            if(skill != null)
            {
                Break(skill);
            }
        }

        public void Break(Skill skill)
        {
            if(IsDoing(skill))
            {
                for (int i = m_lstCurSkill.Count - 1; i > -1 ; i--)
                {
                    if(m_lstCurSkill[i].skillId == skill.skillId)
                    {
                        m_lstCurSkill[i].Break();
                        m_lstCurSkill.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public bool IsDoing(Skill skill)
        {
            for (int i = 0; i < m_lstCurSkill.Count; i++)
            {
                if (m_lstCurSkill[i].skillId == skill.skillId) return true;
            }
            return false;
        }

        public void Update(FP deltaTime)
        {
            for (int i = m_lstCurSkill.Count - 1; i > -1 ; i--)
            {
                m_lstCurSkill[i].Update(deltaTime);
                if(!m_lstCurSkill[i].isDo)
                {
                    m_lstCurSkill.RemoveAt(i);
                }
            }
        }
    }
}
