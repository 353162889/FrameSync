using System;
using System.Xml;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CustomizeEditor
{
    /// <summary>
    /// 用来记录所有资源的引用
    /// </summary>
    public class AssetBundleEntryXML
    {

        public static void AddAssetBundleMapping(Dictionary<string,string> mappings)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(AssetBundlePath.GetAssetBundleEntryPath());
                foreach (XmlElement element in document.FirstChild.NextSibling.ChildNodes)
                {
                    if (element.Name == "AssetMappings")
                    {
                        foreach (var map in mappings)
                        {
                            XmlElement targetElement = document.CreateElement("AssetMapping");
                            targetElement.SetAttribute("assetName", map.Key);
                            targetElement.SetAttribute("bundleName", map.Value);
                            element.AppendChild(targetElement);
                        }
                        break;
                    }
                }

                XmlWriterSettings setting = new XmlWriterSettings();
                setting.Indent = true;
                setting.Encoding = new UTF8Encoding(false);
                string path = AssetBundlePath.GetAssetBundleEntryPath();
                XmlWriter write = XmlWriter.Create(path, setting);
                document.Save(write);

                write.Flush();
                write.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static void CreateEntryXmlFile()
        {
            string path = AssetBundlePath.GetAssetBundleEntryPath();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            try
			{
                EditorPlatformPath.CheckDirExistsForFile(path);
                XmlDocument document = new XmlDocument();
                XmlDeclaration dec = document.CreateXmlDeclaration("1.0", "UTF-8", null);

                XmlElement root = document.CreateElement("root");
                document.AppendChild(root);

                XmlElement mapping = document.CreateElement("AssetMappings");
                mapping.SetAttribute("manifest", AssetBundlePath.GetManifestAssetBundlePath());
                root.AppendChild(mapping);

                XmlWriterSettings setting = new XmlWriterSettings();
                setting.Indent = true;
                setting.Encoding = new UTF8Encoding(false);
                XmlWriter write = XmlWriter.Create(path, setting);
                document.Save(write);

                write.Flush();
                write.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
