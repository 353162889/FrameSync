using Framework;
using Game;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameColliderView))]
public class GameColliderViewEditor : Editor
{
    private GameColliderSet m_cColliderSet;
    private GameColliderItem m_cColliderItem;
    private bool m_bIsPrefab;

    [MenuItem("Tools/更新所有游戏对象碰撞框数据")]
    public static void ResetAllCollider()
    {
        GameColliderSet colliderSet = new GameColliderSet();
        var guids = AssetDatabase.FindAssets("t:GameObject", new string[] { "Assets/ResourceEx/Prefab" });
        int count = guids.Length;
        for (int i = 0; i < count; i++)
        {
            EditorUtility.DisplayProgressBar("更新碰撞框数据", "正在更新碰撞框数据(" + i + "/" + count + ")", i / (float)count);
            string goPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(goPath);
            GameColliderView colliderView = gameObject.GetComponent<GameColliderView>();
            if (colliderView != null)
            {
                GameColliderItem colliderItem = new GameColliderItem();
                colliderItem.path = goPath;
                FindCollider(gameObject.transform, gameObject.transform, colliderItem);
                colliderSet.mLstColliderItem.Add(colliderItem);
            }
        }
        NEUtil.SerializerObject(GameColliderCfgSys.GameColliderPath, colliderSet,GameColliderCfgSys.RelateTypes);
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
        TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(GameColliderCfgSys.GameColliderPath);
        m_cColliderSet = null;
        if (textAsset != null)
        {
            var bytes = textAsset.bytes;
            m_cColliderSet = NEUtil.DeSerializerObjectFromBuff(bytes, typeof(GameColliderSet), GameColliderCfgSys.RelateTypes) as GameColliderSet;
        }
        if (m_cColliderSet == null)
        {
            m_cColliderSet = new GameColliderSet();
        }

        GameColliderView colliderView = (GameColliderView)target;
        PrefabType prefabType = PrefabUtility.GetPrefabType(colliderView.gameObject);
        Debug.Log(prefabType);
        m_bIsPrefab = (prefabType == PrefabType.Prefab || prefabType == PrefabType.PrefabInstance);
        if (m_bIsPrefab)
        {
            UnityEngine.Object prefab = null;
            if (prefabType == PrefabType.PrefabInstance)
            {
                prefab = PrefabUtility.GetPrefabParent(colliderView.gameObject);
            }
            else
            {
                prefab = colliderView.gameObject;
            }
            Debug.Log(prefab);
            if (m_cColliderSet != null && prefab != null)
            {
                string path = AssetDatabase.GetAssetPath(prefab);
                Debug.Log("path:" + path);
                m_cColliderItem = m_cColliderSet.GetColliderItem(path);
                if (m_cColliderItem == null)
                {
                    m_cColliderItem = new GameColliderItem();
                    m_cColliderItem.path = path;
                    m_cColliderSet.mLstColliderItem.Add(m_cColliderItem);
                }
            }
        }
    }

    private void SaveData()
    {
        if (m_cColliderSet == null) return;
        NEUtil.SerializerObject(GameColliderCfgSys.GameColliderPath, m_cColliderSet, GameColliderCfgSys.RelateTypes);
    }

    void OnDisable()
    {
        m_cColliderSet = null;
    }
    public override void OnInspectorGUI()
    {
        GameColliderView colliderView = (GameColliderView)target;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("GameCollider:");
        EditorGUILayout.EndHorizontal();

        PrefabType prefabType = PrefabUtility.GetPrefabType(colliderView.gameObject);
        bool isPrefab = prefabType == PrefabType.Prefab || prefabType == PrefabType.PrefabInstance;
        if (isPrefab != m_bIsPrefab)
        {
            LoadData();
        }
        m_bIsPrefab = isPrefab;

        if (m_cColliderItem == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("当前对象必须是一个预制体");
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("path:" + m_cColliderItem.path);
            EditorGUILayout.EndHorizontal();
            int dataCount = m_cColliderItem.mLstData.Count;
            for (int i = 0; i < dataCount; i++)
            {
                BaseGameColliderData colliderData = m_cColliderItem.mLstData[i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("    " + colliderData.name);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("\tcolliderType", colliderData.gameColliderType.ToString());
                EditorGUILayout.EndHorizontal();
                switch(colliderData.gameColliderType)
                {
                    case GameColliderType.CircleCollider:
                        OnCircleGUI((CircleColliderData)colliderData);
                        break;
                    case GameColliderType.RectCollider:
                        OnRectGUI((RectColliderData)colliderData);
                        break;
                    default:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("\tcolliderType=["+ colliderData.gameColliderType.ToString() + "]需要编写编辑器代码");
                        EditorGUILayout.EndHorizontal();
                        break;
                }
            }
        }
        if (GUILayout.Button("查找所有碰撞框"))
        {
            if (m_cColliderItem != null)
            {
                m_cColliderItem.mLstData.Clear();
            }
            Transform trans = colliderView.transform;
            FindCollider(trans, trans, m_cColliderItem);
            if (GUI.changed)
                EditorUtility.SetDirty(colliderView.gameObject);
            serializedObject.ApplyModifiedProperties();
            SaveData();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private void OnCircleGUI(CircleColliderData colliderData)
    {
        EditorGUILayout.BeginHorizontal();
        var center = colliderData.center;
        EditorGUILayout.Vector3Field("\tcenter", new Vector3(center.x.AsFloat(), center.y.AsFloat(), center.z.AsFloat()));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        var forward = colliderData.forward;
        EditorGUILayout.Vector3Field("\tforward", new Vector3(forward.x.AsFloat(), forward.y.AsFloat(), forward.z.AsFloat()));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("\tradius", colliderData.radius.ToString());
        EditorGUILayout.EndHorizontal();
    }

    private void OnRectGUI(RectColliderData colliderData)
    {
        EditorGUILayout.BeginHorizontal();
        var center = colliderData.center;
        EditorGUILayout.Vector3Field("\tcenter", new Vector3(center.x.AsFloat(), center.y.AsFloat(), center.z.AsFloat()));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        var forward = colliderData.forward;
        EditorGUILayout.Vector3Field("\tforward", new Vector3(forward.x.AsFloat(), forward.y.AsFloat(), forward.z.AsFloat()));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("\thalfWidth", colliderData.halfWidth.ToString());
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("\thalfHeight", colliderData.halfHeight.ToString());
        EditorGUILayout.EndHorizontal();
    }

    private static void FindCollider(Transform root, Transform trans, GameColliderItem gameColliderItem)
    {
        int count = trans.childCount;
        for (int i = 0; i < count; i++)
        {
            var child = trans.GetChild(i);
            if (child.tag == "GameCollider" && gameColliderItem != null)
            {
                Collider collider = child.GetComponent<Collider>();
                if(collider != null)
                {
                    var scale = child.lossyScale;
                    collider.enabled = false;
                    if (scale.x != 1 || scale.y != 1 || scale.z != 1)
                    {
                        EditorUtility.DisplayDialog("提示", "child=" + child.name + "不能有缩放", "知道了");
                        return;
                    }
                    if (collider is SphereCollider)
                    {
                        SphereCollider sphereCollider = (SphereCollider)collider;
                        CircleColliderData colliderData = new CircleColliderData();
                        colliderData.name = child.name;
                        colliderData.gameColliderType = GameColliderType.CircleCollider;
                        Vector3 center = child.TransformPoint(sphereCollider.center);
                        var localCenter = root.InverseTransformPoint(center);
                        colliderData.center = new TSVector(FP.FromFloat(localCenter.x), 0, FP.FromFloat(localCenter.z));
                        var localForward = child.forward;
                        colliderData.forward = new TSVector(FP.FromFloat(localForward.x), 0, FP.FromFloat(localForward.z));
                       
                        colliderData.radius = FP.FromFloat(sphereCollider.radius);
                        gameColliderItem.mLstData.Add(colliderData);
                    }
                    else if(collider is BoxCollider)
                    {
                        BoxCollider boxCollider = (BoxCollider)collider;
                        RectColliderData colliderData = new RectColliderData();
                        colliderData.name = child.name;
                        colliderData.gameColliderType = GameColliderType.RectCollider;
                        Vector3 center = child.TransformPoint(boxCollider.center);
                        var localCenter = root.InverseTransformPoint(center);
                        colliderData.center = new TSVector(FP.FromFloat(localCenter.x), 0, FP.FromFloat(localCenter.z));
                        var localForward = child.forward;
                        colliderData.forward = new TSVector(FP.FromFloat(localForward.x), 0, FP.FromFloat(localForward.z));
                        colliderData.halfWidth = FP.FromFloat(boxCollider.size.x / 2f);
                        colliderData.halfHeight = FP.FromFloat(boxCollider.size.z / 2f);
                        gameColliderItem.mLstData.Add(colliderData);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("提示", "暂时不支持"+collider.GetType().ToString()+"碰撞框(请删除它再重试)", "知道了");
                        return;
                    }
                }
            }
            FindCollider(root, child, gameColliderItem);
        }
    }
}
