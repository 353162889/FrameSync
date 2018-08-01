using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using NodeEditor;

namespace Game
{
    public class BTG_IsHitDecoratorData
    {
        [NEProperty("作用的对象")]
        public BTActionTarget actionTarget;
    }

    [BTGameNode(typeof(BTG_IsHitDecoratorData))]
    [NENodeDesc("装饰某个action操作是否击中目标对象")]
    public class BTG_IsHitDecorator : BaseBTGameDecorator
    {
        private bool m_bIsUnitHurt;
        private BTG_IsHitDecoratorData m_cIsHitData;
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cIsHitData = data as BTG_IsHitDecoratorData;
        }

        public override BTResult OnEnter(AgentObjectBlackBoard blackBoard)
        {
            var actionTarget = m_cIsHitData.actionTarget;
            AgentObject target = null;
            if (actionTarget == BTActionTarget.Host) target = blackBoard.host;
            else if (actionTarget == BTActionTarget.SelectTarget) target = blackBoard.selectAgentObjInfo.agentObj;
            if (target != null && target.agentType == AgentObjectType.Unit)
            {
                Unit unit = (Unit)target.agent;
                unit.OnUnitHurt += OnUnitHurt;
                m_bIsUnitHurt = false;
                BTResult result = base.OnEnter(blackBoard);
                unit.OnUnitHurt -= OnUnitHurt;
                if(m_bIsUnitHurt)
                {
                    return BTResult.Success;
                }
                else
                {
                    return BTResult.Failure;
                }
            }
            else
            {
                return base.OnEnter(blackBoard);
            }
        }

        private void OnUnitHurt(Unit unit, DamageInfo damageInfo)
        {
            m_bIsUnitHurt = true;
        }
    }
}
