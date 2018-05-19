using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public class AgentUnit : AgentObject
    {
        private Unit m_cUnit;
        private uint m_nId;
        private TSVector m_sLastPosition;
        private TSVector m_sLastForward;
        public AgentUnit(Unit unit)
        {
            m_nId = unit.id;
            m_cAgent = unit;
            m_cUnit = unit;
        }

        public override void Clear()
        {
            m_sLastPosition = m_cUnit.curPosition;
            m_sLastForward = m_cUnit.curForward;
            m_cUnit = null;
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
                return AgentObjectType.Unit;
            }
        }

        public override TSVector curForward
        {
            get
            {
                return m_cUnit == null ? m_sLastForward : m_cUnit.curForward;
            }
        }

        public override TSVector curPosition
        {
            get
            {
                return m_cUnit == null ? m_sLastPosition : m_cUnit.curPosition;
            }
        }
    }
}
