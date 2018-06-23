using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    public class BTParallelAndData
    {
    }

    [BTNode(typeof(BTParallelAndData))]
    /// <summary>
    /// 并行与，需要所有子对象都运行完成才返回
    /// </summary>
    public class BTParallelAnd : BTComposite
    {
        protected List<BTResult> m_lstResults = new List<BTResult>();
        protected int m_nCount;
        public BTParallelAnd()
        {
            m_nCount = 0;
        }

        public override void AddChild(BTNode child)
        {
            base.AddChild(child);
            m_lstResults.Add(BTResult.Running);
        }

        public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            int count = m_lstChild.Count;
            for (int i = 0; i < count; i++)
            {
                if(m_lstResults[i] == BTResult.Running)
                {
                    BTResult childResult = m_lstChild[i].OnTick(blackBoard);
                    if(childResult != BTResult.Running)
                    {
                        m_lstResults[i] = childResult;
                        m_nCount++;
                    }
                }
            }
            if(m_nCount == count)
            {
                for (int i = 0; i < count; i++)
                {
                    //只要有一个成功了，那么返回成功
                    if(m_lstResults[i] == BTResult.Success)
                    {
                        return BTResult.Success;
                    }
                }
                return BTResult.Failure;
            }
            return BTResult.Running;
        }

        public override void Clear()
        {
            m_nCount = 0;
            for (int i = 0; i < m_lstResults.Count; i++)
            {
                m_lstResults[i] = BTResult.Running;
            }
            base.Clear();
        }
    }
}
