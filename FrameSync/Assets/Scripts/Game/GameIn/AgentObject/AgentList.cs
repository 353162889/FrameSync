using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class AgentList : IDisposable,IPoolable
    {
        public List<AgentObject> lstData { get; private set; }
        public AgentList()
        {
            lstData = new List<AgentObject>();
        }

        public static AgentList Create()
        {
            if(!ObjectPool<AgentList>.Instance.inited)
            {
                ObjectPool<AgentList>.Instance.Init(10);
            }
            return ObjectPool<AgentList>.Instance.GetObject();
        }

        public void Dispose()
        {
            ObjectPool<AgentList>.Instance.SaveObject(this);
        }

        public void Reset()
        {
            lstData.Clear();
        }
    }
}
