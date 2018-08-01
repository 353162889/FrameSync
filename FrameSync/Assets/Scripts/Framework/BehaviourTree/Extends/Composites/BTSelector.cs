using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{

    public class BTSelectorData
    {

    }

    [BTNode(typeof(BTSelectorData))]
    public class BTSelector : BTComposite
    {
        protected int m_iSelectedIndex;
        public BTSelector()
        {
            m_iSelectedIndex = 0;
        }

        public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            int totalCount = m_lstChild.Count;
            while (m_iSelectedIndex < totalCount)
            {
                var child = m_lstChild[m_iSelectedIndex];
                BTResult childResult = child.OnTick(blackBoard);
                if (childResult == BTResult.Success)
                {
                    return BTResult.Success;
                }
                else if (childResult == BTResult.Running)
                {
                    return BTResult.Running;
                }
                else if (childResult == BTResult.Failure)
                {
                    m_iSelectedIndex++;
                }
            }
            return BTResult.Failure;
        }

        public override void Clear()
        {
            m_iSelectedIndex = 0;
            base.Clear();
        }
    }
}
