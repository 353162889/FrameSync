using Framework;
using GameData;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class RemoteCfgSys : Singleton<RemoteCfgSys>
    {
        private NEDataLoader m_cNEDataLoader;
        public void LoadResCfgs(Action onFinish)
        {
            m_cNEDataLoader = new NEDataLoader();
            List<string> files = new List<string>();
            var lst = ResCfgSys.Instance.GetCfgLst<ResRemote>();
            for (int i = 0; i < lst.Count; i++)
            {
                if(!files.Contains(lst[i].logic_path))
                {
                    files.Add(lst[i].logic_path);
                }
            }
            Remote.Init();
            m_cNEDataLoader.Load(files, Remote.arrRemoteNodeDataType, onFinish);
        }

        public NEData GetRemoteData(int configId)
        {
            var resInfo = ResCfgSys.Instance.GetCfg<ResRemote>(configId);
            if (resInfo == null)
            {
                CLog.LogError("找不到ID = " + configId + " 的远程配置!");
            }
            NEData neData = m_cNEDataLoader.Get(resInfo.logic_path);
            if (neData == null)
            {
                CLog.LogError("找不到路径= " + resInfo.logic_path + "远程配置");
            }
            return neData;
        }

        public void Clear()
        {
            if (m_cNEDataLoader != null)
            {
                m_cNEDataLoader.Clear();
                m_cNEDataLoader = null;
            }
        }
    }
}
