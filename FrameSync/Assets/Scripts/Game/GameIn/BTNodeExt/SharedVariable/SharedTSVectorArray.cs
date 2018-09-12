using UnityEngine;
using System.Collections;
using BTCore;
using Framework;

public class SharedTSVectorArray : BTSharedVariable<TSVector[]>
{
    public static implicit operator SharedTSVectorArray(TSVector[] value) { return new SharedTSVectorArray { mValue = value }; }
}
