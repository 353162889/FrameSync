using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    /// <summary>
    /// 点对点移动时，有时候速度过快，导致会进过多个外部给与的点，需要记录下来（主要用来给表现做处理）
    /// </summary>
    public struct PM_CenterPoints
    {
        public List<TSVector> lstCenterPoint;

        public void Init()
        {
            if (null == lstCenterPoint)
            {
                lstCenterPoint = ResetObjectPool<List<TSVector>>.Instance.GetObject();
            }
        }

        public void Clear()
        {
            if (null != lstCenterPoint)
            {
                ResetObjectPool<List<TSVector>>.Instance.SaveObject(lstCenterPoint);
                lstCenterPoint = null;
            }
        }
    }
    
    public class PointMove
    {
        public delegate void UPointMoveHandler(TSVector position,TSVector forward);
        public delegate void UPointMovePMHandler(TSVector position, TSVector forward,PM_CenterPoints centerPoints);
        public event UPointMoveHandler OnMoveStart;
        public event UPointMoveHandler OnMove;
        public event UPointMovePMHandler OnWillMove;
        public event UPointMoveHandler OnMoveStop;
        private TSVector m_sCurPosition;
        public TSVector curPosition
        {
            get { return m_sCurPosition; }
        }

        //记录（几帧）需要移动的点路径以及一帧中经过的点
        public List<TSVector> lstNextPosition { get { return m_lstNextPosition; } }
        private List<TSVector> m_lstNextPosition;
        public List<PM_CenterPoints> lstNextPositionCenterPoint { get { return m_lstNextPositionCenterPoint; } }
        private List<PM_CenterPoints> m_lstNextPositionCenterPoint;
        private List<TSVector> m_lstOnFrameCenterPosition;
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

        private TSVector m_sTargetPosition;
        public TSVector targetPosition { get { return m_sTargetPosition; } }

        private TSVector m_sNextPosition;

        private Queue<TSVector> m_queuePath;

        private bool m_bIsCalculateFinish;

        private FP m_sStartMoveTime;

        public PointMove()
        {
            m_queuePath = new Queue<TSVector>();
            m_lstNextPosition = new List<TSVector>();
            m_lstNextPositionCenterPoint = new List<PM_CenterPoints>();
            m_lstOnFrameCenterPosition = new List<TSVector>();
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
            if(m_bIsMoving)
            {
                StopMove();
            }
            m_queuePath.Clear();
            m_sTargetPosition = startPosition;
            for (int i = 0; i < movePath.Count; i++)
            {
                m_queuePath.Enqueue(movePath[i]);
                if (i == movePath.Count - 1) m_sTargetPosition = movePath[i];
            }

            m_sNextPosition = startPosition;
            TSVector position = m_sNextPosition;
            TSVector forward = TSVector.forward;
            //主要用来初始化下一个position与方向,因此movelen可以为0
            if (!DequeuePoint(ref m_sNextPosition,ref position, ref forward, 0,false))
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
            if (m_bIsMoving)
            {
                StopMove();
            }
            m_queuePath.Clear();
            m_sTargetPosition = targetPosition;
            m_queuePath.Enqueue(targetPosition);
            m_sNextPosition = startPosition;
            TSVector position = m_sNextPosition;
            TSVector forward = TSVector.forward;
            //主要用来初始化下一个position与方向,因此movelen可以为0
            if (!DequeuePoint(ref m_sNextPosition,ref position,ref forward,0,false))
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
            for (int i = 0; i < m_lstNextPositionCenterPoint.Count; i++)
            {
                m_lstNextPositionCenterPoint[i].Clear();
            }
            m_lstNextPositionCenterPoint.Clear();
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
                    m_lstNextPosition.RemoveAt(0);
                    m_lstNextForward.RemoveAt(0);
                    m_lstNextPositionCenterPoint[0].Clear();
                    m_lstNextPositionCenterPoint.RemoveAt(0);
                    if (null != OnMove)
                    {
                        OnMove(m_sCurPosition,m_sCurForward);
                    }
                    if (!m_bIsCalculateFinish)
                    {
                        TSVector position;
                        TSVector forward;
                        m_lstOnFrameCenterPosition.Clear();
                        bool canCalculate = CalculateNextPosition(deltaTime, out position,out forward);
                        if (canCalculate)
                        {
                            PM_CenterPoints centerPositions = new PM_CenterPoints();
                            //添加中途路径点
                            if (m_lstOnFrameCenterPosition.Count > 0)
                            {
                                centerPositions.Init();
                                for (int i = 0; i < m_lstOnFrameCenterPosition.Count; i++)
                                {
                                    centerPositions.lstCenterPoint.Add(m_lstOnFrameCenterPosition[i]);
                                }
                            }
                            m_lstOnFrameCenterPosition.Clear();

                            m_lstNextPosition.Add(position);
                            m_lstNextPositionCenterPoint.Add(centerPositions);
                            m_lstNextForward.Add(forward);
                            //if (m_lstNextPosition.Count > 2)
                            //{
                            //    int count = m_lstNextPosition.Count;
                            //    if (m_lstNextPositionCenterPoint[count - 1].lstCenterPoint != null)
                            //    {
                            //        var tempLst = m_lstNextPositionCenterPoint[count - 1].lstCenterPoint;
                            //        var start = m_lstNextPosition[count - 2];
                            //        FP len = 0;
                            //        for (int i = 0; i < tempLst.Count; i++)
                            //        {
                            //            len += (tempLst[i] - start).magnitude;
                            //            start = tempLst[i];
                            //        }
                            //        len += (m_lstNextPosition[count - 1] - start).magnitude;
                            //        CLog.LogArgs("moveDis:", len.AsFloat());
                            //    }
                            //    else
                            //    {
                            //        CLog.LogArgs("moveDis:", (m_lstNextPosition[count - 1] - m_lstNextPosition[count - 2]).magnitude.AsFloat());
                            //    }
                            //}

                            if (null != OnWillMove)
                            {
                                OnWillMove(position, forward, centerPositions);
                            }
                        }
                        else
                        {
                            m_bIsCalculateFinish = true;
                        }
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
            m_sCurPosition = position;
            m_sCurForward = forward;
        }

        /// <summary>
        /// 是否能出列一个点，达到移动要求（最后一个点除外，可能不能达到movelen）
        /// 初始当前下一个点和当前点应该是一样的
        /// </summary>
        /// <param name="curNextPosition">当前下一个点</param>
        /// <param name="curPosition">当前位置</param>
        /// <param name="curForward">当前方向</param>
        /// <param name="moveLen">移动距离</param>
        /// <returns>是否成功</returns>
        private bool DequeuePoint(ref TSVector curNextPosition, ref TSVector curPosition, ref TSVector curForward,FP moveLen,bool recordCurrent)
        {
            if(curNextPosition != curPosition)
            {
                CLog.LogError("当前下一个点与当前位置不一致(上层逻辑有问题)");
                return false;
            }
            TSVector preNextPosition;
            if(recordCurrent && moveLen != 0)
            {
                m_lstOnFrameCenterPosition.Add(curNextPosition);
            }
            if (m_queuePath.Count > 0)
            {
                bool succ = false;
                while (moveLen >= 0 && m_queuePath.Count > 0)
                {
                    var tempNextPosition = m_queuePath.Dequeue();
                    if (tempNextPosition == curNextPosition)  continue;
                    preNextPosition = curNextPosition;
                    curNextPosition = tempNextPosition;
                    curForward = curNextPosition - preNextPosition;

                    FP dis = curForward.magnitude;
                    curForward.Normalize();

                    succ = true;
                    if (moveLen < dis)
                    {
                        curPosition = preNextPosition + curForward * moveLen;
                        return true;
                    }
                    else
                    {
                        //记录当前经过的点
                        m_lstOnFrameCenterPosition.Add(tempNextPosition);
                    }
                    moveLen -= dis;
                }
                if (moveLen >= 0 && succ)
                {
                    curPosition = curNextPosition;
                    if (m_lstOnFrameCenterPosition.Count > 0)
                    {
                        if(m_lstOnFrameCenterPosition[m_lstOnFrameCenterPosition.Count - 1] == curNextPosition)
                        {
                            //移除当前记录的经过最后一个点
                            m_lstOnFrameCenterPosition.RemoveAt(m_lstOnFrameCenterPosition.Count - 1);
                        }
                    }
                    
                    return true;
                }
            }
            return false;
        }

        private void PreCalculate()
        {
            PreCalculate(4, FrameSyncSys.OnFrameTime);
        }

        private void PreCalculate(int count, FP deltaTime)
        {
            for (int i = 0; i < count; i++)
            {
                TSVector position;
                TSVector forward;
                m_lstOnFrameCenterPosition.Clear();
                bool canCalculate = CalculateNextPosition(deltaTime, out position,out forward);
                PM_CenterPoints centerPositions = new PM_CenterPoints();
                //添加中途路径点
                if (m_lstOnFrameCenterPosition.Count > 0)
                {
                    centerPositions.Init();
                    for (int j = 0; j < m_lstOnFrameCenterPosition.Count; j++)
                    {
                        centerPositions.lstCenterPoint.Add(m_lstOnFrameCenterPosition[j]);
                    }
                }
                m_lstOnFrameCenterPosition.Clear();

                m_lstNextPosition.Add(position);
                m_lstNextPositionCenterPoint.Add(centerPositions);
                m_lstNextForward.Add(forward);
                if (!canCalculate)
                {
                    m_bIsCalculateFinish = true;
                    break;
                }
            }
        }

        private bool CalculateNextPosition(FP deltaTime, out TSVector position, out TSVector forward)
        {
            if (m_lstNextPosition.Count > 0)
            {
                position = m_lstNextPosition[m_lstNextPosition.Count - 1];
                forward = m_lstNextForward[m_lstNextForward.Count - 1];
            }
            else
            {
                position = m_sCurPosition;
                forward = m_sCurForward;
            }

            TSVector deltaPosition = m_sNextPosition - position;
            if(!deltaPosition.IsZero())
            {
                TSVector deltaForward = deltaPosition.normalized;
                TSVector offset = deltaForward * m_sSpeed * deltaTime;
                if (deltaPosition.sqrMagnitude > offset.sqrMagnitude)
                {
                    position = position + offset;
                }
                else
                {
                    //如果一帧超过下一个点了，那么找出最终点
                    FP len = m_sSpeed * deltaTime;
                    len -= deltaPosition.magnitude;
                    position = m_sNextPosition;
                    if (!DequeuePoint(ref m_sNextPosition,ref position, ref forward,len,true))
                    {
                        return false;
                    }
                }
            }
            else
            {
                FP len = m_sSpeed * deltaTime;
                position = m_sNextPosition;
                if (!DequeuePoint(ref m_sNextPosition,ref position,ref forward, len,true))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
