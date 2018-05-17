using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    public enum BTActionResult
    {
        Ready,
        Running,
    }
    [NENodeCategory("Action")]
    public abstract class BTAction : BTNode
    {
        protected BTActionResult m_eActionResult;
        protected bool m_bIsEnd;
        protected BTBlackBoard m_cBlackBoard;
        public BTAction()
        {
            m_eActionResult = BTActionResult.Ready;
        }
        sealed public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            if(m_eActionResult == BTActionResult.Ready)
            {
                OnEnter(blackBoard);
                m_eActionResult = BTActionResult.Running;
            }
            m_eActionResult = OnRun(blackBoard);
            m_cBlackBoard = blackBoard;
            if (m_eActionResult == BTActionResult.Ready)
            {
                OnExit(blackBoard);
                m_cBlackBoard = null;
            }
            if(m_eActionResult == BTActionResult.Running)
            {
                return BTResult.Running;
            }
            return BTResult.Success;
        }

        public virtual void OnEnter(BTBlackBoard blackBoard) { }
        public virtual BTActionResult OnRun(BTBlackBoard blackBoard) { return BTActionResult.Running; }
        public virtual void OnExit(BTBlackBoard blackBoard) { }

        public override void Clear()
        {
            if(m_eActionResult == BTActionResult.Running)
            {
                OnExit(m_cBlackBoard);
                m_cBlackBoard = null;
            }
            m_eActionResult = BTActionResult.Ready;
        }
    }
}
