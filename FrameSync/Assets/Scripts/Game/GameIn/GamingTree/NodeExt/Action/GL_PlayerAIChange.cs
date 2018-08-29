using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Framework;
using NodeEditor;

namespace Game
{
    public class GL_PlayerAIChangeData
    {
        [NEProperty("触发时间", true)]
        public FP time;
        [NEProperty("开启(或关闭)AI")]
        public bool enableAI;
    }
    [GamingNode(typeof(GL_PlayerAIChangeData))]
    [NENodeDesc("开启或关闭所有玩家的AI")]
    public class GL_PlayerAIChange : BaseTimeLineGamingAction
    {
        private GL_PlayerAIChangeData m_cAIChangeData;
        public override FP time
        {
            get
            {
                return m_cAIChangeData.time;
            }
        }

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cAIChangeData = data as GL_PlayerAIChangeData;
        }

        public override BTActionResult OnRun(GamingBlackBoard blackBoard)
        {
            var lst = PvpPlayerMgr.Instance.lstPlayer;
            for (int i = 0; i < lst.Count; i++)
            {
                lst[i].SetAIEnable(m_cAIChangeData.enableAI);
            }
            return BTActionResult.Ready;
        }

    }
}
