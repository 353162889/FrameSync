using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class HangPointSet
    {
        public List<HangPointItem> mLstHangPointItem = new List<HangPointItem>();
        public HangPointItem GetHangPointItem(string path)
        {
            if (mLstHangPointItem != null)
            {
                for (int i = 0; i < mLstHangPointItem.Count; i++)
                {
                    if (mLstHangPointItem[i].path == path)
                    {
                        return mLstHangPointItem[i];
                    }
                }
            }
            return null;
        }
    }

    public class HangPointItem
    {
        public string path;
        public List<HangPointData> mLstData = new List<HangPointData>();

        public HangPointData GetHangPointData(string name)
        {
            if(mLstData != null)
            {
                for (int i = 0; i < mLstData.Count; i++)
                {
                    if(mLstData[i].name == name)
                    {
                        return mLstData[i];
                    }
                }
            }
            return null;
        }
    }

    public class HangPointData
    {
        public string name;
        public TSVector position;
        public TSVector forward;
    }

    public class HangPointCfgSys : Singleton<HangPointCfgSys>
    {
        public static string HangPointPath = "Assets/ResourceEx/Config/HangPoint/HangPoint.bytes";
        private HangPointSet m_cHangPointSet;
        private Action m_cCallback;
        private string m_sRootDir;

        public void LoadResCfgs(Action onFinish)
        {
            m_cCallback = onFinish;
            m_sRootDir = ResourceSys.Instance.ResRootDir;
            if (!m_sRootDir.EndsWith("/")) m_sRootDir = m_sRootDir + "/";
            var path = HangPointPath.Replace(m_sRootDir, "");
            //配置在非editor下必须打bundle
            ResourceSys.Instance.GetResource(path, OnLoadSucc,OnLoadSucc);
        }

        private void OnLoadSucc(Resource res,string path)
        {
            var callback = m_cCallback;
            m_cCallback = null;
            if (res.isSucc)
            {
                TextAsset textAsset = (TextAsset)res.GetAsset(path);
                byte[] bytes = textAsset.bytes;
                m_cHangPointSet = NEUtil.DeSerializerObjectFromBuff(bytes, typeof(HangPointSet)) as HangPointSet;
            }
            if (callback != null)
            {
                callback();
            }
        }
        //path，资源路径
        public HangPointItem GetHangPointItem(string path)
        {
            HangPointItem hangPointItem = null;
            if(m_cHangPointSet != null)
            {
                path = m_sRootDir + path;
                hangPointItem = m_cHangPointSet.GetHangPointItem(path);
            }
            return hangPointItem;
        }

        public override void Dispose()
        {
            m_cHangPointSet = null;
            base.Dispose();
        }
    }
}
