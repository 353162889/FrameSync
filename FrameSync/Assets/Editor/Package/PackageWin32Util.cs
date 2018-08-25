using UnityEngine;
using UnityEditor;
using System.IO;

namespace EditorPackage
{
    public class PackageWin32Util : ScriptableObject
    {

        [MenuItem("Tools/Test _F2")]
        public static void TestPackage()
        {
            Build(BuildTarget.StandaloneWindows, BuildOptions.None);
        }


        public static void Build(BuildTarget buildTarget, BuildOptions buildOptions)
        {
            UpdateSetting(buildTarget);
            PackageUtil.Build(buildTarget,buildOptions);
        }

        private static void UpdateSetting(BuildTarget buildTarget)
        {
            //string path = PathConfig.BuildAssetBundleRootDir(buildTarget) + "/" + PathConfig.VersionCodeFile;
            //if (!File.Exists(path))
            //{
            //    Debug.LogError("找不到版本文件");
            //    return false;
            //}
            ////读取版本文件
            //string text = File.ReadAllText(path);
            //var arr = text.Split(',');
            //int pkgVersion = 0;
            //if (arr.Length <= 0 || !int.TryParse(arr[0], out pkgVersion))
            //{
            //    Debug.LogError("读取版本文件失败");
            //    return false;
            //}
            //int.TryParse(arr[0], out pkgVersion);
            //return true;
        }
    }
}