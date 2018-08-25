using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EditorPackage
{
    //打包需要的路径配置
    //ps:所有目录都最后都不带"/"
    public class PathConfig
    {
        #region 常量文件配置
        public static string CompanyName = "Default";
        public static string ProductName = "FrameSync";
        public static string BundleVersion = "1.0.0";
        //唯一包名
        public static string ApplicationIdentifier = "com.frame.airplane";
        public static string PackageBundleVersion = "1.0.0";
        public static string AssetBundleManifestName = "assetbundle_manifest".ToLower();
        public static string AssetPathMappingName = "assetpath_mapping".ToLower();
        public static string VersionCodeFile = "versioncode";
        public static string VersionManifestFile = "versionmanifest";
        public static string CompressAssetBundleName = "gameasset";
        #endregion

        #region 平台目录路径
        public static Dictionary<BuildTarget, string> DicPlatformName = new Dictionary<BuildTarget, string> {
            { BuildTarget.StandaloneWindows, "Win32"},
            { BuildTarget.Android, "Android"},
            { BuildTarget.iOS, "iOS"},
        };
        public static Dictionary<BuildTarget, string> DicPlatformExt = new Dictionary<BuildTarget, string> {
            { BuildTarget.StandaloneWindows, ".exe"},
            { BuildTarget.Android, ".apk"},
            { BuildTarget.iOS, ".ipa"},
        };
        public static string PlatformDir(string parentDir, BuildTarget buildTarget) { return string.Format("{0}/{1}",parentDir, DicPlatformName[buildTarget]); }

        
        #endregion

        //Assets目录路径
        public static string AssetsRootDir = ParentDir(PathTools.FormatPath(Application.dataPath),0);
        //项目根目录，Assets目录的上层目录
        public static string ProjectRootDir = ParentDir(AssetsRootDir, 1);
        //放置资源路径
        public static string ResourceRootDir = ChildDir(AssetsRootDir,"ResourceEx");
        //StreamingAsset目录
        public static string StreamingAssetDir = PathTools.FormatPath(Application.streamingAssetsPath);


        #region AssetBundle路径（各种路径）

        //打出的AssetBundle放置路径
        public static string BuildOuterAssetBundleRootDir(BuildTarget buildTarget) { return PlatformDir(AssetBundleRootDir(ProjectRootDir), buildTarget); }
        public static string BuildStreamingAssetsRootDir(BuildTarget buildTarget) { return PlatformDir(AssetBundleRootDir(StreamingAssetDir), buildTarget); }
        #endregion
        //AssetBundle放置在目录名为AssetBundles的目录下
        private static string AssetBundleRootDir(string parentDir) { return string.Format("{0}/AssetBundles", parentDir); }

        #region package 打包的路径
        //包的根目录
        public static string BuildPackageRootDir(BuildTarget buildTarget) { return PlatformDir(PackageRootDir(ProjectRootDir), buildTarget); }
        private static string PackageRootDir(string parentDir) { return string.Format("{0}/Packages", parentDir); }
        #endregion

        //使用同一分隔符

        //上层目录
        public static string ParentDir(string dir, int count)
        {
            if (dir.EndsWith("/")) dir = dir.Substring(0, dir.Length - 1);
            if (count <= 0) return dir;
            dir = dir.Substring(0, dir.LastIndexOf("/"));
            return ParentDir(dir, count - 1);
        }

        public static string ChildDir(string dir,string childDir)
        {
            if (dir.EndsWith("/")) dir = dir.Substring(0, dir.Length - 1);
            if (!childDir.StartsWith("/")) childDir = "/" + childDir;
            string resultDir = dir + childDir;
            if(resultDir.EndsWith("/")) resultDir = resultDir.Substring(0, resultDir.Length - 1);
            return resultDir;
        }
    }
}
