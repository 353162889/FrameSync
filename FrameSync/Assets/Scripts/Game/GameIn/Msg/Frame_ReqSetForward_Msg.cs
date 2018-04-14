using Framework;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [NetMsg]
    public class Frame_ReqSetForward_Msg : MsgBase<Frame_ReqSetForward_Data>
    {
        protected override void HandleMsg(Frame_ReqSetForward_Data msg)
        {
            Unit unit = BattleScene.Instance.GetUnit(msg.unitId);
            if (null != unit)
            {
                unit.SetForward(GameInTool.ToTSVector(msg.forward));
            }
        }
    }
}
