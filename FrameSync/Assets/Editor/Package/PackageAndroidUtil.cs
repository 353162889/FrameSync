using UnityEngine;
using UnityEditor;
using System.IO;

namespace EditorPackage
{
    public class PackageAndroidUtil : ScriptableObject
    {
        public static void Build(BuildTarget buildTarget, BuildOptions buildOptions)
        {
            if (!UpdateSetting(buildTarget)) return;
            PackageUtil.Build(buildTarget, buildOptions);
        }

        private static bool UpdateSetting(BuildTarget buildTarget)
        {
            string path = PathConfig.BuildOuterAssetBundleRootDir(buildTarget) + "/" + PathConfig.VersionCodeFile;
            if (!File.Exists(path))
            {
                Debug.LogError("找不到版本文件");
                return false;
            }
            //读取版本文件
            string text = File.ReadAllText(path);
            var arr = text.Split(',');
            int pkgVersion = 0;
            if (arr.Length <= 0 || !int.TryParse(arr[0], out pkgVersion))
            {
                Debug.LogError("读取版本文件失败");
                return false;
            }
            int.TryParse(arr[0], out pkgVersion);

            PlayerSettings.Android.bundleVersionCode = pkgVersion;

            return true;
        }
    }
}