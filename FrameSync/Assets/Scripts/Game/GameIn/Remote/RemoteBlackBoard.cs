using BTCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class RemoteBlackBoard : BTBlackBoard
    {
        private Remote m_cRemote;
        public Remote remote { get { return m_cRemote; } }
        public RemoteBlackBoard(Remote remote)
        {
            m_cRemote = remote;
        }
    }
}
