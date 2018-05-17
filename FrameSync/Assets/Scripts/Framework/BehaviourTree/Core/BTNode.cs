using NodeEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTCore
{
    public enum BTResult
    {
        Success,
        Running,
        Failure,
    }
    abstract public class BTNode : INENode{
        protected List<BTNode> m_lstChild;
        [NENodeData]
        protected object m_cData;
        public object data { get { return m_cData; } }
       
        public virtual BTResult OnTick(BTBlackBoard blackBoard){ return BTResult.Success; }

        public virtual void AddChild(BTNode child) {
            if(m_lstChild == null)
            {
                m_lstChild = new List<BTNode>();
            }
            m_lstChild.Add(child);
        }

        public virtual NEData GetData()
        {
            if(m_cData == null)
            {
                Debug.LogError(this.GetType() + " m_cData == null,need initialize！");
                return null;
            }
            NEData neData = new NEData();
            neData.data = m_cData;
            if (m_lstChild != null && m_lstChild.Count > 0)
            {
                neData.lstChild = new List<NEData>();
                for (int i = 0; i < m_lstChild.Count; i++)
                {
                    NEData data = m_lstChild[i].GetData();
                    if (data != null)
                    {
                        neData.lstChild.Add(data);
                    }
                }
            }
            return neData;
        }

        public virtual void Clear() {
            if (m_lstChild != null)
            {
                int count = m_lstChild.Count;
                for (int i = 0; i < count; i++)
                {
                    m_lstChild[i].Clear();
                }
            }
        }

    }
}
