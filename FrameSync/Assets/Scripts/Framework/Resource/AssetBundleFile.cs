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
        private string m_cMainAssetBundlePath;
        private AssetBundleManifest m_cManifest;
        private Dictionary<string, string> assetPathToAssetBundleNames = new Dictionary<string, string>();

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
                        m_cMainAssetBundlePath = element.GetAttribute("manifest");
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
            get { return m_cMainAssetBundlePath; }
        }

        public string[] GetDirectDependencies(string assetPath)
        {
            string assetBundleName = GetAssetBundleNameByAssetPath(assetPath);
            if (m_cManifest != null)
            {
                string[] paths = m_cManifest.GetDirectDependencies(assetBundleName);
                return paths;
            }
            return new string[] { };
        }


        public string GetAssetBundleNameByAssetPath(string assetPath)
        {
            if (assetPathToAssetBundleNames.ContainsKey(assetPath))
            {
                return assetPathToAssetBundleNames[assetPath];
            }
            else
            {
                return assetPath.ToLower();
            }
        }
    }
}