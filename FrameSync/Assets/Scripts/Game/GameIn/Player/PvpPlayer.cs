using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class PvpPlayer
    {
        private static TSVector bornPos = TSVector.zero;
        private long m_lId;
        public long id { get { return m_lId; } }

        private Unit m_cUnit;
        public Unit unit { get { return m_cUnit; } }

        private PvpPlayerData m_cPlayerData;

        private bool m_bIsDie;

        public void Init(long id,PvpPlayerData playerData = null)
        {
            m_lId = id;
            m_cPlayerData = playerData;
            m_bIsDie = true;
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.UnitRemove, OnUnitRemove);
        }

        private void OnUnitRemove(object args)
        {
            Unit unit = (Unit)args;
            if(unit == m_cUnit)
            {
                m_cUnit = null;
                CreateUnit();
            }
        }

        public void CreateUnit()
        {
            m_cUnit = BattleScene.Instance.CreateUnit(GameConst.Instance.GetInt("default_player_id"), m_cPlayerData.campId, UnitType.AirShip, bornPos, TSVector.forward);
            m_cUnit.Forbid(UnitForbidType.ForbidForward, UnitForbidFromType.Game);
            int aiId = ((UnitAirShip)m_cUnit).resInfo.ai;
            if (aiId > 0)
            {
                m_cUnit.SetAI(aiId);
                m_cUnit.StartAI();
            }
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
