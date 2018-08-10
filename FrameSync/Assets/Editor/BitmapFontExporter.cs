using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System;

public enum FontAlinType
{
    LeftTop,
    LeftCenter,
}

public class BitmapFontExporter : ScriptableWizard
{
    [MenuItem("Tools/BitmapFontExporter")]
    private static void CreateFont()
    {
        ScriptableWizard.DisplayWizard<BitmapFontExporter>("Create Font");
    }


    public TextAsset fontFile;
    public Texture2D textureFile;
    public FontAlinType alinType = FontAlinType.LeftCenter;

    private void OnWizardCreate()
    {
        if (fontFile == null || textureFile == null)
        {
            return;
        }

        string path = EditorUtility.SaveFilePanelInProject("Save Font", fontFile.name, "", "");

        if (!string.IsNullOrEmpty(path))
        {
            ResolveFont(path);
        }
    }


    private void ResolveFont(string exportPath)
    {
        if (!fontFile) throw new UnityException(fontFile.name + "is not a valid font-xml file");

        Font font = new Font();

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(fontFile.text);

        XmlNode info = xml.GetElementsByTagName("info")[0];
        XmlNodeList chars = xml.GetElementsByTagName("chars")[0].ChildNodes;

        CharacterInfo[] charInfos = new CharacterInfo[chars.Count];
        float maxHeight = 0;
        for (int cnt = 0; cnt < chars.Count; cnt++)
        {
            XmlNode node = chars[cnt];
            CharacterInfo charInfo = new CharacterInfo();

            int id = ToInt(node, "id");
            float x = ToFloat(node, "x");
            float y = ToFloat(node, "y");
            float width = ToFloat(node, "width");
            float height = ToFloat(node, "height");
            if (height > maxHeight) maxHeight = height;
            int xAdvance = ToInt(node, "xadvance");

            charInfo.index = id;
            charInfo.advance = xAdvance;

            charInfo.glyphWidth = (int)width;
            charInfo.glyphHeight = (int)height;

            //纹理映射,默认个的uv坐标是坐上角，而unity的uv坐标是从左下角开始的
            charInfo.uvBottomLeft = new Vector2(x / textureFile.width, 1 - (y + height) / textureFile.height);
            charInfo.uvBottomRight = new Vector2((x + width) / textureFile.width, 1 - (y + height) / textureFile.height);
            charInfo.uvTopLeft = new Vector2(x / textureFile.width, 1 - y / textureFile.height);
            charInfo.uvTopRight = new Vector2((x + width) / textureFile.width, 1 - y / textureFile.height);

            //相对于中心点x的
            charInfo.minX = 0;
            charInfo.maxX = (int)width;

            if (alinType == FontAlinType.LeftCenter)
            {
                //居中时会显示正常
                charInfo.minY = -(int)height / 2;
                charInfo.maxY = (int)height / 2;
            }
            else
            {
                //坐上角时会显示正常
                charInfo.minY = -(int)height;
                charInfo.maxY = 0;
            }
            charInfos[cnt] = charInfo;
        }


        Shader shader = Shader.Find("Unlit/Transparent");
        Material material = new Material(shader);
        material.mainTexture = textureFile;
        AssetDatabase.CreateAsset(material, exportPath + ".mat");


        font.material = material;
        font.name = info.Attributes.GetNamedItem("face").InnerText;
        font.characterInfo = charInfos;
        AssetDatabase.CreateAsset(font, exportPath + ".fontsettings");
        AssetDatabase.Refresh();
        //换行需要设置line spacing
        EditorUtility.DisplayDialog("提示", "资源创建成功，记得设置lineSpacing=" + maxHeight, "确定");
    }

    private int ToInt(XmlNode node, string name)
    {
        return Convert.ToInt32(node.Attributes.GetNamedItem(name).InnerText);
    }

    private float ToFloat(XmlNode node, string name)
    {
        return (float)ToInt(node, name);
    }
}