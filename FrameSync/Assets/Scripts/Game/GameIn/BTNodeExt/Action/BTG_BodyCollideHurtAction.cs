using BTCore;
using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class BTG_BodyCollideHurtActionData
    {
        [NEProperty("触发时间(非时间线内忽略该字段)", true)]
        public FP time;
        [NEProperty("作用的对象")]
        public BTActionTarget actionTarget;
    }

    [BTGameNode(typeof(BTG_BodyCollideHurtActionData))]
    [NENodeDesc("身体之间的碰撞造成伤害")]
    public class BTG_BodyCollideHurtAction : BaseTimeLineBTGameAction
    {
        private BTG_BodyCollideHurtActionData m_cHurtData;
        public override FP time { get { return m_cHurtData.time; } }
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cHurtData = data as BTG_BodyCollideHurtActionData;
        }

        public override BTActionResult OnRun(AgentObjectBlackBoard blackBoard)
        {
            var actionTarget = m_cHurtData.actionTarget;
            
            AgentObject host = blackBoard.host;
            AgentObject target = blackBoard.selectAgentObjInfo.agentObj;
            if (host.agentType == AgentObjectType.Unit && host.agent != null && target.agentType == AgentObjectType.Unit && target.agent != null)
            {
                Unit hostUnit = (Unit)host.agent;
                Unit targetUnit = (Unit)target.agent;
                FP damage = TSMath.Min(hostUnit.hp, targetUnit.hp);
                if (actionTarget == BTActionTarget.Host)
                {
                    HurtUnit(host, host, damage);
                }
                else if (actionTarget == BTActionTarget.SelectTarget)
                {
                    HurtUnit(host, target, damage);
                }
                else if (actionTarget == BTActionTarget.All)
                {
                    HurtUnit(host, target, damage);
                    HurtUnit(target, host, damage);
                }
            }
            return BTActionResult.Ready;
        }
        private void HurtUnit(AgentObject attack, AgentObject defence,FP damage)
        {
            var damageInfo = ObjectPool<DamageInfo>.Instance.GetObject();
            damageInfo.attack = attack;
            damageInfo.defence = defence;
            damageInfo.damage = damage;
            ((Unit)defence.agent).OnHurt(damageInfo);
            ObjectPool<DamageInfo>.Instance.SaveObject(damageInfo);
        }
    }
}
