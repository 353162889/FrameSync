using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    public class BTSequenceData
    {

    }
    [BTNode(typeof(BTSequenceData))]
    public class BTSequence : BTComposite
    {
        protected int m_iSelectedIndex;
        public BTSequence()
        {
            m_iSelectedIndex = 0;
        }

        public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            int totalCount = m_lstChild.Count;
            while(m_iSelectedIndex < totalCount)
            {
                var child = m_lstChild[m_iSelectedIndex];
                BTResult childResult = child.OnTick(blackBoard);
                if(childResult == BTResult.Failure)
                {
                    Clear();
                    return BTResult.Failure;
                }
                else if(childResult == BTResult.Running)
                {
                    return BTResult.Running;
                }
                else if(childResult == BTResult.Success)
                {
                    m_iSelectedIndex++;
                }
            }
            Clear();
            return BTResult.Success;
        }

        public override void Clear()
        {
            m_iSelectedIndex = 0;
            base.Clear();
        }
    }
}
