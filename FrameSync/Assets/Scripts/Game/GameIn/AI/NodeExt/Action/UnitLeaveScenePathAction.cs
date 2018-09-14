using UnityEngine;
using System.Collections;
using Game;
using BTCore;
using Framework;
using System.Collections.Generic;
using NodeEditor;

public class UnitLeaveScenePathActionData
{

}

[AINode(typeof(UnitLeaveScenePathActionData))]
[NENodeDesc("单位离场移动")]
public class UnitLeaveScenePathAction : BaseAIAction
{
    private Unit m_cUnit;
    protected override void OnEnter(AIBlackBoard blackBoard)
    {
        var variable = blackBoard.GetVariable(GameConst.AILeaveScenePathName);
        if (variable != null)
        {
            TSVector[] path = (TSVector[])variable.GetValue();
            if (path != null)
            {
                m_cUnit = (Unit)blackBoard.host.agent;
                if (m_cUnit == null) return;
                if (path.Length > 1)
                {
                    var lst = ResetObjectPool<List<TSVector>>.Instance.GetObject();
                    var firstPoint = path[0];
                    for (int i = 1; i < path.Length; i++)
                    {
                        lst.Add(m_cUnit.curPosition + path[i] - firstPoint);
                    }
                    m_cUnit.Move(lst, MoveFromType.Game);
                    ResetObjectPool<List<TSVector>>.Instance.SaveObject(lst);
                }
            }
        }
        else
        {
            variable = blackBoard.GetVariable(GameConst.AIJoinScenePathName);
            if (variable != null)
            {
                TSVector[] path = (TSVector[])variable.GetValue();
                if (path != null)
                {
                    m_cUnit = (Unit)blackBoard.host.agent;
                    if (m_cUnit == null) return;
                    if (path.Length > 1)
                    {
                        var lst = ResetObjectPool<List<TSVector>>.Instance.GetObject();
                        var firstPoint = path[path.Length - 1];
                        for (int i = path.Length - 2; i > -1; i--)
                        {
                            lst.Add(m_cUnit.curPosition + path[i] - firstPoint);
                        }
                        m_cUnit.Move(lst, MoveFromType.Game);
                        ResetObjectPool<List<TSVector>>.Instance.SaveObject(lst);
                    }
                }
            }
        }
    }

    public override BTActionResult OnRun(AIBlackBoard blackBoard)
    {
        if (m_cUnit == null || !m_cUnit.isMoving)
            return BTActionResult.Ready;
        return BTActionResult.Running;
    }

    public override void OnExit(AIBlackBoard blackBoard)
    {
        m_cUnit = null;
        base.OnExit(blackBoard);
    }
}
