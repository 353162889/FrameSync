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
    [NENode(typeof(BTOrConditionData))]
    public class BTOrCondition : BTCondition
    {
        public override void AddChild(BTNode child)
        {
            if (child is BTCondition)
            {
                base.AddChild(child);
            }
            else
            {
                Debug.LogError("BTOrCondition AddChild is not BTCondition");
            }
        }

        public override bool Evaluate(BTBlackBoard blackBoard)
        {
            bool result = true;
            if (m_lstChild == null)return result;
            int count = m_lstChild.Count;
            for (int i = 0; i < count; i++)
            {
                result = result || (m_lstChild[i] as BTCondition) .Evaluate(blackBoard);
            }
            return result;
        }
    }
}
