using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Security;
using Mono.Xml;
using UnityEngine;

namespace Framework
{
    public class ResCfgSys : Singleton<ResCfgSys>
    {
        public class DataReader<T>
        {
            private static Dictionary<object, T> m_dicData = new Dictionary<object, T>();
            private static List<T> m_lstData = new List<T>();
            public static List<T> lstData {
                get { return m_lstData; }
            }
            public static T Get(object key)
            {
                T value = default(T);
                m_dicData.TryGetValue(key, out value);
                return value;
            }

            public static bool Add(object key, T value)
            {
                if (m_dicData.ContainsKey(key))
                {
                    return false;
                }
                m_lstData.Add(value);
                m_dicData.Add(key, value);
                return true;
            }

            public static void Dispose()
            {
                m_lstData.Clear();
                m_dicData.Clear();
            }

        }
        protected struct ResCfgInfo
        {
            public Type type;
            public PropertyInfo keyProperty;
            public string xmlPath;
        }
        private Dictionary<string, ResCfgInfo> m_dicCfgInfo;
        private MultiResourceObjectLoader m_resObjectLoader;
        private Action m_finishAction;


        public void LoadResCfgs(string resDir, Action OnFinish = null)
        {
            Dispose();
            m_finishAction = OnFinish;
            m_dicCfgInfo = new Dictionary<string, ResCfgInfo>();
            resDir = resDir.Replace('\\', '/');
            if (!resDir.EndsWith("/")) resDir += "/";
            if (resDir.StartsWith("/")) resDir = resDir.Substring(1);
            Assembly assembly = Assembly.GetAssembly(typeof(ResCfgSys));
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                object[] arr = type.GetCustomAttributes(typeof(ResCfgAttribute), false);
                if (arr.Length == 0)
                {
                    continue;
                }
                var properties = type.GetProperties();
                PropertyInfo propertyInfo = null;
                for (int i = 0; i < properties.Length; i++)
                {
                    object[] arrMember = properties[i].GetCustomAttributes(typeof(ResCfgKeyAttribute), false);
                    if (arrMember.Length > 0)
                    {
                        propertyInfo = properties[i];
                        break;
                    }
                }
                if(propertyInfo == null)
                {
                    CLog.LogError("配置名称:"+type.Name+ "找不到唯一key的定义ResCfgAttribute");
                    continue;
                }
                //考虑下需要过滤不在此目录的文件
                ResCfgInfo cfgInfo = new ResCfgInfo();
                cfgInfo.xmlPath = resDir + type.Name + ".xml";
                cfgInfo.type = type;
                cfgInfo.keyProperty = propertyInfo;
                m_dicCfgInfo.Add(cfgInfo.xmlPath, cfgInfo);
                //CLog.LogColorArgs(CLogColor.Red,type.Name,propertyInfo != null ? propertyInfo.Name : "null");
            }
            List<string> names = new List<string>();
            foreach (var item in m_dicCfgInfo)
            {
                names.Add(item.Value.xmlPath);
            }
            //如果是播放模式
            if (Application.isPlaying)
            {
                m_resObjectLoader = new MultiResourceObjectLoader();
                //下载对应的资源
                m_resObjectLoader.LoadList(names, true, OnComplete, OnProgress);
            }
#if UNITY_EDITOR
            else
            {
                for (int i = 0; i < names.Count; i++)
                {
                    var textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(names[i]);
                    if(textAsset != null)
                    {
                        ParseData(names[i], textAsset.text);
                    }
                }
                if (null != m_finishAction)
                {
                    Action action = m_finishAction;
                    m_finishAction = null;
                    action();
                }
            }
#endif
        }

        private void OnComplete(MultiResourceObjectLoader loader)
        {
            m_resObjectLoader.Clear();
            m_resObjectLoader = null;
            if(null != m_finishAction)
            {
                Action action = m_finishAction;
                m_finishAction = null;
                action();
            }
        }

        private void OnProgress(UnityEngine.Object obj,string path)
        {
            string data = ((TextAsset)obj).text;
            ParseData(path, data);
        }

        public void ParseData(string path,string strData)
        {
            SecurityParser parser = new SecurityParser();
            parser.LoadXml(strData);
            SecurityElement element = parser.ToXml();
            ResCfgInfo info;
            if(m_dicCfgInfo.TryGetValue(path, out info))
            {
                Type dataReaderTType = typeof(DataReader<>);
                //Type dataReaderTType = typeof(ResCfgSys).GetNestedType("DataReader`1");
                Type dataReaderDataType = dataReaderTType.MakeGenericType(new Type[] { info.type});
                MethodInfo addMethod = dataReaderDataType.GetMethod("Add", BindingFlags.Static | BindingFlags.Public);
                foreach (SecurityElement node in element.Children)
                {
                    var value = Activator.CreateInstance(info.type, node);
                    var key = info.keyProperty.GetValue(value,null);
                    addMethod.Invoke(null, new object[] { key,value });
                }
            }
        }

        public T GetCfg<T>(object key)
        {
            return DataReader<T>.Get(key);
        }

        public List<T> GetCfgLst<T>()
        {
            return DataReader<T>.lstData;
        }

        public void Dispose<T>()
        {
            DataReader<T>.Dispose();
        }

        public override void Dispose()
        {
            if(m_resObjectLoader != null)
            {
                m_resObjectLoader.Clear();
                m_resObjectLoader = null;
            }
            m_finishAction = null;
            if(m_dicCfgInfo != null)
            {
                foreach (var item in m_dicCfgInfo)
                {
                    Type dataReaderTType = typeof(DataReader<>);
                    Type dataReaderDataType = dataReaderTType.MakeGenericType(new Type[] { item.Value.type });
                    MethodInfo disposeMethod = dataReaderDataType.GetMethod("Dispose", BindingFlags.Static | BindingFlags.Public);
                    disposeMethod.Invoke(null,new object[] { });
                }
            }
            base.Dispose();
        }

    }
}
