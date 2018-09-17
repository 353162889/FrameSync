using BTCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodeEditor;
using Framework;

namespace Game
{
    public class UnitDoSkillActionData
    {
        [NEProperty("触发时间")]
        public FP time;
        [NEProperty("是否需要目标")]
        public bool bNeedTarget;
    }

    [AINode(typeof(UnitDoSkillActionData))]
    [NENodeDesc("从当前技能库中将能释放的技能一起释放")]
    public class UnitDoSkillAction : BaseTimeLineAIAction
    {
        private UnitDoSkillActionData m_cDoSkillData;

        private Unit m_cUnit;
        private Unit m_cTarget;

        public override FP time
        {
            get
            {
                return m_cDoSkillData.time;
            }
        }

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cDoSkillData = data as UnitDoSkillActionData;
        }

        protected override void OnEnter(AIBlackBoard blackBoard)
        {
            m_cUnit = blackBoard.host.agent as Unit;
            Unit hostUnit = (Unit)blackBoard.host.agent;
            if (hostUnit != null)
            {
                AgentObject agentObj = hostUnit.targetAIAgent;
                if (agentObj != null)
                {
                    m_cTarget = (Unit)agentObj.agent;
                }
            }
           
            base.OnEnter(blackBoard);
        }

        public override BTActionResult OnRun(AIBlackBoard blackBoard)
        {
            if (m_cUnit == null) return BTActionResult.Ready;
            if (m_cDoSkillData.bNeedTarget && (m_cTarget == null || m_cTarget.isDie)) return BTActionResult.Ready;
            var lst = m_cUnit.lstActiveSkill;
            //当前所有技能，可释放就释放
            for (int i = 0; i < lst.Count; i++)
            {
                if(m_cUnit.CanDoSkill(lst[i].skillId,SkillFromType.AI))
                {
                    uint targetId = 0;
                    TSVector targetPos = m_cUnit.curPosition;
                    TSVector targetForward = m_cUnit.curForward;
                    if(m_cTarget != null)
                    {
                        targetId = m_cUnit.id;
                        targetPos = m_cTarget.curPosition;
                        targetForward = m_cTarget.curPosition - m_cUnit.curPosition;
                    }
                    m_cUnit.DoSkill(lst[i].skillId, targetId, AgentObjectType.Unit, targetPos, targetForward, SkillFromType.AI);
                    //return BTActionResult.Ready;
                }
            }
            return BTActionResult.Ready;
        }

        public override void OnExit(AIBlackBoard blackBoard)
        {
            m_cUnit = null;
            m_cTarget = null;
            base.OnExit(blackBoard);
        }
    }
}
