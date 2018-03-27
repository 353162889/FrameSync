using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using Proto;

namespace Game
{
    [NetMsg]
    public class Frame_Msg_Test_Msg : MsgBase<Frame_Msg_Test_Data>
    {
        protected override void HandleMsg(Frame_Msg_Test_Data msg)
        {
            CLog.Log("Frame_Msg_Test_Msg:msg="+msg.msg+",frameIndex="+FrameSyncSys.frameIndex + ",frameTime="+FrameSyncSys.time);

        }
    }
}
