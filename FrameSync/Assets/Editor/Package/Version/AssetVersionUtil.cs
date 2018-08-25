using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Text;

namespace EditorPackage
{
    public static class AssetVersionUtil
    {

        /// <summary>
        /// 产生版本文件
        /// </summary>
        /// <param name="pkgVersion">版本号</param>
        /// <param name="rootDir">根目录</param>
        public static void GenerateVersionInfoFile(int pkgVersion, string rootDir)
        {
            rootDir = PathTools.FormatPath(rootDir);
            if (rootDir.EndsWith("/")) rootDir = rootDir.Substring(0, rootDir.Length - 1);
            long hotVersion = GenerateVersionCode();
            WriteVersionCode(rootDir + "/" + PathConfig.VersionCodeFile, pkgVersion, hotVersion);
            var lst = GenerateVersionManifest(rootDir);
            WriteVersionManifest(rootDir + "/" + PathConfig.VersionManifestFile, pkgVersion, hotVersion,lst);
        }

        private static long GenerateVersionCode()
        {
            DateTime dt = DateTime.Now;
            string str = dt.ToString("yyMMddHHmmss");
            return long.Parse(str);
        }

        private static void WriteVersionCode(string path,int pkgVersion,long hotVersion)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(path, pkgVersion.ToString() + "," + hotVersion.ToString());
        }

        private static List<FileVersionInfo> GenerateVersionManifest(string rootDir)
        {
            List<FileVersionInfo> list = new List<FileVersionInfo>();
            FileVersionInfo info = null;
            foreach (string filePath in Directory.GetFiles(rootDir, "*.*", SearchOption.AllDirectories))
            {
                info = FileVersionInfo.Create(filePath, rootDir);
                if(info != null)
                {
                    list.Add(info);
                }
            }
            return list;
        }

        private static void WriteVersionManifest(string path, int pkgVersion,long hotVersion, List<FileVersionInfo> list)
        {
            list.Sort();
            try
            {
                XmlDocument document = new XmlDocument();
                XmlElement root = document.CreateElement("root");
                root.SetAttribute("pkgVersion", pkgVersion.ToString());
                root.SetAttribute("hotVersion", hotVersion.ToString());
                document.AppendChild(root);

                XmlElement file = null;
                foreach(FileVersionInfo info in list)
                {
                    file = document.CreateElement("file");
                    file.SetAttribute("path", info.fileName);
                    file.SetAttribute("md5", info.fileMD5.ToString());
                    file.SetAttribute("size", info.fileSize.ToString());

                    root.AppendChild(file);
                }
                
                XmlWriterSettings setting = new XmlWriterSettings();
                setting.Indent = true;
                setting.OmitXmlDeclaration = true;
                setting.Encoding = Encoding.ASCII;

                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                XmlWriter write = XmlWriter.Create(path, setting);
                document.Save(write);

                write.Flush();
                write.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
   
    public class FileVersionInfo : IComparable<FileVersionInfo>
    {
        public string fileName;
        public string fileMD5;
        public int fileSize;

        public FileVersionInfo() { }

        public static FileVersionInfo Create(string fileFullPath, string fileRootDir)
        {
            fileFullPath = PathTools.FormatPath(fileFullPath);
            if (fileFullPath.EndsWith(".manifest"))
            {
                return null;
            }
            if (fileFullPath.EndsWith(PathConfig.VersionCodeFile) || fileFullPath.EndsWith(PathConfig.VersionManifestFile))
            {
                return null;
            }
            FileVersionInfo info = new FileVersionInfo();
            info.fileMD5 = PathTools.GetFileMD5(fileFullPath);
            FileInfo f = new FileInfo(fileFullPath);
            info.fileSize = (int)f.Length;
            info.fileName = fileFullPath.Replace(fileRootDir + "/", "");
            return info;
        }

        public int CompareTo(FileVersionInfo other)
        {
            return this.fileName.CompareTo(other.fileName);
        }
    }
}
