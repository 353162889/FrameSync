using Framework;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [NetMsg]
    public class S2C_StartBattle_Msg : MsgBase<S2C_StartBattle_Data>
    {
        protected override void HandleMsg(S2C_StartBattle_Data msg)
        {
            GameInTool.InitRandomSeed(msg.seed);
            GlobalEventDispatcher.Instance.Dispatch(GameEvent.StartBattle);
        }
    }
}
