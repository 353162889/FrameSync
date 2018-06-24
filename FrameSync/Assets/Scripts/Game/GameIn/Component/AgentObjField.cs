using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class AgentObjField
    {
        private int m_nCampId;
        private int m_nAgentTypes;
        public List<AgentObject> lstFriend { get; private set; }
        public List<AgentObject> lstEnemy { get; private set; }

        public AgentObjField()
        {
            lstEnemy = new List<AgentObject>();
            lstFriend = new List<AgentObject>();
        }

        public void Init(int campId,int agentTypes)
        {
            m_nCampId = campId;
            m_nAgentTypes = agentTypes;
        }

        public void UpdateField(FP deltaTime)
        {
            lstEnemy.Clear();
            lstFriend.Clear();
            if (CheckAgentType(AgentObjectType.Unit))
            {
                var dic = BattleScene.Instance.dicCampUnits;
                foreach (var item in dic)
                {
                    var lst = item.Value;
                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (lst[i].isDie) continue;
                        if (lst[i].campId == m_nCampId)
                        {
                            lstFriend.Add(lst[i].agentObj);
                        }
                        else
                        {
                            lstEnemy.Add(lst[i].agentObj);
                        }
                    }
                }
            }
            if (CheckAgentType(AgentObjectType.Remote))
            {
                var dic = BattleScene.Instance.dicCampRemotes;
                foreach (var item in dic)
                {
                    var lst = item.Value;
                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (lst[i].campId == m_nCampId)
                        {
                            lstFriend.Add(lst[i].agentObj);
                        }
                        else
                        {
                            lstEnemy.Add(lst[i].agentObj);
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            lstEnemy.Clear();
            lstFriend.Clear();
        }


        private bool CheckAgentType(AgentObjectType agentObjectType)
        {
            return (m_nAgentTypes & (int)agentObjectType) != 0;
        }
    }
}
