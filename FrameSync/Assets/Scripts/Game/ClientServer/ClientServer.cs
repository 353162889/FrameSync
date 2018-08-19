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
        }

        public void StopServer()
        {
            m_bIsRun = false;
            NetSys.Instance.RemoveMsgCallback(NetChannelType.Game, (short)PacketOpcode.C2S_GameReady, OnClientGameReady);
        }

        private void OnClientGameReady(object netObj)
        {
            //先发送开始战斗协议
            S2C_StartBattle_Data startBattleData = new S2C_StartBattle_Data();
            startBattleData.seed = UnityEngine.Random.Range(0, int.MaxValue);
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.S2C_StartBattle, startBattleData);
            //创建玩家
            Frame_CreatePlayer_Data createPlayerData = new Frame_CreatePlayer_Data();
            createPlayerData.playerId = 1;
            createPlayerData.campId = (int)CampType.Camp1;
            createPlayerData.configId = 3;
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_CreatePlayer, createPlayerData);

            //Frame_CreatePlayer_Data createOtherPlayerData = new Frame_CreatePlayer_Data();
            //createOtherPlayerData.playerId = 2;
            //NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_CreatePlayer, createOtherPlayerData);
        }

    }
}
