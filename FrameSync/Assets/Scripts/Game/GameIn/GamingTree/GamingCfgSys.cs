using Framework;
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
            GamingLogic.Init();
            m_cNEDataLoader.Load(files, GamingLogic.arrGamingNodeDataType, onFinish);
        }

        public NEData GetGamingData(int aiId)
        {
            NEData neData = m_cNEDataLoader.Get(aiId);
            if (neData == null)
            {
                CLog.LogError("can not find gamingId = " + aiId + " cfgs!");
            }
            return neData;
        }
    }
}
