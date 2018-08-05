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
            PvpPlayerData data = new PvpPlayerData();
            data.campId = msg.campId;
            player = PvpPlayerMgr.Instance.CreatePlayer(msg.playerId,data);
            GlobalEventDispatcher.Instance.Dispatch(GameEvent.PvpPlayerCreate,player);
        }
    }
}
