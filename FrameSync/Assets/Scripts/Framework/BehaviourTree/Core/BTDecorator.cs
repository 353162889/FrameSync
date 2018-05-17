using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BTCore
{
    [NENodeCategory("Decorator")]
    public abstract class BTDecorator : BTNode
    {
        protected BTNode m_cChild;

        public override void AddChild(BTNode child)
        {
            if (m_cChild != null)
            {
                Debug.LogError(this.GetType() + " has exist child node! add has override it");
                if (m_lstChild != null && m_lstChild.Contains(m_cChild))
                {
                    m_lstChild.Remove(m_cChild);
                }
            }
            base.AddChild(child);
            m_cChild = child;
        }

        sealed public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            BTResult result = OnEnter(blackBoard);
            return Decorate(blackBoard, result);
        }

        public virtual BTResult OnEnter(BTBlackBoard blackBoard)
        {
            if (m_cChild == null) return BTResult.Success;
            return m_cChild.OnTick(blackBoard);
        }

        public virtual BTResult Decorate(BTBlackBoard bloackBoard,BTResult result)
        {
            return result;
        }

        public override void Clear()
        {
            m_cChild = null;
            base.Clear();
        }
    }
}
