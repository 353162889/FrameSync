using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public enum UnitDestoryType
    {
        DieDestory,
        AIFinishDestory,
    }
    public class GSC_UnitDestory : IGamingSysComponent
    {
        private Dictionary<UnitDestoryType, UnitDestoryBase> m_dic = new Dictionary<UnitDestoryType, UnitDestoryBase>();

        public void RegisterUnit(UnitDestoryType destoryType, Unit unit)
        {
            UnitDestoryBase unitDestory = null;
            if(m_dic.TryGetValue(destoryType,out unitDestory))
            {
                unitDestory.Enter(unit);
            }
        }

        public void UnRegisterUnit(Unit unit)
        {
            foreach (var item in m_dic)
            {
                item.Value.Exit(unit);
            }
        }

        public void Enter()
        {
            Add(UnitDestoryType.DieDestory, typeof(UD_DieDestory));
            Add(UnitDestoryType.AIFinishDestory, typeof(UD_AIFinishDestory));
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.AddUnitDestory, OnUnitDestoryAdd);
        }

        private void OnUnitDestoryAdd(object args)
        {
            object[] arr = (object[])args;
            var destoryType = (UnitDestoryType)arr[0];
            var destoryUnit = (Unit)arr[1];
            RegisterUnit(destoryType, destoryUnit);
        }

        public void EnterFinish() { }

        public void Exit()
        {
            foreach (var item in m_dic)
            {
                item.Value.Clear();
            }
            m_dic.Clear();
            GlobalEventDispatcher.Instance.RemoveEvent(GameEvent.AddUnitDestory, OnUnitDestoryAdd);
        }

        public void ExitFinish() { }

        public void Update(FP deltaTime)
        {
            foreach (KeyValuePair<UnitDestoryType, UnitDestoryBase> item in m_dic)
            {
                item.Value.Update(deltaTime);
            }
        }

        private void Add(UnitDestoryType destoryType, Type type)
        {
            var unitDestory = (UnitDestoryBase)Activator.CreateInstance(type);
            unitDestory.Init();
            m_dic.Add(destoryType, unitDestory);
        }
    }
}
