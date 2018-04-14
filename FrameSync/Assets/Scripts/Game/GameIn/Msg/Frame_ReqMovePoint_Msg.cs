using Framework;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [NetMsg]
    public class Frame_ReqMovePoint_Msg : MsgBase<Frame_ReqMovePoint_Data>
    {
        protected override void HandleMsg(Frame_ReqMovePoint_Data msg)
        {
            Unit unit = BattleScene.Instance.GetUnit(msg.unitId);
            if (null != unit)
            {
                TSVector targetPosition = GameInTool.ToTSVector(msg.targetPosition);
                unit.Move(targetPosition);
            }
        }
    }
}
