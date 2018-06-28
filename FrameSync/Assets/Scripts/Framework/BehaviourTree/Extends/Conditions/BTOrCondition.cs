using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BTCore
{
    public class BTOrConditionData
    {

    }
    [BTNode(typeof(BTOrConditionData))]
    public class BTOrCondition : BTCondition
    {
        private List<BTNode> m_lstChild = new List<BTNode>();
        public override void AddChild(BTNode child)
        {
            if (child is BTCondition)
            {
                m_lstChild.Add(child);
            }
            else
            {
                Debug.LogError("BTOrCondition AddChild is not BTCondition");
            }
        }

        public override bool Evaluate(BTBlackBoard blackBoard)
        {
            bool result = true;
            int count = m_lstChild.Count;
            for (int i = 0; i < count; i++)
            {
                result = result || (m_lstChild[i] as BTCondition) .Evaluate(blackBoard);
            }
            return result;
        }

        public override void Clear()
        {
            for (int i = 0; i < m_lstChild.Count; i++)
            {
                m_lstChild[i].Clear();
            }
            base.Clear();
        }
    }
}
