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
        private Action<bool> m_cCallback;
        private AssetBundleManifest m_cManifest;
        private Dictionary<string, string> assetPathToAssetBundleNames = new Dictionary<string, string>();

        public void Init(string file,Action<bool> callback)
        {
            m_cCallback = callback;
            ResourceSys.Instance.GetResource(file, OnLoadAssetMapping, OnLoadAssetMapping, ResourceType.Text);
        }

        private void OnLoadAssetMapping(Resource res, string path)
        {
            try
            {
                if(res.isSucc)
                {
                    string text = res.GetText();
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(text);
                    string manifestFilePath = "";
                    foreach (XmlElement element in document.FirstChild.NextSibling.ChildNodes)
                    {
                        if(element.Name == "manifest")
                        {
                            manifestFilePath = element.GetAttribute("path");
                        }
                        if (element.Name == "assetmapping")
                        {
                            assetPathToAssetBundleNames.Add(element.GetAttribute("assetpath"), element.GetAttribute("assetbundlepath"));
                        }
                    }
                    ResourceSys.Instance.GetResource(manifestFilePath, OnLoadManifest, OnLoadManifest, ResourceType.AssetBundle);
                }
                else
                {
                    if (m_cCallback != null)
                    {
                        CLog.LogError("AssetBundleFile文件初始化失败!");
                        var callback = m_cCallback;
                        m_cCallback = null;
                        callback.Invoke(false);
                    }
                }
            }
            catch (Exception e)
            {
                CLog.LogError(e.Message + "," + e.StackTrace);
            }
        }

        private void OnLoadManifest(Resource res, string path)
        {
            if(res.isSucc)
            {
                m_cManifest = (AssetBundleManifest)res.GetAsset(null);
            }
            else
            {
                CLog.LogError("AssetBundleFile文件初始化失败!");
            }
            if (m_cCallback != null)
            {
                var callback = m_cCallback;
                m_cCallback = null;
                callback.Invoke(res.isSucc);
            }
        }

        public void Clear()
        {
            m_cCallback = null;
            m_cManifest = null;
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