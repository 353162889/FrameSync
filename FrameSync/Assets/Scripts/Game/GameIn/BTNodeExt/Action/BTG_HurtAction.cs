using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Framework;
using NodeEditor;

namespace Game
{
    public class BTG_HurtActionData
    {
        [NEProperty("触发时间(非时间线内忽略该字段)", true)]
        public FP time;
        [NEProperty("作用的对象")]
        public BTActionTarget actionTarget;
    }

    [BTGameNode(typeof(BTG_HurtActionData))]
    [NENodeDesc("对选择器选择的单位进行伤害处理")]
    public class BTG_HurtAction : BaseTimeLineBTGameAction
    {
        private BTG_HurtActionData m_cHurtData;
        public override FP time { get { return m_cHurtData.time; } }
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cHurtData = data as BTG_HurtActionData;
        }

        public override BTActionResult OnRun(AgentObjectBlackBoard blackBoard)
        {
            var actionTarget = m_cHurtData.actionTarget;
            AgentObject target = null;
            if (actionTarget == BTActionTarget.Host) target = blackBoard.host;
            else if (actionTarget == BTActionTarget.SelectTarget) target = blackBoard.selectAgentObjInfo.agentObj;
            if (target != null)
            {
                if(target.agentType == AgentObjectType.Unit && target.agent != null)
                {
                    var unit = (Unit)target.agent;
                    var damageInfo = ObjectPool<DamageInfo>.Instance.GetObject();
                    damageInfo.attack = blackBoard.host;
                    damageInfo.defence = target;
                    FP attack = blackBoard.host.GetAttrValue((int)AttrType.Attack);
                    damageInfo.damage = attack;
                    unit.OnHurt(damageInfo);
                    ObjectPool<DamageInfo>.Instance.SaveObject(damageInfo);
                }
            }
            return BTActionResult.Ready;
        }
    }
}
