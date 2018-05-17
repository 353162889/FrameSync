using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BTCore
{
    public class BTNotConditionData
    {

    }
    [NENode(typeof(BTNotConditionData))]
    public class BTNotCondition : BTCondition
    {
        protected BTCondition m_cChild;

        public override void AddChild(BTNode child)
        {
            if (m_cChild != null)
            {
                Debug.LogError("BTDecorator has exist child node! add has override it");
                if (m_lstChild != null && m_lstChild.Contains(m_cChild))
                {
                    m_lstChild.Remove(m_cChild);
                }
            }
            if (child is BTCondition)
            {
                base.AddChild(child);
                m_cChild = (BTCondition)child;
            }
            else
            {
                Debug.LogError("BTNotCondition AddChild is not BTCondition");
            }
        }

        public override bool Evaluate(BTBlackBoard blackBoard)
        {
            if (m_cChild == null) return true;
            return !m_cChild.Evaluate(blackBoard);
        }

        public override void Clear()
        {
            m_cChild = null;
            base.Clear();
        }
    }
}
