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
    public class SK_CreateLaserActionData
    {
        [NEProperty("触发时间(非时间线内忽略该字段)", true)]
        public FP time;
        [NEProperty("特效名称")]
        public string effectName;
        [NEProperty("挂点")]
        public string hangPoint;
    }
    [SkillNode(typeof(SK_CreateLaserActionData))]
    [NENodeDesc("目标身上创建激光")]
    public class SK_CreateLaserAction : BaseTimeLineSkillAction
    {
        private SK_CreateLaserActionData m_cCreateEffectData;
        private GameObject m_cEffect;
        public override FP time { get { return m_cCreateEffectData.time; } }
        private bool m_bGetLaserEffect;

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cCreateEffectData = data as SK_CreateLaserActionData;
        }

        protected override void OnEnter(SkillBlackBoard blackBoard)
        {
            TSVector position;
            TSVector forward;
            var transfrom = blackBoard.host.GetHangPoint(m_cCreateEffectData.hangPoint, out position, out forward);
            var effect = SceneEffectPool.Instance.CreateEffect(m_cCreateEffectData.effectName, false, transfrom);
            if (transfrom == null)
            {
                effect.transform.position = position.ToUnityVector3();
                effect.transform.forward = forward.ToUnityVector3();
            }
            m_cEffect = effect;
            m_bGetLaserEffect = false;
        }

        public override BTActionResult OnRun(SkillBlackBoard blackBoard)
        {
            if(!m_bGetLaserEffect)
            {
                LaserEffect laserEffect = m_cEffect.GetComponentInChildren<LaserEffect>();
                if (laserEffect != null)
                {
                    blackBoard.SetVariable(GameConst.SkillLaserEffectName, (SharedComponent)laserEffect);
                    m_bGetLaserEffect = true;
                }
            }
            return BTActionResult.Running;
        }

        public override void OnExit(SkillBlackBoard blackBoard)
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
