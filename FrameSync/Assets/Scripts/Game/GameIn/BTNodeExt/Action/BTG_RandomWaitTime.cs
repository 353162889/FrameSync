using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;

namespace Game
{
    public class BTG_RandomWaitTimeData
    {
        [NEProperty("最少时间")]
        public FP minTime;
        [NEProperty("最大时间")]
        public FP maxTime;
    }

    [BTGameNode(typeof(BTG_RandomWaitTimeData))]
    [NENodeDesc("随机等待一段时间")]
    public class BTG_RandomWaitTime : BaseBTGameAction
    {
        private BTG_RandomWaitTimeData m_cWaitTimeData;
        private FP m_sWaitTime;
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cWaitTimeData = data as BTG_RandomWaitTimeData;
            m_sWaitTime = 0;
        }

        protected override void OnEnter(AgentObjectBlackBoard blackBoard)
        {
            m_sWaitTime = GameInTool.Random(m_cWaitTimeData.minTime, m_cWaitTimeData.maxTime);
            base.OnEnter(blackBoard);
        }

        public override BTActionResult OnRun(AgentObjectBlackBoard blackBoard)
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

        public override void OnExit(AgentObjectBlackBoard blackBoard)
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

