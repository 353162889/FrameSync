using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CustomizeEditor
{
    public class PackageTWindow : EditorWindow
    {
        private int platformIndex = 0;
        private PackageType packageType = PackageType.Resource;

        private BuildOptions[] optionArr = new BuildOptions[3] { BuildOptions.Development, BuildOptions.AllowDebugging, BuildOptions.ConnectWithProfiler };
        private bool optionGroupState = false;
        private bool[] optionState = new bool[3] { false, false, false };

        private string[] androidProject = new string[] { "BuYu_AnySDK" };
        private int androidProjectIndex = 0;

        //[MenuItem("Package/BuildView")]
        public static void OpenAssetBundleView()
        {
            EditorWindow.GetWindow<PackageTWindow>().Show();
        }

        void OnEnable()
        {
            this.titleContent = new GUIContent("Build Package");
            this.position = new Rect(400, 200, 700, 400);
            androidProjectIndex = 0;
        }

        void OnDestroy()
        {

        }

        void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Package type", EditorStyles.boldLabel);
            packageType = (PackageType)EditorGUILayout.EnumPopup("type", packageType);

            GUILayout.Space(10);
            androidProjectIndex = EditorGUILayout.Popup("android project",androidProjectIndex, androidProject);
            GUILayout.Label("Build Platform", EditorStyles.boldLabel);
            platformIndex = EditorGUILayout.Popup("Platform", platformIndex, EditorPlatformPath.BuildPlatformNames);
            PackagePath.PlatformName = EditorPlatformPath.BuildPlatformNames[platformIndex];
            PackagePath.PackageExtension = EditorPlatformPath.BuildPlatFormExtensions[platformIndex];

            optionGroupState = EditorGUILayout.BeginToggleGroup("Build Options", optionGroupState);
            optionState[0] = EditorGUILayout.Toggle("Development", optionState[0]);
            optionState[1] = EditorGUILayout.Toggle("AllowDebugging", optionState[1]);
            optionState[2] = EditorGUILayout.Toggle("ConnectWithProfiler", optionState[2]);
            EditorGUILayout.EndToggleGroup();

            GUILayout.Label("Build Package Path", EditorStyles.boldLabel);
            GUILayout.Label(PackagePath.GetPackageDir());

            if (GUILayout.Button("Build Package", GUILayout.Width(200), GUILayout.Height(30)))
            {
                PackageSettingUtil.ChangeSetting();
                PackageAndroidUtil.CopyAndroidToPublish(androidProject[androidProjectIndex]);
                PackageTUtil.Package(packageType, EditorPlatformPath.BuildPlatforms[platformIndex], GetBuildOpetions());
            }
        }

       

        private BuildOptions GetBuildOpetions()
        {
            BuildOptions buildOption = BuildOptions.None;
            if (optionGroupState)
            {
                for (int i = 0; i < optionState.Length; ++i)
                {
                    if (optionState[i])
                    {
                        buildOption |= optionArr[i];
                    }
                }
            }
            return buildOption;
        }
    }
}
