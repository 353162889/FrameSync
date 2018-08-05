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

    }

    [AINode(typeof(UnitDoSkillActionData))]
    [NENodeDesc("从当前技能库中寻找技能并释放")]
    public class UnitDoSkillAction : BaseAIAction
    {
        private Unit m_cUnit;
        protected override void OnEnter(AIBlackBoard blackBoard)
        {
            m_cUnit = blackBoard.host.agent as Unit;
            base.OnEnter(blackBoard);
        }

        public override BTActionResult OnRun(AIBlackBoard blackBoard)
        {
            if (m_cUnit == null) return BTActionResult.Ready;
            var lst = m_cUnit.lstSkill;
            for (int i = 0; i < lst.Count; i++)
            {
                if(m_cUnit.CanDoSkill(lst[i].skillId))
                {
                    m_cUnit.DoSkill(lst[i].skillId, 0, AgentObjectType.Unit, m_cUnit.curPosition, m_cUnit.curForward);
                    return BTActionResult.Running;
                }
            }
            return BTActionResult.Ready;
        }

        public override void OnExit(AIBlackBoard blackBoard)
        {
            m_cUnit = null;
            base.OnExit(blackBoard);
        }
    }
}
