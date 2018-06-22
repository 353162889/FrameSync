using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{

    public enum AgentObjectType
    {
        Unit,
    }

    public abstract class AgentObject
    {

        public static AgentObject GetAgentObject(uint id, AgentObjectType agentType)
        {
            AgentObject agentObj = null;
            switch (agentType)
            {
                case AgentObjectType.Unit:
                    Unit unit = BattleScene.Instance.GetUnit(id);
                    if (unit != null)
                    {
                        agentObj = unit.agentObj;
                    }
                    break;
            }
            return agentObj;
        }

        protected object m_cAgent;
        public object agent { get { return m_cAgent; } }
        public virtual void Clear()
        {
            m_cAgent = null;
        }

        abstract public uint id { get; }
        abstract public AgentObjectType agentType { get; }
        abstract public TSVector curPosition { get; }
        abstract public TSVector curForward { get; }
        abstract public Transform GetHangPoint(string name, out TSVector position, out TSVector forward);
    }
}
