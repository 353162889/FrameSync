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
                string name = string.Format("Config/Gaming/{0}.bytes", lst[i].gaming_id);
                if (!files.Contains(name))
                {
                    files.Add(name);
                }
            }
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
