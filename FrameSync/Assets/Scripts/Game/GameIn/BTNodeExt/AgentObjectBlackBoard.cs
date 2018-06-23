using BTCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public abstract class AgentObjectBlackBoard : BTBlackBoard
    {
        public abstract AgentObject host { get; }
        public SelectAgentObjInfo selectAgentObjInfo { get; set; }
    }
}
