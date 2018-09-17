using UnityEngine;
using System.Collections;
using NodeEditor;
using Framework;
using BTCore;

namespace Game
{
    public class UnitPlayAnimActionData
    {
        [NEProperty("触发时间",true)]
        public FP time;
        [NEProperty("动画名称")]
        public string animName;
        [NEProperty("动画时长")]
        public FP animTime;
        [NEProperty("播完之后的回复动画")]
        public string recoveryAnimName;
    }
    [AINode(typeof(UnitPlayAnimActionData))]
    [NENodeDesc("播放动画")]
    public class UnitPlayAnimAction : BaseTimeLineAIAction
    {
        private UnitPlayAnimActionData m_cAnimData;

        public override FP time
        {
            get
            {
                return m_cAnimData.time;
            }
        }

        private Unit m_cUnit;
        private FP m_fTime;

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cAnimData = data as UnitPlayAnimActionData;
        }

        protected override void OnEnter(AIBlackBoard blackBoard)
        {
            m_cUnit = (Unit)blackBoard.host.agent;
            m_fTime = m_cAnimData.animTime;
            if (m_cUnit != null && !string.IsNullOrEmpty(m_cAnimData.animName))
            {
                m_cUnit.ResetAnimTrigger(m_cAnimData.animName);
                m_cUnit.SetAnimTrigger(m_cAnimData.animName);
            }
            base.OnEnter(blackBoard);
        }

        public override BTActionResult OnRun(AIBlackBoard blackBoard)
        {
            m_fTime -= blackBoard.deltaTime;
            if (m_fTime <= 0) return BTActionResult.Ready;
            return BTActionResult.Running;
        }

        public override void OnExit(AIBlackBoard blackBoard)
        {
            if(m_cUnit != null)
            {
                if (!string.IsNullOrEmpty(m_cAnimData.animName))
                {
                    m_cUnit.ResetAnimTrigger(m_cAnimData.animName);
                }
                if (!string.IsNullOrEmpty(m_cAnimData.recoveryAnimName))
                {
                    m_cUnit.SetAnimTrigger(m_cAnimData.animName);
                }
            }
            m_cUnit = null;
            base.OnExit(blackBoard);
        }
    }
}