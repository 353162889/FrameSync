using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using UnityEngine;

namespace Game
{
    public class AgentRemote : AgentObject
    {
        private Remote m_cRemote;
        private uint m_nId;
        private TSVector m_sLastPosition;
        private TSVector m_sLastForward;
        public AgentRemote(Remote remote)
        {
            m_nId = remote.id;
            m_cAgent = remote;
            m_cRemote = remote;
        }

        public override void Clear()
        {
            m_sLastPosition = m_cRemote.curPosition;
            m_sLastForward = m_cRemote.curForward;
            m_cRemote = null;
            base.Clear();
        }

        public override uint id
        {
            get
            {
                return m_nId;
            }
        }

        public override AgentObjectType agentType
        {
            get
            {
                return AgentObjectType.Remote;
            }
        }

        public override TSVector curForward
        {
            get
            {
                return m_cRemote == null ? m_sLastForward : m_cRemote.curForward;
            }
        }

        public override TSVector curPosition
        {
            get
            {
                return m_cRemote == null ? m_sLastPosition : m_cRemote.curPosition;
            }
        }
       

        public override Transform GetHangPoint(string name, out TSVector position, out TSVector forward)
        {
            if (m_cRemote != null)
            {
                return m_cRemote.GetHangPoint(name, out position, out forward);
            }
            position = curPosition;
            forward = curForward;
            return null;
        }
    }
}
