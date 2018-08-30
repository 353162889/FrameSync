using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public partial class Unit
    {
        private AgentObjectAI m_cAgentObjectAI;
        public List<int> lstSelectAISkill { get { return m_lstSelectAISkill; } }
        private List<int> m_lstSelectAISkill;
        public bool isAIRunning { get { return m_cAgentObjectAI.start; } }
        public void InitAI()
        {
            if (m_cAgentObjectAI == null)
            {
                m_cAgentObjectAI = new AgentObjectAI();
            }
            if (m_lstSelectAISkill == null)
            {
                m_lstSelectAISkill = new List<int>();
            }
        }

        public void SetAISkill(List<int> lstSkill)
        {
            m_lstSelectAISkill.Clear();
            for (int i = 0; i < lstSkill.Count; i++)
            {
                m_lstSelectAISkill.Add(lstSkill[i]);
            }
        }

        public void SetAI(string aiPath)
        {
            if(m_cAgentObjectAI != null)
            {
                m_cAgentObjectAI.Clear();
                m_cAgentObjectAI.Init(this.agentObj, aiPath);
            }
        }

        public void StartAI()
        {
            if (m_cAgentObjectAI != null)
            {
                m_cAgentObjectAI.Start();
            }
        }

        public void StopAI()
        {
            if (m_cAgentObjectAI != null)
            {
                m_cAgentObjectAI.Stop();
            }
        }

        public void ResetAI()
        {
            if (m_cAgentObjectAI != null)
            {
                m_cAgentObjectAI.Clear();
            }
            if(m_lstSelectAISkill != null)
            {
                m_lstSelectAISkill.Clear();
            }
        }

        public void UpdateAI(FP deltaTime)
        {
            if (m_cAgentObjectAI != null)
            {
                m_cAgentObjectAI.Update(deltaTime);
            }
        }

        public void DieAI(DamageInfo damageInfo)
        {
            StopAI();
        }
    }
}
