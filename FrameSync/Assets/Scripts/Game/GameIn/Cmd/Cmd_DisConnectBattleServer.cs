using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public class Cmd_DisConnectBattleServer : CommandBase
    {
        public override void Execute(ICommandContext context)
        {
            base.Execute(context);
            NetSys.Instance.DisConnect(NetChannelType.Game);
            FrameSyncSys.Instance.StopRun();
            this.OnExecuteDone(CmdExecuteState.Success);
        }
    }
}
