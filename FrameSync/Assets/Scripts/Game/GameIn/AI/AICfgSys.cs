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
                string name = string.Format("Config/AI/{0}.bytes", lst[i].ai);
                if (!files.Contains(name))
                {
                    files.Add(name);
                }
            }
            AgentObjectAI.Init();
            m_cNEDataLoader.Load(files, AgentObjectAI.arrAINodeDataType, onFinish);
        }

        public NEData GetAIData(int aiId)
        {
            NEData neData = m_cNEDataLoader.Get(aiId);
            if (neData == null)
            {
                CLog.LogError("can not find aiId = " + aiId + " cfgs!");
            }
            return neData;
        }
    }
}
