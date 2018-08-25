using System;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace EditorPackage
{
    public enum AssetBundlePackingType
    {
        //同样目录下打包优先级 Whole > SubDir > SingleFile
        Whole = 0, 
        SubDir,
        SingleFile,
    }
    public class AssetBundleBuildInfo
    {
        public string searchDirectory;
        public string bundleNameExt;
        public AssetBundlePackingType packingType;
        public string searchPattern;
        public AssetBundleBuildInfo(XmlElement element)
        {
            searchDirectory = element.GetAttribute("searchDirectory").Replace("\\", "/");
            if (searchDirectory.EndsWith("/")) searchDirectory.Substring(0, searchDirectory.Length - 1);
            bundleNameExt = element.GetAttribute("bundleNameExt");
            if (string.IsNullOrEmpty(bundleNameExt)) bundleNameExt = "";
            packingType = (AssetBundlePackingType)Enum.Parse(typeof(AssetBundlePackingType), element.GetAttribute("packingType"));
            searchPattern = element.GetAttribute("searchPattern");
        }
    }

    public class AssetBundleConfig
    {
        private static readonly string CONFIG_PATH = "Assets/Editor/Package/AssetBundle/Config/AssetBundleConfig.xml";
        public static List<AssetBundleBuildInfo> lstBuildInfo = new List<AssetBundleBuildInfo>();
        public static void Init()
        {
            lstBuildInfo.Clear();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(CONFIG_PATH);
            foreach (XmlNode assetElement in xmlDoc.DocumentElement.ChildNodes)
            {
                if (assetElement.Name == "Build")
                {
                    AssetBundleBuildInfo info = new AssetBundleBuildInfo(assetElement as XmlElement);
                    lstBuildInfo.Add(info);
                }
            }
            //按照字符串的长度与是否整个文件夹打包排序（字符串越长的排在前面,如果whole打包，排在前面）
            lstBuildInfo.Sort(Compare);
        }

        public static void Clear()
        {
            lstBuildInfo.Clear();
        }

        private static int Compare(AssetBundleBuildInfo a, AssetBundleBuildInfo b)
        {
            if (a.searchDirectory.Length != b.searchDirectory.Length) return b.searchDirectory.Length - a.searchDirectory.Length;
            return (int)a.packingType - (int)b.packingType;
        }
    }
}
