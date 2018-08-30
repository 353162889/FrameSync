using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Framework;
using NodeEditor;

namespace Game
{
    public class BTG_PlayAudioActionData
    {
        [NEProperty("触发时间",true)]
        public FP time;
        [NEProperty("持续时间")]
        public FP duration;
        [NEProperty("音频ID(请从音效配置中读取)")]
        public int audioId;
    }

    [BTGameNode(typeof(BTG_PlayAudioActionData))]
    [NENodeDesc("播放一段音频")]
    public class BTG_PlayAudioAction : BaseTimeLineBTGameAction
    {
        private BTG_PlayAudioActionData m_cAudioData;
        private FP m_sEndTime;
        private bool m_bAutoDestory;
        private int m_nAudioKey;
        public override FP time
        {
            get
            {
                return m_cAudioData.time;
            }
        }

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cAudioData = data as BTG_PlayAudioActionData;
        }

        protected override void OnEnter(AgentObjectBlackBoard blackBoard)
        {
            base.OnEnter(blackBoard);
            m_sEndTime = FrameSyncSys.time + m_cAudioData.duration;
            m_bAutoDestory = m_cAudioData.duration <= 0;
            m_nAudioKey = GameAudioSys.Instance.Play(m_cAudioData.audioId);
        }

        public override BTActionResult OnRun(AgentObjectBlackBoard blackBoard)
        {
            if (FrameSyncSys.time < m_sEndTime) return BTActionResult.Running;
            return BTActionResult.Ready;
        }

        public override void OnExit(AgentObjectBlackBoard blackBoard)
        {
            if(!m_bAutoDestory)
            {
                GameAudioSys.Instance.Stop(m_cAudioData.audioId,m_nAudioKey);
            }
            m_sEndTime = 0;
            base.OnExit(blackBoard);
        }
    }
}
