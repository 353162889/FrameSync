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
        Action<Vector3[]> action = (Vector3[] points) => {
            if (points != null)
            {
                TSVector[] arr = new TSVector[points.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = TSVector.FromUnitVector3(points[i]);
                }
                fieldInfo.SetValue(obj, arr);
            }
        };
        var window = EditorWindow.GetWindow<PathEditorWindow>();
        int count = 0;
        if (ps != null) count = ps.Length;
        Vector3[] initArr = new Vector3[count];
        for (int i = 0; i < initArr.Length; i++)
        {
            initArr[i] = ps[i].ToUnityVector3();
            Debug.Log("init:"+initArr[i]);
        }
        window.Show();
        window.Init(initArr, action);
    }
}
