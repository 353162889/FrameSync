using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

namespace CustomizeEditor
{
    public static class AssetBundleBuildUtil
    {
        private static AssetBundleBuildResultInfo buildResultInfo;
        public static bool StartBuildAssetBundle(BuildTarget platform, bool isForceRebuild = false,bool onlyBuildLua = false)
        {
            AssetBundleBuildLuaUtil.CheckAndCreateLuaTempDir();

            buildResultInfo = AssetBundleBuildParseUtil.GenarateAssetBundleBuildDatas(AssetBundleConfig.GetAllGroupInfos());

            //暂时移除所有的已有bundle(即每次重新打包)
            //EditorFileOperate.RemoveDir(AssetBundlePath.GetAssetBundlePath());
            //移除不存在的bundle资源文件与文件夹
            RemoveNonexistAssetBundleFiles();

            AssetBundleBuild[] needBuildFileList = buildResultInfo.ToBuildArray();
            HashSet<string> onlyMoveFileList = buildResultInfo.onlyMoveFileList;
            EditorPlatformPath.CheckDirExists(AssetBundlePath.GetAssetBundlePath());

            AssetBundleBuild[] realBuildFileList = needBuildFileList;
            if(onlyBuildLua)
            {
                List<AssetBundleBuild> tempList = new List<AssetBundleBuild>();
                for (int i = 0; i < needBuildFileList.Length; i++)
                {
                    if(needBuildFileList[i].assetBundleName.IndexOf(AssetBundlePath.LuaBundleNamePre) != -1)
                    {
                        tempList.Add(needBuildFileList[i]);
                    }
                }
                realBuildFileList = tempList.ToArray();
            }
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(AssetBundlePath.GetAssetBundlePath(), realBuildFileList, isForceRebuild ? BuildAssetBundleOptions.ForceRebuildAssetBundle : BuildAssetBundleOptions.None, platform);
            AssetBundleBuildLuaUtil.ClearLuaTempDir();
            bool succ = true;
            if (manifest == null)
            {
                Debug.Log("BuildAssetBundle Fail!");
                return false;
            }
            //如果只打lua文件，不需要移动这些
            if(!onlyBuildLua)
            {
                MoveFiles(onlyMoveFileList);
                MoveManifestAB();
            }
            else
            {
                DeleteManifestAB();
            }
           
            succ = succ && GenerateAssetBundleMappings(needBuildFileList);

            //MoveMainAssetBundle();
            //ChageAssetBundleManifetDependToRelativePath();

            AssetDatabase.Refresh();
            if(succ)
            { 
                Debug.Log("BuildAssetBundle Success!");
            }
            else
            {
                Debug.Log("BuildAssetBundle Fail!");
            }
            return true;
        }

        private static void MoveManifestAB()
        {
            string bundlePath = AssetBundlePath.GetAssetBundlePath();
            bundlePath = bundlePath.Replace("\\","/");
            if(bundlePath.EndsWith("/"))
            {
                bundlePath = bundlePath.Substring(0, bundlePath.Length - 1);
            }
            int index = bundlePath.LastIndexOf("/");
            if (index > 0)
            {
                string manifestBundleName = bundlePath.Substring(index + 1);
                string manifestPath = AssetBundlePath.GetAssetBundlePath() + manifestBundleName;

                string toDir = AssetBundlePath.GetMainAssetBundleDir();
                if(!Directory.Exists(toDir))
                {
                    Directory.CreateDirectory(toDir);
                }
                string toManifestBundlePath = toDir + AssetBundlePath.GetManifestAssetBundleName();
                if(File.Exists(toManifestBundlePath))
                {
                    File.Delete(toManifestBundlePath);
                }
                if(File.Exists(toManifestBundlePath + ".manifest"))
                {
                    File.Delete(toManifestBundlePath + ".manifest");
                }
                File.Move(manifestPath, toManifestBundlePath);
                File.Move(manifestPath + ".manifest",toManifestBundlePath + ".manifest");
            }
        }

        private static void DeleteManifestAB()
        {
            string bundlePath = AssetBundlePath.GetAssetBundlePath();
            bundlePath = bundlePath.Replace("\\", "/");
            if (bundlePath.EndsWith("/"))
            {
                bundlePath = bundlePath.Substring(0, bundlePath.Length - 1);
            }
            int index = bundlePath.LastIndexOf("/");
            if (index > 0)
            {
                string manifestBundleName = bundlePath.Substring(index + 1);
                string manifestPath = AssetBundlePath.GetAssetBundlePath() + manifestBundleName;

                File.Delete(manifestPath);
                File.Delete(manifestPath + ".manifest");
            }
        }

        private static void MoveMainAssetBundle()
        {
            //foreach (FileInfo file in Directory.CreateDirectory(AssetBundlePath.GetAssetBundlePath()).GetFiles("*", SearchOption.TopDirectoryOnly))
            //{
            //    if (file.Name.Contains(AssetBundlePath.QualityName))
            //    {
            //        string mainAssetBundlePath = file.Directory.FullName + "\\" + file.Name.Replace(AssetBundlePath.QualityName, AssetBundlePath.GetMainAssetBundleName());
            //        mainAssetBundlePath = mainAssetBundlePath.Replace("\\", "/");
            //        if (File.Exists(mainAssetBundlePath))
            //        {
            //            File.Delete(mainAssetBundlePath);
            //        }
            //        string dir = Path.GetDirectoryName(mainAssetBundlePath);
            //        if (!Directory.Exists(dir))
            //        {
            //            Directory.CreateDirectory(dir);
            //        }
            //        FileUtil.MoveFileOrDirectory(file.FullName, mainAssetBundlePath);
            //    }
            //}
        }

        private static bool GenerateAssetBundleMappings(AssetBundleBuild[] allBuildInfos)
        {
            AssetBundleEntryXML.CreateEntryXmlFile();
            Dictionary<string, string> mapping = new Dictionary<string, string>();
            foreach (AssetBundleBuild build in allBuildInfos)
            {
                if (AssetBundlePath.IsNeedGenerateMapping(build.assetBundleName) && build.assetNames.Length > 0)
                {
                    foreach (string assetPath in build.assetNames)
                    {
                        //获取截取路径
                        string realAssetPath = AssetBundlePath.GetMappingPath(assetPath);
                        if(mapping.ContainsKey(realAssetPath))
                        {
                            Debug.LogError("存在相同的lua文件路径:"+realAssetPath);
                            return false;
                        }
                        mapping.Add(realAssetPath, build.assetBundleName);
                    }
                }
            }
            if (mapping.Count > 0)
            {
                AssetBundleEntryXML.AddAssetBundleMapping(mapping);
            }
            return true;
        }

        private static void MoveFiles(HashSet<string> moveList)
        {
            foreach (string file in moveList)
            {
               
                string filePath = AssetBundlePath.ProjectDirectory + file;
                string path = AssetBundlePath.GetAssetBundlePath() + filePath.Replace("\\", "/").Replace(AssetBundlePath.GameAssetsDirectory,"").ToLower();
                if (!File.Exists(path))
                {
                    string dir = path.Substring(0, path.LastIndexOf('/'));
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
				}
				FileUtil.DeleteFileOrDirectory(path);
                FileUtil.CopyFileOrDirectory(filePath, path);
            }
        }


        private static void RemoveNonexistAssetBundleFiles()
        {
            string assetBundleDir = AssetBundlePath.GetAssetBundlePath();
            if (!Directory.Exists(assetBundleDir))
            {
                return;
            }
            //把所有会的bundle文件统计出来，再去检索已存在的bundle，把不存在的移除掉
            List<AssetBundleBuildSearchInfo> allAssetBundleInfos = new List<AssetBundleBuildSearchInfo>();
            foreach (AssetbundleBuildSearchGroupInfo group in AssetBundleConfig.GetAllGroupInfos())
            {
                allAssetBundleInfos.AddRange(group.buildAssets);
            }

            HashSet<string> onlyMoveFileList = new HashSet<string>();
            //以下不移除
            onlyMoveFileList.Add(AssetBundlePath.GetAssetBundleEntryPath().Replace(assetBundleDir,""));
            onlyMoveFileList.Add(AssetBundlePath.GetManifestAssetBundlePath());
            onlyMoveFileList.Add(AssetBundlePath.GetManifestAssetBundlePath() + ".manifest");

            HashSet<string> hasFilePaths = new HashSet<string>();
            foreach (string s in buildResultInfo.onlyMoveFileList)
            {
                onlyMoveFileList.Add(s);
            }
            foreach (var build in buildResultInfo.needBuildList)
            {
                hasFilePaths.Add(assetBundleDir + build.assetBundleName);
                hasFilePaths.Add(assetBundleDir + build.assetBundleName + ".manifest");
                hasFilePaths.Add(assetBundleDir + build.assetBundleName + ".meta");
                hasFilePaths.Add(assetBundleDir + build.assetBundleName + ".manifest.meta");
            }
            foreach (string path in onlyMoveFileList)
            {
                string p = path.Replace("\\", "/").Replace(AssetBundlePath.ProjectGameAssetsDirectory, "").ToLower();
                hasFilePaths.Add(assetBundleDir + p);
                hasFilePaths.Add(assetBundleDir + p + ".meta");
            }

            string[] files = Directory.GetFiles(assetBundleDir, "*.*", SearchOption.AllDirectories);
            foreach (string f in files)
            {
                string filePath = f.Replace("\\", "/");
                if (!hasFilePaths.Contains(filePath))
                {
                    Debug.Log("移除不存在的历史Bundle>>" + filePath);
                    File.Delete(filePath);
                }
            }

            EditorFileOperate.RemoveEmptyDir(assetBundleDir);
            AssetDatabase.Refresh();
        }

        private static void ChageAssetBundleManifetDependToRelativePath()
        {
            DirectoryInfo di = new DirectoryInfo(AssetBundlePath.GetAssetBundlePath());
            string projectPath = Application.dataPath.Replace("/Assets", "/");
            StringBuilder sb = new StringBuilder();
            string s;
            foreach (FileInfo file in di.GetFiles("*.manifest", SearchOption.AllDirectories))
            {
                s = File.ReadAllText(file.FullName);
                if (s.Contains(projectPath))
                {
                    sb.Append(s);
                    sb.Replace(projectPath, "");

                    File.WriteAllText(file.FullName, sb.ToString());
                    sb.Length = 0;
                }
            }
            sb.Length = 0;
        }
    }
}
