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
        [NEProperty("远程ID")]
        public int remoteId;
        [NEProperty("挂点")]
        public string hangPoint;
        [NEProperty("使用挂点方向")]
        public bool useHangPoint;
    }
    [RemoteNode(typeof(RM_CreateRemoteActionData))]
    [NENodeDesc("从宿主挂点位置创建远程")]
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
            var createRemote = BattleScene.Instance.CreateRemote(m_cActionData.remoteId, remote.campId, bornPosition, targetForward, remote.targetAgentId, remote.targetAgentType, remote.targetPosition, targetForward);
            createRemote.SetAttrValue((int)AttrType.Attack, remote.GetAttrValue((int)AttrType.Attack));
            return BTActionResult.Ready;
        }
    }
}
