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
            var lst = ResCfgSys.Instance.GetCfgLst<ResAirShip>();
            for (int i = 0; i < lst.Count; i++)
            {
                for (int j = 0; j < lst[i].skills.Count; j++)
                {
                    string name = string.Format("Config/Skill/{0}.bytes", lst[i].skills[j]);
                    if (!files.Contains(name))
                    {
                        files.Add(name);
                    }
                }
            }
            Skill.Init();
            m_cNEDataLoader.Load(files, Skill.arrSkillNodeDataType,onFinish);
        }

        public NEData GetSkillData(int skillId)
        {
            NEData neData = m_cNEDataLoader.Get(skillId);
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
