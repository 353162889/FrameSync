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
            m_dicUnit = new Dictionary<uint, Unit>();
            m_cUnitContainer = new DynamicContainer();
            m_cUnitContainer.OnAdd += OnAddUnit;
            m_cUnitContainer.OnRemove += OnRemoveUnit;
            m_cUnitContainer.OnUpdate += OnUpdateUnit;
            FrameSyncSys.Instance.OnFrameSyncUpdate += OnFrameSyncUpdate;
        }

        public Unit GetUnit(uint id)
        {
            Unit unit = null;
            m_dicUnit.TryGetValue(id, out unit);
            return unit;
        }

        private void OnFrameSyncUpdate(FP deltaTime)
        {
            m_cUnitContainer.Update(deltaTime);
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
            GlobalEventDispatcher.Instance.Dispatch(GameEvent.UnitRemove, unit);
        }

        private void OnUpdateUnit(IDynamicObj obj, object param)
        {
            Unit unit = (Unit)obj;
            unit.OnUpdate((FP)param);
        }

        public Unit CreateUnit(int configId,UnitType type, TSVector bornPosition, TSVector bornForward)
        {
            Unit unit = null;
            switch(type)
            {
                case UnitType.AirShip:
                    GameObject go = new GameObject();
                    m_cUnitRoot.AddChildToParent(go);
                    unit = go.AddComponentOnce<UnitAirShip>();
                    break;
            }

            if(unit != null)
            {
                uint unitId = GameInTool.GenerateUnitId();
                unit.Init(unitId, configId, type, bornPosition, bornForward);
                m_cUnitContainer.Add(unit);
            }
            return unit;
        }

        public void DestroyUnit(Unit unit)
        {
            m_cUnitContainer.Remove(unit);
        }

        public void Clear()
        {
            m_nSceneId = -1;
            m_cResScene = null;
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
        }
    }
}
