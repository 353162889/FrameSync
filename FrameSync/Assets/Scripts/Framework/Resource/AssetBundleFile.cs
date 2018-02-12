using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Framework
{ 
    public class AssetBundleFile : MonoBehaviour
    {
        private string mainAssetBundlePath;
        private Dictionary<string, int> assetBundleReferencedCounts = new Dictionary<string, int>();
        private List<AssetBundleManifest> allMmanifestes = new List<AssetBundleManifest>();
        private List<string> allAssetBundles = new List<string>();

        private Dictionary<string, string> assetPathToAssetBundleNames = new Dictionary<string, string>();
        private Dictionary<string, AssetBundleManifest> bundleNameToManifests = new Dictionary<string, AssetBundleManifest>();

        public void Init(string assetBundleEntryXmlContent)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(assetBundleEntryXmlContent);

                foreach (XmlElement element in document.FirstChild.NextSibling.ChildNodes)
                {
                    if (element.Name == "AssetMappings")
                    {
                        mainAssetBundlePath = element.GetAttribute("manifest");
                        foreach (XmlElement mapping in element.ChildNodes)
                        {
                             assetPathToAssetBundleNames.Add(mapping.GetAttribute("assetName"), mapping.GetAttribute("bundleName"));
                        }
                    }
                }

            }
            catch (Exception e)
            {
                CLog.LogError(e.Message + "," +e.StackTrace);
            }
        }

        public string MainAssetBundlePath
        {
            get { return mainAssetBundlePath; }
        }

        public void AddAssetBundleManifest(AssetBundleManifest assetBundleManifest)
        {
            allMmanifestes.Add(assetBundleManifest);
            foreach (string bundleName in assetBundleManifest.GetAllAssetBundles())
            {
                if (!bundleNameToManifests.ContainsKey(bundleName))
                {
                    bundleNameToManifests.Add(bundleName, assetBundleManifest);
                    allAssetBundles.Add(bundleName);
                }

                foreach (string dependPath in assetBundleManifest.GetDirectDependencies(bundleName))
                {
                    if (assetBundleReferencedCounts.ContainsKey(dependPath))
                    {
                        assetBundleReferencedCounts[dependPath]++;
                    }
                    else
                    {
                        assetBundleReferencedCounts[dependPath] = 1;
                    }
                }
            }
        }
        public bool HasReferenced(string assetBundlePath)
        {
            return GetReferencedCount(assetBundlePath) > 0;
        }

        public int GetReferencedCount(string assetBundlePath)
        {
            assetBundlePath = GetAssetBundleNameByAssetPath(assetBundlePath);
            if (assetBundlePath.EndsWith(".unity" + ResourceFileUtil.ASSET_BUNDLE_EXTENSION))
            {
                return 0;
            }
            if (assetBundleReferencedCounts.ContainsKey(assetBundlePath))
            {
                return assetBundleReferencedCounts[assetBundlePath];
            }
            return 0;
        }

        private AssetBundleManifest GetAssetBundleManifest(string assetBundleName)
        {
            assetBundleName = GetAssetBundleNameByAssetPath(assetBundleName);
            if (bundleNameToManifests.ContainsKey(assetBundleName))
            {
                return bundleNameToManifests[assetBundleName];
            }
            return null;
        }

        public List<string> GetAllAssetBundles()
        {
            return allAssetBundles;
        }

        public string GetAssetBundleMD5(string assetBundleName)
        {
            AssetBundleManifest manifest = GetAssetBundleManifest(assetBundleName);
            if (manifest != null)
            {
                return manifest.GetAssetBundleHash(assetBundleName).ToString();
            }
            return null;
        }

        public string[] GetDirectDependencies(string assetBundleName)
        {
            assetBundleName = GetAssetBundleNameByAssetPath(assetBundleName);
            AssetBundleManifest manifest = GetAssetBundleManifest(assetBundleName);
            if (manifest != null)
            {
                string[] paths = manifest.GetDirectDependencies(assetBundleName);

                List<string> list = new List<string>(paths.Length);
                for (int i = 0; i < paths.Length; i++)
                {
                    list.Add(paths[i].Replace(ResourceFileUtil.ASSET_BUNDLE_EXTENSION, ""));
                }
                return list.ToArray();
            }
            return new string[] { };
        }

        public string[] GetAllDependencies(string assetBundleName)
        {
            assetBundleName = GetAssetBundleNameByAssetPath(assetBundleName);
            AssetBundleManifest manifest = GetAssetBundleManifest(assetBundleName);
            if (manifest != null)
            {
                string[] paths = manifest.GetAllDependencies(assetBundleName);
                List<string> list = new List<string>(paths.Length);
                for (int i = 0; i < paths.Length; i++)
                {
                    list.Add(paths[i].Replace(ResourceFileUtil.ASSET_BUNDLE_EXTENSION, ""));
                }
                return paths;
            }
            return new string[] { };
        }

        public string GetAssetBundleNameByAssetPath(string assetPath)
        {
            if (assetPath.EndsWith(ResourceFileUtil.ASSET_BUNDLE_EXTENSION))
            {
                return assetPath;
            }
            if (assetPathToAssetBundleNames.ContainsKey(assetPath))
            {
                return assetPathToAssetBundleNames[assetPath];
            }
            else
            {
                return (assetPath + ResourceFileUtil.ASSET_BUNDLE_EXTENSION).ToLower();
            }
        }
    }
}