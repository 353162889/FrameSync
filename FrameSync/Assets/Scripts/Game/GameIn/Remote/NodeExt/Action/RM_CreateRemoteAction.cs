using BTCore;
using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class RM_CreateRemoteActionData
    {
        [NEProperty("触发时间", true)]
        public FP time;
        public int remoteId;
        public string hangPoint;
        public bool useHangPoint;
    }
    [SkillNode(typeof(RM_CreateRemoteActionData))]
    public class RM_CreateRemoteAction : BaseTimeLineRemoteAction
    {
        private RM_CreateRemoteActionData m_cActionData;
        public override FP time { get { return m_cActionData.time; } }

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cActionData = data as RM_CreateRemoteActionData;
        }

        public override BTActionResult OnRun(RemoteBlackBoard blackBoard)
        {
            Remote remote = blackBoard.remote;
            TSVector bornPosition = remote.curPosition;
            TSVector bornForward = remote.curForward;
            if (!string.IsNullOrEmpty(m_cActionData.hangPoint))
            {
                remote.GetHangPoint(m_cActionData.hangPoint, out bornPosition, out bornForward);
            }
            TSVector targetForward = remote.targetForward;
            if (m_cActionData.useHangPoint)
            {
                targetForward = bornForward;
            }
            BattleScene.Instance.CreateRemote(m_cActionData.remoteId, bornPosition, targetForward, remote.targetAgentId, remote.targetAgentType, remote.targetPosition, targetForward);
            return BTActionResult.Ready;
        }
    }
}
