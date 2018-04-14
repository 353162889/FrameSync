using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using Proto;

namespace Game
{
    [NetMsg]
    public class Frame_CreatePlayer_Msg : MsgBase<Frame_CreatePlayer_Data>
    {
        protected override void HandleMsg(Frame_CreatePlayer_Data msg)
        {
            CLog.Log("创建玩家:"+msg.playerId);
            PvpPlayer player = PvpPlayerMgr.Instance.GetPlayer(msg.playerId);
            if(player != null)
            {
                CLog.LogError("已存在id="+msg.playerId+"的玩家");
                return; 
            }
            player = PvpPlayerMgr.Instance.CreatePlayer(msg.playerId);
            GlobalEventDispatcher.Instance.Dispatch(GameEvent.PvpPlayerCreate,player);
        }
    }
}
