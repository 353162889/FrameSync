using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    public abstract class BTRunningComposite : BTComposite
    {
        protected BTNode m_cRunningChild;

        //可重写
        protected virtual BTResult OnCompositeTick(BTBlackBoard blackBoard)
        {
            return base.OnTick(blackBoard);
        }

        sealed public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            if (m_cRunningChild != null)
            {
                return OnRunningChildTick(m_cRunningChild, blackBoard);
            }
            else
            {
                return OnCompositeTick(blackBoard);
            }
        }

        //如果子对象在running状态中，那么，下一帧直接执行
        protected BTResult OnRunningChildTick(BTNode child, BTBlackBoard blackBoard)
        {
            BTResult result = child.OnTick(blackBoard);
            if (result == BTResult.Running)
            {
                m_cRunningChild = child;
            }
            else
            {
                m_cRunningChild = null;
            }
            return result;
        }

        public override void Clear()
        {
            m_cRunningChild = null;
            base.Clear();
        }
    }
}
