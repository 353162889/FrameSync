using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class PathEditorWindow : EditorWindow
{
    private Vector3[] m_arrInitPoints;
    private Action<Vector3[]> m_cCallback;
    private Vector3[] m_arrDrawPoints;
    private GameObject m_cRoot;
    private int m_nCurPathPoints;
    private bool m_bShowGOs;
    private Vector2 showGOPosition;
    private bool m_bShowDrawPoints;
    private Vector2 showDrawPointPosition;

    private bool m_bAwaysGeneratePath;

    public void Init(Vector3[] points,Action<Vector3[]> callback)
    {
        m_arrInitPoints = points;
        m_cCallback = callback;
        m_nCurPathPoints = 0;
        if (m_arrInitPoints != null && m_arrInitPoints.Length > 0)
        {
            m_arrDrawPoints = new Vector3[m_arrInitPoints.Length];
            for (int i = 0; i < m_arrDrawPoints.Length; i++)
            {
                m_arrDrawPoints[i] = m_arrInitPoints[i];
            }
            m_nCurPathPoints = m_arrInitPoints.Length;
        }
        m_bShowGOs = false;
        m_bShowDrawPoints = false;

        if (m_arrInitPoints != null && m_cRoot != null)
        {
            if (m_arrInitPoints.Length < 2)
            {
                for (int i = 0; i < m_arrInitPoints.Length; i++)
                {
                    CreateGO(m_arrInitPoints[i]);
                }
            }
            else
            {
                Vector3[] goPoints = EditorPathTool.GetPath(m_arrInitPoints, 5);
                for (int i = 0; i < goPoints.Length; i++)
                {
                    CreateGO(goPoints[i]);
                }
            }
        }

        m_bAwaysGeneratePath = false;
    }

    private void OnEnable()
    {
        this.titleContent = new UnityEngine.GUIContent("路径编辑器");
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        if(m_cRoot != null)
        {
            GameObject.DestroyImmediate(m_cRoot);
            m_cRoot = null;
        }
        m_cRoot = new GameObject("PathEditorRoot");
        
    }

    private void CreateGO(Vector3 position)
    {
        GameObject go = new GameObject();
        go.transform.parent = m_cRoot.transform;
        go.transform.localPosition = position;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.SetAsLastSibling();
        go.name = m_cRoot.transform.childCount.ToString();
        IconManager.SetIcon(go, IconManager.LabelIcon.Red);
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        m_arrInitPoints = null;
        m_cCallback = null;
        m_arrDrawPoints = null;
        if (m_cRoot != null)
        {
            GameObject.DestroyImmediate(m_cRoot);
            m_cRoot = null;
        }
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        m_arrInitPoints = null;
        m_cCallback = null;
        m_arrDrawPoints = null;
    }

    private void UpdatePathPoint(Vector3[] arr)
    {
        m_arrDrawPoints = arr;
    }


    private void OnSceneGUI(SceneView sceneView)
    {
        if(m_bAwaysGeneratePath && m_cRoot != null && m_cRoot.transform.childCount > 1 && m_nCurPathPoints > 1)
        {
            GeneratePath();
        }
        if (m_arrDrawPoints != null && m_arrDrawPoints.Length > 1)
        {
            Handles.DrawPolyLine(m_arrDrawPoints);
        }
    }

    private void GeneratePath()
    {
        if (m_cRoot != null)
        {
            int calCount = m_cRoot.transform.childCount;
            if (calCount > 1 && m_nCurPathPoints > 1)
            {
                Vector3[] arr = new Vector3[calCount];
                for (int i = 0; i < calCount; i++)
                {
                    arr[i] = m_cRoot.transform.GetChild(i).position;
                }
                m_arrDrawPoints = EditorPathTool.GetPath(arr, m_nCurPathPoints);
                SceneView.lastActiveSceneView.Repaint();
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "坐标点数量必须大于1个", "确认");
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        m_bAwaysGeneratePath = EditorGUILayout.Toggle("实时计算曲线", m_bAwaysGeneratePath);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        m_nCurPathPoints = EditorGUILayout.IntField("将曲线分成的坐标点数量",m_nCurPathPoints);
        if (GUILayout.Button("计算坐标点"))
        {
            GeneratePath();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("当前场景中节点");
        if(GUILayout.Button("添加节点"))
        {
            CreateGO(Vector3.zero);
        }
     
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        m_bShowGOs = EditorGUILayout.Foldout(m_bShowGOs,"显示场景对象");
        EditorGUILayout.EndHorizontal();
        if(m_bShowGOs && m_cRoot != null)
        {
            int childCount = m_cRoot.transform.childCount;
            int deleteIndex = -1;
            EditorGUILayout.BeginHorizontal();
            using (var scope = new EditorGUILayout.ScrollViewScope(showGOPosition, GUILayout.Height(this.position.height / 2)))
            {
                showGOPosition = scope.scrollPosition;
                for (int i = 0; i < childCount; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.ObjectField("场景对象", m_cRoot.transform.GetChild(i), typeof(GameObject), true);
                    if (GUILayout.Button("删除"))
                    {
                        deleteIndex = i;
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (deleteIndex > -1)
                {
                    var child = m_cRoot.transform.GetChild(deleteIndex);
                    child.parent = null;
                    GameObject.DestroyImmediate(child.gameObject);
                    int resultChildCount = m_cRoot.transform.childCount;
                    for (int i = 0; i < resultChildCount; i++)
                    {
                        child = m_cRoot.transform.GetChild(i);
                        child.gameObject.name = i.ToString();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        int drawPoints = m_arrDrawPoints != null ? m_arrDrawPoints.Length : 0;
        m_bShowDrawPoints = EditorGUILayout.Foldout(m_bShowDrawPoints, "显示显示路径点:"+ drawPoints);
        EditorGUILayout.EndHorizontal();
        if (m_bShowDrawPoints && m_arrDrawPoints != null)
        {
            EditorGUILayout.BeginHorizontal();
            using (var scope = new EditorGUILayout.ScrollViewScope(showDrawPointPosition,GUILayout.Height(this.position.height / 2)))
            {
                showDrawPointPosition = scope.scrollPosition;
                int drawPointCount = m_arrDrawPoints.Length;
                for (int i = 0; i < drawPointCount; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.Vector3Field("坐标", m_arrDrawPoints[i]);
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndHorizontal();
        }


        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("保存数据计算出来的数据"))
        {
            if(m_cCallback != null)
            {
                var callback = m_cCallback;
                m_cCallback = null;
                callback.Invoke(m_arrDrawPoints);
                this.Close();
            }
        }
        EditorGUILayout.EndHorizontal();

    }

   
}

