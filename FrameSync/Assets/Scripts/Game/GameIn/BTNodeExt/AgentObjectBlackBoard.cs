using BTCore;
using Framework;
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
        public virtual FP GetHostAttr(AttrType attrType)
        {
            if (host == null) return 0;
            return host.GetAttrValue((int)attrType);
        }
    }
}
