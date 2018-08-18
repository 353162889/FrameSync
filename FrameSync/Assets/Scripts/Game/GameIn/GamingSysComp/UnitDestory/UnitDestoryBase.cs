using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class UnitDestoryBase
    {
        protected List<Unit> m_lstUnit = new List<Unit>();

        public virtual void Init()
        {
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.UnitRemove, OnUnitRemove);
        }

        public virtual void Clear()
        {
            m_lstUnit.Clear();
            GlobalEventDispatcher.Instance.RemoveEvent(GameEvent.UnitRemove, OnUnitRemove);
        }

        private void OnUnitRemove(object args)
        {
            Unit unit = (Unit)args;
            m_lstUnit.Remove(unit);
        }

        public virtual void Enter(Unit unit)
        {
            if(!m_lstUnit.Contains(unit))
            {
                m_lstUnit.Add(unit);
            }
        }

        public virtual void Exit(Unit unit)
        {
            m_lstUnit.Remove(unit);
        }

        public virtual void Update(FP deltaTime)
        {
            for (int i = m_lstUnit.Count - 1; i > -1; i--)
            {
                var unit = m_lstUnit[i];
                if(Check(unit))
                {
                    Exit(unit);
                    BattleScene.Instance.DestroyUnit(unit);
                }
            }
        }

        protected virtual bool Check(Unit unit)
        {
            return false;
        }
    }
}
