using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public class GSC_EnemyRefresh : IGamingSysComponent
    {
        private TSRect m_sRefreshArea;
        public void Enter()
        {
            CLog.LogColorArgs("现在是单机，所以使用camera的viewport作为随机位置的区域，后期需要更正");
            m_sRefreshArea = TSRect.FromUnityRect(CameraSys.Instance.viewPort);
            m_sRefreshArea = new TSRect(m_sRefreshArea.x, m_sRefreshArea.y + m_sRefreshArea.height / 2, m_sRefreshArea.width, m_sRefreshArea.height / 2);
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.UnitRemove, OnUnitRemove);
            for (int i = 0; i < 5; i++)
            {
                var pos = GameInTool.RandomInRect(m_sRefreshArea);
                UnitAirShip airShip = (UnitAirShip)BattleScene.Instance.CreateUnit(GameConst.Instance.GetInt("default_enemy_id"), (int)CampType.Camp2, UnitType.AirShip, pos, TSVector.back);
                if (airShip.resInfo.ai > 0)
                {
                    airShip.SetAI(airShip.resInfo.ai);
                }
                airShip.StartAI();
            }
            
        }

        private void OnUnitRemove(object args)
        {
            Unit unit = (Unit)args;
            if (unit.campId != (int)CampType.Camp2) return;
            var pos = GameInTool.RandomInRect(m_sRefreshArea);
            UnitAirShip airShip = (UnitAirShip)BattleScene.Instance.CreateUnit(unit.configId, unit.campId, unit.unitType, pos, TSVector.back);
            if (airShip.resInfo.ai > 0)
            {
                airShip.SetAI(airShip.resInfo.ai);
            }
            airShip.StartAI();
        }

        public void EnterFinish() { }

        public void Exit()
        {
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.UnitRemove, OnUnitRemove);
        }

        public void ExitFinish() { }

        public void Update(FP deltaTime)
        {
        }
    }
}
