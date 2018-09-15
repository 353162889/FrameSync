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
        public List<Skill> lstSkill { get { return m_lstSkill; } }
        private List<Skill> m_lstSkill;
        public List<Skill> lstActiveSkill { get { return m_lstActiveskill; } }
        private List<Skill> m_lstActiveskill;
        public List<Skill> lstPassiveSkill { get { return m_lstPassiveSkill; } }
        private List<Skill> m_lstPassiveSkill;
        public List<Skill> lstCurSkill { get { return m_lstCurSkill; } }
        private List<Skill> m_lstCurSkill;
        public SkillExecutor()
        {
            m_lstSkill = new List<Skill>();
            m_lstCurSkill = new List<Skill>();
            m_lstActiveskill = new List<Skill>();
            m_lstPassiveSkill = new List<Skill>();
        }

        public void Init(AgentObject agentObject)
        {
            m_cAgentObject = agentObject;
        }

        public void AddSkill(int skillId)
        {
            if (GetSkill(skillId) != null)
            {
                CLog.LogError("skillId = "+ skillId +" is exist!");
                return;
            }
            Skill skill = SkillPool.Instance.GetSkill(skillId);
            if (skill != null)
            {
                skill.Init(m_cAgentObject);
                m_lstSkill.Add(skill);
                if(skill.skillType == SkillType.Active)
                {
                    m_lstActiveskill.Add(skill);
                }
                else if(skill.skillType == SkillType.Passive)
                {
                    m_lstPassiveSkill.Add(skill);
                    Do(skill, 0, AgentObjectType.Unit, TSVector.zero, TSVector.forward);
                }
            }
        }

        public void RemoveSkill(int skillId)
        {
            Break(skillId);
            for (int i = m_lstPassiveSkill.Count - 1 ; i > -1; i--)
            {
                var skill = m_lstPassiveSkill[i];
                if (skill.skillId == skillId)
                {
                    m_lstPassiveSkill.RemoveAt(i);
                    break;
                }
            }

            for (int i = m_lstActiveskill.Count - 1; i > -1; i--)
            {
                var skill = m_lstActiveskill[i];
                if (skill.skillId == skillId)
                {
                    m_lstActiveskill.RemoveAt(i);
                    break;
                }
            }
            for (int i = m_lstSkill.Count - 1; i > -1; i--)
            {
                var skill = m_lstSkill[i];
                if (skill.skillId == skillId)
                {
                    m_lstSkill.RemoveAt(i);
                    SkillPool.Instance.SaveSkill(skillId, skill);
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

        public void BreakAll()
        {
            for (int i = m_lstCurSkill.Count - 1; i > -1; i--)
            {
                m_lstCurSkill[i].Break();
                m_lstCurSkill.RemoveAt(i);
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

        public void Clear()
        {
            BreakAll();
            m_lstActiveskill.Clear();
            m_lstPassiveSkill.Clear();
            for (int i = 0; i < m_lstSkill.Count; i++)
            {
                SkillPool.Instance.SaveSkill(m_lstSkill[i].skillId, m_lstSkill[i]);
            }
            m_lstCurSkill.Clear();
            m_lstSkill.Clear();
            m_cAgentObject = null;
        }
    }
}
