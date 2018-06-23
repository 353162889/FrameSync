using Framework;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{

    public enum UnitType
    {
        AirShip,//飞船
    }

    public class BattleScene : Singleton<BattleScene>
    {
        private int m_nSceneId;
        public int sceneId { get { return m_nSceneId; } }

        private ResScene m_cResScene;
        public ResScene resSceneInfo { get { return m_cResScene; } }

        private GameObject m_cUnitRoot;
        public GameObject unitRoot { get { return m_cUnitRoot; } }

        private Dictionary<uint, Unit> m_dicUnit;
        private DynamicContainer m_cUnitContainer;

        private GameObject m_cRemoteRoot;
        public GameObject remoteRoot { get { return m_cRemoteRoot; } }

        private Dictionary<uint, Remote> m_dicRemote;
        private DynamicContainer m_cRemoteContainer;
       

        public void Init(int sceneId)
        {
            m_nSceneId = sceneId;
            m_cResScene = ResCfgSys.Instance.GetCfg<ResScene>(m_nSceneId);
            if(m_cResScene == null)
            {
                CLog.LogError("找不到sceneId="+m_nSceneId+"的场景配置");
                return;
            }
            m_cUnitRoot = new GameObject("UnitRoot");
            m_cUnitRoot.name = "UnitRoot";
            GameObject.DontDestroyOnLoad(m_cUnitRoot);

            m_cRemoteRoot = new GameObject("RemoteRoot");
            m_cRemoteRoot.name = "RemoteRoot";
            GameObject.DontDestroyOnLoad(m_cRemoteRoot);

            m_dicUnit = new Dictionary<uint, Unit>();
            m_cUnitContainer = new DynamicContainer();
            m_cUnitContainer.OnAdd += OnAddUnit;
            m_cUnitContainer.OnRemove += OnRemoveUnit;
            m_cUnitContainer.OnUpdate += OnUpdateUnit;

            m_dicRemote = new Dictionary<uint, Remote>();
            m_cRemoteContainer = new DynamicContainer();
            m_cRemoteContainer.OnAdd += OnAddRemote;
            m_cRemoteContainer.OnRemove += OnRemoveRemote;
            m_cRemoteContainer.OnUpdate += OnUpdateRemote;

            BehaviourPool<UnitAirShip>.Instance.Init(30);
            BehaviourPool<Remote>.Instance.Init(100);

            FrameSyncSys.Instance.OnFrameSyncUpdate += OnFrameSyncUpdate;


        }

        public Unit GetUnit(uint id)
        {
            Unit unit = null;
            m_dicUnit.TryGetValue(id, out unit);
            return unit;
        }

        public Remote GetRemote(uint id)
        {
            Remote remote = null;
            m_dicRemote.TryGetValue(id, out remote);
            return remote;
        }

        private void OnFrameSyncUpdate(FP deltaTime)
        {
            m_cUnitContainer.Update(deltaTime);
            m_cRemoteContainer.Update(deltaTime);
        }

        private void OnAddUnit(IDynamicObj obj, object param)
        {
            Unit unit = (Unit)obj;
            m_dicUnit.Add(unit.id, unit);
            GlobalEventDispatcher.Instance.Dispatch(GameEvent.UnitAdd, unit);
        }

        private void OnRemoveUnit(IDynamicObj obj, object param)
        {
            Unit unit = (Unit)obj;
            m_dicUnit.Remove(unit.id);
            switch (unit.unitType)
            {
                case UnitType.AirShip:
                    BehaviourPool<UnitAirShip>.Instance.SaveObject((UnitAirShip)unit);
                    break;
            }
            GlobalEventDispatcher.Instance.Dispatch(GameEvent.UnitRemove, unit);
        }

        private void OnUpdateUnit(IDynamicObj obj, object param)
        {
            Unit unit = (Unit)obj;
            unit.OnUpdate((FP)param);
        }

        private void OnAddRemote(IDynamicObj obj, object param)
        {
            Remote remote = (Remote)obj;
            m_dicRemote.Add(remote.id, remote);
        }

        private void OnRemoveRemote(IDynamicObj obj, object param)
        {
            Remote remote = (Remote)obj;
            m_dicRemote.Remove(remote.id);
            BehaviourPool<Remote>.Instance.SaveObject(remote);
        }

        private void OnUpdateRemote(IDynamicObj obj, object param)
        {
            Remote remote = (Remote)obj;
            remote.OnUpdate((FP)param);
        }

        public Unit CreateUnit(int configId,UnitType type, TSVector bornPosition, TSVector bornForward)
        {
            Unit unit = null;
            switch(type)
            {
                case UnitType.AirShip:
                    unit = BehaviourPool<UnitAirShip>.Instance.GetObject(m_cUnitRoot.transform);
                    break;
            }

            if(unit != null)
            {
                uint unitId = GameInTool.GenerateUnitId();
                unit.name = "unit_"+ type + "_"+unitId;
                unit.Init(unitId, configId, type, bornPosition, bornForward);
                m_cUnitContainer.Add(unit);
            }
            return unit;
        }

        public void DestroyUnit(Unit unit)
        {
            m_cUnitContainer.Remove(unit);
        }

        public Remote CreateRemote(int configId, TSVector position, TSVector forward, uint targetAgentId, AgentObjectType targetAgentType, TSVector targetPosition, TSVector targetForward)
        {
            Remote remote = BehaviourPool<Remote>.Instance.GetObject(m_cRemoteRoot.transform);
            uint remoteId = GameInTool.GenerateRemoteId();
            remote.Init(remoteId, configId, position, forward, targetAgentId, targetAgentType, targetPosition, targetForward);
            m_cRemoteContainer.Add(remote);
            return remote;
        }

        public void DestroyRemote(Remote remote)
        {
            m_cRemoteContainer.Remove(remote);
        }

        public void Clear()
        {
            m_nSceneId = -1;
            m_cResScene = null;
            BehaviourPool<UnitAirShip>.Instance.Dispose();
            BehaviourPool<Remote>.Instance.Dispose();
            FrameSyncSys.Instance.OnFrameSyncUpdate -= OnFrameSyncUpdate;
            if (m_cUnitContainer != null)
            {
                m_cUnitContainer.OnAdd -= OnAddUnit;
                m_cUnitContainer.OnRemove -= OnRemoveUnit;
                m_cUnitContainer.OnUpdate -= OnUpdateUnit;
                m_cUnitContainer.Clear();
                m_cUnitContainer = null;
            }
            if(m_cUnitRoot != null )
            {
                GameObject.Destroy(m_cUnitRoot);
                m_cUnitRoot = null;
            }
            if(m_cRemoteContainer != null)
            {
                m_cRemoteContainer.OnAdd -= OnAddRemote;
                m_cRemoteContainer.OnRemove -= OnRemoveRemote;
                m_cRemoteContainer.OnUpdate -= OnUpdateRemote;
                m_cRemoteContainer.Clear();
                m_cRemoteContainer = null;
            }
            if(m_cRemoteRoot != null)
            {
                GameObject.Destroy(m_cRemoteRoot);
                m_cRemoteRoot = null;
            }
        }
    }
}
