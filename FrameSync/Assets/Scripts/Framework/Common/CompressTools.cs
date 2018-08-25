using UnityEngine;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using System;

/// <summary>
/// Zip压缩与解压缩 
/// </summary>
public class CompressTools
{
    /// <summary>
    /// 压缩单个文件
    /// </summary>
    /// <param name="fileToZip">要压缩的文件</param>
    /// <param name="zipedFile">压缩后的文件</param>
    /// <param name="compressionLevel">压缩等级</param>
    /// <param name="blockSize">每次写入大小</param>
    public static void CompressFile(string fileToZip, string zipedFile, int compressionLevel, int blockSize)
    {

        //如果文件没有找到，则报错
        if (!System.IO.File.Exists(fileToZip))
        {
            throw new System.IO.FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
        }
        fileToZip = fileToZip.Replace("\\", "/");

        using (System.IO.FileStream ZipFile = System.IO.File.Create(zipedFile))
        {
            using (ZipOutputStream ZipStream = new ZipOutputStream(ZipFile))
            {
                using (System.IO.FileStream StreamToZip = new System.IO.FileStream(fileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    string fileName = fileToZip.Substring(fileToZip.LastIndexOf("/") + 1);

                    ZipEntry ZipEntry = new ZipEntry(fileName);

                    ZipStream.PutNextEntry(ZipEntry);

                    ZipStream.SetLevel(compressionLevel);

                    byte[] buffer = new byte[blockSize];

                    int sizeRead = 0;

                    try
                    {
                        do
                        {
                            sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                            ZipStream.Write(buffer, 0, sizeRead);
                        }
                        while (sizeRead > 0);
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }

                    StreamToZip.Close();
                }

                ZipStream.Finish();
                ZipStream.Close();
            }

            ZipFile.Close();
        }
    }

    /// <summary>
    /// 压缩多层目录
    /// </summary>
    /// <param name="strDirectory">The directory.</param>
    /// <param name="zipedFile">The ziped file.</param>
    public static void CompressDir(string strDirectory, string zipedFile,Action<string> onCompress = null)
    {
        using (System.IO.FileStream ZipFile = System.IO.File.Create(zipedFile))
        {
            using (ZipOutputStream s = new ZipOutputStream(ZipFile))
            {
                s.SetLevel(6);
                CompressStep(strDirectory, s, "", onCompress);
            }
        }
    }

    /// <summary>
    /// 递归遍历目录
    /// </summary>
    /// <param name="strDirectory">The directory.</param>
    /// <param name="s">The ZipOutputStream Object.</param>
    /// <param name="parentPath">The parent path.</param>
    private static void CompressStep(string strDirectory, ZipOutputStream s, string parentPath,Action<string> onCompress = null)
    {
        strDirectory = strDirectory.Replace("\\", "/");
        if (!strDirectory.EndsWith("/"))
        {
            strDirectory += "/";
        }
        Crc32 crc = new Crc32();

        string[] filenames = Directory.GetFileSystemEntries(strDirectory);

        foreach (string oneFile in filenames)// 遍历所有的文件和目录
        {
            string file = oneFile.Replace("\\", "/");
            if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
            {
                string pPath = parentPath;
                pPath += file.Substring(file.LastIndexOf("/") + 1);
                pPath += "/";
                CompressStep(file, s, pPath);
            }

            else // 否则直接压缩文件
            {
                //打开需要压缩文件
                using (FileStream fs = File.OpenRead(file))
                {

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);

                    string fileName = parentPath + file.Substring(file.LastIndexOf("/") + 1);
                    ZipEntry entry = new ZipEntry(fileName);

                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;

                    fs.Close();

                    crc.Reset();
                    crc.Update(buffer);

                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);

                    s.Write(buffer, 0, buffer.Length);
                    try
                    {
                        if (onCompress != null)
                        {
                            onCompress.Invoke(file);
                        }
                    }catch(Exception e)
                    {
                        Debug.LogError(e.Message+"\n"+e.StackTrace);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 解压缩一个 zip 文件。
    /// </summary>
    /// <param name="zipedFile">The ziped file.</param>
    /// <param name="strDirectory">The STR directory.</param>
    public static void UnCompress(string zipedFile, string strDirectory)
    {
        UnCompress(File.OpenRead(zipedFile), strDirectory);
    }

    public static void UnCompress(byte[] bytes, string strDirectory)
    {
        Stream stream = new MemoryStream(bytes);
        strDirectory = strDirectory.Replace("\\", "/");
        UnCompress(stream, strDirectory);
    }

    public static void UnCompress(Stream stream, string strDirectory)
    {
        if (strDirectory == "")
            strDirectory = Directory.GetCurrentDirectory();
        if (!strDirectory.EndsWith("/"))
            strDirectory = strDirectory + "/";
        using (ZipInputStream s = new ZipInputStream(stream))
        {
            ZipEntry theEntry;

            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = "";
                string pathToZip = "";
                pathToZip = theEntry.Name.Replace("\\", "/");

                if (pathToZip != "")
                {
                    directoryName = GetDirectoryName(pathToZip);
                }

                if (directoryName.StartsWith("/"))
                {
                    directoryName = directoryName.Substring(1, directoryName.Length - 1);
                }
                string curDirectory = strDirectory + directoryName;
                if (!Directory.Exists(curDirectory))
                {
                    Directory.CreateDirectory(curDirectory);
                }

                string fileName = GetFileName(pathToZip);
                if (fileName != "")
                {
                    using (FileStream streamWriter = File.Create(curDirectory + fileName))
                    {
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);

                            if (size > 0)
                                streamWriter.Write(data, 0, size);
                            else
                                break;
                        }
                        streamWriter.Close();
                    }
                }
            }

            s.Close();
        }
    }

    public static int GetUnCompressFileCount(Stream stream)
    {
        int count = 0;
        using (ZipInputStream s = new ZipInputStream(stream))
        {
            ZipEntry theEntry;

            while ((theEntry = s.GetNextEntry()) != null)
            {
                count++;
            }
            s.Close();
        }
        return count;
    }

    //获取目录，如果目录不为空，则添加"/"
    public static string GetDirectoryName(string path)
    {
        path = path.Replace("\\", "/");
        if (path.EndsWith("/")) return path;
        int index = path.LastIndexOf("/");
        if (index != -1)
        {
            return path.Substring(0, index + 1);
        }
        return "";
    }

    //获取当前文件名（全称）
    public static string GetFileName(string path)
    {
        path = path.Replace("\\", "/");
        if (path.EndsWith("/")) return "";
        int index = path.LastIndexOf("/");
        if (index != -1)
        {
            return path.Substring(index + 1);
        }
        return path;
    }

}