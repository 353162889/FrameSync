using System;
using System.Xml;
using UnityEditor;
using System.IO;
using UnityEngine;

namespace CustomizeEditor
{
    public class AssetBundlePath
    {
        public readonly static string ASSET_BUNDLE_EXTENSION = ".assetbundle";

        public readonly static string GameAssetsDirectory = Application.dataPath + "/Resources/";
		public readonly static string ProjectGameAssetsDirectory = "Assets/Resources/";
        public readonly static string ProjectDirectory = Application.dataPath.Replace("Assets", "");
        public readonly static string ProjectDirWithAsset = Application.dataPath + "/";

        public readonly static string LuaTempDir = Application.dataPath + "/Resources/LuaCode/";
        public readonly static string LuaBundleNamePre = "luacode";

        private static string assetBundlePath = "{0}/AssetBundles/{1}/";
        private static string versionCodePath = "{0}/AssetBundles/{1}/VersionCode.txt";

        private static string mainAssetBundleDir = "{0}/AssetBundles/{1}/asset_bundle_config/";                          //游戏资源的配置目录，放置manifest文件，以及资源路径对bundle的映射文件
        private static string manifestAssetBundleName = "manifest_asset_bundles" + ASSET_BUNDLE_EXTENSION;
        private static string manifestAssetPath = "asset_bundle_config/" + manifestAssetBundleName;

        private static string assetBundleEntryPath = "{0}/AssetBundles/{1}/asset_bundle_entry.xml";

        private static string[] MappingCutOutStartWith = new string[] {
            "Assets/Resources/"
        };

        public static string PackingPlatformName
        {
            get;
            set;
        }

        private static string _exportAssetBundleDirectory = Application.dataPath + "/..";
        public static string ExportAssetBundleDirectory
        {
            get { return _exportAssetBundleDirectory; }
            set { _exportAssetBundleDirectory = value; }
        }
        public static string GetAssetBundlePath()
        {
            return GetAssetBundlePath(PackingPlatformName);
        }
        public static string GetAssetBundlePath(string platformName)
        {
            return Path.GetFullPath(string.Format(assetBundlePath, ExportAssetBundleDirectory, platformName)).Replace("\\", "/");
        }
        public static string GetVersionCodePath()
        {
            return GetVersionCodePath(PackingPlatformName);
        }
        public static string GetVersionCodePath(string platformName)
        {
            return Path.GetFullPath(string.Format(versionCodePath, ExportAssetBundleDirectory, platformName)).Replace("\\", "/");
        }
        public static string GetAssetBundleEntryPath()
        {
            return GetAssetBundleEntryPath(PackingPlatformName);
        }
        public static string GetAssetBundleEntryPath(string platformName)
        {
            return Path.GetFullPath(string.Format(assetBundleEntryPath, ExportAssetBundleDirectory, platformName)).Replace("\\", "/");
        }
        public static string GetManifestAssetBundleName()
        {
            return manifestAssetBundleName;
        }

        public static string GetManifestAssetBundlePath()
        {
            return manifestAssetPath;
        }

        public static string GetMainAssetBundleDir()
        {
            return GetMainAssetBundleDir(PackingPlatformName);
        }

        private static string GetMainAssetBundleDir(string platformName)
        {
            return Path.GetFullPath(string.Format(mainAssetBundleDir, ExportAssetBundleDirectory, platformName)).Replace("\\","/");
        }

        public static bool IsNeedGenerateMapping(string assetBundleName)
        {
            return true;
        }

        public static string GetMappingPath(string assetPath)
        {
            for (int i = 0; i < MappingCutOutStartWith.Length; i++)
            {
                if (assetPath.StartsWith(MappingCutOutStartWith[i]))
                {
                    return assetPath.Replace(MappingCutOutStartWith[i],"");
                }
            }
            return assetPath;
        }
    }
}
