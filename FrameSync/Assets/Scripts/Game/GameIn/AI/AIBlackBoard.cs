using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{

    public class AIBlackBoard : AgentObjectBlackBoard
    {
        private AgentObjectAI m_cAI;
        public AgentObjectAI ai { get { return m_cAI; } }

        public override AgentObject host
        {
            get
            {
                return m_cAI.host;
            }
        }

        public AIBlackBoard(AgentObjectAI ai)
        {
            m_cAI = ai;
        }
    }
}
