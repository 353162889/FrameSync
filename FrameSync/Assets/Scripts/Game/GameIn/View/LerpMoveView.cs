using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    //差值过渡的表现移动处理
    public class LerpMoveView : MonoBehaviour
    {
        private Vector3 m_sStartPosition;
        private Vector3 m_sNextPosition;
        private Queue<Vector3> m_queuePosition = new Queue<Vector3>();
        private float m_fStartTime;
        private float m_fAverageTime;
        private float m_fTargetAverageTime;
        private float m_fTargetAverageTimeSpeed;
        //开始移动时，会有几个点忽略掉，这个是需要加的额外时间
        private int m_nMoveCount;
        private bool m_bCanMove;
        private float m_fLerpTime;

        public void Init()
        {
        }

        //表现移动，开始时每次直接移动到最后一个点lstPosition.Count-1
        public void StartMove(Vector3 startPosition, List<Vector3> lstPosition)
        {
            StopMove();
            m_sStartPosition = startPosition;
            SetPosition(startPosition);

            Vector3 lastPosition = lstPosition[lstPosition.Count - 1];
            float percent = 1f / lstPosition.Count;
            for (int i = 0; i < lstPosition.Count; i++)
            {
                Vector3 pos = Vector3.Lerp(m_sStartPosition, lastPosition, (i + 1) * percent);
                m_queuePosition.Enqueue(pos);
            }
            m_bCanMove = DequeuePoint();
            m_nMoveCount = lstPosition.Count;
            float onFrameTime = FrameSyncSys.OnFrameTime.AsFloat();
            m_fTargetAverageTime = m_fAverageTime = onFrameTime;
            m_fTargetAverageTimeSpeed = 0;
            //因为逻辑帧那边开始移动与更新移动是在同一帧中运行，这边初始时间多一个点的时间
            m_fStartTime = Time.time - m_fAverageTime * (m_nMoveCount + 1);
            m_fLerpTime = 0;
        }

        public void StopMove()
        {
            m_queuePosition.Clear();
            m_nMoveCount = 0;
            m_fStartTime = Time.time;
            m_fTargetAverageTime = m_fAverageTime = 0;
            m_fTargetAverageTimeSpeed = 0;
            m_fLerpTime = 0;
            m_sNextPosition = Vector3.zero;
            m_bCanMove = false;
        }

        public void Move(Vector3 pos, int logicPointCount)
        {
            m_nMoveCount++;
            m_queuePosition.Enqueue(pos);
            float nextAverageTime = (Time.time - m_fStartTime) / m_nMoveCount;
            int curPointCount = m_queuePosition.Count + 1;
            m_fTargetAverageTime = nextAverageTime;
           
            //如果表现位置与逻辑位置相差n个逻辑点位，开始加速或减速
            if (Mathf.Abs(logicPointCount - curPointCount) > 2)
            {
                if (logicPointCount > curPointCount)
                {
                    //当前表现位置减速
                    m_fTargetAverageTime = nextAverageTime * 100f;
                }
                else if (logicPointCount < curPointCount)
                {
                    //当前表现位置加速
                    m_fTargetAverageTime = nextAverageTime * 0.01f;
                }
            }
            m_fTargetAverageTimeSpeed = Mathf.Abs(m_fTargetAverageTime - nextAverageTime) * 0.1f;

            UpdateAverageTime(nextAverageTime);
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
            if (!m_bCanMove)
            {
                m_bCanMove = DequeuePoint();
                if (!m_bCanMove) return;
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
                        LerpPosition();
                    }
                    else
                    {
                        SetPosition(m_sNextPosition);
                        m_fLerpTime = 0;
                        CLog.LogColorArgs(CLogColor.Red, "wait");
                    }
                }
                else
                {
                    LerpPosition();
                }
            }
        }

        private void LerpPosition()
        {
            if (m_fAverageTime > 0)
            {
                var pos = Vector3.Lerp(m_sStartPosition, m_sNextPosition, m_fLerpTime / m_fAverageTime);
                SetPosition(pos);
                //过渡差值
                float nextAverageTime = Mathf.MoveTowards(m_fAverageTime, m_fTargetAverageTime, m_fTargetAverageTimeSpeed);
                UpdateAverageTime(nextAverageTime);
            }
            else
            {
                m_sStartPosition = m_sNextPosition;
                m_bCanMove = DequeuePoint();
                SetPosition(m_sStartPosition);
            }
        }

        private void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        private bool DequeuePoint()
        {
            if (m_queuePosition.Count > 0)
            {
                m_sNextPosition = m_queuePosition.Dequeue();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
