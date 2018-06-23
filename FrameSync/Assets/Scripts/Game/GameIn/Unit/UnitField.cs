using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{ 
    public partial class Unit
    {
        public List<Unit> m_lstFriend { get; private set; }
        public List<Unit> m_lstEnemy { get; private set; }

        public void InitField()
        {
            m_lstEnemy = new List<Unit>();
            m_lstFriend = new List<Unit>();
        }

        public void UpdateField(FP deltaTime)
        {
            var dic = BattleScene.Instance.dicCampUnits;
            m_lstEnemy.Clear();
            m_lstFriend.Clear();
            foreach (var item in dic)
            {
                var lst = item.Value;
                for (int i = 0; i < lst.Count; i++)
                {
                    if (lst[i].isDie) continue;
                    if(lst[i].campId == this.campId)
                    {
                        m_lstFriend.Add(lst[i]);
                    }
                    else
                    {
                        m_lstEnemy.Add(lst[i]);
                    }
                }
            }
        }

        public void DisposeField()
        {
            m_lstFriend.Clear();
            m_lstEnemy.Clear();
        }
    }
}
