using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public class GSC_UnitHitItem : IGamingSysComponent
    {
        private List<UnitItem> m_lstRemoveUnit;
        public void Enter()
        {
            m_lstRemoveUnit = new List<UnitItem>();
        }

        public void EnterFinish(){}

        public void Exit()
        {
        }

        public void ExitFinish() {}

        public void Update(FP deltaTime)
        {
            var lstItem = BattleScene.Instance.lstItem;
            var lstPlayer = PvpPlayerMgr.Instance.lstPlayer;
            for (int i = 0; i < lstItem.Count; i++)
            {
                var unitItem = lstItem[i];
                for (int j = 0; j < lstPlayer.Count; j++)
                {
                    var unit = lstPlayer[j].unit;
                    if(unit != null && !unit.isDie)
                    {
                        if(unit.gameCollider.CheckCircle(unitItem.curPosition,unitItem.resInfo.radius,true))
                        {
                            unitItem.HitPlayer(lstPlayer[j],unit);
                            m_lstRemoveUnit.Add(unitItem);
                            break;
                        }
                    }
                }
            }
            for (int i = 0; i < m_lstRemoveUnit.Count; i++)
            {
                BattleScene.Instance.DestroyUnit(m_lstRemoveUnit[i]);
            }
            m_lstRemoveUnit.Clear();
        }
    }
}
