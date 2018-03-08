using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework;

namespace Game
{
    public enum GameVersionMode
    {
        Release,    //正常模式
        Debug       //测试模式
    }

    public enum GameNetMode
    {
        Network,    //联网
        StandAlone, //单机
    }
    public class GameStarter  : MonoBehaviour
    {
        public GameVersionMode versionMode = GameVersionMode.Debug;
        public GameNetMode netMode = GameNetMode.StandAlone;
        public static StateContainerBase GameGlobalState { get; private set; }
        void Awake()
        {
            var stateData = GameStateCfg.GetConfig(versionMode, netMode);
            if(stateData == null)
            {
                CLog.LogError("没找版本模式为:"+versionMode+",网络模式为:"+netMode+"的状态配置");
                return;
            }
            InitSingleton();
            GameGlobalState = (StateContainerBase)CreateState(stateData);
            GameGlobalState._OnEnter();
        }

        protected StateBase CreateState(GameStateData stateData)
        {
            StateBase state = Activator.CreateInstance(stateData.mClassType) as StateBase;
            if(stateData.mSubStateData != null)
            {
                StateContainerBase stateContainer = (StateContainerBase)state;
                for (int i = 0; i < stateData.mSubStateData.Length; i++)
                {
                    var data = stateData.mSubStateData[i];
                    StateBase subState = CreateState(data);
                    stateContainer.AddState((int)data.mStateType, subState, data.mDefaultState);
                }
            }
            return state;
        }

        //初始化所有单例
        protected void InitSingleton()
        {
            gameObject.AddComponentOnce<NetSys>();
            gameObject.AddComponentOnce<FrameSyncSys>();
            gameObject.AddComponentOnce<ResourceSys>();
            ResourceSys.Instance.Init(true, "Assets/ResourceEx");
            gameObject.AddComponentOnce<UpdateScheduler>();
            gameObject.AddComponentOnce<TouchDispatcher>();
        }

        void Update()
        {
            if(GameGlobalState != null)
            {
                GameGlobalState._OnUpdate();
            }
        }

        void LateUpdate()
        {
            if (GameGlobalState != null)
            {
                GameGlobalState._OnLateUpdate();
            }
        }
    }
}
