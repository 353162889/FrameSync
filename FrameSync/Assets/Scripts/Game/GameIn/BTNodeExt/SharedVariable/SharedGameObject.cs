using UnityEngine;
using System.Collections;
using BTCore;
using Game;

public class SharedGameObject : BTSharedVariable<GameObject>
{
    public static implicit operator SharedGameObject(GameObject value) { return new SharedGameObject { mValue = value }; }
}
