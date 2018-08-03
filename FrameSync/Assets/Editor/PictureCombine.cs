using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PictureCombine {

    [MenuItem("Assets/Custom/CombineSameSizePicture[png,OneLine]")]
    public static void CombinePicture()
    {
        object[] objs = Selection.objects;
        if(objs != null)
        {
            List<Texture2D> lst = new List<Texture2D>();
            for (int i = 0; i < objs.Length; i++)
            {
                if(objs[i] is Texture2D)
                {
                    var tex = objs[i] as Texture2D;
                    lst.Add(tex);
                    TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(tex));
                    ti.isReadable = true;
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tex));
                }
            }
            if (lst.Count > 0)
            {
                int width = 0;
                int height = 0;
                int perWidth = lst[0].width;
                int perHeight = lst[0].height;
                for (int i = 0; i < lst.Count; i++)
                {
                    if(perWidth != lst[i].width || perHeight != lst[i].height)
                    {
                        Debug.LogError("图片"+lst[i].name+"的宽高和其他图片的宽高不相等，合并失败");
                        return;
                    }
                    width += lst[i].width;
                    if (lst[i].height > height) height = lst[i].height;
                }
                Texture2D tex = new Texture2D(width, height);
                for (int i = 0; i < lst.Count; i++)
                {
                    Color32[] colors = lst[i].GetPixels32(0);
                    tex.SetPixels32(i * perWidth, 0, lst[i].width, lst[i].height, colors);
                }
                tex.Apply();
                //保存图片
                var bytes = tex.EncodeToPNG();
                string path = AssetDatabase.GetAssetPath(lst[0]);
                path = path.Replace("Assets/", "");
                int lastIndex = path.LastIndexOf("/");
                path = path.Substring(0, lastIndex);
                if (path.StartsWith("/")) path = path.Substring(1);
                string dir = Application.dataPath + "/" + path + "/CombinePicture/";
                string file = dir + "combine_"+DateTime.Now.ToString("HH_mm_ss") + ".png";
                Debug.Log("生成路径:"+ file);
                if(!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllBytes(file, bytes);
                AssetDatabase.Refresh();
            }
        }
    }
}
