using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class SkillPool : Singleton<SkillPool>
    {
        private Dictionary<int, Queue<Skill>> m_dicPool = new Dictionary<int, Queue<Skill>>();

        public Skill GetSkill(int configId)
        {
            Queue<Skill> queue = null;
            if (m_dicPool.TryGetValue(configId, out queue))
            {
                if (queue.Count > 0)
                {
                    return queue.Dequeue();
                }
            }
            NEData neData = SkillCfgSys.Instance.GetSkillData(configId);
            if (neData == null) return null;
            Skill skill = new Skill(configId,neData);
            return skill;
        }

        public void SaveSkill(int configId, Skill skill)
        {
            Queue<Skill> queue = null;
            if (!m_dicPool.TryGetValue(configId, out queue))
            {
                queue = new Queue<Skill>();
                m_dicPool.Add(configId, queue);
            }
            skill.Clear();
            queue.Enqueue(skill);
        }

        public void Clear()
        {
            m_dicPool.Clear();
        }
    }
}
