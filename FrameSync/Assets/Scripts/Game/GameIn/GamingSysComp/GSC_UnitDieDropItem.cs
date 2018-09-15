using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public class GSC_UnitDieDropItem : IGamingSysComponent
    {
        public void Enter()
        {
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.UnitDie, OnUnitDie);
        }

        private void OnUnitDie(object args)
        {
            DamageInfo damageInfo = (DamageInfo)args;
            Unit unit = (Unit)damageInfo.defence.agent;
            if (unit.unitType == UnitType.AirShip && unit.campId != (int)CampType.Camp1)
            {
                UnitAirShip airShip = (UnitAirShip)unit;
                if(airShip.resInfo.die_drop_ids.Count > 0 && airShip.resInfo.die_drop_rate > 0)
                {
                    if(GameInTool.Random(FP.FromFloat(100f)) < airShip.resInfo.die_drop_rate)
                    {
                        FP totalWeight = 0;
                        for (int i = 0; i < airShip.resInfo.die_drop_weights.Count; i++)
                        {
                            totalWeight += airShip.resInfo.die_drop_weights[i];
                        }
                        FP targetWeight = GameInTool.Random(totalWeight);
                        FP curWeight = 0;
                        int index = 0;
                        for (int i = 0; i < airShip.resInfo.die_drop_weights.Count; i++)
                        {
                            curWeight += airShip.resInfo.die_drop_weights[i];
                            if(targetWeight < curWeight)
                            {
                                index = i;
                                break;
                            }
                        }
                        int itemId = airShip.resInfo.die_drop_ids[index];
                        Unit item = BattleScene.Instance.CreateUnit(itemId, (int)CampType.Camp1, UnitType.Item, airShip.curPosition, TSVector.forward);
                        item.Forbid(UnitForbidType.ForbidForward, UnitForbidFromType.Game);
                        item.StartAI();
                        GlobalEventDispatcher.Instance.DispatchByParam(GameEvent.AddUnitDestory, UnitDestoryType.AIFinishDestory, item);
                    }
                }
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
