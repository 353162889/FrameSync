using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proto;
using UnityEngine;

namespace Game
{
    //客户端模拟的服务器，用来做单机的一些处理
    public class ClientServer : SingletonMonoBehaviour<ClientServer>
    {
        private bool m_bIsRun = false;
        public bool isRun { get { return m_bIsRun; } }
        public void StartServer()
        {
            m_bIsRun = true;
            //监听进入战斗消息，给客户端发送初始化玩家的消息
            NetSys.Instance.AddMsgCallback(NetChannelType.Game, (short)PacketOpcode.C2S_GameReady, OnClientGameReady);
            NetSys.Instance.AddMsgCallback(NetChannelType.Game, (short)PacketOpcode.C2S_JoinMatch, OnJoinMatch);
        }
      
        public void StopServer()
        {
            m_bIsRun = false;
            NetSys.Instance.RemoveMsgCallback(NetChannelType.Game, (short)PacketOpcode.C2S_JoinMatch, OnJoinMatch);
            NetSys.Instance.RemoveMsgCallback(NetChannelType.Game, (short)PacketOpcode.C2S_GameReady, OnClientGameReady);
        }

        private void OnJoinMatch(object netObj)
        {
            S2C_JoinMatchResult_Data joinMatchResult = new S2C_JoinMatchResult_Data();
            joinMatchResult.status = true;
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.S2C_JoinMatchResult, joinMatchResult);
            S2C_MatchResult_Data matchResult = new S2C_MatchResult_Data();
            matchResult.status = true;
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.S2C_MatchResult, matchResult);
        }


        private void OnClientGameReady(object netObj)
        {
            //开始帧同步
            var channel = NetSys.Instance.GetChannel(NetChannelType.Game);
            ((StandAloneSocketClient)channel.socketClient).StartFrameSync();

            //先发送开始战斗协议
            S2C_StartBattle_Data startBattleData = new S2C_StartBattle_Data();
            startBattleData.seed = UnityEngine.Random.Range(0, int.MaxValue);
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.S2C_StartBattle, startBattleData);
            //创建玩家
            Frame_CreatePlayer_Data createPlayerData = new Frame_CreatePlayer_Data();
            createPlayerData.playerId = 1;
            createPlayerData.campId = (int)CampType.Camp1;
            createPlayerData.configId = 9001;
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_CreatePlayer, createPlayerData);

            //Frame_CreatePlayer_Data createOtherPlayerData = new Frame_CreatePlayer_Data();
            //createOtherPlayerData.playerId = 2;
            //NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_CreatePlayer, createOtherPlayerData);
        }

    }
}
