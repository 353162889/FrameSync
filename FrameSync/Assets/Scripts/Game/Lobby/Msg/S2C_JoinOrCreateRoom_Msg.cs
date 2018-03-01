using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using Proto;

namespace Game
{
    [NetMsg]
    public class S2C_JoinOrCreateRoom_Msg : MsgBase<S2C_JoinOrCreateRoom_Data>
    {
        protected override void HandleMsg(S2C_JoinOrCreateRoom_Data msg)
        {
            CLog.Log("S2C_JoinOrCreateRoom_Msg,status="+msg.status+",roomId="+msg.roomId);
        }
    }
}
