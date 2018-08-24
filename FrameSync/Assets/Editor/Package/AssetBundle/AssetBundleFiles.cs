using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EditorPackage
{
    public static class AssetBundleFiles
    {
        public static Dictionary<string, ABFile> dicABFile = new Dictionary<string, ABFile>();

        public static void Init()
        {
            try
            {
                EditorUtility.DisplayProgressBar("AssetBundleFiles", "初始化AssetBundle配置文件", 0);
                dicABFile.Clear();
                //初始化配置的ABFile
                AssetBundleConfig.Init();
                var lst = AssetBundleConfig.lstBuildInfo;
                //lst有保证目录的先后顺序
                for (int i = 0; i < lst.Count; i++)
                {
                    BuildConfigABFile(lst[i]);
                }
                AssetBundleConfig.Clear();

                //查找所有ABFile的引用
                string[] files = dicABFile.Keys.ToArray();
                for (int i = 0; i < files.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("AssetBundleFiles", "查找AssetBundle需要的资源", (float)i / files.Length);
                    SearchABFile(files[i]);
                }
                //更新所有ABFile的bundle名称，如果只有一个引用，bundleName为空
                foreach (var item in dicABFile)
                {
                    if (!item.Value.hasBundleName)
                    {
                        if (item.Value.refByFiles.Count > 1)
                        {
                            item.Value.bundleName = PathTools.GetFilePathWithoutExt(item.Key);
                        }
                    }
                }
            }finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static void Clear()
        {
            dicABFile.Clear();
        }

        private static void SearchABFile(string assetFile)
        {
            string[] dependencies = AssetDatabase.GetDependencies(assetFile,true);
            if(dependencies != null)
            {
                for (int i = 0; i < dependencies.Length; i++)
                {
                    if (!dependencies[i].EndsWith(".cs") && !dependencies[i].EndsWith(".meta"))
                    {
                        ABFile abFile;
                        if (dicABFile.TryGetValue(dependencies[i], out abFile))
                        {
                            //找出来的依赖有可能是自己
                            if (assetFile != dependencies[i] && !abFile.isRefByFile(assetFile))
                            {
                                abFile.refByFiles.Add(assetFile);
                            }
                        }
                        else
                        {
                            abFile = new ABFile();
                            abFile.refByFiles.Add(assetFile);
                            dicABFile.Add(dependencies[i], abFile);
                            SearchABFile(dependencies[i]);
                        }
                    }
                }
            }
        }

        private static void BuildConfigABFile(AssetBundleBuildInfo buildInfo)
        {
            string bundleName = "";
            List<string> dirs = new List<string>();
            List<string> files = new List<string>();
            switch (buildInfo.packingType)
            {
                case AssetBundlePackingType.Whole:
                    files.Clear();
                    PathTools.GetAllFiles(PathTools.UnityAssetPathToPath(buildInfo.searchDirectory), files, null, "*.*", SearchOption.AllDirectories, new List<string> { ".meta"});
                    bundleName = buildInfo.searchDirectory + "_" + buildInfo.packingType.ToString() + "_" + buildInfo.bundleNameExt;
                    for (int i = 0; i < files.Count; i++)
                    {
                        AddConfigFileABFile(files[i], bundleName);
                    }
                    break;
                case AssetBundlePackingType.SubDir:
                    dirs.Clear();
                    PathTools.GetDirectories(PathTools.UnityAssetPathToPath(buildInfo.searchDirectory), dirs, "*", SearchOption.TopDirectoryOnly);
                    for (int i = 0; i < dirs.Count; i++)
                    {
                        bundleName = PathTools.PathToUnityAssetPath(dirs[i]) + "_" + buildInfo.packingType.ToString() + "_" + buildInfo.bundleNameExt;
                        files.Clear();
                        PathTools.GetAllFiles(dirs[i], files, null, "*.*", SearchOption.AllDirectories, new List<string> { ".meta" });
                        for (int j = 0; j < files.Count; j++)
                        {
                            AddConfigFileABFile(files[j], bundleName);
                        }
                    }
                    break;
                case AssetBundlePackingType.SingleFile:
                    files.Clear();
                    PathTools.GetAllFiles(PathTools.UnityAssetPathToPath(buildInfo.searchDirectory), files, null, "*.*", SearchOption.AllDirectories, new List<string> { ".meta" });
                    for (int i = 0; i < files.Count; i++)
                    {
                        string curFile = PathTools.PathToUnityAssetPath(files[i]);
                        string pathWithoutExt = PathTools.GetFilePathWithoutExt(curFile);
                        bundleName = pathWithoutExt + "_" + buildInfo.packingType.ToString() + "_" + buildInfo.bundleNameExt;
                        AddConfigFileABFile(files[i], bundleName);
                    }
                    break;
            }
        }

        private static void AddConfigFileABFile(string file,string bundleName)
        {
            if(string.IsNullOrEmpty(bundleName))
            {
                Debug.LogError("文件"+file+"的bundleName不能为空");
                return;
            }
            file = PathTools.FormatPath(file);
            string assetFile = PathTools.PathToUnityAssetPath(file);
            ABFile abFile;
            if(!dicABFile.TryGetValue(assetFile, out abFile))
            {
                abFile = new ABFile();
                abFile.bundleName = bundleName;
                dicABFile.Add(assetFile, abFile);
            }
        }
    }

    public class ABFile
    {
        //被那些文件引用
        public List<string> refByFiles = new List<string>();
        //打包的bundle名称
        public string bundleName = "";

        public bool hasBundleName { get { return !string.IsNullOrEmpty(bundleName); } }
        public bool isRefByFile(string file)
        {
            for (int i = 0; i < refByFiles.Count; i++)
            {
                if (refByFiles[i] == file) return true;
            }
            return false;
        }

    }
}
