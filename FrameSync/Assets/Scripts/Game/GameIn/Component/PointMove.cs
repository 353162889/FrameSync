using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public class PointMove
    {
        public delegate void UPointMoveHandler(TSVector position,TSVector forward);
        public event UPointMoveHandler OnMoveStart;
        public event UPointMoveHandler OnMove;
        public event UPointMoveHandler OnWillMove;
        public event UPointMoveHandler OnMoveStop;
        private TSVector m_sCurPosition;
        public TSVector curPosition
        {
            get { return m_sCurPosition; }
        }

        public List<TSVector> lstNextPosition { get { return m_lstNextPosition; } }
        private List<TSVector> m_lstNextPosition;
        private List<TSVector> m_lstNextForward;

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

        private bool m_bIsCalculateFinish;

        private FP m_sStartMoveTime;

        public PointMove()
        {
            m_queuePath = new Queue<TSVector>();
            m_lstNextPosition = new List<TSVector>();
            m_lstNextForward = new List<TSVector>();
            m_bIsMoving = false;
            m_sNextPosition = TSVector.zero;
            m_sCurForward = TSVector.forward;
            m_sSpeed = 0;
        }

        public void Clear()
        {
            StopMove();
            OnMoveStart = null;
            OnMove = null;
            OnMoveStop = null;
            OnWillMove = null;
        }
        public bool Move(TSVector startPosition, List<TSVector> movePath, FP speed)
        {
            if (movePath.Count <= 0) return false;
            m_queuePath.Clear();
            for (int i = 0; i < movePath.Count; i++)
            {
                m_queuePath.Enqueue(movePath[i]);
            }
            TSVector forward;
            if(!DequeuePoint(startPosition, out forward))
            {
                return false;
            }
            SetPosition(startPosition, forward);
            m_bIsMoving = true;
            m_bIsCalculateFinish = false;
            m_sSpeed = speed;
            PreCalculate();
            m_sStartMoveTime = FrameSyncSys.time;
            if (null != OnMoveStart)
            {
                OnMoveStart(m_sCurPosition,m_sCurForward);
            }
            return true;
        }

        public bool Move(TSVector startPosition, TSVector targetPosition,FP speed)
        {
            m_queuePath.Clear();
            m_queuePath.Enqueue(targetPosition);
            TSVector forward;
            if(!DequeuePoint(startPosition,out forward))
            {
                return false;
            }
            SetPosition(startPosition, forward);
            m_bIsMoving = true;
            m_bIsCalculateFinish = false;
            m_sSpeed = speed;
            PreCalculate();
            m_sStartMoveTime = FrameSyncSys.time;
            if (null != OnMoveStart)
            {
                OnMoveStart(m_sCurPosition,m_sCurForward);
            }
            return true;
        }

        public void StopMove()
        {
            if(null != OnMoveStop)
            {
                OnMoveStop(m_sCurPosition, m_sCurForward);
            }
            m_queuePath.Clear();
            m_lstNextPosition.Clear();
            m_lstNextForward.Clear();
            m_bIsMoving = false;
            m_bIsCalculateFinish = false;
            m_sSpeed = 0;
        }

        public void SetSpeed(FP speed)
        {
            if (m_sSpeed == speed) return;
            m_sSpeed = speed;
        }
        //执行完Move后立即回执行OnUpdate(即开始移动与移动在同一帧内)(为了处理在移动的同时更改移动方向，会卡一帧的问题)
        public void OnUpdate(FP deltaTime)
        {
            if (m_bIsMoving)
            {
                if (m_lstNextPosition.Count > 0)
                {
                    TSVector lastPosition = m_sCurPosition;
                    m_sCurPosition = m_lstNextPosition[0];
                    m_sCurForward = m_lstNextForward[0];
                    //TSVector curForward = m_sCurPosition - lastPosition;
                    //if(curForward != TSVector.zero)
                    //{
                    //    curForward.Normalize();
                    //    m_sCurForward = curForward;
                    //}
                    m_lstNextPosition.RemoveAt(0);
                    m_lstNextForward.RemoveAt(0);
                    if (null != OnMove)
                    {
                        OnMove(m_sCurPosition,m_sCurForward);
                    }
                    if (!m_bIsCalculateFinish)
                    {
                        TSVector position;
                        TSVector forward;
                        bool canCalculate = CalculateNextPosition(deltaTime, out position,out forward);
                        m_lstNextPosition.Add(position);
                        m_lstNextForward.Add(forward);
                        if(null != OnWillMove)
                        {
                            OnWillMove(position,forward);
                        }
                        if (!canCalculate) m_bIsCalculateFinish = true;
                    }
                }
                else
                {
                    StopMove();
                }
            }
        }


        private void SetPosition(TSVector position, TSVector forward)
        {
            StopMove();
            m_sCurPosition = position;
            m_sCurForward = forward;
        }

        private bool DequeuePoint(TSVector curPosition,out TSVector forward)
        {
            forward = TSVector.forward;
            if (m_queuePath.Count > 0)
            {
                while(m_queuePath.Count > 0)
                {
                    m_sNextPosition = m_queuePath.Dequeue();
                    forward = m_sNextPosition - curPosition;
                    if (!forward.IsZero())
                    {
                        forward.Normalize();
                        return true;
                    }
                }
            }
            return false;
        }

        private void PreCalculate()
        {
            PreCalculate(3, FrameSyncSys.OnFrameTime);
        }

        private void PreCalculate(int count, FP deltaTime)
        {
            for (int i = 0; i < count; i++)
            {
                TSVector position;
                TSVector forward;
                bool canCalculate = CalculateNextPosition(deltaTime, out position,out forward);
                m_lstNextPosition.Add(position);
                m_lstNextForward.Add(forward);
                if (!canCalculate)
                {
                    m_bIsCalculateFinish = true;
                    break;
                }
            }
        }

        private bool CalculateNextPosition(FP deltaTime, out TSVector position,out TSVector forward)
        {
            TSVector curPosition;
            if (m_lstNextPosition.Count > 0)
            {
                curPosition = m_lstNextPosition[m_lstNextPosition.Count - 1];
            }
            else
            {
                curPosition = m_sCurPosition;
            }
            return CalculateNextPosition(curPosition, deltaTime, out position,out forward);
        }

        private bool CalculateNextPosition(TSVector curPosition, FP deltaTime, out TSVector position, out TSVector forward)
        {
            position = TSVector.zero;
            forward = m_sCurForward;
            TSVector deltaPosition = m_sNextPosition - curPosition;
            if(!deltaPosition.IsZero())
            {
                TSVector deltaForward = deltaPosition.normalized;
                TSVector offset = deltaForward * m_sSpeed * deltaTime;
                if (deltaPosition.sqrMagnitude > offset.sqrMagnitude)
                {
                    position = curPosition + offset;
                }
                else
                {
                    position = m_sNextPosition;
                    if (!DequeuePoint(position,out forward))
                    {
                        forward = m_sCurForward;
                        return false;
                    }
                }
            }
            else
            {
                position = m_sNextPosition;
                if (!DequeuePoint(position,out forward))
                {
                    forward = m_sCurForward;
                    return false;
                }
            }
            return true;
        }
    }
}
