using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace NodeEditor
{
    public class NEUtil
    {
        public static object DeSerializerObject(string path, Type type,Type[] extraTypes = null)
        {
            object obj = null;
            if (!File.Exists(path))
            {
                return null;
            }

            using (Stream streamFile = new FileStream(path, FileMode.Open))
            {
                if (streamFile == null)
                {
                    Debug.LogError("OpenFile Erro");
                    return obj;
                }

                try
                {
                    if (streamFile != null)
                    {
                        XmlSerializer xs = null;
                        if (extraTypes == null)
                        {
                            xs = new XmlSerializer(type);
                        }
                        else
                        {
                            xs = new XmlSerializer(type, extraTypes);
                        }
                        obj = xs.Deserialize(streamFile);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("SerializerObject Erro:" + ex.ToString());
                }
            }

            return obj;
        }

        public static object DeSerializerObjectFromBuff(byte[] buff, Type type, Type[] extraTypes = null)
        {
            object objRet = null;
            using (MemoryStream stream = new MemoryStream(buff))
            {
                try
                {
                    XmlSerializer xs = null;
                    if (extraTypes == null)
                    {
                        xs = new XmlSerializer(type);
                    }
                    else
                    {
                        xs = new XmlSerializer(type,extraTypes);
                    }
                  
                    objRet = xs.Deserialize(stream);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Deserialize Error:" + ex.ToString());
                }
            }

            return objRet;
        }

        public static void SerializerObject(string path, object obj,Type[] extraTypes = null)
        {
            if (File.Exists(path))
            { // remove exist file to fix unexcept text
                File.Delete(path);
            }


            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                    return;
                }
            }

            using (Stream streamFile = new FileStream(path, FileMode.OpenOrCreate))
            {
                if (streamFile == null)
                {
                    Debug.LogError("OpenFile Erro");
                    return;
                }

                try
                {
                    string strDirectory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(strDirectory))
                    {
                        Directory.CreateDirectory(strDirectory);
                    }
                    XmlSerializer xs = null; ;
                    if (extraTypes != null)
                    {
                        xs = new XmlSerializer(obj.GetType(), extraTypes);
                    }
                    else
                    {
                        xs = new XmlSerializer(obj.GetType());
                    }
                    TextWriter writer = new StreamWriter(streamFile, Encoding.UTF8);
                    xs.Serialize(writer, obj);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("DeSerializerObject Erro:" + ex.ToString());
                }
            }
        }
    }
}
