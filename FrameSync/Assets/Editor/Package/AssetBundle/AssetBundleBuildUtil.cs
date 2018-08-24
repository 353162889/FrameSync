using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace EditorPackage
{
    public class AssetBundleBuildUtil
    {
        [MenuItem("Tools/TestBuildAssetBundle _F1")]
        public static void BuildTest()
        {
            BuildAssetBundle(BuildTarget.StandaloneWindows);
        }
        public static void BuildAssetBundle(BuildTarget buildTarget)
        {
            //清除bundle名称
            ClearAssetBundleNames();
            //设置bundle名称
            SetAssetBundleName();
            //删除所有bundle
            if (!RemoveAlllBundle(buildTarget)) return;
            //打bundle
            if (!BuildAssetBundles(buildTarget)) return;
            //删除资源与bundle的映射文件
            if (!GenerateAssetMapping(buildTarget)) return;
            //删除所有的.manifest结尾的文件
            if (!DeleteAllManifest(buildTarget)) return;
            //清除bundle名称
            ClearAssetBundleNames();
        }

        private static bool GenerateAssetMapping(BuildTarget buildTarget)
        {
            try
            {
                string buildPath = PathConfig.BuildAssetBundleRootDir(buildTarget);
                //更改Manifest文件的名字
                string manifestPath = buildPath + "/" + buildPath.Substring(buildPath.LastIndexOf("/") + 1);
                string targetManifestPath = buildPath + "/" + PathConfig.AssetBundleManifestName;
                if (File.Exists(manifestPath))
                {
                    File.Move(manifestPath, targetManifestPath);
                }

                string assetPathMappingPath = buildPath + "/" + PathConfig.assetPathMappingName;
                if (File.Exists(assetPathMappingPath))
                {
                    File.Delete(assetPathMappingPath);
                }
                //获取映射
                var assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
                Dictionary<string, string> mapping = new Dictionary<string, string>();
                string resDir = PathTools.PathToUnityAssetPath(PathConfig.ResourceRootDir);
                for (int i = 0; i < assetBundleNames.Length; i++)
                {
                    string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleNames[i]);
                    for (int j = 0; j < assetPaths.Length; j++)
                    {
                        if (assetPaths[j].StartsWith(resDir))
                        {
                            string subAssetPath = assetPaths[j].Replace(resDir+"/", "");
                            if (!mapping.ContainsKey(subAssetPath))
                            {
                                mapping.Add(subAssetPath, assetBundleNames[i]);
                            }
                            else
                            {
                                Debug.LogError(subAssetPath + "被打入了多个bundle中");
                            }
                        }
                    }
                }
                //写入xml文件
                XmlDocument document = new XmlDocument();
                XmlDeclaration dec = document.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = document.CreateElement("root");
                document.AppendChild(root);
                XmlElement manifestNode = document.CreateElement("manifest");
                manifestNode.SetAttribute("path", targetManifestPath.Replace(buildPath+"/",""));
                root.AppendChild(manifestNode);
                foreach (var item in mapping)
                {
                    XmlElement assetMappingNode = document.CreateElement("assetmapping");
                    assetMappingNode.SetAttribute("assetpath", item.Key);
                    assetMappingNode.SetAttribute("assetbundlepath", item.Value);
                    root.AppendChild(assetMappingNode);
                }
                XmlWriterSettings setting = new XmlWriterSettings();
                setting.Indent = true;
                setting.Encoding = new UTF8Encoding(false);
                XmlWriter write = XmlWriter.Create(assetPathMappingPath, setting);
                document.Save(write);

                write.Flush();
                write.Close();
                return true;
            }catch(Exception e)
            {
                Debug.LogError("产生资源映射文件失败\n" + e.Message + "\n" + e.StackTrace);
                return false;
            }
        }

        private static bool DeleteAllManifest(BuildTarget buildTarget)
        {
            try
            {
                string buildPath = PathConfig.BuildAssetBundleRootDir(buildTarget);
                List<string> files = new List<string>();
                PathTools.GetAllFiles(buildPath, files, null, "*.manifest", SearchOption.AllDirectories);
                for (int i = 0; i < files.Count; i++)
                {
                    if (File.Exists(files[i]))
                    {
                        File.Delete(files[i]);
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                Debug.LogError("删除manifest文件出错\n"+e.Message+"\n"+e.StackTrace);
                return false;
            }
        }

        private static bool BuildAssetBundles(BuildTarget buildTarget)
        {
            string buildPath = PathConfig.BuildAssetBundleRootDir(buildTarget);
            if (!Directory.Exists(buildPath)) Directory.CreateDirectory(buildPath);
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(buildPath, BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle, buildTarget);
            if (manifest == null)
            {
                Debug.LogError("BuildAssetBundle Fail!");
                return false;
            }
            
            return true;
        }

        private static bool RemoveAlllBundle(BuildTarget buildTarget)
        {
            try
            {
                string path = PathConfig.BuildAssetBundleRootDir(buildTarget);
                PathTools.RemoveDir(path);
                return true;
            }
            catch(Exception e)
            {
                Debug.LogError("删除AssetBundle文件失败!path="+ PathConfig.BuildAssetBundleRootDir(buildTarget) + "\n" +e.Message + "\n" + e.StackTrace);
                return false;
            }
        }

        private static void ClearAssetBundleNames()
        {
            var arr = AssetDatabase.GetAllAssetBundleNames();
            if(arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    AssetDatabase.RemoveAssetBundleName(arr[i], true);
                }
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        private static void SetAssetBundleName()
        {
            try
            {
                AssetBundleFiles.Init();
                var dic = AssetBundleFiles.dicABFile;
                int index = 0;
                foreach (var item in dic)
                {
                    EditorUtility.DisplayProgressBar("AssetBundleFiles", "设置资源的AssetBundleName", (float)index / dic.Count);
                    if (item.Value.hasBundleName)
                    {
                        AssetImporter assetImport = AssetImporter.GetAtPath(item.Key);
                        if (assetImport != null)
                        {
                            assetImport.assetBundleName = item.Value.bundleName;
                        }
                    }
                    index++;
                }
                AssetBundleFiles.Clear();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
