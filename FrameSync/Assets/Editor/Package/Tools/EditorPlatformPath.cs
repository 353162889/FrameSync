using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomizeEditor
{ 
    public class EditorPlatformPath
    {
        public static readonly string GameSourceDir = "Assets/Resources";

        private static readonly string Win32 = "Win32";
        private static readonly string Mac = "Mac";
        private static readonly string WebPlayer = "WebPlayer";
        private static readonly string Android = "Android"; 
        private static readonly string IOS = "iOS";

	    public static BuildTarget[] BuildPlatforms = { BuildTarget.StandaloneWindows64, BuildTarget.Android, BuildTarget.iOS };
        public static string[] BuildPlatformNames = { Win32, Android, IOS };
        public static string[] BuildPlatFormExtensions = {".exe",".apk",".ipa" };


        public static string GetPlatformName(BuildTarget target = BuildTarget.StandaloneWindows)
        {
            string platform = Win32;
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                    platform = Win32;
                    break;
                case BuildTarget.iOS:
                    platform = IOS;
                    break;
                case BuildTarget.Android:
                    platform = Android;
                    break;
                default:
                    break;
            }
            return platform;
        }

        public static string UnityPath(string path)
        {
            path = path.Replace("\\", "/");
            return path;
        }

	    public static void GetAllFilesRecursively(string dir,List<string> files, string ignoreDir = null)
	    {
		    if(!Directory.Exists(dir))
		    {
			    return;
		    }
		    //获取所有文件
		    string[] innerFiles = Directory.GetFiles(dir);
		    files.AddRange(innerFiles);

		    //递归获取子目录的文件
		    var subdirs = Directory.GetDirectories(dir);
		    foreach(var subdir in subdirs)
		    {
                if(ignoreDir == null || subdir != ignoreDir)
                {
                    GetAllFilesRecursively(subdir, files);
                }
		    }
	    }

	    public static void GetAllFiles(string dir,List<string> files,string withExtention)
	    {
		    if(!Directory.Exists(dir))
		    {
			    return;
		    }
		    //获取所有文件
		    string[] innerFiles = Directory.GetFiles(dir);
		
		    int count = innerFiles.Length;
		    for(int i = 0; i < count; ++i)
		    {
			    if(innerFiles[i].EndsWith(withExtention))
                {
                    files.Add(innerFiles[i]);
                }
            }
        }

        public static void CheckDirExists(string dir)
        {
            DirectoryInfo info = new DirectoryInfo(dir);
            if (!info.Exists)
            {
                info.Create();
            }
        }

        public static void CheckDirExistsForFile(string file)
        {
		    file = UnityPath(file);
            int index = file.LastIndexOf("/");
            string dir = file.Substring(0, index);
            DirectoryInfo info = new DirectoryInfo(dir);
            if (!info.Exists)
            {
                info.Create();
            }
        }

	    public static string GetFileNameForPath(string assetPath)
	    {
		    assetPath = UnityPath(assetPath);
		    int index = assetPath.LastIndexOf("/");
		    if(index > 0)
		    {
			    return assetPath.Substring(index+1);
		    }
		    return assetPath;
	    }

        public static string GetGameSourceRelativePath(string assetPath)
        {
            assetPath = UnityPath(assetPath);
            if (!assetPath.StartsWith(GameSourceDir))
            {
                return assetPath;
            }
            return assetPath.Substring(GameSourceDir.Length + 1);
        }

       
    }
}