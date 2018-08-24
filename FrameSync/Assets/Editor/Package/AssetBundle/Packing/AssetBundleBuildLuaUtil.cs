using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace CustomizeEditor
{
    public class AssetBundleBuildLuaUtil
    {
        public static void CheckAndCreateLuaTempDir()
        {
            string path = AssetBundlePath.LuaTempDir;
            if(Directory.Exists(path))
            {
                EditorFileOperate.RemoveDir(path);
            }
            Directory.CreateDirectory(path);
        }

        public static void ClearLuaTempDir()
        {
            EditorFileOperate.RemoveDir(AssetBundlePath.LuaTempDir);
        }

        //将lua文件拷贝到临时文件，并改后缀名
        public static string CopyLuaFileToTempDirAndRenameExt(string path)
        {
            if (Directory.Exists(path))
            {
                string tempDir = AssetBundlePath.LuaTempDir;
                DirectoryInfo dir = Directory.CreateDirectory(path);
                //string toDir = tempDir + dir.FullName.Replace("\\","/").Replace(AssetBundlePath.ProjectDirWithAsset, "");
                string toDir = tempDir;
                EditorFileOperate.CopyToRenameExtension(path, toDir, "*.lua", ".bytes");
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                return toDir;
            }
            return null;
        }
    }
}
