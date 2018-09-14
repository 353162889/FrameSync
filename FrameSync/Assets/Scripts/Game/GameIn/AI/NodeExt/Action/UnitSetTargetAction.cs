using UnityEngine;
using System.Collections;
using Game;
using NodeEditor;
using BTCore;
using Framework;

public class UnitSetTargetActionData
{

}

[AINode(typeof(UnitSetTargetActionData))]
[NENodeDesc("设置当前AI目标")]
public class UnitSetTargetAction : BaseAIAction
{
    protected override void OnEnter(AIBlackBoard blackBoard)
    {
        var target = blackBoard.selectAgentObjInfo.agentObj;
        if (target != null)
        {
            Unit hostUnit = (Unit)blackBoard.host.agent;
            if(hostUnit != null)
            {
                hostUnit.targetAIAgent = target;
            }
        }
    }

    public override BTActionResult OnRun(AIBlackBoard blackBoard)
    {
        return BTActionResult.Ready;
    }
}
