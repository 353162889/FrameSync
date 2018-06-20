using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{

    [NENodeCategory("Composite")]
    public abstract class BTComposite : BTNode
    {
        protected List<BTNode> m_lstChild = new List<BTNode>();

        public override void AddChild(BTNode child)
        {
            m_lstChild.Add(child);
        }

        public override void Clear()
        {
            int count = m_lstChild.Count;
            for (int i = 0; i < count; i++)
            {
                m_lstChild[i].Clear();
            }
        }
    }
}
