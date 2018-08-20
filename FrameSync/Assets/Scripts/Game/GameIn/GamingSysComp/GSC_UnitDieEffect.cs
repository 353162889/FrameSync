﻿using System;
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
            Unit unit = (Unit)args;
            if(unit.unitType == UnitType.AirShip)
            {
                UnitAirShip airShip = (UnitAirShip)unit;
                var effect = SceneEffectPool.Instance.CreateEffect("eff_blast0", true, null);
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
