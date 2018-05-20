using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework;
using GameData;

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
        void Start()
        {
            InitSingleton();
            ResCfgSys.Instance.LoadResCfgs("Config/Data", OnLoadCfgs);
        }

        private void OnLoadCfgs()
        {
            SkillCfgSys.Instance.LoadResCfgs("Config/Skill", OnLoadSkillCfgs);
        }

        private void OnLoadSkillCfgs()
        {
            RemoteCfgSys.Instance.LoadResCfgs("Config/Remote", OnLoadRemoteCfgs);
        }

        private void OnLoadRemoteCfgs()
        {
            InitUI();
            InitState();
        }

        private void InitState()
        {
            var stateData = GameStateCfg.GetConfig(versionMode, netMode);
            if (stateData == null)
            {
                CLog.LogError("没找版本模式为:" + versionMode + ",网络模式为:" + netMode + "的状态配置");
                return;
            }
            GameGlobalState = (StateContainerBase)CreateState(stateData);
            GameGlobalState._OnEnter();
        }

        private void InitUI()
        {
            List<ResUI> lst = ResCfgSys.Instance.GetCfgLst<ResUI>();
            for (int i = 0; i < lst.Count; i++)
            {
                ViewSys.Instance.RegistUIPath(lst[i].name, lst[i].prefab);
            }
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
            if (netMode == GameNetMode.Network)
            {
                NetSys.Instance.CreateChannel(NetChannelType.Game,NetChannelModeType.Tcp);
            }
            else if(netMode == GameNetMode.StandAlone)
            {
                NetSys.Instance.CreateChannel(NetChannelType.Game, NetChannelModeType.StandAlone);
                gameObject.AddComponentOnce<ClientServer>();
                ClientServer.Instance.StartServer();
            }
            gameObject.AddComponentOnce<FrameSyncSys>();
            gameObject.AddComponentOnce<ResourceSys>();
            ResourceSys.Instance.Init(true, "Assets/ResourceEx");
            gameObject.AddComponentOnce<UpdateScheduler>();
            gameObject.AddComponentOnce<TouchDispatcher>();
            GameObject goPool = new GameObject();
            goPool.name = "GameObjectPool";
            GameObject.DontDestroyOnLoad(goPool);
            goPool.AddComponentOnce<GameObjectPool>();
            GameObject uiGO = transform.Find("UIContainer").gameObject;
            uiGO.AddComponentOnce<ViewSys>();
           
        }

        void Update()
        {
            if(GameGlobalState != null)
            {
                GameGlobalState._OnUpdate();
            }

            //if (Input.GetMouseButtonUp(0))
            //{
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    RaycastHit hitInfo;
            //    if (Physics.Raycast(ray, out hitInfo))
            //    {
            //        if (null != PvpPlayerMgr.Instance.mainPlayer)
            //        {
            //            Unit unit = PvpPlayerMgr.Instance.mainPlayer.unit;
            //            if (null != unit)
            //            {
            //                //unit.ReqMove(TSVector.FromUnitVector3(hitInfo.point));
            //                var direction = TSVector.FromUnitVector3(hitInfo.point - unit.transform.position);
            //                direction.Normalize();
            //                CLog.LogColorArgs("#ff0000", direction);
            //                unit.ReqMoveForward(direction);
            //            }
            //        }
            //    }
            //}
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
