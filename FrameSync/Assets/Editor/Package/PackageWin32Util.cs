using UnityEngine;
using UnityEditor;
using System.IO;

namespace EditorPackage
{
    public class PackageWin32Util : ScriptableObject
    {

        public static void Build(BuildTarget buildTarget, BuildOptions buildOptions)
        {
            PackageUtil.Build(buildTarget,buildOptions);
        }
    }
}