using UnityEngine;
using NodeEditor;
using Framework;
using BTCore;

namespace Game
{

    public class GL_RandomWaitTimeData
    {
        [NEProperty("触发时间")]
        public FP time;
        [NEProperty("最小时间")]
        public FP minTime;
        [NEProperty("最大时间")]
        public FP maxTime;
    }

    [GamingNode(typeof(GL_RandomWaitTimeData))]
    [NENodeDesc("等待一段时间")]
    public class GL_RandomWaitTime : BaseTimeLineGamingAction
    {
        private GL_RandomWaitTimeData m_cWaitTimeData;
        private FP m_sWaitTime;

        public override FP time
        {
            get
            {
                return m_cWaitTimeData.time;
            }
        }

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cWaitTimeData = data as GL_RandomWaitTimeData;
            m_sWaitTime = 0;
        }


        protected override BTActionResult OnRun(GamingBlackBoard blackBoard)
        {
            m_sWaitTime -= blackBoard.deltaTime;
            if (m_sWaitTime <= 0)
            {
                return BTActionResult.Ready;
            }
            else
            {
                return BTActionResult.Running;
            }
        }

        protected override void OnEnter(GamingBlackBoard blackBoard)
        {
            m_sWaitTime = GameInTool.Random(m_cWaitTimeData.minTime,m_cWaitTimeData.maxTime);
            base.OnEnter(blackBoard);
        }

        protected override void OnExit(GamingBlackBoard blackBoard)
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