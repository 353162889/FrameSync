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
            NetChannel netChannel = null;
            if (!BattleInfo.standAlone)
            {
                netChannel = NetSys.Instance.CreateChannel(NetChannelType.Game, NetChannelModeType.Tcp);
            }
            else
            {
                netChannel = NetSys.Instance.CreateChannel(NetChannelType.Game, NetChannelModeType.StandAlone);
            }
            netChannel.InitHeartBeat((short)PacketOpcode.C2S_HeartBeat, (short)PacketOpcode.S2C_HeartBeat);
            netChannel.SetHeartBeatSpaceTime(1000d);
            netChannel.SetHeartBeatEnable(true);
            ClientServer.Instance.StartServer();
            NetSys.Instance.AddMsgCallback(NetChannelType.Game, (short)PacketOpcode.S2C_MatchResult, OnMatchResult);
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
                    joinMatch.matchCount = BattleInfo.matchCount;
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
                GlobalEventDispatcher.Instance.Dispatch(GameEvent.StartMatchOther);
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
