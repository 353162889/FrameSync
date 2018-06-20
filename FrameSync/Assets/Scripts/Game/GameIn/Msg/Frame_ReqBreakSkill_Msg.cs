using Framework;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [NetMsg]
    public class Frame_ReqBreakSkill_Msg : MsgBase<Frame_ReqBreakSkill_Data>
    {
        protected override void HandleMsg(Frame_ReqBreakSkill_Data msg)
        {
            Unit unit = BattleScene.Instance.GetUnit(msg.unitId);
            if (unit != null)
            {
                unit.BreakSkill(msg.skillId);
            }
        }
    }
}
