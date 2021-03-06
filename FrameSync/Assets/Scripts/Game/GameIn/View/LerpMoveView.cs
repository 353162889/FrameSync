﻿using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public struct LPM_CenterPoints
    {
        public List<Vector3> lstCenterPoint;

        public void Init()
        {
            if (null == lstCenterPoint)
            {
                lstCenterPoint = ResetObjectPool<List<Vector3>>.Instance.GetObject();
            }
        }

        public void Clear()
        {
            if (null != lstCenterPoint)
            {
                ResetObjectPool<List<Vector3>>.Instance.SaveObject(lstCenterPoint);
                lstCenterPoint = null;
            }
        }

        public static LPM_CenterPoints FromPM_CenterPoints(PM_CenterPoints centerPoints)
        {
            LPM_CenterPoints lpm_centerPoints = new LPM_CenterPoints();
            if(centerPoints.lstCenterPoint != null)
            {
                lpm_centerPoints.Init();
                for (int i = 0; i < centerPoints.lstCenterPoint.Count; i++)
                {
                    lpm_centerPoints.lstCenterPoint.Add(centerPoints.lstCenterPoint[i].ToUnityVector3());
                }
            }
            return lpm_centerPoints;
        }
    }

    //差值过渡的表现移动处理
    public class LerpMoveView : MonoBehaviour
    {
        private Vector3 m_sStartPosition;
        private Vector3 m_sNextPosition;
        private LPM_CenterPoints m_sNextCenterPoints;
        private Queue<Vector3> m_queuePosition = new Queue<Vector3>();
        private Queue<LPM_CenterPoints> m_queueCenterPoints = new Queue<LPM_CenterPoints>();
        private float m_fStartTime;
        private float m_fAverageTime;
        private float m_fTargetAverageTime;
        //开始移动时，会有几个点忽略掉，这个是需要加的额外时间
        private int m_nMoveCount;
        private int m_nStartMoveCount;//移动前几个点不改变移速
        private bool m_bCanMove;
        private float m_fLerpTime;
        private List<Vector3> m_lstCurStartAndNextPositions = new List<Vector3>();
        private List<float> m_lstCurStartAndNextRate = new List<float>();
        private int m_nMoveTimes;
        private float m_fOnFrameDistance;
        private float m_fDelayMoveTime;
        private bool m_bIsChangeSpeed;

        public void Init()
        {
        }

        //表现移动，开始时每次直接移动到最后一个点lstPosition.Count-1
        public void StartMove(Vector3 startPosition, List<Vector3> lstPosition,bool stopToMove)
        {
            StopMove();
            if (lstPosition.Count <= 0) return;
            m_sStartPosition = startPosition;
            SetPosition(startPosition);

            Vector3 lastPosition = lstPosition[lstPosition.Count - 1];
            float percent = 1f / lstPosition.Count;
            for (int i = 0; i < lstPosition.Count; i++)
            {
                Vector3 pos = Vector3.Lerp(m_sStartPosition, lastPosition, (i + 1) * percent);
                m_queuePosition.Enqueue(pos);
                m_queueCenterPoints.Enqueue(new LPM_CenterPoints());
            }
            m_fOnFrameDistance = 1f;
            if(lstPosition.Count > 1)
            {
                m_fOnFrameDistance = (lstPosition[1] - lstPosition[0]).magnitude;
            }
            m_nMoveCount = m_queuePosition.Count;
            m_nStartMoveCount = m_nMoveCount;
            m_bCanMove = DequeuePoint();
            float onFrameTime = FrameSyncSys.OnFrameTime.AsFloat();
            m_fTargetAverageTime = m_fAverageTime = onFrameTime;
            if(stopToMove)
            {
                m_fStartTime = Time.time - m_fAverageTime * (m_nMoveCount);
            }
            else
            {
                //如果当前移动不是从停止开始移动的，那么move方法会在同一帧中调用，所以，时间需要靠前处理
                m_fStartTime = Time.time - m_fAverageTime * (m_nMoveCount + 1);
            }
            
            m_fLerpTime = 0;
            m_nMoveTimes = 0;
            if (stopToMove)
            {
                //表现位置延迟一帧再移动（影子跟随）
                m_fDelayMoveTime = onFrameTime / 2f;
            }
            else
            {
                m_fDelayMoveTime = 0;
            }
        }

        public void StopMove()
        {
            //CLog.LogArgs("View StopMove");
            m_sNextCenterPoints.Clear();
            m_queuePosition.Clear();
            while(m_queueCenterPoints.Count > 0)
            {
                var centerPoint = m_queueCenterPoints.Dequeue();
                centerPoint.Clear();
            }
            m_nMoveCount = 0;
            m_nStartMoveCount = 0;
            m_fStartTime = Time.time;
            m_fTargetAverageTime = m_fAverageTime = 0;
            m_fLerpTime = 0;
            m_sNextPosition = Vector3.zero;
            m_bCanMove = false;
            m_nMoveTimes = 0;
            m_fDelayMoveTime = float.MaxValue;
            m_bIsChangeSpeed = false;
        }

        public void StopMove(Vector3 startPosition, Vector3 targetPosition,float speed)
        {
            StopMove();
            float dis = (targetPosition - startPosition).magnitude;
            if (Mathf.Approximately(dis, 0)) return;
            m_sStartPosition = startPosition;
            SetPosition(startPosition);
            m_queuePosition.Enqueue(targetPosition);
            m_queueCenterPoints.Enqueue(new LPM_CenterPoints());
          
            m_fOnFrameDistance = 1f;
            m_nMoveCount = m_queuePosition.Count;
            m_nStartMoveCount = m_nMoveCount;
            m_bCanMove = DequeuePoint();
            m_fTargetAverageTime = m_fAverageTime = dis / speed;
            m_fStartTime = Time.time - m_fAverageTime * (m_nMoveCount);
            m_fLerpTime = 0;
            m_nMoveTimes = 0;
            m_fDelayMoveTime = 0;
        }

        public void WillMove(Vector3 position, LPM_CenterPoints willCenterPoint)
        {
            m_nMoveCount++;
            m_queuePosition.Enqueue(position);
            m_queueCenterPoints.Enqueue(willCenterPoint);
        }

        public void Move(Vector3 curPosition, int moveTimes)
        {
            m_nStartMoveCount--;
            float nextAverageTime = (Time.time - m_fStartTime) / m_nMoveCount;
            //大于一定距离开始加速或减速(缓慢，不保证最终一定不相差很大的距离)
            //CLog.LogArgs("move", curPosition, transform.position,"sub", (curPosition - transform.position).magnitude, "nextAverageTime", nextAverageTime,"subTime",Time.time - m_fStartTime,"count",m_nMoveCount);
            //前几个点不计算位置误差
            if (m_nStartMoveCount <= 0)
            {
                float dis = (transform.position - curPosition).magnitude;

                if (m_bIsChangeSpeed && dis <= m_fOnFrameDistance / 2)
                {
                    m_bIsChangeSpeed = false;
                }

                //防止位置相差很大
                if (m_bIsChangeSpeed || dis > m_fOnFrameDistance)
                {
                    m_fTargetAverageTime = nextAverageTime;
                    //加速
                    if (m_nMoveTimes < moveTimes)
                    {
                        //m_fTargetAverageTime = nextAverageTime * 0.75f;
                        nextAverageTime = nextAverageTime * 0.9f;
                        //CLog.LogArgs("加速:", dis, "m_nMoveTimes", m_nMoveTimes, "moveTimes", moveTimes);
                    }
                    //减速
                    else
                    {
                        //m_fTargetAverageTime = nextAverageTime * 1.5f;
                        nextAverageTime = nextAverageTime * 1.1f;
                        //CLog.LogArgs("减速:", dis, "m_nMoveTimes", m_nMoveTimes, "moveTimes", moveTimes);
                    }
                    m_bIsChangeSpeed = true;
                }
            }

            UpdateAverageTime(nextAverageTime);
            //if (!m_bIsChangeSpeed)
            //{
            //    UpdateAverageTime(nextAverageTime);
            //}
        }

        private void UpdateAverageTime(float averageTime)
        {
            float lastLerpTime = m_fLerpTime;  
            float lastAverageTime = m_fAverageTime;
            m_fAverageTime = averageTime;
            if (lastAverageTime > 0)
            {
                m_fLerpTime = m_fLerpTime / lastAverageTime * m_fAverageTime;
            }
            else
            {
                m_fLerpTime = 0;
            }
        }

        void Update()
        {
            if (m_fDelayMoveTime > 0)
            {
                m_fDelayMoveTime -= Time.deltaTime;
            }
            if (m_fDelayMoveTime <= 0)
            {
                if (!m_bCanMove)
                {
                    m_sStartPosition = m_sNextPosition;
                    m_bCanMove = DequeuePoint();
                    if (!m_bCanMove) return;
                    else
                    {
                        UpdateStartAndNextPosition();
                    }
                }
                else
                {
                    m_fLerpTime += Time.deltaTime;
                    if (m_fLerpTime > m_fAverageTime)
                    {
                        m_fLerpTime -= m_fAverageTime;
                        m_sStartPosition = m_sNextPosition;
                        m_bCanMove = DequeuePoint();
                        if (m_bCanMove)
                        {
                            UpdateStartAndNextPosition();
                            LerpPosition();
                        }
                        else
                        {
                            SetPosition(m_sNextPosition);
                            m_fLerpTime = 0;
                            //CLog.LogColorArgs(CLogColor.Red, "wait");
                        }
                    }
                    else
                    {
                        LerpPosition();
                    }
                }
            }
        }

        private void UpdateStartAndNextPosition()
        {
            m_lstCurStartAndNextPositions.Clear();
            m_lstCurStartAndNextRate.Clear();
            if (m_sNextCenterPoints.lstCenterPoint != null)
            {
                float totalLen = 0;
                var startPos = m_sStartPosition;
                m_lstCurStartAndNextPositions.Add(m_sStartPosition);
                m_lstCurStartAndNextRate.Add(0);
                for (int i = 0; i < m_sNextCenterPoints.lstCenterPoint.Count; i++)
                {
                    var pos = m_sNextCenterPoints.lstCenterPoint[i];
                    float len = (pos - startPos).magnitude;
                    m_lstCurStartAndNextPositions.Add(pos);
                    m_lstCurStartAndNextRate.Add(len + totalLen);
                    totalLen += len;
                    startPos = pos;
                }
                m_lstCurStartAndNextPositions.Add(m_sNextPosition);
                totalLen = totalLen + (m_sNextPosition - startPos).magnitude;
                m_lstCurStartAndNextRate.Add(totalLen);
                for (int i = 1; i < m_lstCurStartAndNextPositions.Count; i++)
                {
                    float dis = m_lstCurStartAndNextRate[i];
                    m_lstCurStartAndNextRate[i] = dis / totalLen;

                    if(m_lstCurStartAndNextRate[i] - 1f > float.Epsilon)
                    {
                        //CLog.LogError("算出来的比率大于1，有问题:"+ m_lstCurStartAndNextRate[i]);
                        m_lstCurStartAndNextRate[i] = Mathf.Clamp01(m_lstCurStartAndNextRate[i]);
                    }
                }
            }
        }

        private void LerpPosition()
        {
            if (m_fAverageTime > 0)
            {
                Vector3 pos;
                float lerpTime = m_fLerpTime / m_fAverageTime;
                lerpTime = Mathf.Clamp01(lerpTime);
                if (m_sNextCenterPoints.lstCenterPoint != null)
                {
                    int i = 1;
                    for (; i < m_lstCurStartAndNextRate.Count; i++)
                    {
                        if (lerpTime <= m_lstCurStartAndNextRate[i])
                        {
                            break;
                        }
                    }
                    float lerp = (lerpTime - m_lstCurStartAndNextRate[i - 1]) / (m_lstCurStartAndNextRate[i] - m_lstCurStartAndNextRate[i - 1]);
                    pos = Vector3.Lerp(m_lstCurStartAndNextPositions[i - 1], m_lstCurStartAndNextPositions[i], lerp);
                    //pos = Vector3.Lerp(m_sStartPosition, m_sNextPosition, lerpTime);
                }
                else
                {
                    pos = Vector3.Lerp(m_sStartPosition, m_sNextPosition, lerpTime);
                }

                SetPosition(pos);
                //if (m_bIsChangeSpeed)
                //{
                //    //过渡差值
                //    float nextAverageTime = Mathf.MoveTowards(m_fAverageTime, m_fTargetAverageTime, 1 * Time.deltaTime);
                //    if (nextAverageTime != m_fAverageTime)
                //    {
                //        UpdateAverageTime(nextAverageTime);
                //    }
                //}
            }
            else
            {
                m_sStartPosition = m_sNextPosition;
                m_bCanMove = DequeuePoint();
                if (m_bCanMove) UpdateStartAndNextPosition();
                SetPosition(m_sStartPosition);
            }
        }

        private void SetPosition(Vector3 position)
        {
            transform.position = position;
            //CLog.LogArgs("SetPosition", position);
        }

        private bool DequeuePoint()
        {
            if (m_queuePosition.Count > 0)
            {
                m_sNextPosition = m_queuePosition.Dequeue();
                m_sNextCenterPoints.Clear();
                m_sNextCenterPoints = m_queueCenterPoints.Dequeue();
                m_nMoveTimes++;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
