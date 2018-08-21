using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace NodeEditor
{
    public class NEDataLoader
    {
        private Dictionary<object, NEData> m_dicData = new Dictionary<object, NEData>();

        private MultiResourceLoader m_resLoader;
        private Action m_finishAction;
        private Type[] m_arrParseTypes;

        public NEData Get(object key)
        {
            NEData data = null;
            m_dicData.TryGetValue(key, out data);
            return data;
        }

        public void Load(List<string> files,Type[] arrParseTypes, Action OnFinish = null)
        {
            if(files.Count == 0)
            {
                if (OnFinish != null) OnFinish();
                return;
            }
            m_arrParseTypes = arrParseTypes;
            m_finishAction = OnFinish;
            //如果是播放模式
            if (Application.isPlaying)
            {
                //下载对应的资源
                m_resLoader = new MultiResourceLoader(ResourceSys.Instance);
                m_resLoader.LoadList(files, OnComplete, OnProgress, ResourceType.Bytes);
            }
            else
            {
                for (int i = 0; i < files.Count; i++)
                {
                    var textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(files[i]);
                    if (textAsset != null)
                    {
                        ParseData(textAsset.bytes);
                    }
                }
                if (null != m_finishAction)
                {
                    Action action = m_finishAction;
                    m_finishAction = null;
                    action();
                }
            }
        }

        private void OnComplete(MultiResourceLoader loader)
        {
            m_resLoader.Clear();
            m_resLoader = null;
            m_arrParseTypes = null;
            if (null != m_finishAction)
            {
                Action action = m_finishAction;
                m_finishAction = null;
                action();
            }
        }

        private void OnProgress(Resource res,string path)
        {
            if (res.isSucc)
            {
                byte[] data = res.GetBytes();
                ParseData(data);
            }
        }

        private void ParseData(byte[] bytesData)
        {
            NEData neData = NEUtil.DeSerializerObjectFromBuff(bytesData, typeof(NEData), m_arrParseTypes) as NEData;
            Type type = neData.data.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            bool hasPropertyKey = false;
            foreach (var item in fields)
            {
                if (item.GetCustomAttributes(typeof(NEPropertyKeyAttribute), true).Length > 0)
                {
                    object key = item.GetValue(neData.data);
                    m_dicData.Add(key, neData);
                    hasPropertyKey = true;
                    break;
                }
            }
            if(!hasPropertyKey)
            {
                CLog.LogError(neData.data.GetType()+" can not find NEPropertyKeyAttribute!");
            }
        }
    }
}
