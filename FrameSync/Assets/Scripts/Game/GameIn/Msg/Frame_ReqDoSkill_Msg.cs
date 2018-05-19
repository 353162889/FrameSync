using Framework;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [NetMsg]
    public class Frame_ReqDoSkill_Msg : MsgBase<Frame_ReqDoSkill_Data>
    {
        protected override void HandleMsg(Frame_ReqDoSkill_Data msg)
        {
            Unit unit = BattleScene.Instance.GetUnit(msg.unitId);
            if (unit != null)
            {
                unit.DoSkill(msg.skillId, msg.targetAgentId, (AgentObjectType)msg.targetAgentType, GameInTool.ToTSVector(msg.position), GameInTool.ToTSVector(msg.forward));
            }
        }
    }
}
