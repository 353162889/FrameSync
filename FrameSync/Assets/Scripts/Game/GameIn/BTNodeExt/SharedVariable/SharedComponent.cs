using UnityEngine;
using System.Collections;
using BTCore;
using Game;

public class SharedComponent : BTSharedVariable<Component>
{
    public static implicit operator SharedComponent(Component value) { return new SharedComponent { mValue = value }; }
}

