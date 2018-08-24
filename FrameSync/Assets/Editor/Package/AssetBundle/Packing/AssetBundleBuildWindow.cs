using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using UnityEditor.Sprites;

namespace CustomizeEditor
{
	
    public class AssetBundleBuildWindow : EditorWindow
    {
        private int platformIndex = 0;
        private bool isForceRebuild = false;
        private bool isOnlyBuildLua = false;

        //[MenuItem("AssetBundle/BuildView")]
        public static void OpenAssetBundleView()
        {
            EditorWindow.GetWindow<AssetBundleBuildWindow>().Show();
        }

        void OnEnable()
        {
            this.titleContent = new GUIContent("Build AssetBundle");
            this.position = new Rect(400, 200, 700, 350);
        }

        void OnDestroy()
        {

        }

        void OnGUI()
        {
            GUILayout.Space(10);
            AssetBundleConfig.Init();

            GUILayout.Label("Build Platform", EditorStyles.boldLabel);
            platformIndex = EditorGUILayout.Popup("Platform", platformIndex, EditorPlatformPath.BuildPlatformNames);
            AssetBundlePath.PackingPlatformName = EditorPlatformPath.BuildPlatformNames[platformIndex];

            GUILayout.BeginHorizontal();
            isForceRebuild = GUILayout.Toggle(isForceRebuild, "  Force Rebuild");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            isOnlyBuildLua = GUILayout.Toggle(isOnlyBuildLua, "  Only Build LuaFile（只打lua文件,仅支持lua文件中内容更改，不支持添加或删除lua文件）");
            GUILayout.EndHorizontal();

            GUILayout.Label("Build AssetBundle Path", EditorStyles.boldLabel);
            GUILayout.Label(AssetBundlePath.GetAssetBundlePath());

            //GUILayout.BeginHorizontal();
            //if (GUILayout.Button("Included Shaders", GUILayout.Width(200), GUILayout.Height(30)))
            //{
            //    IncludeShaders();
            //}
            //GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Build AssetBundle", GUILayout.Width(200), GUILayout.Height(30)))
            {
                AssetBundleBuildUtil.StartBuildAssetBundle(EditorPlatformPath.BuildPlatforms[platformIndex], isForceRebuild, isOnlyBuildLua);
                //打出版本文件
                AssetVersionUtil.GenerateVersionInfoFile(AssetBundlePath.GetAssetBundlePath());
            }

            //分析器后面再加
            //if (GUILayout.Button("Asset Bundle Analyze", GUILayout.Width(150), GUILayout.Height(30)))
            //{
            //    AssetBundleBuildUtil.IsBuild = false;
            //    AssetBundleBuildUtil.StartBuildAssetBundle(EditorPlatformPath.BuildPlatforms[platformIndex]);//, groupNames[groupIndex]);
            //    AssetBundleAnalyzeUtil.StartAnalyze(AssetBundlePath.PackingPlatformName, AssetBundlePath.QualityName);
            //}
            //GUILayout.Space(20);
            //if (GUILayout.Button("Manifest Analyze", GUILayout.Width(150), GUILayout.Height(30)))
            //{
            //    AssetBundleBuildUtil.IsBuild = false;
            //    AssetBundleAnalyzeUtil.AnalizeBuildedAll(AssetBundlePath.GetAssetBundlePath());
            //    AssetBundleAnalyzeUtil.StartAnalyze(AssetBundlePath.PackingPlatformName, AssetBundlePath.QualityName);
            //}
            GUILayout.EndHorizontal();
            
        }

        private void IncludeShaders()
        {
            List<string> shaders = new List<string>();
            string[] mats = AssetDatabase.FindAssets("t:Material");
            if(mats != null)
            {
                foreach (var item in mats)
                {
                    string path = AssetDatabase.GUIDToAssetPath(item);
                    Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                    if(mat != null && mat.shader  != null)
                    {
                        if(!shaders.Contains(mat.shader.name))
                        {
                            shaders.Add(mat.shader.name);
                        }
                    }
                }
            }

            Debug.Log("shaders.count:"+shaders.Count);
            SerializedObject graphicsSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
            SerializedProperty it = graphicsSettings.GetIterator();
            SerializedProperty dataPoint;
            while (it.NextVisible(true))
            {
                if (it.name == "m_AlwaysIncludedShaders")
                {
                    it.ClearArray();
                    if(shaders != null)
                    {
                        for (int i = 0; i < shaders.Count; i++)
                        {
                            it.InsertArrayElementAtIndex(i);
                            dataPoint = it.GetArrayElementAtIndex(i);
                            dataPoint.objectReferenceValue = Shader.Find(shaders[i]);
                        }
                    }
                    graphicsSettings.ApplyModifiedProperties();
                }
            }
            Debug.Log("includeShaders更改完成");
        }
       
    }
}
