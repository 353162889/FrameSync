using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Framework;
using NodeEditor;

namespace Game
{
    public class RM_TemplateHurtActionData
    {
        [NEProperty("触发时间(非时间线内忽略该字段)", true)]
        public FP time;
        [NEProperty("作用的对象")]
        public BTActionTarget actionTarget;
    }

    [BTGameNode(typeof(RM_TemplateHurtActionData))]
    [NENodeDesc("对选择器选择的单位进行伤害处理")]
    public class RM_TemplateHurtAction : BaseTimeLineRemoteAction
    {
        private RM_TemplateHurtActionData m_cHurtData;
        public override FP time { get { return m_cHurtData.time; } }
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cHurtData = data as RM_TemplateHurtActionData;
        }

        public override BTActionResult OnRun(RemoteBlackBoard blackBoard)
        {
            var actionTarget = m_cHurtData.actionTarget;
            AgentObject target = null;
            if (actionTarget == BTActionTarget.Host) target = blackBoard.host;
            else if (actionTarget == BTActionTarget.SelectTarget) target = blackBoard.selectAgentObjInfo.agentObj;
            if (target != null)
            {
                if (target.agentType == AgentObjectType.Unit && target.agent != null)
                {
                    var unit = (Unit)target.agent;
                    var damageInfo = ObjectPool<DamageInfo>.Instance.GetObject();
                    damageInfo.attack = blackBoard.host;
                    damageInfo.defence = target;
                    FP attack = blackBoard.host.GetAttrValue((int)AttrType.Attack);
                    damageInfo.damage = attack + blackBoard.remote.resInfo.add_damage;
                    unit.OnHurt(damageInfo);
                    ObjectPool<DamageInfo>.Instance.SaveObject(damageInfo);
                }
            }
            return BTActionResult.Ready;
        }
    }
}
