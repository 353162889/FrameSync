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
        Item,//可被吃的东西
    }

    public enum CampType
    {
        Camp1 = 0,
        Camp2,
        CampMax,
    }

    public class DamageInfo : IPoolable
    {
        public AgentObject attack;
        public AgentObject defence;
        public FP damage;

        public void Reset()
        {
            attack = null;
            defence = null;
            damage = 0;
        }

        public void CloneFrom(DamageInfo damageInfo)
        {
            attack = damageInfo.attack;
            defence = damageInfo.defence;
            damage = damageInfo.damage;
        }
    }

    public class BattleScene : Singleton<BattleScene>
    {
        private int m_nSceneId;
        public int sceneId { get { return m_nSceneId; } }

        private ResScene m_cResScene;
        public ResScene resSceneInfo { get { return m_cResScene; } }
        private TSRect m_sViewRect;
        public TSRect viewRect { get { return m_sViewRect; } }

        private GameObject m_cUnitRoot;
        public GameObject unitRoot { get { return m_cUnitRoot; } }

        private Dictionary<uint, Unit> m_dicUnit;
        private Dictionary<int, List<Unit>> m_dicCampUnits;
        public Dictionary<int, List<Unit>> dicCampUnits { get { return m_dicCampUnits; } }
        private DynamicContainer m_cUnitContainer;
        public List<UnitItem> lstItem { get { return m_lstItem;} }
        private List<UnitItem> m_lstItem;

        private GameObject m_cRemoteRoot;
        public GameObject remoteRoot { get { return m_cRemoteRoot; } }

        private Dictionary<uint, Remote> m_dicRemote;
        private Dictionary<int, List<Remote>> m_dicCampRemotes;
        public Dictionary<int, List<Remote>> dicCampRemotes { get { return m_dicCampRemotes; } }
        private DynamicContainer m_cRemoteContainer;

        private Dictionary<int, AgentObjField> m_dicCampUnitField;
        private Dictionary<int, AgentObjField> m_dicCampRemoteField;


        public void Init(int sceneId)
        {
            m_nSceneId = sceneId;
            m_cResScene = ResCfgSys.Instance.GetCfg<ResScene>(m_nSceneId);
            if(m_cResScene == null)
            {
                CLog.LogError("找不到sceneId="+m_nSceneId+"的场景配置");
                return;
            }
            m_sViewRect = new TSRect(-m_cResScene.view_width / 2, -m_cResScene.view_height / 2, m_cResScene.view_width, m_cResScene.view_height);
            m_cUnitRoot = new GameObject("UnitRoot");
            m_cUnitRoot.name = "UnitRoot";
            GameObject.DontDestroyOnLoad(m_cUnitRoot);

            m_cRemoteRoot = new GameObject("RemoteRoot");
            m_cRemoteRoot.name = "RemoteRoot";
            GameObject.DontDestroyOnLoad(m_cRemoteRoot);

            m_dicUnit = new Dictionary<uint, Unit>();
            m_dicCampUnits = new Dictionary<int, List<Unit>>();
            m_lstItem = new List<UnitItem>();
            m_cUnitContainer = new DynamicContainer();
            m_cUnitContainer.OnAdd += OnAddUnit;
            m_cUnitContainer.OnRemove += OnRemoveUnit;
            m_cUnitContainer.OnUpdate += OnUpdateUnit;

            m_dicRemote = new Dictionary<uint, Remote>();
            m_dicCampRemotes = new Dictionary<int, List<Remote>>();
            m_cRemoteContainer = new DynamicContainer();
            m_cRemoteContainer.OnAdd += OnAddRemote;
            m_cRemoteContainer.OnRemove += OnRemoveRemote;
            m_cRemoteContainer.OnUpdate += OnUpdateRemote;

            int campMax = (int)CampType.CampMax;
            m_dicCampUnitField = new Dictionary<int, AgentObjField>();
            for (int i = 0; i < campMax; i++)
            {
                AgentObjField agentObjField = new AgentObjField();
                agentObjField.Init(i, (int)AgentObjectType.Unit);
                m_dicCampUnitField.Add(i, agentObjField);
            }

            m_dicCampRemoteField = new Dictionary<int, AgentObjField>();
            for (int i = 0; i < campMax; i++)
            {
                AgentObjField agentObjField = new AgentObjField();
                agentObjField.Init(i, (int)AgentObjectType.Remote);
                m_dicCampRemoteField.Add(i, agentObjField);
            }

            BehaviourPool<UnitAirShip>.Instance.Init(30);
            BehaviourPool<UnitItem>.Instance.Init(50);
            BehaviourPool<Remote>.Instance.Init(300);
            ObjectPool<GameCollider>.Instance.Init(200);
            ObjectPool<DamageInfo>.Instance.Init(20);

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

        public AgentObjField GetField(int campId,AgentObjectType agentObjType)
        {
            if(agentObjType == AgentObjectType.Unit)
            {
                return m_dicCampUnitField[campId];
            }
            else if(agentObjType == AgentObjectType.Remote)
            {
                return m_dicCampRemoteField[campId];
            }
            return null;
        }

        private void OnFrameSyncUpdate(FP deltaTime)
        {
            foreach (KeyValuePair<int,AgentObjField> item in m_dicCampUnitField)
            {
                item.Value.UpdateField(deltaTime);
            }

            foreach (KeyValuePair<int, AgentObjField> item in m_dicCampRemoteField)
            {
                item.Value.UpdateField(deltaTime);
            }

            m_cUnitContainer.Update(deltaTime);
            m_cRemoteContainer.Update(deltaTime);
        }

        private void OnAddUnit(IDynamicObj obj, object param)
        {
            Unit unit = (Unit)obj;
            m_dicUnit.Add(unit.id, unit);
            if (unit.unitType == UnitType.Item) m_lstItem.Add((UnitItem)unit);
            List<Unit> lst;
            m_dicCampUnits.TryGetValue(unit.campId, out lst);
            if(lst == null)
            {
                lst = new List<Unit>();
                m_dicCampUnits.Add(unit.campId, lst);
            }
            lst.Add(unit);
            GlobalEventDispatcher.Instance.Dispatch(GameEvent.UnitAdd, unit);
        }

        private void OnRemoveUnit(IDynamicObj obj, object param)
        {
            Unit unit = (Unit)obj;
            m_dicUnit.Remove(unit.id);
            if (unit.unitType == UnitType.Item) m_lstItem.Remove((UnitItem)unit);
            List<Unit> lst;
            if(m_dicCampUnits.TryGetValue(unit.campId, out lst))
            {
                lst.Remove(unit);
            }
            switch (unit.unitType)
            {
                case UnitType.AirShip:
                    BehaviourPool<UnitAirShip>.Instance.SaveObject((UnitAirShip)unit);
                    break;
                case UnitType.Item:
                    BehaviourPool<UnitItem>.Instance.SaveObject((UnitItem)unit);
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
            List<Remote> lst;
            m_dicCampRemotes.TryGetValue(remote.campId, out lst);
            if (lst == null)
            {
                lst = new List<Remote>();
                m_dicCampRemotes.Add(remote.campId, lst);
            }
            m_dicRemote.Add(remote.id, remote);
        }

        private void OnRemoveRemote(IDynamicObj obj, object param)
        {
            Remote remote = (Remote)obj;
            m_dicRemote.Remove(remote.id);
            List<Remote> lst;
            if (m_dicCampRemotes.TryGetValue(remote.campId, out lst))
            {
                lst.Remove(remote);
            }
            BehaviourPool<Remote>.Instance.SaveObject(remote);
        }

        private void OnUpdateRemote(IDynamicObj obj, object param)
        {
            Remote remote = (Remote)obj;
            remote.OnUpdate((FP)param);
            if(!viewRect.Contains(remote.curPosition.x,remote.curPosition.z))
            {
                remote.End();
            }
        }

        public Unit CreateUnit(int configId,int campId, UnitType type, TSVector bornPosition, TSVector bornForward)
        {
            Unit unit = null;
            switch(type)
            {
                case UnitType.AirShip:
                    unit = BehaviourPool<UnitAirShip>.Instance.GetObject(m_cUnitRoot.transform);
                    break;
                case UnitType.Item:
                    unit = BehaviourPool<UnitItem>.Instance.GetObject(m_cUnitRoot.transform);
                    break;
            }

            if(unit != null)
            {
                uint unitId = GameInTool.GenerateUnitId();
				unit.name = "unit_"+ type + "_"+unitId;
                unit.Init(unitId, configId, campId, type, bornPosition, bornForward);
                m_cUnitContainer.Add(unit);
            }
            return unit;
        }

        public void DestroyUnit(Unit unit)
        {
            m_cUnitContainer.Remove(unit);
        }

        public Remote CreateRemote(int configId,int campId, TSVector position, TSVector forward, uint targetAgentId, AgentObjectType targetAgentType, TSVector targetPosition, TSVector targetForward)
        {
            Remote remote = BehaviourPool<Remote>.Instance.GetObject(m_cRemoteRoot.transform);
            uint remoteId = GameInTool.GenerateRemoteId();
            remote.Init(remoteId, configId, campId, position, forward, targetAgentId, targetAgentType, targetPosition, targetForward);
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
            foreach (var item in m_dicCampUnitField)
            {
                item.Value.Clear();
            }
            m_dicCampUnitField.Clear();
            foreach (var item in m_dicCampRemoteField)
            {
                item.Value.Clear();
            }
            m_dicCampRemoteField.Clear();
            m_dicCampUnits.Clear();
            m_dicCampRemotes.Clear();
            m_dicUnit.Clear();
            m_lstItem.Clear();
            m_dicRemote.Clear();
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
