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
        }
        window.Show();
        window.Init(initArr, action);
    }

    public static void ShowPointEditorWindow(FieldInfo fieldInfo, System.Object obj)
    {
        TSVector[] ps = (TSVector[])fieldInfo.GetValue(obj);
        Action<Vector3[]> action = (Vector3[] points) =>
        {
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
        }
        window.Show();
        window.Init(initArr, action,false);
    }

    public static void ShowSelectSingleAirShipWindow(FieldInfo fieldInfo, System.Object obj)
    {
        var window = EditorWindow.GetWindow<SelectUnitWindow>();
        Action<List<int>> action = (List<int> lst) => {
            if (lst.Count <= 0) return;
            int id = lst[0];
            fieldInfo.SetValue(obj, id);
        };
        window.Show();
        window.Init(Game.UnitType.AirShip, action);
    }

    public static void ShowSelectMultiAirShipWindow(FieldInfo fieldInfo, System.Object obj)
    {
        var window = EditorWindow.GetWindow<SelectUnitWindow>();
        Action<List<int>> action = (List<int> lst) =>
        {
            if (lst.Count <= 0) return;
            fieldInfo.SetValue(obj, lst.ToArray());
        };
        window.Show();
        window.Init(Game.UnitType.AirShip, action);
    }
}
