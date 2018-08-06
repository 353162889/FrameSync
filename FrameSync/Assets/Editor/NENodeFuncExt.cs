using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Framework;
using UnityEditor;

public class NENodeFuncExt
{
    public static void ShowPathEditorWindow(FieldInfo fieldInfo, System.Object obj)
    {
        TSVector[] ps = (TSVector[])fieldInfo.GetValue(obj);
        Action<TSVector[]> action = (TSVector[] points) => {
            fieldInfo.SetValue(obj, points);
        };
        var window = EditorWindow.GetWindow<PathEditorWindow>();
        window.Init(ps, action);
        window.Show();
    }
}
