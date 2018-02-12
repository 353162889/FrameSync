using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Framework
{
	public static class StringUtil
	{
		public static bool IsEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		public static string Serialize<T>(T obj)
		{
			if (obj == null)
			{
				CLog.LogError ("Serialize obj can not null!");
				return null;
			}
			try
			{
				IFormatter formatter = new BinaryFormatter();
				MemoryStream ms = new MemoryStream();
				formatter.Serialize(ms,obj);
				ms.Position = 0;
				byte[] buffer = new byte[ms.Length];
				ms.Read(buffer,0,buffer.Length);
				ms.Flush();
				ms.Close();
				return Convert.ToBase64String(buffer);
			}
			catch(Exception ex)
			{
				throw new Exception ("Serialize fail,reason:"+ex.Message);
			}
		}

		public static T Deserialize<T>(string str)
		{
			if (str.IsEmpty ())
			{
				CLog.LogError ("Deserialize str can not null!");
				return default(T);
			}
			try
			{
				IFormatter formatter = new BinaryFormatter();
				byte[] buffer = Convert.FromBase64String(str);
				MemoryStream ms = new MemoryStream(buffer);
				T obj = (T)formatter.Deserialize(ms);
				ms.Flush();
				ms.Close();
				return obj;
			}
			catch(Exception ex)
			{
				throw new Exception ("Serialize fail,reason:"+ex.Message);
			}
		}
	}


}

