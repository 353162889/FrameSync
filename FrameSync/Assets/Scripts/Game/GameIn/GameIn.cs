﻿using System;
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
            NetSys.Instance.OnNetChannelDisConnect += OnNetChannelDisConnect;
            m_cLevelInfo = ResCfgSys.Instance.GetCfg<ResLevel>(BattleInfo.levelId);
            FrameSyncSys.Instance.StopRun();
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

        private void OnNetChannelDisConnect(NetChannelType obj)
        {
            CommonUtil.ShowPopup("服务器断开连接，返回到主界面", "提示", () =>
            {
                this.ParentSwitchState((int)GameStateType.GameOut);
            }, () =>
            {
                this.ParentSwitchState((int)GameStateType.GameOut);
            });
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
            CameraSys.Instance.Init(BattleScene.Instance.viewRect.ToUnityRect());
        }

        private void OnJoinScene(CommandBase obj)
        {
            if (obj.State == CmdExecuteState.Success)
            {
                m_cJoinSequence = null;
                CLog.Log("进入场景成功");
                //初始化一些数据
                PvpPlayerMgr.Instance.Init();
                m_cSysCompContainer = new GamingSysCompContainer();
                //注册逻辑
                m_cSysCompContainer.RegisterComp(GamingSysCompType.UnitDestory, new GSC_UnitDestory());
                m_cSysCompContainer.RegisterComp(GamingSysCompType.UnitDieEffect, new GSC_UnitDieEffect());
                m_cSysCompContainer.RegisterComp(GamingSysCompType.UnitHitItem, new GSC_UnitHitItem());
                m_cSysCompContainer.RegisterComp(GamingSysCompType.UnitDieDropItem, new GSC_UnitDieDropItem());

                GlobalEventDispatcher.Instance.AddEvent(GameEvent.StartBattle, OnStartBattle);
                GlobalEventDispatcher.Instance.AddEvent(GameEvent.PvpPlayerCreate, OnPlayerCreate);
                //向服务器发送准备完成消息
                C2S_GameReady_Data data = new C2S_GameReady_Data();
                NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.C2S_GameReady, data);
            }
            else
            {
                CommonUtil.ShowPopup("进入游戏失败，返回到主界面", "提示", () =>
                {
                    this.ParentSwitchState((int)GameStateType.GameOut);
                }, () =>
                {
                    this.ParentSwitchState((int)GameStateType.GameOut);
                });
            }
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
                bool preIsDo = m_cGamingLogic.isDo;
                if (!m_cGamingLogic.Update(deltaTime))
                {
                    if (preIsDo != m_cGamingLogic.isDo)
                    {
                        this.ParentSwitchState((int)GameStateType.GameOut);
                    }
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
            m_cGamingLogic = new GamingLogic();
            m_cGamingLogic.Init(m_cLevelInfo.logic_path);
        }

        private void OnPlayerCreate(object args)
        {
            PvpPlayer player = (PvpPlayer)args;
            //初始化其他数据
         
            if (BattleInfo.userId == player.id)
            {
                //player.CreateUnit(TSVector.zero);
                PvpPlayerMgr.Instance.SetMainPlayer(player);
                ViewSys.Instance.Close("LoadingView");
                ViewSys.Instance.Open("FightView");
            }
            else
            {
                //player.CreateUnit(TSVector.zero);
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
            AudioSys.Instance.StopAll();
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
            NetSys.Instance.OnNetChannelDisConnect -= OnNetChannelDisConnect;
            //断开连接操作
            NetSys.Instance.DisConnect(NetChannelType.Game);
        }
    }
}
