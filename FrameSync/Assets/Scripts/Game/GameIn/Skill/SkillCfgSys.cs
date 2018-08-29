using Framework;
using GameData;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class SkillCfgSys : Singleton<SkillCfgSys>
    {
        private NEDataLoader m_cNEDataLoader;
        public void LoadResCfgs(Action onFinish)
        {
            m_cNEDataLoader = new NEDataLoader();
            List<string> files = new List<string>();
            var lst = ResCfgSys.Instance.GetCfgLst<ResSkill>();
            for (int i = 0; i < lst.Count; i++)
            {
                if (!files.Contains(lst[i].logic_path))
                {
                    files.Add(lst[i].logic_path);
                }
            }
            Skill.Init();
            m_cNEDataLoader.Load(files, Skill.arrSkillNodeDataType,onFinish);
        }

        public NEData GetSkillData(int skillId)
        {
            var resInfo =  ResCfgSys.Instance.GetCfg<ResSkill>(skillId);
            NEData neData = m_cNEDataLoader.Get(resInfo.logic_path);
            if(neData == null)
            {
                CLog.LogError("can not find skillId = "+skillId +" cfgs!");
            }
            return neData;
        }

        public void Clear()
        {
            if(m_cNEDataLoader != null)
            {
                m_cNEDataLoader.Clear();
                m_cNEDataLoader = null;
            }
        }
    }
}
