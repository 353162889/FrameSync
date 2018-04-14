using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework;

namespace Game
{
    public class UPointMoveView : MonoBehaviour
    {
        private bool m_bIsMoving;
        public bool isMoving
        {
            get { return m_bIsMoving; }
        }

        private float m_fSpeed;
        public float curSpeed
        {
            get { return m_fSpeed; }
        }

        private float m_fBeginSpeed;
        public float beginSpeed { get { return m_fBeginSpeed; } }

        private Vector3 m_sNextPosition;

        private Queue<Vector3> m_queuePath;

        private UPointMove m_cPointMove;

        private float m_fMoveTime;
        private float m_fMoveTotalDistance;
        private float m_fMoveDistance;

        public void Init(UPointMove pointMove)
        {
            m_queuePath = new Queue<Vector3>();
            m_bIsMoving = false;
            m_sNextPosition = Vector3.zero;
            m_fBeginSpeed = m_fSpeed = 0;
            m_fMoveTime = 0;
            m_fMoveDistance = 0;
            m_fMoveTotalDistance = 0;
            m_cPointMove = pointMove;
        }

        public void Move(Vector3 startPosition, List<Vector3> movePath, float speed)
        {
            if (movePath.Count <= 0) return;
            SetPosition(startPosition);
            m_fMoveTime = 0;
            m_fMoveDistance = 0;
            m_fMoveTotalDistance = (movePath[0] - startPosition).magnitude;
            for (int i = 0; i < movePath.Count; i++)
            {
                m_queuePath.Enqueue(movePath[i]);
                m_fMoveTotalDistance += (movePath[i] - movePath[i - 1]).magnitude;
            }
            m_bIsMoving = true;
            m_fBeginSpeed = m_fSpeed = speed;
            if (!DequeuePoint())
            {
                StopMove();
            }
        }

        public void Move(Vector3 startPosition, Vector3 targetPosition, float speed)
        {
            SetPosition(startPosition);
            m_fMoveTime = 0;
            m_fMoveDistance = 0;
            m_fMoveTotalDistance = (targetPosition - startPosition).magnitude;
            m_queuePath.Enqueue(targetPosition);
            m_bIsMoving = true;
            m_fBeginSpeed = m_fSpeed = speed;
            if (!DequeuePoint())
            {
                StopMove();
            }
        }

        public void StopMove()
        {
            m_queuePath.Clear();
            m_bIsMoving = false;
            m_sNextPosition = Vector3.zero;
            m_fBeginSpeed = m_fSpeed = 0;
            m_fMoveTime = 0;
            m_fMoveDistance = 0;
            m_fMoveTotalDistance = 0;
        }

        private void SetPosition(Vector3 position)
        {
            StopMove();
            transform.position = position;
        }


        private float checkWaitTime = 0.2f;
        private float startCheckWaitTime = 0f;
        private float precision = 1.5f;
        public void OnUpdate(float deltaTime)
        {
            if (m_bIsMoving)
            {
                m_fMoveTime += deltaTime;
                startCheckWaitTime += deltaTime;
                if (startCheckWaitTime > checkWaitTime && (transform.position - m_cPointMove.curPosition.ToUnityVector3()).magnitude > precision)
                {
                    //重新计算移动的距离
                    m_fMoveDistance = m_fMoveTotalDistance - GetLeaveDistance();
                    if (m_fMoveDistance < 0) m_fMoveDistance = 0;
                    transform.position = m_cPointMove.curPosition.ToUnityVector3();
                    return;
                }

                
                //if (m_fMoveTime > 0 && m_cPointMove.moveTime.AsFloat() < (m_cPointMove.moveTotalDistance.AsFloat() / m_fBeginSpeed))
                //{
                //    //float targetSpeed = (m_cPointMove.moveTime.AsFloat() * m_fBeginSpeed) / m_fMoveTime * ((m_fMoveTotalDistance - m_fMoveDistance) / (m_cPointMove.moveTotalDistance - m_cPointMove.moveDistance).AsFloat());
                //    float targetSpeed = (m_cPointMove.moveTime.AsFloat() * m_fBeginSpeed) / m_fMoveTime;
                //    m_fSpeed = Mathf.MoveTowards(m_fSpeed, targetSpeed, 1f);
                //}
                //else
                //{
                //    m_fSpeed = Mathf.MoveTowards(m_fSpeed, m_fBeginSpeed, 1f);
                //}
                //CLog.LogArgs("speed",m_fSpeed, "m_cPointMove.moveTime", m_cPointMove.moveTime, "m_fMoveTime", m_fMoveTime);
                Vector3 deltaPosition = m_sNextPosition - transform.position;
                if (deltaPosition != Vector3.zero)
                {
                    transform.forward = deltaPosition.normalized;
                }
                Vector3 offset = transform.forward * m_fSpeed * deltaTime;
                if (deltaPosition.sqrMagnitude > offset.sqrMagnitude)
                {
                    transform.position += offset;
                    m_fMoveDistance += offset.magnitude;
                }
                else
                {
                    transform.position = m_sNextPosition;
                    m_fMoveDistance += deltaPosition.magnitude;
                    if (!DequeuePoint())
                    {
                        StopMove();
                    }
                }
            }
        }

        private float GetLeaveDistance()
        {
            float dis = (m_sNextPosition - transform.position).magnitude;
            if(m_queuePath.Count > 0)
            {
                dis += (m_queuePath.Peek() - m_sNextPosition).magnitude;
                for (int i = 0; i < m_queuePath.Count; i++)
                {
                    if(i > 1)
                    {
                        dis += (m_queuePath.ElementAt(i) - m_queuePath.ElementAt(i - 1)).magnitude;
                    }
                }
            }
            return dis;
        }

        public bool DequeuePoint()
        {
            if (m_queuePath.Count > 0)
            {
                m_sNextPosition = m_queuePath.Dequeue();
                Vector3 forward = m_sNextPosition - transform.position;
                if (forward != Vector3.zero)
                {
                    forward.Normalize();
                    transform.forward = forward;
                }
                return true;
            }
            return false;
        }

        void Update()
        {
            OnUpdate(Time.deltaTime);
        }

        public void Clear()
        {
            StopMove();
            m_cPointMove = null;
        }
    }
}
