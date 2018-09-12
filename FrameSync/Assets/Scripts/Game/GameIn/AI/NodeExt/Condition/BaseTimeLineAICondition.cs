using UnityEngine;
using System.Collections;
using BTCore;
using Game;
using Framework;

public abstract class BaseTimeLineAICondition : BaseAICondition, IBTTimeLineNode
{
    public abstract FP time { get; }
}
