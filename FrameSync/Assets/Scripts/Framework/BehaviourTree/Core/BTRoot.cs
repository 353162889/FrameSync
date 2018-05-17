using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BTCore
{
    public class BTRootData
    {
    }
    [NENode(typeof(BTRootData))]
    [NENodeDisplay(false, true, false)]
    [NENodeName("Root")]
    public class BTRoot : BTNode
    {
        protected BTNode m_cChild;

        public override void AddChild(BTNode child)
        {
            if (m_cChild != null)
            {
                Debug.LogError("BTRoot has exist child node! add has override it");
                if (m_lstChild != null && m_lstChild.Contains(m_cChild))
                {
                    m_lstChild.Remove(m_cChild);
                }
            }
            m_cChild = child;
            base.AddChild(child);
        }

        sealed public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            if (m_cChild != null) return m_cChild.OnTick(blackBoard);
            return BTResult.Success;
        }

        public override void Clear()
        {
            m_cChild = null;
            base.Clear();
        }
    }
}
