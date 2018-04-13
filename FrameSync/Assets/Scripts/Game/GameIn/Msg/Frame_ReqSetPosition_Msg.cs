using Framework;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [NetMsg]
    public class Frame_ReqSetPosition_Msg : MsgBase<Frame_ReqSetPosition_Data>
    {
        protected override void HandleMsg(Frame_ReqSetPosition_Data msg)
        {
            Unit unit = BattleScene.Instance.GetUnit(msg.unitId);
            if(null != unit)
            {
                unit.SetPosition(GameInTool.ToTSVector(msg.position));
            }
        }
    }
}
