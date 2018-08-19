using Framework;
using Game;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class SelectUnitWindow : EditorWindow
{

    private UnitType m_eUnitType;
    private Action<List<int>> m_cCallback;
    private bool m_bInit;
    private Vector2 m_sScrollPos;
    private int[] m_arrSelect;
    private bool m_bAllSelect;

    public void Init(UnitType unitType, Action<List<int>> callback)
    {
        m_eUnitType = unitType;
        m_cCallback = callback;
        m_bInit = false;
        ResCfgSys.Instance.LoadResCfgs("Assets/ResourceEx/Config/Data",()=> { m_bInit = true; });
       
     
    }
    private void OnEnable()
    {
        this.titleContent = new UnityEngine.GUIContent("选择单位ID");
        m_arrSelect = null;
    }

    private void OnDisable()
    {
        m_bInit = false;
        ResCfgSys.Instance.Dispose();
    }

    private void OnGUI()
    {
        if (!m_bInit)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("当前窗口未初始化，请重新打开");
            EditorGUILayout.EndHorizontal();
            return;
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("当前选择单位类型:" + m_eUnitType);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("------------------------------------");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        bool oldAllSelect = m_bAllSelect;
        m_bAllSelect = EditorGUILayout.Toggle("全选", m_bAllSelect);
        bool changeAllSelect = m_bAllSelect != oldAllSelect;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        using (var scope = new EditorGUILayout.ScrollViewScope(m_sScrollPos, GUILayout.Width(this.position.width), GUILayout.Height(350)))
        {
            m_sScrollPos = scope.scrollPosition;

            
            switch (m_eUnitType)
            {
                case UnitType.AirShip:
                    List<ResAirShip> lst = ResCfgSys.Instance.GetCfgLst<ResAirShip>();
                    if (m_arrSelect == null)
                    {
                        m_arrSelect = new int[lst.Count];
                        if(m_bAllSelect)
                        {
                            for (int i = 0; i < lst.Count; i++)
                            {
                                m_arrSelect[i] = lst[i].id;
                            }
                        }
                    }
                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (changeAllSelect)
                        {
                            if (m_bAllSelect)
                            {
                                m_arrSelect[i] = lst[i].id;
                            }
                            else
                            {
                                m_arrSelect[i] = 0;
                            }
                        }
                        DrawItem(lst[i].id, lst[i].name,i);
                    }
                    break;
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("确认选择"))
        {
            List<int> lst = new List<int>();
            for (int i = 0; i < m_arrSelect.Length; i++)
            {
                if(m_arrSelect[i] > 0)
                {
                    lst.Add(m_arrSelect[i]);
                }
            }
            if(lst.Count <= 0)
            {
                EditorUtility.DisplayDialog("提示", "当前未选择id", "确定");
            }
            else
            {
                if(m_cCallback != null)
                {
                    var callback = m_cCallback;
                    m_cCallback = null;
                    callback.Invoke(lst);
                    this.Close();
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawItem(int id,string name,int index)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ID:" + id, GUILayout.Width(80));
        EditorGUILayout.LabelField(name, GUILayout.Width(200));
        bool select = m_arrSelect[index] > 0;
        select = EditorGUILayout.Toggle(select);
        if(select)
        {
            m_arrSelect[index] = id;
        }
        else
        {
            m_arrSelect[index] = 0;
        }
        EditorGUILayout.EndHorizontal();
    }
}
