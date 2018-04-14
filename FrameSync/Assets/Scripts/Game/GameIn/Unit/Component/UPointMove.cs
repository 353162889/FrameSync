using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public class UPointMove
    {
        private TSVector m_sCurPosition;
        public TSVector curPosition
        {
            get { return m_sCurPosition; }
        }
        private TSVector m_sCurForward;
        public TSVector curForward
        {
            get { return m_sCurForward; }
        }

        private bool m_bIsMoving;
        public bool isMoving
        {
            get { return m_bIsMoving; }
        }

        private FP m_sSpeed;
        public FP speed
        {
            get { return m_sSpeed; }
        }

        private TSVector m_sNextPosition;

        private Queue<TSVector> m_queuePath;

        private FP m_sMoveTime;
        public FP moveTime { get { return m_sMoveTime; } }
        private FP m_sMoveTotalDistance;
        public FP moveTotalDistance { get { return m_sMoveTotalDistance; } }
        private FP m_sMoveDistance;
        public FP moveDistance { get { return m_sMoveDistance; } }

        public UPointMove()
        {
            m_queuePath = new Queue<TSVector>();
            m_bIsMoving = false;
            m_sNextPosition = TSVector.zero;
            m_sCurForward = TSVector.forward;
            m_sSpeed = 0;
            m_sMoveTime = 0;
            m_sMoveDistance = 0;
            m_sMoveTotalDistance = 0;
        }
       
        public void Move(TSVector startPosition, List<TSVector> movePath, FP speed)
        {
            if (movePath.Count <= 0) return;
            SetPosition(startPosition);
            m_sMoveTime = 0;
            m_sMoveDistance = 0;
            m_sMoveTotalDistance = (movePath[0] - startPosition).magnitude;
            for (int i = 0; i < movePath.Count; i++)
            {
                m_queuePath.Enqueue(movePath[i]);
                if(i > 0)
                {
                    m_sMoveTotalDistance += (movePath[i] - movePath[i - 1]).magnitude;
                }
            }
            m_bIsMoving = true;
            m_sSpeed = speed;
            if (!DequeuePoint())
            {
                StopMove();
            }
        }

        public void Move(TSVector startPosition, TSVector targetPosition,FP speed)
        {
            SetPosition(startPosition);
            m_sMoveTime = 0;
            m_sMoveDistance = 0;
            m_sMoveTotalDistance = (targetPosition - startPosition).magnitude;
            m_queuePath.Enqueue(targetPosition);
            m_bIsMoving = true;
            m_sSpeed = speed;
            if (!DequeuePoint())
            {
                StopMove();
            }
        }

        public void StopMove()
        {
            m_queuePath.Clear();
            m_bIsMoving = false;
            m_sNextPosition = TSVector.zero;
            m_sSpeed = 0;
            m_sMoveTime = 0;
            m_sMoveDistance = 0;
            m_sMoveTotalDistance = 0;
        }

        private void SetPosition(TSVector position)
        {
            StopMove();
            m_sCurPosition = position;
        }

        public void SetSpeed(FP speed)
        {
            if (m_sSpeed == speed) return;
            m_sSpeed = speed;
        }

        public void OnUpdate(FP deltaTime)
        {
            if (m_bIsMoving)
            {
                m_sMoveTime += deltaTime;
                TSVector deltaPosition = m_sNextPosition - m_sCurPosition;
                if (deltaPosition != TSVector.zero) m_sCurForward = deltaPosition.normalized;
                TSVector offset = m_sCurForward * m_sSpeed * deltaTime;
                if (deltaPosition.sqrMagnitude > offset.sqrMagnitude)
                {
                    m_sCurPosition += offset;
                    m_sMoveDistance += offset.magnitude;
                }
                else
                {
                    m_sCurPosition = m_sNextPosition;
                    m_sMoveDistance += deltaPosition.magnitude;
                    if (!DequeuePoint())
                    {
                        StopMove();
                    }
                }
            }
        }

        public bool DequeuePoint()
        {
            if (m_queuePath.Count > 0)
            {
                m_sNextPosition = m_queuePath.Dequeue();
                TSVector forward = m_sNextPosition - m_sCurPosition;
                if (forward != TSVector.zero)
                {
                    forward.Normalize();
                    m_sCurForward = forward;
                }
                return true;
            }
            return false;
        }
    }
}
