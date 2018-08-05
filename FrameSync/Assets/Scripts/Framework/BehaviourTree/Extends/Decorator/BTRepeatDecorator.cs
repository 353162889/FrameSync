using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodeEditor;
using Framework;

namespace BTCore
{
    public class BTRepeatDecoratorData
    {
        [NEProperty("重复执行次数（0为无限次）")]
        public int times;
        [NEProperty("执行间隔(0为每帧)")]
        public FP spaceTime;
    }
    [BTNode(typeof(BTRepeatDecoratorData))]
    [NENodeDesc("重复执行当前子节点操作，直到最后一次返回成功，如果在执行间隔中，返回运行中")]
    public class BTRepeatDecorator : BTDecorator
    {
        private BTRepeatDecoratorData m_cRepeatData;
        private int m_nTimes;
        private FP m_sExecuteTime;
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cRepeatData = data as BTRepeatDecoratorData;
            m_nTimes = 0;
            m_sExecuteTime = 0;
        }

        public override BTResult OnEnter(BTBlackBoard blackBoard)
        {
            m_sExecuteTime += blackBoard.deltaTime;
            if (m_sExecuteTime >= m_cRepeatData.spaceTime)
            {
                m_sExecuteTime -= m_cRepeatData.spaceTime;
                m_nTimes++;
                BTResult result = base.OnEnter(blackBoard);
                if(result != BTResult.Running)
                {
                    if (m_cChild != null) m_cChild.Clear();
                }
                return result;
            }
            else
            {
                return BTResult.Running;
            }
        }

        public override BTResult Decorate(BTBlackBoard bloackBoard, BTResult result)
        {
            if (m_cRepeatData.times == 0 || m_nTimes < m_cRepeatData.times)
            {
                return BTResult.Running;
            }
            else
            {
                m_nTimes = 0;
                m_sExecuteTime = 0;
                return BTResult.Success;
            }
        }

        public override void Clear()
        {
            m_nTimes = 0;
            m_sExecuteTime = 0;
            base.Clear();
        }
    }
}
