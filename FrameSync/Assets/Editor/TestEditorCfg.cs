using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Framework;
using System;
using GameData;

public class TestEditorCfg {
    [MenuItem("Test/TestCfg")]
    public static void Test()
    {
        ResCfgSys.Instance.LoadResCfgs("Assets/ResourceEx",OnFinish);
    }

    private static void OnFinish()
    {
        var lst = ResCfgSys.Instance.GetCfgLst<ResTest>();
        for (int i = 0; i < lst.Count; i++)
        {
            CLog.LogArgs(lst[i].id, lst[i].point, lst[i].desc);
        }
        var lst1 = ResCfgSys.Instance.GetCfgLst<ResTest1>();
        for (int i = 0; i < lst1.Count; i++)
        {
            CLog.LogArgs(lst1[i].id, lst1[i].point, lst1[i].desc);
        }
    }
}
