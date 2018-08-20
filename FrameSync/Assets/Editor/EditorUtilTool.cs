using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EditorUtilTool
{

    public static string GetDirectory(string path)
    {
        path = path.Replace("\\", "/");
        string dir = path.Substring(0, path.LastIndexOf("/"));
        return dir;
    }

    public static string GetLastDirectoryName(string path)
    {
        string dir = GetDirectory(path);
        int lastIdx = dir.LastIndexOf("/");
        return dir.Substring(lastIdx + 1, dir.Length - lastIdx - 1);
    }

    public static string GetFileName(string path,bool containExtName)
    {
        path = path.Replace("\\", "/");
        int lastIdx = path.LastIndexOf("/");
        string name = path.Substring(lastIdx + 1, path.Length - lastIdx - 1);
        if(!containExtName)
        {
            name = name.Substring(0, name.LastIndexOf("."));
        }
        return name;
    }

    public static GameObject AddChild(GameObject parent,GameObject child, string name)
    {
        if (child == null)
        {
            child = new GameObject();
        }
        child.transform.parent = parent.transform;
        child.transform.localPosition = Vector3.zero;
        child.transform.localScale = Vector3.one;
        child.transform.localEulerAngles = Vector3.zero;
        child.name = name;
        return child;
    }
}
