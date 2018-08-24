using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomizeEditor
{
    public class EditorFileOperate
    {
        public static void CopyToRenameExtension(string from, string to, string searchParttern, string extension, List<string> movedFiles = null)
        {
            if (Directory.Exists(from))
            {
                if (!Directory.Exists(to))
                {
                    Directory.CreateDirectory(to);
                }
                to = to.Replace("\\", "/");

                if (to[to.Length - 1] != '/')
                    to = to + "/";
                string[] files = Directory.GetFiles(from,searchParttern);

                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = Path.GetFileName(files[i]).Replace("\\", "/");
                    CopyAndRenameExtension(files[i], to + fileName, extension, movedFiles);
                }

                string[] directorys = Directory.GetDirectories(from);
                for (int i = 0; i < directorys.Length; i++)
                {
                    string tempDir = directorys[i].Replace("\\", "/");
                    int index = tempDir.LastIndexOf("/");
                    if (index > 0)
                    {
                        string directoryName = tempDir.Substring(index + 1);
                        CopyToRenameExtension(directorys[i].Replace("\\", "/"), to + directoryName,searchParttern,extension,movedFiles);
                    }
                }
            }
            else if (File.Exists(from))
            {
                CopyAndRenameExtension(from, to, extension,movedFiles);
            }
        }

        private static void CopyAndRenameExtension(string src,string desc, string extension,List<string> movedFiles = null)
        {
            desc = desc.Replace("\\","/");
            int index = desc.LastIndexOf(".");
            if (index > 0)
            {
                string prePath = desc.Substring(0, index);
                File.Copy(src, prePath + extension,true);
                if(movedFiles != null)
                {
                    movedFiles.Add(prePath + extension);
                }
            }
        }

        //拷贝文件夹内容到新的文件夹下
        public static void CopyTo(string from, string to, string excludeExtension = null)
        {
            if (Directory.Exists(from))
            {
                if (!Directory.Exists(to))
                {
                    Directory.CreateDirectory(to);
                }
                to = to.Replace("\\","/");

                if (to[to.Length - 1] != '/')
                    to = to + "/";
                string[] files = Directory.GetFiles(from);

                for (int i = 0; i < files.Length; i++)
                {
                    if(!string.IsNullOrEmpty(excludeExtension) && files[i].EndsWith(".manifest"))
                    {
                        continue;
                    }
                    string fileName = Path.GetFileName(files[i]).Replace("\\", "/");
                    File.Copy(files[i].Replace("\\", "/"), to + fileName, true);
                }

                string[] directorys = Directory.GetDirectories(from);
                for (int i = 0; i < directorys.Length; i++)
                {
                    string tempDir = directorys[i].Replace("\\", "/");
                    int index = tempDir.LastIndexOf("/");
                    if(index > 0)
                    {
                        string directoryName = tempDir.Substring(index + 1);
                        CopyTo(directorys[i].Replace("\\", "/"), to + directoryName, excludeExtension);
                    }
                }
            }
            else if (File.Exists(from))
            {
                if (!string.IsNullOrEmpty(excludeExtension) && from.EndsWith(excludeExtension))
                {
                    return;
                }
                File.Copy(from, to, true);
            }
        }

        //更改文件夹下所有文件的扩展名
        public static void RenameDirExtension(string path, string searchParttern, string extension)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path, searchParttern, SearchOption.AllDirectories);
                foreach (var f in files)
                {
                    string filePath = f.Replace("\\", "/");
                    RenameFileExtension(filePath, extension);
                }
            }
        }

        //更改文件扩展名
        public static void RenameFileExtension(string path, string extension)
        {
            if(File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                int index = path.LastIndexOf(".");
                if(index > 0)
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
            string[] files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
            foreach (var path in files)
            {
                string filePath = path.Replace("\\", "/");
                File.Delete(filePath);
            }
            RemoveEmptyDir(dirPath);
        }

        public static void RemoveEmptyDir(string dirPath)
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

        public static void RemoveFile(string path)
        {
            if (!File.Exists(path)) return;
            File.Delete(path);
        }
    }


}
