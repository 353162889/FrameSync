using UnityEngine;
using System.Collections;
using BTCore;
using Game;

public class SharedAgentObject : BTSharedVariable<AgentObject>
{
    public static implicit operator SharedAgentObject(AgentObject value) { return new SharedAgentObject { mValue = value }; }
}
