using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class GamingSysCompContainer
    {
        private Dictionary<GamingSysCompType, IGamingSysComponent> m_dicGamingSysComp = new Dictionary<GamingSysCompType, IGamingSysComponent>();

        public IGamingSysComponent GetSysComp(GamingSysCompType sysCompType)
        {
            IGamingSysComponent comp = null;
            m_dicGamingSysComp.TryGetValue(sysCompType, out comp);
            return comp;
        }

        public void RegisterComp(GamingSysCompType type, IGamingSysComponent comp)
        {
            m_dicGamingSysComp.Add(type, comp);
        }

        public void Enter()
        {
            foreach (var item in m_dicGamingSysComp)
            {
                item.Value.Enter();
            }

            foreach (var item in m_dicGamingSysComp)
            {
                item.Value.EnterFinish();
            }
        }

        public void Update(FP deltaTime)
        {
            foreach (var item in m_dicGamingSysComp)
            {
                item.Value.Update(deltaTime);
            }
        }

        public void Exit()
        {
            foreach (var item in m_dicGamingSysComp)
            {
                item.Value.Exit();
            }
            foreach (var item in m_dicGamingSysComp)
            {
                item.Value.ExitFinish();
            }
        }

        public void Clear()
        {
            m_dicGamingSysComp.Clear();
        }
    }
}
