using Framework;
using Game;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HangPointView))]
public class HangPointViewEditor : Editor
{
    private HangPointSet m_cHangPointSet;
    private HangPointItem m_cHangPointItem;
    private bool m_bIsPrefab;

    [MenuItem("Tools/更新所有挂点数据")]
    public static void ResetAllHangPoint()
    {
        HangPointSet hangPointSet = new HangPointSet();
        var guids = AssetDatabase.FindAssets("t:GameObject",new string[] { "Assets/ResourceEx/Prefab" });
        int count = guids.Length;
        for (int i = 0; i < count; i++)
        {
            EditorUtility.DisplayProgressBar("更新挂点数据", "正在更新挂点数据("+i+"/"+count+")", i / (float)count);
            string goPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(goPath);
            HangPointView hangPointView = gameObject.GetComponent<HangPointView>();
            if(hangPointView != null)
            {
                HangPointItem hangPointItem = new HangPointItem();
                hangPointItem.path = goPath;
                FindHangPoint(gameObject.transform, gameObject.transform, null, null, hangPointItem);
                hangPointSet.mLstHangPointItem.Add(hangPointItem);
            }
        }
        NEUtil.SerializerObject(HangPointCfgSys.HangPointPath, hangPointSet);
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void OnEnable()
    {
        LoadData();
    }

    private void LoadData()
    {
        TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(HangPointCfgSys.HangPointPath);
        m_cHangPointSet = null;
        if (textAsset != null)
        {
            var bytes = textAsset.bytes;
            m_cHangPointSet = NEUtil.DeSerializerObjectFromBuff(bytes, typeof(HangPointSet)) as HangPointSet;
        }
        if (m_cHangPointSet == null)
        {
            m_cHangPointSet = new HangPointSet();
        }

        HangPointView hangPoint = (HangPointView)target;
        PrefabType prefabType = PrefabUtility.GetPrefabType(hangPoint.gameObject);
        Debug.Log(prefabType);
        m_bIsPrefab = (prefabType == PrefabType.Prefab || prefabType == PrefabType.PrefabInstance);
        if(m_bIsPrefab)
        {
            UnityEngine.Object prefab = null;
            if (prefabType == PrefabType.PrefabInstance)
            {
                prefab = PrefabUtility.GetPrefabParent(hangPoint.gameObject);
            }
            else
            {
                prefab = hangPoint.gameObject;
            }
            Debug.Log(prefab);
            if (m_cHangPointSet != null && prefab != null)
            {
                string path = AssetDatabase.GetAssetPath(prefab);
                Debug.Log("path:" + path);
                m_cHangPointItem = m_cHangPointSet.GetHangPointItem(path);
                if(m_cHangPointItem == null)
                {
                    m_cHangPointItem = new HangPointItem();
                    m_cHangPointItem.path = path;
                    m_cHangPointSet.mLstHangPointItem.Add(m_cHangPointItem);
                }
            }
        }
    }

    private void SaveData()
    {
        if (m_cHangPointSet == null) return;
        NEUtil.SerializerObject(HangPointCfgSys.HangPointPath, m_cHangPointSet);
    }

    void OnDisable()
    {
        m_cHangPointSet = null;
    }
    public override void OnInspectorGUI()
    {
        var transNameProperty = serializedObject.FindProperty("m_lstTransName");
        var transProperty = serializedObject.FindProperty("m_lstTrans");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("HangPoint_Transform:");
        EditorGUILayout.EndHorizontal();
        int count = transNameProperty.arraySize;
        for (int i = 0; i < count; i++)
        {
            var prop = transNameProperty.GetArrayElementAtIndex(i);
            var objProp = transProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(objProp, new GUIContent(prop.stringValue));
            EditorGUILayout.EndHorizontal();
        }
       
        HangPointView hangPoint = (HangPointView)target;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("HangPoint_Position:");
        EditorGUILayout.EndHorizontal();

        PrefabType prefabType = PrefabUtility.GetPrefabType(hangPoint.gameObject);
        bool isPrefab = prefabType == PrefabType.Prefab || prefabType == PrefabType.PrefabInstance;
        if(isPrefab != m_bIsPrefab)
        {
            LoadData();
        }
        m_bIsPrefab = isPrefab;

        if (m_cHangPointItem == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("当前对象必须是一个预制体");
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("path:" + m_cHangPointItem.path);
            EditorGUILayout.EndHorizontal();
            int dataCount = m_cHangPointItem.mLstData.Count;
            for (int i = 0; i < dataCount; i++)
            {
                HangPointData hangPointData = m_cHangPointItem.mLstData[i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("    " + hangPointData.name);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                var pos = hangPointData.position;
                EditorGUILayout.Vector3Field("\tposition", new Vector3(pos.x.AsFloat(), pos.y.AsFloat(), pos.z.AsFloat()));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                var forward = hangPointData.forward;
                EditorGUILayout.Vector3Field("\tforward", new Vector3(forward.x.AsFloat(), forward.y.AsFloat(), forward.z.AsFloat()));
                EditorGUILayout.EndHorizontal();
            }
        }
        if (GUILayout.Button("查找所有挂点"))
        {
            transNameProperty.ClearArray();
            transProperty.ClearArray();
            if (m_cHangPointItem != null)
            {
                m_cHangPointItem.mLstData.Clear();
            }
            Transform trans = hangPoint.transform;
            FindHangPoint(trans, trans, transNameProperty, transProperty, m_cHangPointItem);
            if (GUI.changed)
                EditorUtility.SetDirty(hangPoint.gameObject);
            serializedObject.ApplyModifiedProperties();
            SaveData();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private static void FindHangPoint(Transform root, Transform trans, SerializedProperty transNameProperty, SerializedProperty transProperty,HangPointItem hangPointItem)
    {
        int count = trans.childCount;
        for (int i = 0; i < count; i++)
        {
            var child = trans.GetChild(i);
            if (child.tag == "HangPoint_Transform" && transNameProperty != null && transProperty != null)
            {
                transNameProperty.InsertArrayElementAtIndex(transNameProperty.arraySize);
                transProperty.InsertArrayElementAtIndex(transProperty.arraySize);
                var prop = transNameProperty.GetArrayElementAtIndex(transNameProperty.arraySize - 1);
                var objPorp = transProperty.GetArrayElementAtIndex(transProperty.arraySize - 1);
                prop.stringValue = child.name;
                objPorp.objectReferenceValue = child;
            }
            else if(child.tag == "HangPoint_Position" && hangPointItem != null)
            {
                HangPointData hangPointData = new HangPointData();
                hangPointData.name = child.name;
                var pos = child.position;
                var localPos = root.InverseTransformPoint(pos);
                hangPointData.position = new TSVector(FP.FromFloat(localPos.x), FP.FromFloat(localPos.y), FP.FromFloat(localPos.z));

                var forward = child.forward;
                var localForward = root.InverseTransformDirection(forward);
                hangPointData.forward = new TSVector(FP.FromFloat(localForward.x), FP.FromFloat(localForward.y), FP.FromFloat(localForward.z));
                hangPointItem.mLstData.Add(hangPointData);
            }
            FindHangPoint(root, child, transNameProperty, transProperty, hangPointItem);
        }
    }
}
