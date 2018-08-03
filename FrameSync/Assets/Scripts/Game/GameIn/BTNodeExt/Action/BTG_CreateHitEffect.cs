using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Framework;
using NodeEditor;
using UnityEngine;

namespace Game
{
    public class BTG_CreateHitEffectData
    {
        [NEProperty("触发时间(非时间线内忽略该字段)", true)]
        public FP time;
        [NEProperty("特效名称")]
        public string effectName;
        [NEProperty("是否使用击中点(不使用默认为对象中心点)")]
        public bool useHitPoint;
        [NEProperty("是否使用击中方向")]
        public bool useHitDirection;
        [NEProperty("特效播放时间（0为自动销毁）")]
        public FP playTime;
    }
    [BTGameNode(typeof(BTG_CreateHitEffectData))]
    [NENodeDesc("在击中目标上点创建特效,如果没有击中对象，不会创建特效（对象移除时需要做移除处理）")]
    public class BTG_CreateHitEffect : BaseTimeLineBTGameAction
    {
        private BTG_CreateHitEffectData m_cCreateHitEffectData;
        private GameObject m_cEffect;
        public override FP time { get { return m_cCreateHitEffectData.time; } }

        private FP m_sEndTime;
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cCreateHitEffectData = data as BTG_CreateHitEffectData;
        }

        protected override void OnEnter(AgentObjectBlackBoard blackBoard)
        {
            AgentObject target = blackBoard.selectAgentObjInfo.agentObj;
            if (target != null)
            {
                TSVector position = target.curPosition;
                TSVector forward = TSVector.forward;
                if (m_cCreateHitEffectData.useHitPoint) position = blackBoard.selectAgentObjInfo.hitPoint;
                if (m_cCreateHitEffectData.useHitDirection) forward = blackBoard.selectAgentObjInfo.hitDirect;
                bool autoDestory = m_cCreateHitEffectData.playTime <= 0;
                var effect = SceneEffectPool.Instance.CreateEffect(m_cCreateHitEffectData.effectName, autoDestory, null);
                effect.transform.position = position.ToUnityVector3();
                effect.transform.forward = forward.ToUnityVector3();
                if (!autoDestory) m_cEffect = effect;
            }
            m_sEndTime = FrameSyncSys.time + m_cCreateHitEffectData.playTime;
        }

        public override BTActionResult OnRun(AgentObjectBlackBoard blackBoard)
        {
            if (FrameSyncSys.time < m_sEndTime) return BTActionResult.Running;
            else return BTActionResult.Ready;
        }

        public override void OnExit(AgentObjectBlackBoard blackBoard)
        {
            if (m_cEffect != null)
            {
                SceneEffectPool.Instance.DestroyEffectGO(m_cEffect);
                m_cEffect = null;
            }
            base.OnExit(blackBoard);
        }
    }
}

