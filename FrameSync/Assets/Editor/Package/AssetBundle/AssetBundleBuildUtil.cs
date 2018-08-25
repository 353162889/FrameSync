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
        public static bool BuildAssetBundle(BuildTarget buildTarget,bool md5Name = false)
        {
            //清除bundle名称
            ClearAssetBundleNames();
            //设置bundle名称
            SetAssetBundleName(md5Name);
            //删除所有bundle
            if (!RemoveAlllBundle(buildTarget)) return false;
            //打bundle
            if (!BuildAssetBundles(buildTarget)) return false;
            //删除资源与bundle的映射文件
            if (!GenerateAssetMapping(buildTarget)) return false;
            //删除所有的.manifest结尾的文件
            if (!DeleteAllManifest(buildTarget)) return false;
            //清除bundle名称
            ClearAssetBundleNames();
            Debug.Log("BuildAssetBundle succ");
            return true;
        }

        public static void ClearAssetBundleNames()
        {
            var arr = AssetDatabase.GetAllAssetBundleNames();
            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("AssetBundleFiles", "清除资源的AssetBundleName", (float)i / arr.Length);
                    AssetDatabase.RemoveAssetBundleName(arr[i], true);
                }
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
            EditorUtility.ClearProgressBar();
        }

        public static void SetAssetBundleName(bool md5Name)
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
                            if (md5Name)
                            {
                                assetImport.assetBundleName = PathTools.GetStringMD5(item.Value.bundleName);
                            }
                            else
                            {
                                assetImport.assetBundleName = item.Value.bundleName;
                            }
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

        private static bool GenerateAssetMapping(BuildTarget buildTarget)
        {
            try
            {
                string buildPath = PathConfig.BuildOuterAssetBundleRootDir(buildTarget);
                //更改Manifest文件的名字
                string manifestPath = buildPath + "/" + buildPath.Substring(buildPath.LastIndexOf("/") + 1);
                string targetManifestPath = buildPath + "/" + PathConfig.AssetBundleManifestName;
                if (File.Exists(manifestPath))
                {
                    File.Move(manifestPath, targetManifestPath);
                }

                string assetPathMappingPath = buildPath + "/" + PathConfig.AssetPathMappingName;
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
                document.CreateXmlDeclaration("1.0", "UTF-8", null);
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
                string buildPath = PathConfig.BuildOuterAssetBundleRootDir(buildTarget);
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
            string buildPath = PathConfig.BuildOuterAssetBundleRootDir(buildTarget);
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
                string path = PathConfig.BuildOuterAssetBundleRootDir(buildTarget);
                PathTools.RemoveDir(path);
                return true;
            }
            catch(Exception e)
            {
                Debug.LogError("删除AssetBundle文件失败!path="+ PathConfig.BuildOuterAssetBundleRootDir(buildTarget) + "\n" +e.Message + "\n" + e.StackTrace);
                return false;
            }
        }

        
        //自动加入shader
        private void IncludeShaders()
        {
            List<string> shaders = new List<string>();
            string[] mats = AssetDatabase.FindAssets("t:Material");
            if (mats != null)
            {
                foreach (var item in mats)
                {
                    string path = AssetDatabase.GUIDToAssetPath(item);
                    Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                    if (mat != null && mat.shader != null)
                    {
                        if (!shaders.Contains(mat.shader.name))
                        {
                            shaders.Add(mat.shader.name);
                        }
                    }
                }
            }

            Debug.Log("shaders.count:" + shaders.Count);
            SerializedObject graphicsSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
            SerializedProperty it = graphicsSettings.GetIterator();
            SerializedProperty dataPoint;
            while (it.NextVisible(true))
            {
                if (it.name == "m_AlwaysIncludedShaders")
                {
                    it.ClearArray();
                    if (shaders != null)
                    {
                        for (int i = 0; i < shaders.Count; i++)
                        {
                            it.InsertArrayElementAtIndex(i);
                            dataPoint = it.GetArrayElementAtIndex(i);
                            dataPoint.objectReferenceValue = Shader.Find(shaders[i]);
                        }
                    }
                    graphicsSettings.ApplyModifiedProperties();
                }
            }
            Debug.Log("includeShaders更改完成");
        }
    }
}
