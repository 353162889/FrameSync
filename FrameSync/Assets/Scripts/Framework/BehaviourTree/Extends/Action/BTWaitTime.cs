using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    public class BTWaitTimeData
    {
        [NEProperty("触发时间")]
        public FP exeTime;
        [NEProperty("等待时间")]
        public FP time;
    }

    [BTNode(typeof(BTWaitTimeData))]
    [NENodeDesc("等待一段时间")]
    public class BTWaitTime : BTAction, IBTTimeLineNode
    {
        private BTWaitTimeData m_cWaitTimeData;
        private FP m_sWaitTime;

        public FP time
        {
            get
            {
                return m_cWaitTimeData.exeTime;
            }
        }

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cWaitTimeData = data as BTWaitTimeData;
            m_sWaitTime = 0;
        }

        public override BTActionResult OnRun(BTBlackBoard blackBoard)
        {
            m_sWaitTime -= blackBoard.deltaTime;
            if(m_sWaitTime <= 0)
            {
                return BTActionResult.Ready;
            }
            else
            {
                return BTActionResult.Running;
            }
        }

        public override void OnEnter(BTBlackBoard blackBoard)
        {
            m_sWaitTime = m_cWaitTimeData.time;
            base.OnEnter(blackBoard);
        }

        public override void OnExit(BTBlackBoard blackBoard)
        {
            m_sWaitTime = 0;
            base.OnExit(blackBoard);
        }

        public override void Clear()
        {
            m_sWaitTime = 0;
            base.Clear();
        }
    }
}
