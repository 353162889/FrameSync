using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

namespace EditorPackage
{
    public static class PackageUtil
    {
        public static void Build(BuildTarget buildTarget, BuildOptions buildOptions)
        {
            PlayerSettings.companyName = PathConfig.CompanyName;
            PlayerSettings.productName = PathConfig.ProductName;
            PlayerSettings.applicationIdentifier = PathConfig.ApplicationIdentifier;
            PlayerSettings.bundleVersion = PathConfig.BundleVersion;
            //删除StreamingAsset目录中所有东西
            ClearStreamingAssetDir();
            //将AssetBundle资源压缩并拷贝到StreamingAsset目录中
            CompressABToStreamingAssetDir(buildTarget);
            //构建包
            BuildPackage(buildTarget,buildOptions);
            //删除StreamingAsset目录中所有东西
            ClearStreamingAssetDir();
        }

        private static void BuildPackage(BuildTarget buildTarget, BuildOptions buildOptions)
        {
            string packageName = string.Format("{0}_v{1}_{2}_{3}{4}", PlayerSettings.productName,PlayerSettings.bundleVersion, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmm"), PathConfig.DicPlatformExt[buildTarget]);
            string path = PathConfig.BuildPackageRootDir(buildTarget) + "/" + packageName;
            BuildPipeline.BuildPlayer(GetAllBuildScenes(), path, buildTarget, buildOptions);
        }

        //删除StreamingAsset目录中所有东西
        public static void ClearStreamingAssetDir()
        {
            PathTools.RemoveDir(PathConfig.StreamingAssetDir);
            AssetDatabase.Refresh();
        }

        public static bool CompressABToStreamingAssetDir(BuildTarget buildTarget)
        {
            try
            {
                EditorUtility.DisplayCancelableProgressBar("Package", "正在压缩AssetBundle", 0);
                string streamingAssetDir = PathConfig.BuildStreamingAssetsRootDir(buildTarget);
                if (!Directory.Exists(streamingAssetDir)) Directory.CreateDirectory(streamingAssetDir);
                string compressPath = streamingAssetDir + "/" + PathConfig.CompressAssetBundleName;
                CompressTools.CompressDir(PathConfig.BuildOuterAssetBundleRootDir(buildTarget), compressPath);
                AssetDatabase.Refresh();
                return true;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static string[] GetAllBuildScenes()
        {
            List<string> names = new List<string>();
            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
            {
                if (e == null)
                    continue;
                if (e.enabled)
                {
                    names.Add(e.path);
                }
            }
            return names.ToArray();
        }

    }
}