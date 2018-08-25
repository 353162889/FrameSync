using UnityEngine;
using UnityEditor;

public class PathDirectoryOpenTool : ScriptableObject
{
    [MenuItem("Tools/Path/OpenPersistentDataPath _F3")]
    static void DoOpenPersistentDataPath()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath);
    }
}