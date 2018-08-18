using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using Proto;
using GameData;

namespace Game
{
    public class GameIn : StateBase
    {
        private CommandSequence m_cJoinSequence;
        private GamingSysCompContainer m_cSysCompContainer;
        private GamingLogic m_cGamingLogic;
        private ResLevel m_cLevelInfo;
        protected override void OnEnter()
        {
            FrameSyncSys.Instance.StopRun();
            BattleInfo.Clear();
            BattleInfo.userId = 1;
            BattleInfo.levelId = GameConst.Instance.GetInt("default_level_id");
            m_cLevelInfo = ResCfgSys.Instance.GetCfg<ResLevel>(BattleInfo.levelId);
            BattleInfo.sceneId = m_cLevelInfo.scene_id;
            SceneEffectPool.Instance.Clear();
            SceneGOPool.Instance.Clear();
            ViewSys.Instance.Open("LoadingView");
            m_cJoinSequence = new CommandSequence();
            var cmdConnectBattleServer = new Cmd_ConnectBattleServer();
            var cmdLoadScene = new Cmd_LoadScene();
            var cmdPreload = new Cmd_Preload();
            cmdLoadScene.On_Done += OnLoadSceneDone;
            cmdPreload.On_Done += OnPreLoad;
            m_cJoinSequence.AddSubCommand(cmdConnectBattleServer);
            m_cJoinSequence.AddSubCommand(cmdLoadScene);
            m_cJoinSequence.AddSubCommand(cmdPreload);
            m_cJoinSequence.On_Done += OnJoinScene;
            m_cJoinSequence.Execute();
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
        }

        private void OnPreLoad(CommandBase obj)
        {
            //出事化战斗场景数据
            BattleScene.Instance.Init(BattleInfo.sceneId);
            CameraSys.Instance.Init();
            m_cGamingLogic = new GamingLogic();
            m_cGamingLogic.Init(m_cLevelInfo.gaming_id);
        }

        private void OnJoinScene(CommandBase obj)
        {
            m_cJoinSequence = null;
            CLog.Log("进入场景成功");
            //初始化一些数据
            PvpPlayerMgr.Instance.Init();
            m_cSysCompContainer = new GamingSysCompContainer();
            //注册逻辑
            m_cSysCompContainer.RegisterComp(GamingSysCompType.UnitDestory, new GSC_UnitDestory());

            GlobalEventDispatcher.Instance.AddEvent(GameEvent.StartBattle, OnStartBattle);
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.PvpPlayerCreate, OnPlayerCreate);
            //向服务器发送准备完成消息
            C2S_GameReady_Data data = new C2S_GameReady_Data();
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.C2S_GameReady, data);
        }

        private void OnStartBattle(object args)
        {
            CLog.Log("开始战斗");
            FrameSyncSys.Instance.OnFirstFrameRun += OnFirstFrameRun;
            FrameSyncSys.Instance.OnFrameSyncUpdate += OnFrameSyncUpdate;
            FrameSyncSys.Instance.StartRun();//开始帧同步
        }

        private void OnFrameSyncUpdate(FP deltaTime)
        {
            if(null != m_cSysCompContainer)
            {
                m_cSysCompContainer.Update(deltaTime);
            }
            if(null != m_cGamingLogic)
            {
                if (!m_cGamingLogic.Update(deltaTime))
                {
                    //this.ParentSwitchState((int)GameStateType.GameOut);
                }
            }
        }

        private void OnFirstFrameRun(FP deltaTime)
        {
            FrameSyncSys.Instance.OnFirstFrameRun -= OnFirstFrameRun;
            //第一帧运行，帧同步开始运行
            if(null != m_cSysCompContainer)
            {
                m_cSysCompContainer.Enter();
            }
        }

        private void OnPlayerCreate(object args)
        {
            PvpPlayer player = (PvpPlayer)args;
            //初始化其他数据
         
            if (BattleInfo.userId == player.id)
            {
                player.CreateUnit();
                PvpPlayerMgr.Instance.SetMainPlayer(player);
                ViewSys.Instance.Close("LoadingView");
                ViewSys.Instance.Open("FightView");
            }
            else
            {
                player.CreateUnit();
            }
            CLog.Log("初始化其他战斗的数据");
        }

        protected override void OnExit()
        {
            CLog.Log("游戏结束");
            if(m_cSysCompContainer != null)
            {
                m_cSysCompContainer.Exit();
            }
            //停止帧同步运行
            FrameSyncSys.Instance.OnFrameSyncUpdate -= OnFrameSyncUpdate;
            ViewSys.Instance.Close("FightView");
            CameraSys.Instance.Clear();
            PvpPlayerMgr.Instance.Clear();
            BattleScene.Instance.Clear();
            SceneEffectPool.Instance.Clear();
            SceneGOPool.Instance.Clear();
            if (m_cJoinSequence != null)
            {
                m_cJoinSequence.OnDestroy();
                m_cJoinSequence = null;
            }

            //断开连接操作
            NetSys.Instance.DisConnect(NetChannelType.Game);
        }
    }
}
