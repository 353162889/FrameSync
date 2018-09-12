using UnityEngine;
using System.Collections;
using Game;

public class UD_AIFinishDestory : UnitDestoryBase
{
    protected override bool Check(Unit unit)
    {
        return unit.isAIFinish;
    }
}
