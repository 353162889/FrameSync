using UnityEngine;
using UnityEditor;
using System.Linq;

namespace EditorPackage
{
    public class AssetBundleBuildWindow : EditorWindow
    {
        [MenuItem("Tools/Package/AssetBundleBuildWindow _F1")]
        static void OpenAssetBundleWindow()
        {
            EditorWindow.GetWindow<AssetBundleBuildWindow>().Show();
        }

        private BuildTarget[] m_arrBuildTarget;
        private string[] m_arrBuildTargetDesc;
        private int m_nBuildTargetIdx;
        private bool m_bOpenFolder;
        private bool m_bUseMD5Name;
        private int m_nPkgVersion;

        private void OnEnable()
        {
            m_arrBuildTarget = PathConfig.DicPlatformName.Keys.ToArray();
            m_arrBuildTargetDesc = PathConfig.DicPlatformName.Values.ToArray();
            m_nBuildTargetIdx = 0;
            BuildTarget curBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            for (int i = 0; i < m_arrBuildTarget.Length; i++)
            {
                if (curBuildTarget == m_arrBuildTarget[i])
                {
                    m_nBuildTargetIdx = i;
                    break;
                }
            }
            m_bOpenFolder = true;
            m_bUseMD5Name = false;
            m_nPkgVersion = 1;
            this.titleContent = new GUIContent("构建AssetBundle窗口");
        }

        private void OnDisable()
        {
            AssetBundleBuildUtil.ClearAssetBundleNames();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            m_nBuildTargetIdx = EditorGUILayout.Popup("构建平台", m_nBuildTargetIdx, m_arrBuildTargetDesc); ;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            m_bOpenFolder = EditorGUILayout.Toggle("构建完成后打开目录", m_bOpenFolder);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            m_bUseMD5Name = EditorGUILayout.Toggle("AssetBundle使用MD5名字", m_bUseMD5Name);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            m_nPkgVersion = EditorGUILayout.IntField("包版本号",m_nPkgVersion);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("开始构建AssetBundle"))
            {
                BuildTarget buildTarget = m_arrBuildTarget[m_nBuildTargetIdx];
                if (AssetBundleBuildUtil.BuildAssetBundle(buildTarget, m_bUseMD5Name))
                {
                    AssetVersionUtil.GenerateVersionInfoFile(m_nPkgVersion, PathConfig.BuildOuterAssetBundleRootDir(buildTarget));
                    if (m_bOpenFolder)
                    {
                        //打开包所在的文件夹
                        System.Diagnostics.Process.Start(PathConfig.BuildOuterAssetBundleRootDir(buildTarget));
                    }
                }
                else
                {
                    Debug.LogError("AssetBundle构建失败！");
                }
            }
            if(GUILayout.Button("打开bundle依赖查找窗口（工具）"))
            {
                AssetBundleBuildUtil.ClearAssetBundleNames();
                AssetBundleBuildUtil.SetAssetBundleName(m_bUseMD5Name);
                AssetBundleBrowser.AssetBundleBrowserMain.ShowWindow();
            }
        }
    }
}