using UnityEngine;
using System.Collections;
using Framework;
using NodeEditor;
using BTCore;

namespace Game
{
    public class SK_UpdateLaserLengthData
    {
        [NEProperty("默认激光长度")]
        public FP nLength;
        [NEProperty("原点（挂点）位置")]
        public string hangPoint;
    }

    [SkillNode(typeof(SK_UpdateLaserLengthData))]
    [NENodeDesc("更新激光的长度")]
    public class SK_UpdateLaserLength : BaseSkillAction
    {
        private SK_UpdateLaserLengthData m_cLaserData;

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cLaserData = data as SK_UpdateLaserLengthData;
        }

        protected override void OnEnter(SkillBlackBoard blackBoard)
        {
            base.OnEnter(blackBoard);
        }

        public override BTActionResult OnRun(SkillBlackBoard blackBoard)
        {
            var variable = blackBoard.GetVariable(GameConst.SkillLaserEffectName);
            if (variable == null) return BTActionResult.Ready;
            LaserEffect laserEffect = (LaserEffect)variable.GetValue();
            if (laserEffect == null) return BTActionResult.Ready;
            float length = m_cLaserData.nLength.AsFloat();
            if (blackBoard.selectAgentObjInfo.agentObj != null)
            {
                TSVector position;
                TSVector forward;
                var transfrom = blackBoard.host.GetHangPoint(m_cLaserData.hangPoint, out position, out forward);
                length = (blackBoard.selectAgentObjInfo.hitPoint.ToUnityVector3() - transfrom.position).magnitude;
            }
            laserEffect.UpdateLength(length);
            return BTActionResult.Ready;
        }

        public override void OnExit(SkillBlackBoard blackBoard)
        {
            base.OnExit(blackBoard);
        }
    }
}