using Framework;
using Proto;
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
            if (!BattleInfo.standAlone)
            {
                NetSys.Instance.CreateChannel(NetChannelType.Game, NetChannelModeType.Tcp);
            }
            else
            {
                NetSys.Instance.CreateChannel(NetChannelType.Game, NetChannelModeType.StandAlone);
            }
            ClientServer.Instance.StartServer();
            NetSys.Instance.BeginConnect(NetChannelType.Game, BattleInfo.ip, BattleInfo.port, OnConnect);
        }

        public override void OnDestroy()
        {
            NetSys.Instance.RemoveMsgCallback(NetChannelType.Game, (short)PacketOpcode.S2C_MatchResult, OnMatchResult);
            base.OnDestroy();
        }

        private void OnConnect(bool succ, NetChannelType channel)
        {
            if(channel == NetChannelType.Game)
            {
                if(succ)
                {
                    CLog.Log("连接战斗服务器成功");
                    C2S_JoinMatch_Data joinMatch = new C2S_JoinMatch_Data();
                    NetSys.Instance.AddMsgCallback(NetChannelType.Game, (short)PacketOpcode.S2C_MatchResult, OnMatchResult, true);
                    NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.C2S_JoinMatch, (short)PacketOpcode.S2C_JoinMatchResult,joinMatch,OnJoinMatchResult);
                }
                else
                {
                    CLog.LogError("连接战斗服务器失败");
                    this.OnExecuteDone(CmdExecuteState.Fail);
                }
            }
        }

        private void OnMatchResult(object netObj)
        {
            S2C_MatchResult_Data matchResult = (S2C_MatchResult_Data)netObj;
            if(matchResult.status)
            {
                CLog.Log("匹配成功");
                this.OnExecuteDone(CmdExecuteState.Success);
            }
            else
            {
                CLog.LogError("匹配失败");
                this.OnExecuteDone(CmdExecuteState.Fail);
            }
        }

        private void OnJoinMatchResult(object netObj)
        {
            S2C_JoinMatchResult_Data joinMatchResult = (S2C_JoinMatchResult_Data)netObj;
            if (joinMatchResult.status)
            {
                CLog.Log("进入匹配成功");
            }
            else
            {
                CLog.LogError("进入匹配失败");
                this.OnExecuteDone(CmdExecuteState.Fail);
            }
            
        }
    }
}
