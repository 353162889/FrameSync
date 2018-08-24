using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace CustomizeEditor
{
    public class PackageAndroidUtil
    {
        public static List<string> ExclusiveFileExtentions = new List<string>(new string[] { ".meta", ".svn", ".manifest" });
        public static List<string> ExclusiveFileNames = new List<string>(new string[] { "android-support", "unity-classes" });
        public static string PublishDir = "Assets/Plugins/Android";
        public static string StreamingAssetsDir = "Assets/StreamingAssets";
        public static string AndroidRootDir = "../AndroidProject";

        public static string SDKResDir = "res";  // all files
        public static string SDKBinDir = "bin";  // jar 
        public static string SDKAssetDir = "assets";
        public static string SDKLibDir = "libs";
        public static string SDKManifest = "AndroidManifest.xml";


        private static void CopyToPublish(string from, string to, List<string> files)
        {
            int count = files.Count;
            for (int i = 0; i < count; ++i)
            {
                string file = files[i];
                file = file.Replace("\\", "/");

                bool isExcusive = false;
                for (int j = 0; j < ExclusiveFileNames.Count; ++j)
                {
                    if (file.Contains(ExclusiveFileNames[j]))
                    {
                        isExcusive = true;
                        break;
                    }
                }
                if (isExcusive)
                {
                    continue;
                }

                int index = file.LastIndexOf(".");
                if (index > -1)
                {
                    string ext = file.Substring(index);
                    if (ExclusiveFileExtentions.Contains(ext))
                    {
                        continue;
                    }
                }

                string destFile = file.Replace(from, to);
                if (File.Exists(destFile))
                {
                    File.Delete(destFile);
                }

                index = destFile.LastIndexOf("/");
                string destFileDir = destFile.Substring(0, index);
                DirectoryInfo info = new DirectoryInfo(destFileDir);
                if (!info.Exists)
                {
                    info.Create();
                }

                File.Copy(file, destFile);
            }

            AssetDatabase.Refresh();
        }

        public static void ClearPublishDir()
        {
            if (Directory.Exists(PublishDir))
            {
                Directory.Delete(PublishDir, true);
            }

            if (Directory.Exists(StreamingAssetsDir))
            {
                Directory.Delete(StreamingAssetsDir, true);
            }
            AssetDatabase.Refresh();
        }

        //[MenuItem("Test/TestCopy")]
        //public static void CopyAndroidToPublish()
        //{
        //    CopyAndroidToPublish("BuYu_AnySDK");

        //}
        public static void CopyAndroidToPublish(string androidName)
        {
            ClearPublishDir();
            string from = AndroidRootDir + "/" + androidName;
            string to = PublishDir;

            if (!Directory.Exists(to))
            {
                Directory.CreateDirectory(to);
            }

            List<string> files = new List<string>();
            //sdk manifest 
            files.Add(from + "/" + SDKManifest);
            //sdk lib
            string libDic = from + "/" + SDKLibDir;
            EditorPlatformPath.GetAllFilesRecursively(libDic, files);
            //sdk bin
            string binDir = from + "/" + SDKBinDir;
            EditorPlatformPath.GetAllFiles(binDir, files, ".jar");
            //sdk assets
            string assetsDir = from + "/" + SDKAssetDir;
            EditorPlatformPath.GetAllFilesRecursively(assetsDir, files);
            //sdk res
            string resDir = from + "/" + SDKResDir;
            EditorPlatformPath.GetAllFilesRecursively(resDir, files);

            CopyToPublish(from, to, files);
        }

    }
}
