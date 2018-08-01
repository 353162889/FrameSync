using Framework;
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
