using BTCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class RemoteBlackBoard : AgentObjectBlackBoard
    {
        private Remote m_cRemote;
        public Remote remote { get { return m_cRemote; } }

        public override AgentObject host
        {
            get
            {
                return remote.agentObj;
            }
        }

        public void Init(Remote remote)
        {
            m_cRemote = remote;
        }

        public override void Clear()
        {
            m_cRemote = null;
            base.Clear();
        }
    }
}
