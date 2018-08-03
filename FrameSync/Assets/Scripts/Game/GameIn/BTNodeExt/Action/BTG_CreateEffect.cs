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
    public class BTG_CreateEffectData
    {
        [NEProperty("触发时间(非时间线内忽略该字段)", true)]
        public FP time;
        [NEProperty("作用的对象")]
        public BTActionTarget actionTarget;
        [NEProperty("特效名称")]
        public string effectName;
        [NEProperty("挂点")]
        public string hangPoint;
        [NEProperty("特效播放时间（0为自动销毁）")]
        public FP playTime;
    }
    [BTGameNode(typeof(BTG_CreateEffectData))]
    [NENodeDesc("目标身上某个挂点中创建特效（对象移除时需要做移除处理）")]
    public class BTG_CreateEffect : BaseTimeLineBTGameAction
    {
        private BTG_CreateEffectData m_cCreateEffectData;
        private GameObject m_cEffect;
        public override FP time { get { return m_cCreateEffectData.time; } }

        private FP m_sEndTime;
        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cCreateEffectData = data as BTG_CreateEffectData;
        }

        protected override void OnEnter(AgentObjectBlackBoard blackBoard)
        {
            var actionTarget = m_cCreateEffectData.actionTarget;
            AgentObject target = null;
            if (actionTarget == BTActionTarget.Host) target = blackBoard.host;
            else if (actionTarget == BTActionTarget.SelectTarget) target = blackBoard.selectAgentObjInfo.agentObj;
            if (target != null)
            {
                TSVector position;
                TSVector forward;
                var transfrom = target.GetHangPoint(m_cCreateEffectData.hangPoint, out position, out forward);
                bool autoDestory = m_cCreateEffectData.playTime <= 0;
                var effect = SceneEffectPool.Instance.CreateEffect(m_cCreateEffectData.effectName, autoDestory, transfrom);
                if(transfrom == null)
                {
                    effect.transform.position = position.ToUnityVector3();
                    effect.transform.forward = forward.ToUnityVector3();
                }
                if (!autoDestory) m_cEffect = effect;
            }
            m_sEndTime = FrameSyncSys.time + m_cCreateEffectData.playTime;
        }

        public override BTActionResult OnRun(AgentObjectBlackBoard blackBoard)
        {
            if (FrameSyncSys.time < m_sEndTime) return BTActionResult.Running;
            else return BTActionResult.Ready;
        }

        public override void OnExit(AgentObjectBlackBoard blackBoard)
        {
            if(m_cEffect != null)
            {
                SceneEffectPool.Instance.DestroyEffectGO(m_cEffect);
                m_cEffect = null;
            }
            base.OnExit(blackBoard);
        }
    }
}
