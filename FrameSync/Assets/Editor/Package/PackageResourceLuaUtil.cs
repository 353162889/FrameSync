using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace CustomizeEditor
{
    public class PackageResourceLuaUtil
    {
        public static void CheckAndCreateLuaTempDir()
        {
            string path = PackagePath.ResourceLuaCodePath;
            if (Directory.Exists(path))
            {
                EditorFileOperate.RemoveDir(path);
            }
            Directory.CreateDirectory(path);
        }

        public static void ClearLuaTempDir()
        {
            EditorFileOperate.RemoveDir(PackagePath.ResourceLuaCodePath);
        }

        //将lua文件拷贝到resource下，并改后缀名
        public static void CopyLuaFileToResourceDirAndRenameExt()
        {
            string[] paths = PackagePath.LuaSouceCodePaths;
            string toDir = PackagePath.ResourceLuaCodePath;
            List<string> movedFiles = new List<string>();
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                if(Directory.Exists(path))
                {
                    EditorFileOperate.CopyToRenameExtension(path, toDir, "*.lua", ".bytes", movedFiles);
                }
            }
            //将移动的文件保存起来
            if(File.Exists(PackagePath.ResourceLuaCodeFileCfgPath))
            {
                File.Delete(PackagePath.ResourceLuaCodeFileCfgPath);
            }
            string fileNames = "";
            for (int i = 0; i < movedFiles.Count; i++)
            {
                string file = movedFiles[i];
                string realFileName = file.Replace(PackagePath.GameAssetsDirectory,"");
                fileNames += realFileName + ",";
            }
            if(fileNames.EndsWith(","))
            {
                fileNames = fileNames.Substring(0,fileNames.Length - 1);
            }
            File.WriteAllText(PackagePath.ResourceLuaCodeFileCfgPath, fileNames);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}
