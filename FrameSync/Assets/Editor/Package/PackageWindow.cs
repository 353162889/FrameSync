using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;
using System.Collections.Generic;

namespace EditorPackage
{
    public class PackageWindow : EditorWindow
    {
        [MenuItem("Tools/Package/PackageWindow _F2")]
        static void OpenPackageWindow()
        {
            EditorWindow.GetWindow<PackageWindow>().Show();
        }

        private BuildTarget[] m_arrBuildTarget;
        private string[] m_arrBuildTargetDesc;
        private int m_nBuildTargetIdx;
        private bool m_bOpenFolder;
        private bool m_bRebuildAssetBundle;
        private bool m_bUseMD5Name;
        private int m_nPkgVersion;
        private bool m_bDeleteAllOldPackage;//是否删除所有旧包

        private BuildOptions[] m_arrOption = new BuildOptions[] { BuildOptions.Development, BuildOptions.AllowDebugging, BuildOptions.ConnectWithProfiler, BuildOptions.AutoRunPlayer };
        private bool[] m_arrOptionState;

        private Dictionary<BuildTarget, Action<BuildTarget, BuildOptions>> m_dic = new Dictionary<BuildTarget, Action<BuildTarget, BuildOptions>> {
            { BuildTarget.StandaloneWindows,PackageWin32Util.Build},
            { BuildTarget.Android,PackageAndroidUtil.Build},
        };

        private void OnEnable()
        {
            m_arrOptionState = new bool[m_arrOption.Length];
            for (int i = 0; i < m_arrOptionState.Length; i++)
            {
                m_arrOptionState[i] = false;
            }
            m_arrBuildTarget = PathConfig.DicPlatformName.Keys.ToArray();
            m_arrBuildTargetDesc = PathConfig.DicPlatformName.Values.ToArray();
            m_nBuildTargetIdx = 0;
            BuildTarget curBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            for (int i = 0; i < m_arrBuildTarget.Length; i++)
            {
                if(curBuildTarget == m_arrBuildTarget[i])
                {
                    m_nBuildTargetIdx = i;
                    break;
                }
            }
            m_bOpenFolder = true;
            m_bRebuildAssetBundle = true;
            m_bUseMD5Name = false;
            m_nPkgVersion = 1;
            this.titleContent = new GUIContent("打包窗口");
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
            m_bRebuildAssetBundle = EditorGUILayout.Toggle("是否重新打AssetBundle", m_bRebuildAssetBundle);
            EditorGUILayout.EndHorizontal();
            if (m_bRebuildAssetBundle)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                m_bUseMD5Name = EditorGUILayout.Toggle("AssetBundle使用MD5名字", m_bUseMD5Name);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.BeginHorizontal();
            m_nPkgVersion = EditorGUILayout.IntField("包版本号", m_nPkgVersion);
            EditorGUILayout.EndHorizontal();

            if (m_arrOption.Length > 0)
            {
                m_arrOptionState[0] = EditorGUILayout.BeginToggleGroup(m_arrOption[0].ToString(), m_arrOptionState[0]);
                for (int i = 1; i < m_arrOptionState.Length; i++)
                {
                    m_arrOptionState[i] = EditorGUILayout.Toggle(m_arrOption[i].ToString(), m_arrOptionState[i]);
                }
                EditorGUILayout.EndToggleGroup();
            }

            EditorGUILayout.BeginHorizontal();
            m_bDeleteAllOldPackage = EditorGUILayout.Toggle("删除所有旧包文件", m_bDeleteAllOldPackage);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("开始打包"))
            {
                BuildTarget buildTarget = m_arrBuildTarget[m_nBuildTargetIdx];
                if (m_bDeleteAllOldPackage)
                {
                    PathTools.RemoveDir(PathConfig.BuildPackageRootDir(buildTarget));
                }
                BuildOptions buildOptions = GetBuildOptions();
                Action<BuildTarget, BuildOptions> action = null;
                if (!m_dic.TryGetValue(buildTarget, out action))
                {
                    EditorUtility.DisplayDialog("提示", "打包失败,未找到" + buildTarget+"的打包方法", "确定");
                    return;
                }
                if (m_bRebuildAssetBundle)
                {
                    if (AssetBundleBuildUtil.BuildAssetBundle(buildTarget, m_bUseMD5Name))
                    {
                        AssetVersionUtil.GenerateVersionInfoFile(m_nPkgVersion, PathConfig.BuildOuterAssetBundleRootDir(buildTarget));
                        action.Invoke(buildTarget, buildOptions);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("提示", "AssetBundle构建失败！", "确定");
                        return;
                    }
                }
                else
                {
                    if(!Directory.Exists(PathConfig.BuildOuterAssetBundleRootDir(buildTarget)))
                    {
                        EditorUtility.DisplayDialog("提示", "打包失败,未找到AssetBundle目录" + PathConfig.BuildOuterAssetBundleRootDir(buildTarget), "确定");
                        return;
                    }
                    AssetVersionUtil.GenerateVersionInfoFile(m_nPkgVersion, PathConfig.BuildOuterAssetBundleRootDir(buildTarget));
                    action.Invoke(buildTarget, buildOptions);
                }

                if ((buildOptions & BuildOptions.AutoRunPlayer) == 0)
                {
                    //如果不是自动运行，打开包目录
                    //打开包所在的文件夹
                    System.Diagnostics.Process.Start(PathConfig.BuildPackageRootDir(buildTarget));
                }
                Debug.Log("打包成功");
            }
        }

        private BuildOptions GetBuildOptions()
        {
            BuildOptions buildOption = BuildOptions.None;
            if (m_arrOptionState.Length > 0 && m_arrOptionState[0])
            {
                for (int i = 0; i < m_arrOptionState.Length; ++i)
                {
                    if (m_arrOptionState[i])
                    {
                        buildOption |= m_arrOption[i];
                    }
                }
            }
            return buildOption;
        }
    }
}
