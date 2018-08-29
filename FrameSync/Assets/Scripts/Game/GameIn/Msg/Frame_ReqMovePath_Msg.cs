using Framework;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [NetMsg]
    public class Frame_ReqMovePath_Msg : MsgBase<Frame_ReqMovePath_Data>
    {
        protected override void HandleMsg(Frame_ReqMovePath_Data msg)
        {
            Unit unit = BattleScene.Instance.GetUnit(msg.unitId);
            if(unit != null)
            {
                //这个移动是一帧处理的
                List<TSVector> lstPath = GameInTool.ToLstTSVector(msg.paths);
                unit.Move(lstPath,MoveFromType.Player);
            }
        }
    }
}
