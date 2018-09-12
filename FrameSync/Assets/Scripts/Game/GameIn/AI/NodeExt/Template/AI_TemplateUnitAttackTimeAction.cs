using UnityEngine;
using System.Collections;
using Game;
using Framework;
using NodeEditor;
using BTCore;

public class AI_TemplateUnitAttackTimeActionData
{
}
[AINode(typeof(AI_TemplateUnitAttackTimeActionData))]
[NENodeDesc("AI模板，等待一定的攻击时间")]
public class AI_TemplateUnitAttackTimeAction : BaseAIAction
{
    private FP m_sWaitTime;

    protected override void OnEnter(AIBlackBoard blackBoard)
    {
        m_sWaitTime = 0;
        if(blackBoard.host.agentType == AgentObjectType.Unit)
        {
            Unit unit = (Unit)blackBoard.host.agent;
            if(unit.unitType == UnitType.AirShip)
            {
                m_sWaitTime = ((UnitAirShip)unit).resInfo.ai_attack_time;
            }
        }
        base.OnEnter(blackBoard);
    }

    public override BTActionResult OnRun(AIBlackBoard blackBoard)
    {
        m_sWaitTime -= blackBoard.deltaTime;
        if (m_sWaitTime <= 0) return BTActionResult.Ready;
        return BTActionResult.Running;
    }

    public override void OnExit(AIBlackBoard blackBoard)
    {
        m_sWaitTime = 0;
        base.OnExit(blackBoard);
    }
}
