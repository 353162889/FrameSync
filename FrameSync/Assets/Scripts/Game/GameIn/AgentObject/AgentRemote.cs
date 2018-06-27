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
        private int m_nLastCampId;
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
            m_nLastCampId = m_cRemote.campId;
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

        public override int campId
        {
            get
            {
                return m_cRemote == null ? m_nLastCampId : m_cRemote.campId;
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

        public override TSVector lastForward
        {
            get
            {
                return m_cRemote == null ? m_sLastForward : m_cRemote.lastForward;
            }
        }

        public override TSVector lastPosition
        {
            get
            {
                return m_cRemote == null ? m_sLastPosition : m_cRemote.lastPosition;
            }
        }

        public override GameCollider gameCollider
        {
            get
            {
                return m_cRemote == null ? null : m_cRemote.gameCollider;
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

        public override Transform GetHangPoint(string name, TSVector cPosition, TSVector cForward, out TSVector position, out TSVector forward)
        {
            if (m_cRemote != null)
            {
                return m_cRemote.GetHangPoint(name, cPosition, cForward, out position, out forward);
            }
            position = curPosition;
            forward = curForward;
            return null;
        }
    }
}
