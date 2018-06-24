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
        Unit = 1 << 0,
        Remote = 1 << 1,
    }

    public abstract class AgentObject
    {

        static AgentObject()
        {
            ResetObjectPool<List<AgentObject>>.Instance.Init(5, (List<AgentObject> lst) => { lst.Clear(); });
        }

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
                case AgentObjectType.Remote:
                    Remote remote = BattleScene.Instance.GetRemote(id);
                    if(remote != null)
                    {
                        agentObj = remote.agentObj;
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
        abstract public int campId { get; }
        abstract public TSVector curPosition { get; }
        abstract public TSVector curForward { get; }
        abstract public TSVector lastPosition { get; }
        abstract public TSVector lastForward { get; }
        abstract public Transform GetHangPoint(string name, out TSVector position, out TSVector forward);
        abstract public Transform GetHangPoint(string name, TSVector cPosition, TSVector cForward, out TSVector position, out TSVector forward);
    }
}
