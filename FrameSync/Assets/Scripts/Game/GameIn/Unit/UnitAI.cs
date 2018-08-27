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
        public bool isAIRunning { get { return m_cAgentObjectAI.start; } }
        public void InitAI()
        {
            m_cAgentObjectAI = new AgentObjectAI();
        }

        public void SetAI(int configId)
        {
            if(m_cAgentObjectAI != null)
            {
                m_cAgentObjectAI.Clear();
            }
            m_cAgentObjectAI.Init(this.agentObj, configId);
        }

        public void StartAI()
        {
            return;
            m_cAgentObjectAI.Start();
        }

        public void StopAI()
        {
            m_cAgentObjectAI.Stop();
        }

        public void ResetAI()
        {
            m_cAgentObjectAI.Clear();
        }

        public void UpdateAI(FP deltaTime)
        {
            m_cAgentObjectAI.Update(deltaTime);
        }

        public void DieAI(DamageInfo damageInfo)
        {
            StopAI();
        }
    }
}
