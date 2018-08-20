using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public class GSC_UnitDieEffect : IGamingSysComponent
    {
        public void Enter()
        {
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.UnitDie, OnUnitDie);
        }

        private void OnUnitDie(object args)
        {
            DamageInfo damageInfo = (DamageInfo)args;
            Unit unit = (Unit)damageInfo.defence.agent;
            if(unit.unitType == UnitType.AirShip)
            {
                UnitAirShip airShip = (UnitAirShip)unit;
                var effect = SceneEffectPool.Instance.CreateEffect(airShip.resInfo.die_effect, true, null);
                effect.transform.position = unit.curPosition.ToUnityVector3();
            }
        }

        public void EnterFinish()
        {
        }

        public void Exit()
        {
            GlobalEventDispatcher.Instance.RemoveEvent(GameEvent.UnitDie, OnUnitDie);
        }

        public void ExitFinish()
        {
        }

        public void Update(FP deltaTime)
        {
        }
    }
}
