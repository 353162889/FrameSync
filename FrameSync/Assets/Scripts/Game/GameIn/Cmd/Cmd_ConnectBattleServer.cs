using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class Cmd_ConnectBattleServer : CommandBase
    {
        public override void Execute(ICommandContext context)
        {
            base.Execute(context);
            NetSys.Instance.BeginConnect(NetChannelType.Game, "127.0.0.1", 8080, OnConnect);
        }

        private void OnConnect(bool succ, NetChannelType channel)
        {
            if(channel == NetChannelType.Game)
            {
                if(succ)
                {
                    CLog.Log("连接战斗服务器成功");
                    FrameSyncSys.Instance.StartRun();//开始帧同步
                    this.OnExecuteDone(CmdExecuteState.Success);
                }
                else
                {
                    CLog.LogError("连接战斗服务器失败");
                    this.OnExecuteDone(CmdExecuteState.Fail);
                }
            }
        }
    }
}
