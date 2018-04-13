using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using Proto;

namespace Game
{
    public class GameIn : StateBase
    {
        private CommandSequence m_cJoinSequence;
        protected override void OnEnter()
        {
            ViewSys.Instance.Open("LoadingView");
            m_cJoinSequence = new CommandSequence();
            var cmdConnectBattleServer = new Cmd_ConnectBattleServer();
            var cmdLoadScene = new Cmd_LoadScene();
            cmdLoadScene.On_Done += OnLoadSceneDone;
            m_cJoinSequence.AddSubCommand(cmdConnectBattleServer);
            m_cJoinSequence.AddSubCommand(cmdLoadScene);
            m_cJoinSequence.On_Done += OnJoinScene;
            GameInContext context = new GameInContext();
            context.sceneId = GameConst.Instance.GetInt("default_scene_id");
            m_cJoinSequence.Execute(context);
        }

        protected override void OnUpdate()
        {
            if(m_cJoinSequence != null)
            {
                m_cJoinSequence.OnUpdate();
            }
        }

        private void OnLoadSceneDone(CommandBase obj)
        {
            //出事化战斗场景数据
            Cmd_LoadScene cmdScene = obj as Cmd_LoadScene;
            BattleScene.Instance.Init(cmdScene.sceneId);
        }

        private void OnJoinScene(CommandBase obj)
        {
            m_cJoinSequence = null;
            CLog.Log("进入场景成功");
            //初始化一些数据
            PvpPlayerMgr.Instance.Init();

            GlobalEventDispatcher.Instance.AddEvent(GameEvent.StartBattle, OnStartBattle);
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.PvpPlayerCreate, OnPlayerCreate);
            //向服务器发送准备完成消息
            C2S_GameReady_Data data = new C2S_GameReady_Data();
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.C2S_GameReady, data);
        }

        private void OnStartBattle(object args)
        {
            CLog.Log("开始战斗");
        }

        private void OnPlayerCreate(object args)
        {
            PvpPlayer player = (PvpPlayer)args;
            PvpPlayerMgr.Instance.SetMainPlayer(player);
            CLog.Log("初始化其他战斗的数据");
            //初始化其他数据
            player.CreateUnit();
            ViewSys.Instance.Close("LoadingView");
            ViewSys.Instance.Open("FightView");
        }

        protected override void OnExit()
        {
            ViewSys.Instance.Close("FightView");
            PvpPlayerMgr.Instance.Clear();
            if (m_cJoinSequence != null)
            {
                m_cJoinSequence.OnDestroy();
                m_cJoinSequence = null;
            }

            //断开连接操作
        }
    }
}
