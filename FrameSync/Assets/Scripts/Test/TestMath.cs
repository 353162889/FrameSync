using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using UnityEngine;

public class TestMath : MonoBehaviour
{
    void Start()
    {
        FP a = 3;
        FP b = 2;
        FP c = a / b;
        CLog.Log(c.ToString());
    }
}
