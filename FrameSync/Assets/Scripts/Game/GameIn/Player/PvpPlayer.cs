using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class PvpPlayer
    {
        private long m_lId;
        public long id { get { return m_lId; } }

        private Unit m_cUnit;
        public Unit unit { get { return m_cUnit; } }

        public void Init(long id,PvpPlayerData playerData = null)
        {
            m_lId = id;
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.UnitRemove, OnUnitRemove);
        }

        private void OnUnitRemove(object args)
        {
            Unit unit = (Unit)args;
            if(unit == m_cUnit)
            {
                m_cUnit = null;
            }
        }

        public void CreateUnit()
        {
            m_cUnit = BattleScene.Instance.CreateUnit(1, (int)CampType.Camp1, UnitType.AirShip, TSVector.zero, TSVector.forward);
        }

        public void FrameUpdate(FP deltaTime)
        {

        }

        public void Clear()
        {
            GlobalEventDispatcher.Instance.RemoveEvent(GameEvent.UnitRemove, OnUnitRemove);
            m_cUnit = null;
        }
    }
}
