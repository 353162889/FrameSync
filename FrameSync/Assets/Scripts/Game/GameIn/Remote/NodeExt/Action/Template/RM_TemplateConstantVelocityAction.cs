using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Framework;
using NodeEditor;

namespace Game
{
    public class RM_TemplateConstantVelocityActionData
    {
        [NEProperty("触发时间",true)]
        public FP time;
    }
    [RemoteNode(typeof(RM_TemplateConstantVelocityActionData))]
    public class RM_TemplateConstantVelocityAction : BaseTimeLineRemoteAction
    {
        private RM_TemplateConstantVelocityActionData m_cActionData;
        public override FP time { get { return m_cActionData.time; } }
        private FP m_sDistance;
        private TSVector m_sTargetPosition;
        private PointMove m_cPointMove;
        private Remote m_cRemote;
        private BTActionResult m_eCurActionResult;

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cActionData = data as RM_TemplateConstantVelocityActionData;
            m_cPointMove = new PointMove();
        }

        protected override void OnEnter(RemoteBlackBoard blackBoard)
        {
            m_sDistance = 0;
            m_cRemote = blackBoard.remote;
            var remoteTargetType = blackBoard.remote.remoteTargetType;
            switch(remoteTargetType)
            {
                case RemoteTargetType.Target:
                    m_sTargetPosition = m_cRemote.target.curPosition;
                    break;
                case RemoteTargetType.TargetForward:
                    m_sTargetPosition = m_cRemote.curPosition + m_cRemote.targetForward * m_cRemote.resInfo.max_move_distance;
                    break;
                case RemoteTargetType.TargetPosition:
                    m_sTargetPosition = m_cRemote.targetPosition;
                    break;
            }
            m_cPointMove.Clear();
            m_cPointMove.OnMoveStart += OnStartMove;
            m_cPointMove.OnMove += OnMove;
            m_cPointMove.OnMoveStop += OnStopMove;
            m_cPointMove.OnWillMove += OnWillMove;
            m_cPointMove.Move(m_cRemote.curPosition, m_sTargetPosition,m_cRemote.resInfo.move_speed);
            m_eCurActionResult = BTActionResult.Running;
        }

        private void OnStartMove(TSVector position, TSVector forward,bool stopToMove)
        {
            m_cRemote.StartMove(position, m_cPointMove.lstNextPosition,stopToMove);
        }

        private void OnMove(TSVector position, TSVector forward)
        {
            m_cRemote.Move(position,m_cPointMove.moveTimes);
            m_sDistance += (m_cRemote.curPosition - m_cRemote.lastPosition).magnitude;
            if (m_sDistance >= m_cRemote.resInfo.max_move_distance)
            {
                m_eCurActionResult = BTActionResult.Ready;
            }
        }

        private void OnStopMove(TSVector position, TSVector forward)
        {
            m_cRemote.StopMove();
            m_eCurActionResult = BTActionResult.Ready;
        }

        private void OnWillMove(TSVector willPosition, TSVector willforward,PM_CenterPoints willCenterPoints)
        {
            m_cRemote.WillMove(willPosition, willforward, willCenterPoints);
        }

        public override void OnExit(RemoteBlackBoard blackBoard)
        {
            m_cPointMove.Clear();
            m_cRemote = null;
            base.OnExit(blackBoard);
        }

        public override BTActionResult OnRun(RemoteBlackBoard blackBoard)
        {
            m_cPointMove.OnUpdate(blackBoard.deltaTime);
            Remote remote = blackBoard.remote;
            if(remote.remoteTargetType == RemoteTargetType.Target)
            {
                var nextTargetPosition = remote.target.curPosition;
                if(nextTargetPosition != m_sTargetPosition)
                {
                    m_sTargetPosition = nextTargetPosition;
                    m_cPointMove.Move(remote.curPosition, m_sTargetPosition, remote.resInfo.move_speed);
                    m_eCurActionResult = BTActionResult.Running;
                }
            }
            
            return m_eCurActionResult;
        }
    }
}
