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


namespace CustomizeEditor
{
    public enum AssetBundlePackingType
    {
        SubDir,
        Whole,
        SingleFile,
        Lua,
    }

    public class AssetbundleBuildSearchGroupInfo
    {
        public readonly string groupName;
        public readonly AssetBundleBuildSearchInfo[] buildAssets;

        public AssetbundleBuildSearchGroupInfo(XmlElement element)
        {
            groupName = element.GetAttribute("groupName");

            List<AssetBundleBuildSearchInfo> list = new List<AssetBundleBuildSearchInfo>();
            foreach (XmlNode assetElement in element.ChildNodes)
            {
                if (assetElement.Name == "Asset")
                {
                    string[] searchDirectories = (assetElement as XmlElement).GetAttribute("searchDirectory").Split(',');
                    foreach (XmlNode buildElement in assetElement.ChildNodes)
                    {
                        if (buildElement.Name == "Build")
                        {
                            foreach (string searchDirectory in searchDirectories)
                            {
                                list.Add(new AssetBundleBuildSearchInfo(searchDirectory, buildElement as XmlElement));
                            }
                        }
                    }
                }
            }
            buildAssets = list.ToArray();
        }
    }
    public class AssetBundleBuildSearchInfo
    {
        public readonly string searchDirectory;
        public readonly AssetBundlePackingType packingType;
        public readonly bool isOnlyMoveFile;
        public readonly bool isOnlyTopDir;
        public readonly string assetBundleName;
        public readonly string[] assetFilters;
        public readonly string mainAssetType;
        public readonly string[] ignoreDirNames;

        public bool isValidAsset(FileInfo asset)
        {
            if(ignoreDirNames != null && ignoreDirNames.Length > 0)
            {
                string fullName = asset.FullName.Replace("\\", "/");
                foreach(string dirName in ignoreDirNames)
                {
                    if(fullName.Contains("/" + dirName + "/"))
                    {
                        return false;
                    }
                }
            }
            string fileName = asset.Name.ToLower();
            bool isFind = false;
            if(assetFilters == null)
            {
                isFind = true;
            }
            else
            { 
                foreach (string s in assetFilters)
                {
                    if (fileName.EndsWith(s.ToLower()))
                    {
                        isFind = true;
                        break;
                    }
                }
            }
            return isFind;
        }

        public bool isMainAsset(string assetPath)
        {
            if (mainAssetType != null && mainAssetType != "")
            {
                return new FileInfo(assetPath).Extension.ToLower() == mainAssetType;
            }
            return false;
        }

        public AssetBundleBuildSearchInfo(string searchDirectory, XmlElement element)
        {
            this.searchDirectory = searchDirectory;
            string st = element.GetAttribute("packingType");
            if (st == "Whole")
            {
                this.packingType = AssetBundlePackingType.Whole;
                this.assetBundleName = "{DirPath}/{DirName}";
            }
            else if (st == "SubDir")
            {
                this.packingType = AssetBundlePackingType.SubDir;
                this.assetBundleName = "{SubDirPath}/{SubDirName}";
            }
            else if (st == "SingleFile")
            {
                this.packingType = AssetBundlePackingType.SingleFile;
                this.assetBundleName = "{FilePath}";
            }
            else if(st == "Lua")
            {
                this.packingType = AssetBundlePackingType.Lua;
                this.assetBundleName = "{DirPath}/{DirName}";
            }
            else
            {
                throw new Exception("Unknown AssetBundleBuildSearchType " + st);
            }
            this.isOnlyMoveFile = element.GetAttribute("onlyMoveFile") == "true";
            this.isOnlyTopDir = element.GetAttribute("onlyTopDir") == "true";

            if(element.GetAttribute("assetBundleName") != "")
            {
                this.assetBundleName = element.GetAttribute("assetBundleName");
            }
            if(element.GetAttribute("ignoreDirNames") != "")
            {
                this.ignoreDirNames = element.GetAttribute("ignoreDirNames").Split(',');
            }
            string filters = element.GetAttribute("assetFilters");
            if (filters != null && filters != null)
            {
                this.assetFilters = filters.Split(',');
            }

            string mainType = element.GetAttribute("mainAssetType");
            if (mainType != null && mainType != "")
            {
                this.mainAssetType = mainType;
            }
        }
    }

    public static class AssetBundleConfig
    {
        public static readonly string CONFIG_PATH = "Assets/Editor/AssetBundle/Config/AssetBundleConfig.xml";
        private static Dictionary<string, AssetbundleBuildSearchGroupInfo> groups = new Dictionary<string, AssetbundleBuildSearchGroupInfo>();
        public static void Init()
        {
            groups.Clear();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(CONFIG_PATH);
            foreach (XmlNode assetElement in xmlDoc.DocumentElement.ChildNodes)
            {
                if (assetElement.Name == "PackingGroup")
                {
                    AssetbundleBuildSearchGroupInfo info = new AssetbundleBuildSearchGroupInfo(assetElement as XmlElement);
                    groups.Add(info.groupName, info);
                }
            }
        }


        public static AssetbundleBuildSearchGroupInfo[] GetAllGroupInfos()
        {
            List<AssetbundleBuildSearchGroupInfo> list = new List<AssetbundleBuildSearchGroupInfo>();
            foreach (var info in groups)
            {
                list.Add(info.Value);
            }
            return list.ToArray();
        }

        public static AssetbundleBuildSearchGroupInfo[] GetAssetbundleBuildGroupInfos(string groupName)
        {
            List<AssetbundleBuildSearchGroupInfo> list = new List<AssetbundleBuildSearchGroupInfo>();
            foreach(var info in groups)
            {
                if(groupName == info.Key)
                {
                    list.Add(info.Value);
                }
            }
            return list.ToArray();
        }
    }
}
