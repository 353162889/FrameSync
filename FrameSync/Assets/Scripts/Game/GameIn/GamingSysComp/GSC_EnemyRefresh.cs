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
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.UnitRemove, OnUnitRemove);
            for (int i = 0; i < 5; i++)
            {
                var pos = GameInTool.RandomInRect(m_sRefreshArea);
                BattleScene.Instance.CreateUnit(1, (int)CampType.Camp2, UnitType.AirShip, pos, TSVector.forward);
            }
            
        }

        private void OnUnitRemove(object args)
        {
            Unit unit = (Unit)args;
            var pos = GameInTool.RandomInRect(m_sRefreshArea);
            BattleScene.Instance.CreateUnit(unit.configId, unit.campId, unit.unitType, pos, TSVector.forward);
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
