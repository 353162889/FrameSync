using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework;
using GameData;
using System.Collections;
using System.IO;

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
        private int m_nLastWidth;
        private int m_nLastHeight;
        void Awake()
        {
            Debug.Log("GameStarter[Awake]");
            m_nLastWidth = Screen.width;
            m_nLastHeight = Screen.height;
            Application.runInBackground = true;
            Screen.orientation = ScreenOrientation.Portrait;
            Application.targetFrameRate = 200;
        }
        void Start()
        {

            Debug.Log("GameStarter[Start]");
            InitSingleton();

            StartCoroutine(UnCompressPackage());
        }

        private IEnumerator UnCompressPackage()
        {

#if !UNITY_EDITOR
            string unCompressPath = ResourceFileUtil.PersistentLoadPath;
            string streamingAssetsPath = ResourceFileUtil.FILE_SYMBOL + ResourceFileUtil.StreamingAssetsPath;
            if (!File.Exists(ResourceFileUtil.PersistentLoadPath + "versionmanifest"))
            {
                string url = streamingAssetsPath + "gameasset";
                WWW www = new WWW(url);
                yield return www;
                if(!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogError("下载资源:"+url+"失败");
                    yield break;
                }
                var bytes = www.bytes;
                Debug.Log("bytes.length:"+bytes.Length);
                try
                {
                    CompressTools.UnCompress(bytes, unCompressPath);
                }
                catch(Exception e)
                {
                    Debug.LogError("解压文件:" + url + "到" + unCompressPath +"失败\n"+e.Message+"\n"+e.StackTrace);
                    yield break;
                }
            }
#endif
            if (ResourceSys.Instance.DirectLoadMode)
            {
                LoadConfigs();
            }
            else
            {
                ResourceSys.Instance.assetBundleFile.Init("assetpath_mapping", OnLoadAssetBundleFile);
            }
            yield return null;
        }

        private void OnLoadAssetBundleFile(bool succ)
        {
            if (succ)
            {
                LoadConfigs();
            }
        }

        private void LoadConfigs()
        {
            ResCfgSys.Instance.Dispose();
            ResCfgSys.Instance.LoadResCfgs("Config/Data", OnLoadResCfg);
        }
        private int m_nConfingIndex;
        private void OnLoadResCfg()
        {
            m_nConfingIndex = 6;
            SkillCfgSys.Instance.LoadResCfgs(OnLoadOneConfig);
            RemoteCfgSys.Instance.LoadResCfgs(OnLoadOneConfig);
            HangPointCfgSys.Instance.LoadResCfgs(OnLoadOneConfig);
            GameColliderCfgSys.Instance.LoadResCfgs(OnLoadOneConfig);
            AICfgSys.Instance.LoadResCfgs(OnLoadOneConfig);
            GamingCfgSys.Instance.LoadResCfgs(OnLoadOneConfig);
        }

        private void OnLoadOneConfig()
        {
            m_nConfingIndex--;
            if(m_nConfingIndex==0)
            {
                OnLoadConfigs();
            }
        }

        private void OnLoadConfigs()
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
                ViewSys.Instance.RegistUIPath(lst[i].name, PathTool.GetBasePrefabPath(lst[i].prefab));
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
            gameObject.AddComponentOnce<ConsoleLogger>();
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
            bool directLoadMode = true;
#if !UNITY_EDITOR || BUNDLE_MODE
            directLoadMode = false;
#endif
            ResourceSys.Instance.Init(directLoadMode, "Assets/ResourceEx");
            gameObject.AddComponentOnce<UpdateScheduler>();
            gameObject.AddComponentOnce<TouchDispatcher>();
            //初始化对象池
            GameObject goPool = new GameObject();
            goPool.name = "GameObjectPool";
            GameObject.DontDestroyOnLoad(goPool);
            goPool.AddComponentOnce<ResourceObjectPool>();
            goPool.AddComponentOnce<CoreGOPool>();
            goPool.AddComponentOnce<SceneGOPool>();
            goPool.AddComponentOnce<PrefabPool>();

            //初始化特效池
            GameObject sceneEffectPool = new GameObject();
            sceneEffectPool.name = "SceneEffectPool";
            GameObject.DontDestroyOnLoad(sceneEffectPool);
            sceneEffectPool.AddComponentOnce<SceneEffectPool>();

            GameObject uiEffectPool = new GameObject();
            uiEffectPool.name = "UIEffectPool";
            GameObject.DontDestroyOnLoad(uiEffectPool);
            uiEffectPool.AddComponentOnce<UIEffectPool>();

            GameObject uiGO = transform.Find("UIContainer").gameObject;
            uiGO.AddComponentOnce<ViewSys>();

            gameObject.AddComponentOnce<AudioSys>();
            GameAudioSys.Instance.SetVolume(0.2f);

            gameObject.AddComponentOnce<FPSMono>();

            gameObject.AddComponentOnce<GizmosUtility>();

            ResetObjectPool<List<int>>.Instance.Init(10,(List<int> lst)=>{ lst.Clear(); });
            ResetObjectPool<List<TSVector>>.Instance.Init(100,(List<TSVector> lst)=> { lst.Clear(); });
            ResetObjectPool<List<Vector3>>.Instance.Init(100, (List<Vector3> lst) => { lst.Clear(); });

        }

       
        void Update()
        {
            if(GameGlobalState != null)
            {
                GameGlobalState._OnUpdate();
            }
            if(m_nLastWidth != Screen.width || m_nLastHeight != Screen.height)
            {
                GlobalEventDispatcher.Instance.Dispatch(GameEvent.ResolutionUpdate);
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
