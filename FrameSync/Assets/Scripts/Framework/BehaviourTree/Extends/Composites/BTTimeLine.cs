﻿using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    public interface IBTTimeLineNode
    {
        FP time { get; }
    }

    public class BTTimeLineData
    {
        [NEProperty("初始时间(Editor下生效)",true)]
        public FP startRunTime;
    }
    [BTNode(typeof(BTTimeLineData))]
    public class BTTimeLine : BTComposite
    {
        private BTTimeLineData m_cTimeLineData;
        private bool m_bIsEnd = true;
        private FP m_sTime;
        private int m_nCount = 0;
        protected List<BTResult> m_lstResults = new List<BTResult>();

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cTimeLineData = data as BTTimeLineData;
        }

        public override void AddChild(BTNode child)
        {
            if (child is IBTTimeLineNode)
            {
                base.AddChild(child);
                m_lstResults.Add(BTResult.Running);
                m_lstChild.Sort(TimeSort);
            }
            else
            {
                CLog.LogError("BTTimeLine AddChild is not IBTTimeLineNode");
            }
        }

        private int TimeSort(BTNode a,BTNode b)
        {
            FP timeA = ((IBTTimeLineNode)a).time;
            FP timeB = ((IBTTimeLineNode)b).time;
            if (timeA > timeB) return 1;
            else if (timeA < timeB) return -1;
            return 0; 
        }

        public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            FP deltaTime = blackBoard.deltaTime;
            if(m_bIsEnd)
            {
               
                m_bIsEnd = false;
#if UNITY_EDITOR
                m_sTime = m_cTimeLineData.startRunTime;
#else
                m_sTime = 0;
#endif
                m_nCount = 0;
            }
            int count = m_lstChild.Count;
            for (int i = 0; i < count; i++)
            {
                if(m_lstResults[i] == BTResult.Running)
                {
                    BTNode child = m_lstChild[i];
                    IBTTimeLineNode timeLineNode = child as IBTTimeLineNode;
                    if(timeLineNode.time <= m_sTime)
                    {
                        BTResult result = child.OnTick(blackBoard);
                        if(result != BTResult.Running)
                        {
                            m_lstResults[i] = result;
                            m_nCount++;
                        }
                    }
                }
            }
            m_sTime += deltaTime;
            if (m_nCount == count)
            {
                Clear();
                return BTResult.Success;
            }
            return BTResult.Running;
        }

        public override void Clear()
        {
            m_bIsEnd = true;
            m_nCount = 0;
            for (int i = 0; i < m_lstResults.Count; i++)
            {
                m_lstResults[i] = BTResult.Running;
            }
            base.Clear();
        }
    }
}
