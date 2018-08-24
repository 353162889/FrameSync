using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomizeEditor
{
    public class PackagePath
    {
        public static string PackageRoot = Application.dataPath + "/../..";
        private static string PackageRootDir = "{0}/Release/{1}/";

        public static string PlatformName;
        public static string PackageExtension;

        private static string PackageName = "{0}_{1}_{2}{3}";

        //lua源代码的路径
        public static string[] LuaSouceCodePaths = new string[] {
            Application.dataPath +"/Lua",
            Application.dataPath +"/ToLua/Lua",
        };
        public readonly static string GameAssetsDirectory = Application.dataPath + "/Resources/";
        //resource模式需要将lua拷贝到的路径下
        public readonly static string ResourceLuaCodePath = Application.dataPath + "/Resources/LuaCode/";
        public readonly static string ResourceLuaCodeFileCfgPath = ResourceLuaCodePath + "LuaCodes.bytes";

        public readonly static string OutABDir = Application.dataPath + "/../AssetBundles/";  //已打好的bundle目录
        public readonly static string StreamingAssetABDir = Application.streamingAssetsPath + "/AssetBundles/"; //将bundle拷贝的目录

        public readonly static string OutVersionFile = "version/versioncode.txt";

        public readonly static string ResourceVersionFile = Application.dataPath + "/Resources/Launch/versioncode.txt";

        //压缩bundle的名称
        public readonly static string CompressBundlesFileName = "bundle.zip";
        public readonly static string[] sceneNames = new string[] {
            "Assets/3DFishing.unity",
            "Assets/EmptyScene.unity"
        };

        public static string GetPackageDir()
        {
            return string.Format(PackageRootDir, PackageRoot, PlatformName);
        }

        public static string GetPackageName(PackageType packageType)
        {
            DateTime currTime = DateTime.Now;
            return string.Format(PackageName,currTime.ToString("yyyyMMdd"),currTime.ToString("HHmm"),packageType.ToString(),PackageExtension);
        }

        //获取到打出包的路径名（文件）
        public static string GetPackagePath(PackageType packageType)
        {
            return GetPackageDir() + packageType.ToString() + "/" + GetPackageName(packageType);
        }
        //获取到打出包的路径名（目录）
        public static string GetPackagePathDir(PackageType packageType)
        {
            return GetPackageDir() + packageType.ToString() + "/";
        }

        public static string GetOutAssetBundleDir()
        {
            return OutABDir + PlatformName + "/";
        }

        public static string GetStreamingAssetABDir()
        {
            return StreamingAssetABDir + PlatformName + "/";
        }

        public static string GetStreamingAssetABZipFilePath()
        {
            return StreamingAssetABDir + PlatformName + "/"+ CompressBundlesFileName;
        }

        public static string GetOutVersionFile()
        {
            return GetOutAssetBundleDir() + OutVersionFile;
        }

    }
}
