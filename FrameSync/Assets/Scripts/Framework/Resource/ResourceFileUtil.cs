using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{ 
    public class ResourceFileUtil : MonoBehaviour
    {
        public static readonly string Win32 = "Win32";
        public static readonly string Mac = "Win32";
        public static readonly string WebPlayer = "WebPlayer";
        public static readonly string Android = "Android";
        public static readonly string iOS = "iOS";

    #if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR_WIN)
            public static string RunPlatform = Win32;
    #elif (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
		    public static string RunPlatform = Mac;
    #elif UNITY_IOS
		    public static string RunPlatform = iOS;
    #elif UNITY_ANDROID
		    public static string RunPlatform = Android;
    #elif UNITY_WEBPLAYER
		    public static string RunPlatform = WebPlayer;
    #endif

        public static string StreamingAssetsPath;
        //内部文件的前缀，StreamingAssets目录
        public static string FILE_SYMBOL;
        //外部文件前缀，如persistentDataPath或网络地址
        public static string OUTER_FILE_SYMBOL;

        //bundle时资源加载路径
        public static string PersistentLoadPath;
       
        private ResourceContainer _resourceContainer;
        //在unity加载资源的根路径
        private string ResRootDir;

        public void Init(string resRootDir)
        {
            ResRootDir = resRootDir;
            if (!ResRootDir.EndsWith("/")) ResRootDir += "/";
#if UNITY_EDITOR
            StreamingAssetsPath = Application.dataPath + "/StreamingAssets/";
            PersistentLoadPath = Application.dataPath + "/../AssetBundles/" + RunPlatform + "/";
            FILE_SYMBOL = @"file://";
            OUTER_FILE_SYMBOL = @"file:///";
#elif (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)
            StreamingAssetsPath = Application.dataPath + "/StreamingAssets/AssetBundles/" + RunPlatform + "/";
            PersistentLoadPath = Application.persistentDataPath + "/AssetBundles/" + RunPlatform + "/";
            FILE_SYMBOL = @"file://";
            OUTER_FILE_SYMBOL = @"file:///";
#elif UNITY_IOS
            StreamingAssetsPath = Application.dataPath+"/Raw/AssetBundles/" + RunPlatform + "/";
            PersistentLoadPath = Application.persistentDataPath + "/AssetBundles/" + RunPlatform + "/";
		    FILE_SYMBOL = "file://";
		    OUTER_FILE_SYMBOL = "file://";
#elif UNITY_ANDROID
            StreamingAssetsPath = Application.dataPath+"!/assets/AssetBundles/" + RunPlatform + "/";
            PersistentLoadPath = Application.persistentDataPath + "/AssetBundles/" + RunPlatform + "/";
		    FILE_SYMBOL = "jar:file://";
		    OUTER_FILE_SYMBOL = "file://";
#endif

            _resourceContainer = gameObject.AddComponentOnce<ResourceContainer>();
        }

        //public string GetRemotePath(string url,string path)
        //{
        //    if(!url.EndsWith("/"))
        //    {
        //        url += "/";
        //    }
        //    if(url.StartsWith("http"))
        //    {
        //        return url + "/AssetBundles/"+ RunPlatform + "/" + path;
        //    }
        //    else
        //    {
        //        return OUTER_FILE_SYMBOL + url + "/AssetBundles/" + RunPlatform + "/" + path;
        //    }
        //}

        /// <summary>
        /// 统一路径格式 unifiedPath = unifiedPath.Replace ('\\', '/');
        /// </summary>
        public string UnifyPath(string unifiedPath)
        {
            unifiedPath = unifiedPath.Replace('\\', '/');
            return unifiedPath;
        }

        /// <summary>
        /// 获取文件后缀名
        /// </summary>
        public string GetFileExtention(string file)
        {
            string extenstion = string.Empty;
            int index = file.LastIndexOf(".");
            if (index > -1)
            {
                extenstion = file.Substring(index);
            }
            return extenstion;
        }
        
        /// <summary>
        /// 获取文件的完整路径
        /// </summary>
        /// <param name="file">内部使用的相对路径</param>
        /// <param name="resType">资源类型</param>
        /// <param name="checkFileSymbol">是否使用前缀(unity中WWW加载资源需要前缀)</param>
        /// <returns></returns>
        public string FullPathForFile(string file, ResourceType resType,bool checkFileSymbol)
        {
            string fullPath;
            if (!_resourceContainer.DirectLoadMode)
            {
                file = file.ToLower();
                //前缀是www才需要的
                if (checkFileSymbol)
                {
                    fullPath = OUTER_FILE_SYMBOL + PersistentLoadPath + file;
                }
                else
                {
                    fullPath = PersistentLoadPath + file;
                }
            }
            else
            {
                if (_resourceContainer.DirInResources)
                {
                    //如果是resource模式加载，移除后缀
                    int index = file.LastIndexOf(".");
                    if (index > -1)
                    {
                        file = file.Substring(0, index);
                    }
                    fullPath = file;
                }
                else
                {
                    fullPath = ResRootDir + file;
                }
            }
            return fullPath;
        }

        /// <summary>
        /// 检查路径是否存在，不存在则创建它
        /// </summary>
        public void CheckDirExists(string dir)
        {
            DirectoryInfo info = new DirectoryInfo(dir);
            if (!info.Exists)
            {
                info.Create();
            }
        }

        /// <summary>
        /// 检查文件路径是否存在，不存在则创建它
        /// </summary>
        public void CheckDirExistsForFile(string file)
        {
            int index = file.LastIndexOf("/");
            string dir = file.Substring(0, index);
            DirectoryInfo info = new DirectoryInfo(dir);
            if (!info.Exists)
            {
                info.Create();
            }
        }

        /// <summary>
        /// 获取文件长度
        /// </summary>
        public long GetFileLength(string fullPath)
        {
            long len = 0;
            if (File.Exists(fullPath))
            {
                using (FileStream fileStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    len = fileStream.Length;
                }
            }
            return len;
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        public void WriteFile(string str, string fullPath)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            WriteFile(bytes, fullPath);
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        public void WriteFile(byte[] bytes, string fullPath)
        {
            DirectoryInfo info = new DirectoryInfo(fullPath.Substring(0, fullPath.LastIndexOf('/')));
            if (!info.Exists)
            {
                info.Create();
            }
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
            }
        }

        /// <summary>
        /// 复制整个目录，包含子目录和文件
        /// </summary>
        public bool CopyDirectory(string srcDir, string destDir)
        {
            try
            { 
                if(!Directory.Exists(srcDir))
                {
                    CLog.LogError("CopyDirectory Error, srcDir not exist!srcDir=" + srcDir);
                    return false;
                }
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                DirectoryInfo srcDirInfo = new DirectoryInfo(srcDir);
                FileInfo[] files = srcDirInfo.GetFiles("*",SearchOption.TopDirectoryOnly);
                foreach (FileInfo file in files)
                {
                    file.CopyTo(Path.Combine(destDir, file.Name));
                }
                DirectoryInfo[] directories = srcDirInfo.GetDirectories();
                foreach (DirectoryInfo dir in directories)
                {
                    CopyDirectory(Path.Combine(srcDir, dir.Name), Path.Combine(destDir, dir.Name));
                }
                return true;
            }
            catch(Exception e)
            {
                CLog.LogError("CopyDirectory Error!srcDir=" + srcDir+ ",destDir=" + destDir+",msg="+e.Message+ ",StackTrace=" + e.StackTrace);
                return false;
            }
        }

        public void RemoveFile(string path)
        {
            try
            { 
                if(!File.Exists(path))
                {
                    return;
                }
                File.Delete(path);
            }catch(Exception e)
            {
                CLog.LogError("Remove File Error!path="+path);
            }
        }

        public void RemoveDir(string dirPath)
        {
            try
            { 
                if (!Directory.Exists(dirPath))
                {
                    return;
                }
                string[] files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
                foreach (var path in files)
                {
                    string filePath = path.Replace("\\", "/");
                    File.Delete(filePath);
                }
                RemoveEmptyDir(dirPath);
            }catch(Exception e)
            {
                CLog.LogError("remove dir error!path="+dirPath);
            }
        }

        public void RemoveEmptyDir(string dirPath)
        {
            foreach (string path in Directory.GetDirectories(dirPath))
            {
                RemoveEmptyDir(path);
            }
            if (Directory.GetDirectories(dirPath).Length == 0 && Directory.GetFiles(dirPath, "*.*").Length == 0)
            {
                Directory.Delete(dirPath, true);
            }
        }
    }
}