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

    [NENode(typeof(BTSelectorData))]
    public class BTSelector : BTRunningComposite
    {
        protected int m_iSelectedIndex;
        public BTSelector()
        {
            m_iSelectedIndex = 0;
        }

        protected override BTResult OnCompositeTick(BTBlackBoard blackBoard)
        {
            int curIndex = m_iSelectedIndex;
            int selectedCount = 0;
            int totalCount = m_lstChild.Count;
            while(selectedCount < totalCount)
            {
                curIndex = curIndex % totalCount;
                var child = m_lstChild[curIndex];
                BTResult childResult = this.OnRunningChildTick(child, blackBoard);
                if(childResult == BTResult.Failure)
                {
                    curIndex++;
                    selectedCount++;
                }
                else
                {
                    m_iSelectedIndex = curIndex;
                    return childResult;
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
