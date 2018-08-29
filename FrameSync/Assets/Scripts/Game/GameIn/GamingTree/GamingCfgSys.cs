using Framework;
using GameData;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class GamingCfgSys : Singleton<GamingCfgSys>
    {
        private NEDataLoader m_cNEDataLoader;
        public void LoadResCfgs(Action onFinish)
        {
            m_cNEDataLoader = new NEDataLoader();
            List<string> files = new List<string>();
            var lst = ResCfgSys.Instance.GetCfgLst<ResLevel>();
            for (int i = 0; i < lst.Count; i++)
            {
                if (!files.Contains(lst[i].logic_path))
                {
                    files.Add(lst[i].logic_path);
                }
            }
            GamingLogic.Init();
            m_cNEDataLoader.Load(files, GamingLogic.arrGamingNodeDataType, onFinish);
        }

        public NEData GetGamingData(string logicPath)
        {
            NEData neData = m_cNEDataLoader.Get(logicPath);
            if (neData == null)
            {
                CLog.LogError("找不到路径 = " + logicPath + " 游戏逻辑配置!");
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
