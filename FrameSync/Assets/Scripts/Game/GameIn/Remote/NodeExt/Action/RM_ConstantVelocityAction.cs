using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Framework;
using NodeEditor;

namespace Game
{
    public class RM_ConstantVelocityActionData
    {
        [NEProperty("触发时间",true)]
        public FP time;
        [NEProperty("移动速度")]
        public FP speed;
        [NEProperty("最大移动距离")]
        public FP maxDistance = 100;
    }
    [RemoteNode(typeof(RM_ConstantVelocityActionData))]
    public class RM_ConstantVelocityAction : BaseTimeLineRemoteAction
    {
        private RM_ConstantVelocityActionData m_cActionData;
        public override FP time { get { return m_cActionData.time; } }
        private FP m_sDistance;
        private TSVector m_sTargetPosition;
        private TSVector m_sForward;

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cActionData = data as RM_ConstantVelocityActionData;
        }

        protected override void OnEnter(RemoteBlackBoard blackBoard)
        {
            m_sDistance = 0;
            Remote remote = blackBoard.remote;
            var remoteTargetType = blackBoard.remote.remoteData.remoteTarget;
            switch(remoteTargetType)
            {
                case RemoteTargetType.Target:
                    m_sTargetPosition = remote.target.curPosition;
                    break;
                case RemoteTargetType.TargetForward:
                    m_sTargetPosition = remote.targetForward * m_cActionData.maxDistance;
                    break;
                case RemoteTargetType.TargetPosition:
                    m_sTargetPosition = remote.targetPosition;
                    break;
            }
            m_sForward = m_sTargetPosition - remote.curPosition;
            if (!m_sForward.IsZero()) m_sForward.Normalize();
        }

        public override BTActionResult OnRun(RemoteBlackBoard blackBoard)
        {
            if (m_sForward.IsZero()) return BTActionResult.Ready;
            Remote remote = blackBoard.remote;
            var offset = m_sForward * m_cActionData.speed * blackBoard.deltaTime;
            var nextPosition = remote.curPosition + offset;
            var nextForward = (m_sTargetPosition - nextPosition);
            bool finish = false;
            if(nextForward.IsZero())
            {
                finish = true;
            }
            nextForward.Normalize();
            if(!finish && TSVector.Angle(nextForward,m_sForward) > 90)
            {
                finish = true;
                nextPosition = m_sTargetPosition;
                offset = m_sTargetPosition - remote.curPosition;
            }
            remote.SetPosition(nextPosition);
            m_sDistance += offset.magnitude;

            if(remote.remoteData.remoteTarget == RemoteTargetType.Target)
            {
                m_sTargetPosition = remote.target.curPosition;
                m_sForward = m_sTargetPosition - remote.curPosition;
                if (!m_sForward.IsZero()) m_sForward.Normalize();
            }
            if (finish || m_sDistance >= m_cActionData.maxDistance)
            {
                return BTActionResult.Ready;
            }
            return BTActionResult.Running;
        }
    }
}
