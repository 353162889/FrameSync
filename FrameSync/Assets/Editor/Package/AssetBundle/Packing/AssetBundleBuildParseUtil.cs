using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace CustomizeEditor
{
    public static class AssetBundleBuildParseUtil
    {
        private static AssetBundleBuildResultInfo resultInfo = new AssetBundleBuildResultInfo();

        public static AssetBundleBuildResultInfo GenarateAssetBundleBuildDatas(AssetbundleBuildSearchGroupInfo[] allGroups)
        {
            resultInfo.Clear();
            foreach(AssetbundleBuildSearchGroupInfo group in allGroups)
            {
                foreach (AssetBundleBuildSearchInfo info in group.buildAssets)
                {
                    HandleBuildInfo(info);
                }
            }
            return resultInfo;
            //return AssetBundleBuildOptimizeUtil.OptimizeAssetBundleBuildDatas(resultInfo);
        }

        private static void HandleBuildInfo(AssetBundleBuildSearchInfo info)
        {
            switch (info.packingType)
            {
                case AssetBundlePackingType.Whole:
                    BundleWhole(info);
                    break;
                case AssetBundlePackingType.SubDir:
                    BundleSubDir(info);
                    break;
                case AssetBundlePackingType.SingleFile:
                    BundleSingleFile(info);
                    break;
                case AssetBundlePackingType.Lua:
                    BundleLuaFile(info);
                    break;
            }
        }

        private static void BundleWhole(AssetBundleBuildSearchInfo info)
        {
            string path = AssetBundlePath.GameAssetsDirectory + info.searchDirectory;
            if (Directory.Exists(path))
            {
                DirectoryInfo dir = Directory.CreateDirectory(path);
                string assetBundleName = info.assetBundleName
                    .Replace("{DirPath}", dir.FullName.Replace("\\", "/"))
                    .Replace("{DirName}", dir.Name)
                    .Replace(AssetBundlePath.GameAssetsDirectory, "");
                AddAssetBundleBuild(info, assetBundleName, dir.GetFiles("*", info.isOnlyTopDir ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories));
            }
            else
            {
                Debug.LogError("[BundleWhole]path not find!path = "+ path);
            }
        }

        private static void BundleSubDir(AssetBundleBuildSearchInfo info)
        {
            string path = AssetBundlePath.GameAssetsDirectory + info.searchDirectory;
            if (Directory.Exists(path))
            {
                DirectoryInfo dir = Directory.CreateDirectory(path);
                foreach(DirectoryInfo subDir in dir.GetDirectories())
                {
                    string assetBundleName = info.assetBundleName
                        .Replace("{SubDirPath}", subDir.FullName.Replace("\\", "/"))
                        .Replace("{SubDirName}", subDir.Name)
                        .Replace(AssetBundlePath.GameAssetsDirectory, "");
                    AddAssetBundleBuild(info, assetBundleName, subDir.GetFiles("*", info.isOnlyTopDir ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories));
                }
            }
            else
            {
                Debug.LogError("[BundleSubDir]path not find!path = " + path);
            }
        }

        private static void BundleSingleFile(AssetBundleBuildSearchInfo info)
        {
            string path = AssetBundlePath.GameAssetsDirectory + info.searchDirectory;
            if (Directory.Exists(path))
            {
                DirectoryInfo dir = Directory.CreateDirectory(path);
                foreach(FileInfo file in dir.GetFiles("*", info.isOnlyTopDir ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories))
                {
                    string assetBundleName = info.assetBundleName
                        .Replace("{FilePath}", file.FullName.Replace("\\", "/"))
                        .Replace(AssetBundlePath.GameAssetsDirectory, "");
                    AddAssetBundleBuild(info, assetBundleName, file);
                }
            }
            else
            {
                Debug.LogError("[BundleSingleFile]path not find!path = " + path);
            }
        }

        private static void BundleLuaFile(AssetBundleBuildSearchInfo info)
        {
            string path = AssetBundlePath.GameAssetsDirectory + info.searchDirectory;
            if (Directory.Exists(path))
            {
                string tempDir = AssetBundlePath.LuaTempDir;
                if (tempDir.EndsWith("/"))
                {
                    tempDir = tempDir.Substring(0,tempDir.Length - 1);
                }
                DirectoryInfo dir = new DirectoryInfo(tempDir);

                //获取到已有的lua文件(因为可能有多个地方copy lua文件到临时目录)
                FileInfo[] preFiles = dir.GetFiles("*", SearchOption.AllDirectories);

                //将当前lua文件拷贝到临时文件中
                string newPath = AssetBundleBuildLuaUtil.CopyLuaFileToTempDirAndRenameExt(path);
                string assetBundleName = info.assetBundleName
                   .Replace("{DirPath}", dir.FullName.Replace("\\", "/"))
                   .Replace("{DirName}", dir.Name)
                   .Replace(AssetBundlePath.GameAssetsDirectory, "");
                FileInfo[] files = dir.GetFiles("*", info.isOnlyTopDir ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
                //排除已有的文件
                List<FileInfo> addFiles = new List<FileInfo>();
                if(files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        if(preFiles != null)
                        {
                            bool isPreFile = false;
                            for (int j = 0; j < preFiles.Length; j++)
                            {
                                if(preFiles[j].FullName == files[i].FullName)
                                {
                                    isPreFile = true;
                                    break;
                                }
                            }
                            if(!isPreFile)
                            {
                                addFiles.Add(files[i]);
                            }
                        }
                        else
                        {
                            addFiles.Add(files[i]);
                        }
                    }
                }
                AddAssetBundleBuild(info, assetBundleName, addFiles.ToArray());
            }
            else
            {
                Debug.LogError("[BundleLuaFile]path not find!path = " + path);
            }
        }

        private static void AddAssetBundleBuild(AssetBundleBuildSearchInfo info, string assetBundleName, params FileInfo[] allAssets)
        {
            HashSet<string> assetPaths = new HashSet<string>();

            foreach (FileInfo asset in allAssets)
            {
                if (info.isValidAsset(asset))
                {
                    if (info.isOnlyMoveFile)
                    {
                        resultInfo.onlyMoveFileList.Add(asset.FullName.Replace("\\", "/").Replace(AssetBundlePath.ProjectDirectory, ""));
                    }
                    else
                    {
                        assetPaths.Add(asset.FullName.Replace("\\", "/").Replace(AssetBundlePath.ProjectDirectory, ""));
                    }
                }
            }
            if (assetPaths.Count > 0)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = assetBundleName.ToLower() + AssetBundlePath.ASSET_BUNDLE_EXTENSION;
                string[] arr = new string[assetPaths.Count];
                assetPaths.CopyTo(arr, 0, assetPaths.Count);
                build.assetNames = arr;
                resultInfo.needBuildList.Add(build);
            }
            assetPaths.Clear();
        }
    }
    
    public class AssetBundleBuildResultInfo
    {
        public HashSet<AssetBundleBuild> needBuildList = new HashSet<AssetBundleBuild>();
        public HashSet<string> onlyMoveFileList = new HashSet<string>();

        public void Clear()
        {
            needBuildList.Clear();
            onlyMoveFileList.Clear();
        }

        public AssetBundleBuild[] ToBuildArray()
        {
            List<AssetBundleBuild> list = new List<AssetBundleBuild>(needBuildList.Count);
            foreach(AssetBundleBuild build in needBuildList)
            {
                if(build.assetNames != null && build.assetNames.Length > 0)
                {
                    list.Add(build);
                }
            }
            AssetBundleBuild[] arr = list.ToArray();
            list.Clear();
            return arr;
        }
    }
}
