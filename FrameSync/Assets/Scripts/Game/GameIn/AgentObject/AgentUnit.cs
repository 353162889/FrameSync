using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using UnityEngine;

namespace Game
{
    public class AgentUnit : AgentObject
    {
        private Unit m_cUnit;
        private uint m_nId;
        private int m_nLastCampId;
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
            m_nLastCampId = m_cUnit.campId;
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

        public override int campId
        {
            get
            {
                return m_cUnit == null ? m_nLastCampId : m_cUnit.campId;
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

        public override TSVector lastForward
        {
            get
            {
                return m_cUnit == null ? m_sLastForward : m_cUnit.lastForward;
            }
        }

        public override TSVector lastPosition
        {
            get
            {
                return m_cUnit == null ? m_sLastPosition : m_cUnit.lastPosition;
            }
        }

        public override GameCollider gameCollider
        {
            get
            {
                return m_cUnit == null ? null : m_cUnit.gameCollider;
            }
        }

        public override Transform GetHangPoint(string name, out TSVector position, out TSVector forward)
        {
            if(m_cUnit != null)
            {
                return m_cUnit.GetHangPoint(name, out position, out forward);
            }
            position = curPosition;
            forward = curForward;
            return null;
        }

        public override Transform GetHangPoint(string name, TSVector cPosition, TSVector cForward, out TSVector position, out TSVector forward)
        {
            if (m_cUnit != null)
            {
                return m_cUnit.GetHangPoint(name, cPosition, cForward, out position, out forward);
            }
            position = curPosition;
            forward = curForward;
            return null;
        }

        public override FP GetAttrValue(int key)
        {
            if (m_cUnit != null)
            {
                return m_cUnit.GetAttrValue(key);
            }
            return 0;
        }

        public override void SetAttrValue(int key, FP value)
        {
            if (m_cUnit != null)
            {
                m_cUnit.SetAttrValue(key, value);
            }
        }
    }
}
