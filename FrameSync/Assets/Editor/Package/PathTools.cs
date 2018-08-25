using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace EditorPackage
{
    public static class PathTools
    {
        /// <summary>
        /// 产生MD5
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileMD5(string path)
        {
            using (FileStream get_file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                MD5CryptoServiceProvider get_md5 = new MD5CryptoServiceProvider();
                byte[] hash_byte = get_md5.ComputeHash(get_file);
                get_file.Close();

                string result = System.BitConverter.ToString(hash_byte);
                result = result.Replace("-", "");
                return result;
            }
        }


        public static string GetStringMD5(string str)
        {
            byte[] data = Encoding.GetEncoding("utf-8").GetBytes(str);
            MD5CryptoServiceProvider get_md5 = new MD5CryptoServiceProvider();
            byte[] hash_byte = get_md5.ComputeHash(data);
            string result = "";
            for (int i = 0; i < hash_byte.Length; i++)
            {
                result += hash_byte[i].ToString("x2");
            }
            return result;
        }

        /// <summary>
        /// 一般路径转换为unity资源加载路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string PathToUnityAssetPath(string path)
        {
            path = FormatPath(path);
            return path.Replace(FormatPath(Application.dataPath), "Assets");
        }

        /// <summary>
        /// unity资源路径转换为一般路径
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string UnityAssetPathToPath(string assetPath)
        {
            assetPath = FormatPath(assetPath);
            return assetPath.Replace("Assets", FormatPath(Application.dataPath));
        }

        public static string FormatPath(string path)
        {
            return path.Replace("\\", "/");
        }

        public static void GetAllFiles(string dir, List<string> files, string ignoreDir = null,string searchPattern = "*.*",SearchOption searchOption = SearchOption.TopDirectoryOnly, List<string> excludeExtensions = null)
        {
            if (!Directory.Exists(dir))
            {
                return;
            }
            dir = FormatPath(dir);
            //获取所有文件
            string[] innerFiles = Directory.GetFiles(dir,searchPattern, SearchOption.TopDirectoryOnly);
            if (innerFiles != null)
            {
                for (int i = 0; i < innerFiles.Length; i++)
                {
                    bool add = true;
                    if (excludeExtensions != null)
                    {
                        for (int j = 0; j < excludeExtensions.Count; j++)
                        {
                            if(innerFiles[i].EndsWith(excludeExtensions[j]))
                            {
                                add = false;
                                break;
                            }
                        }
                    }
                    if(add)
                    {
                        files.Add(FormatPath(innerFiles[i]));
                    }
                }
            }

            if (searchOption == SearchOption.AllDirectories)
            {
                //递归获取子目录的文件
                var subdirs = Directory.GetDirectories(dir);
                foreach (var subdir in subdirs)
                {
                    if (string.IsNullOrEmpty(ignoreDir) || subdir != ignoreDir)
                    {
                        GetAllFiles(subdir, files,ignoreDir,searchPattern,searchOption, excludeExtensions);
                    }
                }
            }
        }

        public static void GetDirectories(string dir,List<string> dirs, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (!Directory.Exists(dir))
            {
                return;
            }
            dir = FormatPath(dir);
            string[] innerDirs = Directory.GetDirectories(dir, searchPattern, searchOption);
            if (innerDirs != null)
            {
                for (int i = 0; i < innerDirs.Length; i++)
                {
                    string resultDir = FormatPath(innerDirs[i]);
                    if (resultDir.EndsWith("/")) resultDir = resultDir.Substring(0, resultDir.Length - 1);
                    dirs.Add(resultDir);
                }
            }
        }

        /// <summary>
        /// 通过文件路径获取不带后缀的路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFilePathWithoutExt(string path)
        {
            path = FormatPath(path);
            if (!File.Exists(path))
            {
                Debug.LogError("路径:" + path + "不是一个文件");
                return "";
            }
            path = path.Substring(0, path.LastIndexOf("."));
            return path;
        }

        /// <summary>
        /// 通过路径获取文件名称
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="includeExt">是否包括后缀</param>
        /// <returns></returns>
        public static string GetFileNameForPath(string path,bool includeExt)
        {
            path = FormatPath(path);
            if(!File.Exists(path))
            {
                Debug.LogError("路径:"+path+"不是一个文件");
                return "";
            }
            string file = "";
            int index = path.LastIndexOf("/");
            if (index > -1)
            {
                file = path.Substring(index + 1);
            }
            if(!includeExt)
            {
                file = file.Substring(0, file.LastIndexOf("."));
            }
            return file;
        }
        /// <summary>
        /// 获取文件后缀
        /// </summary>
        /// <param name="file">文件名</param>
        /// <returns></returns>
        public static string GetFileExt(string file)
        {
            string ext = "";
            int index = file.LastIndexOf(".");
            if (index > -1)
            {
                ext = file.Substring(index + 1, file.Length - index - 1);
            }
            return ext;
        }

        /// <summary>
        /// 获取路径获取文件后缀
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string GetFileExtForPath(string path)
        {
            string fileName = GetFileNameForPath(path,true);
            return GetFileExt(fileName);
        }
        /// <summary>
        /// 通过路径获取文件目录
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string GetFileDirForPath(string path)
        {
            path = FormatPath(path);
            if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
            if(Directory.Exists(path))
            {
                return path;
            }
            int index = path.LastIndexOf("/");
            if (index > -1)
            {
                return path.Substring(0,index);
            }
            return path;
        }

        /// <summary>
        /// 将文件夹下所有文件拷贝到另一个文件中
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="searchParttern"></param>
        /// <param name="extension"></param>
        /// <param name="movedFiles"></param>
        public static void CopyToRenameExtension(string from, string to, string searchParttern, string extension, List<string> movedFiles = null)
        {
            if (Directory.Exists(from))
            {
                if (!Directory.Exists(to))
                {
                    Directory.CreateDirectory(to);
                }
                to = FormatPath(to);

                if (to[to.Length - 1] != '/')
                    to = to + "/";
                string[] files = Directory.GetFiles(from, searchParttern);

                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = FormatPath(Path.GetFileName(files[i]));
                    CopyAndRenameExtension(files[i], to + fileName, extension, movedFiles);
                }

                string[] directorys = Directory.GetDirectories(from);
                for (int i = 0; i < directorys.Length; i++)
                {
                    string tempDir = FormatPath(directorys[i]);
                    int index = tempDir.LastIndexOf("/");
                    if (index > -1)
                    {
                        string directoryName = tempDir.Substring(index + 1);
                        CopyToRenameExtension(FormatPath(directorys[i]), to + directoryName, searchParttern, extension, movedFiles);
                    }
                }
            }
            else if (File.Exists(from))
            {
                CopyAndRenameExtension(from, to, extension, movedFiles);
            }
        }

        private static void CopyAndRenameExtension(string src, string desc, string extension, List<string> movedFiles = null)
        {
            desc = FormatPath(desc);
            int index = desc.LastIndexOf(".");
            if (index > -1)
            {
                string prePath = desc.Substring(0, index);
                File.Copy(src, prePath + extension, true);
                if (movedFiles != null)
                {
                    movedFiles.Add(prePath + extension);
                }
            }
        }

        //拷贝文件夹内容到新的文件夹下
        public static void CopyTo(string from, string to,string searchPattern = "*.*", List<string> excludeExtensions = null)
        {
            if (Directory.Exists(from))
            {
                if (!Directory.Exists(to))
                {
                    Directory.CreateDirectory(to);
                }
                to = FormatPath(to);
                if (to[to.Length - 1] != '/')
                    to = to + "/";
                string[] files = Directory.GetFiles(from, searchPattern);

                for (int i = 0; i < files.Length; i++)
                {
                    if(excludeExtensions != null)
                    {
                        bool isExcludeExtension = false;
                        for (int j = 0; j < excludeExtensions.Count; j++)
                        {
                            if(files[i].EndsWith(excludeExtensions[j]))
                            {
                                isExcludeExtension = true;
                                break;
                            }
                        }
                        if (isExcludeExtension) continue;
                    }
                    string fileName = FormatPath(Path.GetFileName(files[i]));
                    File.Copy(FormatPath(files[i]), to + fileName, true);
                }

                string[] directorys = Directory.GetDirectories(from);
                for (int i = 0; i < directorys.Length; i++)
                {
                    string tempDir = FormatPath(directorys[i]);
                    int index = tempDir.LastIndexOf("/");
                    if (index > -1)
                    {
                        string directoryName = tempDir.Substring(index + 1);
                        CopyTo(FormatPath(directorys[i]), to + directoryName, searchPattern,excludeExtensions);
                    }
                }
            }
            else if (File.Exists(from))
            {
                if (excludeExtensions != null)
                {
                    for (int j = 0; j < excludeExtensions.Count; j++)
                    {
                        if (from.EndsWith(excludeExtensions[j]))
                        {
                            return;
                        }
                    }
                }
                File.Copy(from, to, true);
            }
        }

        /// <summary>
        /// 将源文件拷贝到目的目录中
        /// </summary>
        /// <param name="from">当前目录</param>
        /// <param name="to">目的目录</param>
        /// <param name="files">需要拷贝的文件列表</param>
        public static void CopyToPublish(string from, string to, List<string> files)
        {
            if(!Directory.Exists(from))
            {
                return;
            }
            if (!Directory.Exists(to))
            {
                return;
            }
            from = FormatPath(from);
            to = FormatPath(to);
            if (from.EndsWith("/")) from = from.Substring(0, from.Length - 1);
            if (to.EndsWith("/")) to = to.Substring(0,from.Length - 1);
            int count = files.Count;
            for (int i = 0; i < count; ++i)
            {
                string file = files[i];
                file = FormatPath(file);

                string destFile = file.Replace(from, to);
                if (File.Exists(destFile))
                {
                    File.Delete(destFile);
                }

                string destFileDir = GetFileDirForPath(destFile);
                DirectoryInfo info = new DirectoryInfo(destFileDir);
                if (!info.Exists)
                {
                    info.Create();
                }

                File.Copy(file, destFile);
            }
        }

        //更改文件夹下所有文件的扩展名
        public static void RenameExtensionInDirectory(string dir, string searchParttern, string extension)
        {
            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir, searchParttern, SearchOption.AllDirectories);
                foreach (var f in files)
                {
                    string filePath = FormatPath(f);
                    RenameFileExtension(filePath, extension);
                }
            }
        }

        //更改文件扩展名
        public static void RenameFileExtension(string path, string extension)
        {
            if (File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                int index = path.LastIndexOf(".");
                if (index > -1)
                {
                    string prePath = path.Substring(0, index);
                    File.Copy(path, prePath + extension);
                    File.Delete(path);
                }
            }
        }

        public static void RemoveDir(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                return;
            }
            string[] files = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories);
            foreach (var path in files)
            {
                string filePath = FormatPath(path);
                File.Delete(filePath);
            }
            RemoveEmptyDir(dirPath);
        }

        public static void RemoveEmptyDir(string dirPath)
        {
            if (!Directory.Exists(dirPath)) return;
            foreach (string path in Directory.GetDirectories(dirPath))
            {
                RemoveEmptyDir(path);
            }
            if (Directory.GetDirectories(dirPath).Length == 0 && Directory.GetFiles(dirPath, "*").Length == 0)
            {
                Directory.Delete(dirPath, true);
            }
        }

        public static void RemoveFile(string path)
        {
            if (!File.Exists(path)) return;
            File.Delete(path);
        }
    }
}
