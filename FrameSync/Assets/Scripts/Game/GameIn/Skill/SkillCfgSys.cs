using Framework;
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
        public void LoadResCfgs(string resDir,Action onFinish)
        {
            m_cNEDataLoader = new NEDataLoader();
            List<string> files = new List<string>();
            //files.Add("Config/Skill/test.bytes");
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
    }
}
