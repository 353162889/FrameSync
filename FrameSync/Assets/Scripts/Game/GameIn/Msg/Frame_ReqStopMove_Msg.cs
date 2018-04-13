using Framework;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [NetMsg]
    public class Frame_ReqStopMove_Msg : MsgBase<Frame_ReqStopMove_Data>
    {
        protected override void HandleMsg(Frame_ReqStopMove_Data msg)
        {
            Unit unit = BattleScene.Instance.GetUnit(msg.unitId);
            if (unit != null)
            {
                unit.StopMove();
            }
        }
    }
}
