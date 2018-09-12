using UnityEngine;
using System.Collections;
using NodeEditor;
using BTCore;
using Game;

[NENodeCategory("AI/Condidion")]
public class BaseAICondition : BTCondition
{
    sealed public override bool Evaluate(BTBlackBoard blackBoard)
    {
        return this.Evaluate((AIBlackBoard)blackBoard);
    }

    protected virtual bool Evaluate(AIBlackBoard blackBoard) { return true; }
}