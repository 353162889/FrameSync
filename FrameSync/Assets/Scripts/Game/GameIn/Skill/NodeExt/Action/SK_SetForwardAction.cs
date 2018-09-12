using UnityEngine;
using System.Collections;
using Game;
using NodeEditor;
using Framework;
using BTCore;

public class SK_SetForwardActionData
{
    [NEProperty("触发时间", true)]
    public FP time;
    [NEProperty("立即转向",true)]
    public bool bImmediately;
}

[SkillNode(typeof(SK_SetForwardActionData))]
[NENodeDesc("设置当前方向")]
public class SK_SetForwardAction : BaseTimeLineSkillAction
{
    private SK_SetForwardActionData m_cSetForwardData;
    private bool m_moveEnd;
    public override FP time
    {
        get
        {
            return m_cSetForwardData.time;
        }
    }
    private Unit m_cUnit;

    protected override void OnInitData(object data)
    {
        base.OnInitData(data);
        m_cSetForwardData = data as SK_SetForwardActionData;
    }

    protected override void OnEnter(SkillBlackBoard blackBoard)
    {
        if(blackBoard.host.agentType == AgentObjectType.Unit)
        {
            m_cUnit = (Unit)blackBoard.host.agent;
            m_cUnit.SetForward(blackBoard.skill.targetForward, ForwardFromType.Skill, m_cSetForwardData.bImmediately);
        }
        else if(blackBoard.host.agentType == AgentObjectType.Remote)
        {
            Remote remote = (Remote)blackBoard.host.agent;
            remote.SetForward(blackBoard.skill.targetForward);
        }
    }

    public override BTActionResult OnRun(SkillBlackBoard blackBoard)
    {
        if (m_cUnit != null && m_cUnit.isRotating) return BTActionResult.Running;
        return BTActionResult.Ready;
    }

    public override void OnExit(SkillBlackBoard blackBoard)
    {
        m_cUnit = null;
        base.OnExit(blackBoard);
    }
}
