using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace CustomizeEditor
{
    public enum PackageType
    {
        Resource,
        AssetBundle
    }

    public class PackageUtil
    {
        public static void Package(PackageType packageType, BuildTarget buildTarget,BuildOptions buildOptions)
        {
            if(packageType == PackageType.Resource)
            {
                BuildResource(buildTarget, buildOptions);
            }
            else if(packageType == PackageType.AssetBundle)
            {
                BuildAssetBundle(buildTarget, buildOptions);
            }
            Debug.Log("Build package finish!");
        }

        private static void BuildResource(BuildTarget buildTarget,BuildOptions buildOptions)
        {
            //新建resource文件下的Lua代码目录
            PackageResourceLuaUtil.CheckAndCreateLuaTempDir();
            //将lua文件拷贝到Resource下
            PackageResourceLuaUtil.CopyLuaFileToResourceDirAndRenameExt();
            //打包
            string packageName = PackagePath.GetPackagePath(PackageType.Resource);
            Directory.CreateDirectory(packageName);
            BuildPipeline.BuildPlayer(GetAllBuildScenes(), packageName, buildTarget, buildOptions);
            //最后删除临时目录
            PackageResourceLuaUtil.ClearLuaTempDir();
            AssetDatabase.Refresh();

            //打开包所在的文件夹
            System.Diagnostics.Process.Start(PackagePath.GetPackagePathDir(PackageType.Resource));
        }

        private static void BuildAssetBundle(BuildTarget buildTarget,BuildOptions buildOptions)
        {
            //将所有的bundle文件拷贝到streamingasset文件
            PackageAssetBundleUtil.CheckAndCreateAssetBundleDir();
            //将assetbundle压缩到streamingAsset
            //if(!PackageAssetBundleUtil.CopyABToStreamingAssetDir())
            if(!PackageAssetBundleUtil.CompressABToStreamingAssetDir())
            {
                PackageAssetBundleUtil.ClearAssetBundleDir();
                AssetDatabase.Refresh();
                return;
            }
            //将版本文件拷贝到Resource目录
            if(!PackageAssetBundleUtil.CopyVersionTxtToResourceDir())
            {
                PackageAssetBundleUtil.ClearAssetBundleDir();
                AssetDatabase.Refresh();
                return;
            }
            //打包
            string packageName = PackagePath.GetPackagePath(PackageType.AssetBundle);
            Directory.CreateDirectory(packageName);
            Debug.Log("packageName:"+ packageName);

            BuildPipeline.BuildPlayer(PackagePath.sceneNames,packageName, buildTarget, buildOptions);

            //删除streamingAsset中的bundle目录
            PackageAssetBundleUtil.ClearAssetBundleDir();
            //删除拷贝到Resources目录的版本文件
            PackageAssetBundleUtil.ClearResourceVersionFileDir();
            AssetDatabase.Refresh();

            //打开包所在的文件夹
            System.Diagnostics.Process.Start(PackagePath.GetPackagePathDir(PackageType.AssetBundle));
        }

        public static string[] GetAllBuildScenes()
        {
            List<string> names = new List<string>();
            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
            {
                if (e == null)
                    continue;
                if (e.enabled)
                {
                    names.Add(e.path);
                }
            }
            return names.ToArray();
        }
    }
}
