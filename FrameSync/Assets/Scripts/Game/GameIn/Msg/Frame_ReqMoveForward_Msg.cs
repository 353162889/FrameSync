using Framework;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [NetMsg]
    public class Frame_ReqMoveForward_Msg : MsgBase<Frame_ReqMoveForward_Data>
    {
        protected override void HandleMsg(Frame_ReqMoveForward_Data msg)
        {
            Unit unit = BattleScene.Instance.GetUnit(msg.unitId);
            if (unit != null)
            {
                unit.MoveForward(GameInTool.ToTSVector(msg.forward),FP.FromSourceLong(msg.len));
            }
        }
    }
}
