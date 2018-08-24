using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CustomizeEditor
{
    public class PackageAssetBundleUtil
    {
        public static void CheckAndCreateAssetBundleDir()
        {
            string path = PackagePath.StreamingAssetABDir;
            if (Directory.Exists(path))
            {
                EditorFileOperate.RemoveDir(path);
            }
            Directory.CreateDirectory(path);
        }

        public static void ClearAssetBundleDir()
        {
            Debug.Log("移除streamingAssets目录");
            EditorFileOperate.RemoveDir(PackagePath.StreamingAssetABDir);
        }

        public static bool CopyABToStreamingAssetDir()
        {
            string from = PackagePath.GetOutAssetBundleDir();
            if(!Directory.Exists(from))
            {
                Debug.LogError("can not find ab dir:path =" + from);
                return false;
            }
            string to = PackagePath.GetStreamingAssetABDir();
            EditorFileOperate.CopyTo(from, to, ".manifest");
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            return true;
        }

        public static bool CompressABToStreamingAssetDir()
        {
            Debug.Log("正在压缩bundle到StreamingAssets目录中...");
            string from = PackagePath.GetOutAssetBundleDir();
            if (!Directory.Exists(from))
            {
                Debug.LogError("can not find ab dir:path =" + from);
                return false;
            }
           
            string toDir = PackagePath.GetStreamingAssetABDir();
            if(!Directory.Exists(toDir))
            {
                Directory.CreateDirectory(toDir);
            }
            string to = PackagePath.GetStreamingAssetABZipFilePath();
            try
            {
                //压缩时，会将/../这些东西打到压缩文件，导致解压时会有问题（拿文件相对目录时）
                from = new DirectoryInfo(from).FullName;
                //LaunchCompress.CompressDirExcept(from, to, ".manifest");
            }
            catch (Exception e)
            {
                Debug.LogError("compress bundle fail!msg:" + e.Message+ ",StackTrace:" + e.StackTrace);
                return false;
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            return true;
        }

        //将打包的版本文件拷贝到Resource目录下（用来与包外版本号对比用，发生未知错误时的判定）
        public static bool CopyVersionTxtToResourceDir()
        {
            string from = PackagePath.GetOutVersionFile();
            string to = PackagePath.ResourceVersionFile;
            if(!File.Exists(from))
            {
                Debug.LogError("can not find version file:path="+from);
                return false;
            }
            EditorFileOperate.CopyTo(from, to, null);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            return true;
        }

        public static void ClearResourceVersionFileDir()
        {
            EditorFileOperate.RemoveFile(PackagePath.ResourceVersionFile);
        }
    }
}
