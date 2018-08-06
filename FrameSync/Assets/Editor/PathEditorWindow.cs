using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

public class PathEditorWindow : EditorWindow
{
    private TSVector[] m_arrInitPoints;
    private Action<TSVector[]> m_cCallback;
    public void Init(TSVector[] points,Action<TSVector[]> callback)
    {
        m_arrInitPoints = points;
        m_cCallback = callback;
    }

    private void OnEnable()
    {
        this.titleContent = new UnityEngine.GUIContent("路径编辑器");
    }

    private void OnGUI()
    {

    }
}

