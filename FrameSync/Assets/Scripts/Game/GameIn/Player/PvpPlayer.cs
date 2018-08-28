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

        private PvpPlayerData m_cPlayerData;

        private TSVector m_sBornPos;
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
                CreateUnit(m_sBornPos);
            }
        }

        public void SetBornPos(TSVector pos)
        {
            m_sBornPos = pos;
        }

        public Unit CreateUnit(TSVector pos)
        {
            m_sBornPos = pos;
            m_cUnit = BattleScene.Instance.CreateUnit(m_cPlayerData.configId, m_cPlayerData.campId, UnitType.AirShip, m_sBornPos, TSVector.forward);
            m_cUnit.Forbid(UnitForbidType.ForbidForward, UnitForbidFromType.Game);
            m_cUnit.StartAI();
            GlobalEventDispatcher.Instance.DispatchByParam(GameEvent.AddUnitDestory, UnitDestoryType.DieDestory, m_cUnit);
            return m_cUnit;
        }

        public void FrameUpdate(FP deltaTime)
        {
            //if(FrameSyncSys.time > 5 && m_cUnit != null && !m_cUnit.isDie && !m_cUnit.isAIRunning)
            //{
            //    m_cUnit.StartAI();
            //}
        }

        public void Clear()
        {
            GlobalEventDispatcher.Instance.RemoveEvent(GameEvent.UnitRemove, OnUnitRemove);
            m_cUnit = null;
        }
    }
}
