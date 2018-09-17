using BTCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodeEditor;
using Framework;

namespace Game
{
    public class UnitDoOneSkillActionData
    {
        [NEProperty("触发时间")]
        public FP time;
        [NEProperty("是否需要目标")]
        public bool bNeedTarget;
    }

    [AINode(typeof(UnitDoOneSkillActionData))]
    [NENodeDesc("从当前技能库中随机寻找一个技能并向目标释放(当前只能释放一个技能)")]
    public class UnitDoOneSkillAction : BaseTimeLineAIAction
    {
        private UnitDoOneSkillActionData m_cDoSkillData;

        private Unit m_cUnit;
        private Unit m_cTarget;
        private List<int> m_lstSkillId;

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
            m_cDoSkillData = data as UnitDoOneSkillActionData;
            m_lstSkillId = new List<int>();
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
            var lst = m_cUnit.lstCurSkill;
            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i].skillType == SkillType.Active) return BTActionResult.Running;
            }
            lst = m_cUnit.lstActiveSkill;
            m_lstSkillId.Clear();
            //当前所有技能，可释放就释放
            for (int i = 0; i < lst.Count; i++)
            {
                m_lstSkillId.Add(lst[i].skillId);
            }

            while(m_lstSkillId.Count > 0)
            {
                int idx = GameInTool.Random(m_lstSkillId.Count);
                int skillId = m_lstSkillId[idx];
                if (m_cUnit.CanDoSkill(skillId, SkillFromType.AI))
                {
                    uint targetId = 0;
                    TSVector targetPos = m_cUnit.curPosition;
                    TSVector targetForward = m_cUnit.curForward;
                    if (m_cTarget != null)
                    {
                        targetId = m_cUnit.id;
                        targetPos = m_cTarget.curPosition;
                        targetForward = m_cTarget.curPosition - m_cUnit.curPosition;
                    }
                    m_cUnit.DoSkill(skillId, targetId, AgentObjectType.Unit, targetPos, targetForward, SkillFromType.AI);
                    return BTActionResult.Ready;
                }
                else
                {
                    m_lstSkillId.RemoveAt(idx);
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
