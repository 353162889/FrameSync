using Framework;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public partial class Unit
    {
        protected SkillExecutor m_cSkillExecutor;

        public void ReqDoSkill(int skillId,uint targetAgentId,AgentObjectType targetAgentType,TSVector position,TSVector forward)
        {
            if(CanDoSkill(skillId))
            {
                Frame_ReqDoSkill_Data data = new Frame_ReqDoSkill_Data();
                data.unitId = this.id;
                data.skillId = skillId;
                data.targetAgentId = targetAgentId;
                data.targetAgentType = (int)targetAgentType;
                data.position = GameInTool.ToProtoVector2(position);
                data.forward = GameInTool.ToProtoVector2(forward);
                NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_ReqDoSkill, data);
            }
        }

        public void DoSkill(int skillId, uint targetAgentId, AgentObjectType targetAgentType, TSVector position, TSVector forward)
        {
            if(CanDoSkill(skillId))
            {
                m_cSkillExecutor.Do(skillId, targetAgentId, targetAgentType, position, forward);
            }
        }

        public void ReqBreakSkill(int skillId)
        {
            if (CanBreakSkill(skillId))
            {
                Frame_ReqBreakSkill_Data data = new Frame_ReqBreakSkill_Data();
                data.unitId = this.id;
                data.skillId = skillId;
                NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_ReqBreakSkill, data);
            }
        }

        public void BreakSkill(int skillId)
        {
            if(CanBreakSkill(skillId))
            {
                m_cSkillExecutor.Break(skillId);
            }
        }

        public void AddSkill(int skillId)
        {
            m_cSkillExecutor.AddSkill(skillId);
        }

        public void RemoveSkill(int skillId)
        {
            m_cSkillExecutor.RemoveSkill(skillId);
        }

        public bool CanDoSkill(int skillId)
        {
            return m_cSkillExecutor.CanDo(skillId);
        }

        public bool CanBreakSkill(int skillId)
        {
            return IsDoingSkill(skillId);
        }

        public bool IsDoingSkill(int skillId)
        {
            Skill skill = m_cSkillExecutor.GetSkill(skillId);
            if (skill == null)
            {
                return m_cSkillExecutor.IsDoing(skill);
            }
            return false;
        }


        protected void InitSkill()
        {
            m_cSkillExecutor = new SkillExecutor(this.agentObj);
        }

        protected void UpdateSkill(FP deltaTime)
        {
            if(null != m_cSkillExecutor)
            {
                m_cSkillExecutor.Update(deltaTime);
            }
        }

        protected void ResetSkill()
        {

        }

        protected void DisposeSkill()
        {

        }
    }
}
