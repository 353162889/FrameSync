using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public delegate void PrefabPoolHandler(GameObject go);

    public class PrefabPool : SingletonMonoBehaviour<PrefabPool>
    {
    }
}
