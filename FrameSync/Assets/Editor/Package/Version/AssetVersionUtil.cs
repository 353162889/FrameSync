using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
//using Framework;
using System.Xml;
using System.Text;

namespace CustomizeEditor
{
    public static class AssetVersionUtil
    {
        private static string VersionCodeFile = "/version/versioncode.txt";
        private static string VersionManifestFile = "/version/versionmanifest.xml";
        private static string AssetRootDirectory;

        public static void GenerateVersionInfoFile(string assetRootDirectory)
        {
            //GameConfig.Load();
            //AssetRootDirectory = assetRootDirectory;
            //long hotVersion = GenerateVersionCode();
            //WriteVersionCode(GameConfig.PkgVersion, hotVersion);
            //List<FileVersionInfo> list = GenerateVersionManifest();
            //WriteVersionManifest(GameConfig.ShowVersion, GameConfig.PkgVersion, hotVersion, GenerateVersionManifest());
        }

        private static long GenerateVersionCode()
        {
            DateTime dt = DateTime.Now;
            string str = dt.ToString("yyMMddHHmmss");
            return long.Parse(str);
        }

        private static void WriteVersionCode(int pkgVersion,long hotVersion)
        {
            string path = AssetRootDirectory + VersionCodeFile;
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(path, pkgVersion.ToString() + "," + hotVersion.ToString());
        }

        private static List<FileVersionInfo> GenerateVersionManifest()
        {
            List<FileVersionInfo> list = new List<FileVersionInfo>();
            FileVersionInfo info = null;
            foreach(string filePath in Directory.GetFiles(AssetRootDirectory, "*.*", SearchOption.AllDirectories))
            {
                info = FileVersionInfo.Create(filePath, AssetRootDirectory);
                if(info != null)
                {
                    list.Add(info);
                }
            }
            return list;
        }

        private static void WriteVersionManifest(string showVersion, int pkgVersion,long hotVersion, List<FileVersionInfo> list)
        {
            list.Sort();
            try
            {
                XmlDocument document = new XmlDocument();
                XmlElement root = document.CreateElement("root");
                root.SetAttribute("showVersion", showVersion.ToString());
                root.SetAttribute("pkgVersion", pkgVersion.ToString());
                root.SetAttribute("hotVersion", hotVersion.ToString());
                document.AppendChild(root);

                XmlElement file = null;
                foreach(FileVersionInfo info in list)
                {
                    file = document.CreateElement("file");
                    file.SetAttribute("path", info.fileName);
                    file.SetAttribute("crc", info.fileCrc.ToString());
                    file.SetAttribute("size", info.fileSize.ToString());

                    root.AppendChild(file);
                }
                
                XmlWriterSettings setting = new XmlWriterSettings();
                setting.Indent = true;
                setting.OmitXmlDeclaration = true;
                setting.Encoding = Encoding.ASCII;

                string path = AssetRootDirectory + VersionManifestFile;
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
        public uint fileCrc;
        public int fileSize;

        public FileVersionInfo() { }

        public static FileVersionInfo Create(string filePath, string fileRootDir)
        {
            if (filePath.EndsWith(".manifest"))
            {
                return null;
            }
            if (filePath.Contains("versioncode.txt") || filePath.Contains("versionmanifest.xml"))
            {
                return null;
            }
            FileVersionInfo info = new FileVersionInfo();
			if (filePath.EndsWith(".assetbundle"))
            {
                BuildPipeline.GetCRCForAssetBundle(filePath, out info.fileCrc);
            }
            else
            {
                //CRC32 crc = new CRC32();
                //FileStream fs = new FileStream(filePath, FileMode.Open);
                //info.fileCrc = (uint)crc.StreamCRC(fs);
            }
            FileInfo f = new FileInfo(filePath);
            info.fileSize = (int)f.Length;
            info.fileName = filePath.Replace(fileRootDir, "").Replace("\\", "/");
            return info;
        }

        public int CompareTo(FileVersionInfo other)
        {
            return this.fileName.CompareTo(other.fileName);
        }
    }
}
