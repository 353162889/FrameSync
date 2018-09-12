using UnityEngine;
using System.Collections;
using Game;
using NodeEditor;

public class UnitCheckExistTargetConditionData
{

}

[AINode(typeof(UnitCheckExistTargetConditionData))]
[NENodeDesc("检测当前目标是否死亡")]
[NENodeDisplay(true,false,true)]
public class UnitCheckExistTargetCondition : BaseAICondition
{
    protected override bool Evaluate(AIBlackBoard blackBoard)
    {
        Unit hostUnit = (Unit)blackBoard.host.agent;
        if (hostUnit == null) return false;
        AgentObject agentObj = hostUnit.targetAIAgent;
        if (agentObj == null) return false;
        Unit unit = (Unit)agentObj.agent;
        if (unit == null || unit.isDie) return false;
        return true;
    }
}
