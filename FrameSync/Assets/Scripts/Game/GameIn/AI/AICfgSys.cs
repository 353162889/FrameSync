using Framework;
using GameData;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class AICfgSys : Singleton<AICfgSys>
    {
        private NEDataLoader m_cNEDataLoader;
        public void LoadResCfgs(Action onFinish)
        {
            m_cNEDataLoader = new NEDataLoader();
            List<string> files = new List<string>();
            var lst = ResCfgSys.Instance.GetCfgLst<ResAirShip>();
            for (int i = 0; i < lst.Count; i++)
            {
                var aiPath = lst[i].ai_path;
                if (!string.IsNullOrEmpty(aiPath) && !files.Contains(aiPath))
                {
                    files.Add(aiPath);
                }
            }
            var lstItem = ResCfgSys.Instance.GetCfgLst<ResItem>();
            for (int i = 0; i < lstItem.Count; i++)
            {
                var aiPath = lstItem[i].ai_path;
                if (!string.IsNullOrEmpty(aiPath) && !files.Contains(aiPath))
                {
                    files.Add(aiPath);
                }
            }

            AgentObjectAI.Init();
            m_cNEDataLoader.Load(files, AgentObjectAI.arrAINodeDataType, onFinish);
        }

        public NEData GetAIData(string aiPath)
        {
            NEData neData = m_cNEDataLoader.Get(aiPath);
            if (neData == null)
            {
                CLog.LogError("找不到路径 = " + aiPath + " AI配置!");
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
