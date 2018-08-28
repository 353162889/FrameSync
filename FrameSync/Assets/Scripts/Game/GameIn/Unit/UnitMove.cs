using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using Proto;
using UnityEngine;

namespace Game
{
    //unit的移动部分
    public partial class Unit
    {
        public delegate void UnitMoveHandler(TSVector position, TSVector forward);
        public event UnitMoveHandler OnUnitMoveStart;
        public event UnitMoveHandler OnUnitMove;
        public event UnitMoveHandler OnUnitMoveStop;
        protected PointMove m_cMove;
        protected ForwardRotate m_cRotate;
        protected LerpMoveView m_cLerpMoveView;

        public bool isMoving { get { return m_cMove.isMoving; } }

        public void ReqMove(List<TSVector> movePath)
        {
            if (movePath.Count <= 0) return;
            if(CanMove())
            {
                Frame_ReqMovePath_Data data = new Frame_ReqMovePath_Data();
                data.unitId = id;
                for (int i = 0; i < movePath.Count; i++)
                {
                    data.paths.Add(GameInTool.ToProtoVector2(movePath[i]));
                }
                NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_ReqMovePath, data);
            }
        }

        public void Move(List<TSVector> movePath)
        {
            if (CanMove())
            {
                m_cMove.Move(m_sCurPosition, movePath, this.moveSpeed);
            }
        }

        public void ReqMove(TSVector targetPosition)
        {
            if(CanMove() && (!m_cMove.isMoving || m_cMove.targetPosition != targetPosition))
            {
                Frame_ReqMovePoint_Data data = new Frame_ReqMovePoint_Data();
                data.unitId = id;
                data.targetPosition = GameInTool.ToProtoVector2(targetPosition);
                NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_ReqMovePoint, data);
            }
        }

        public void Move(TSVector targetPosition)
        {
            if (CanMove())
            {
                m_cMove.Move(m_sCurPosition, targetPosition, this.moveSpeed);
            }
        }

        public void ReqMoveForward(TSVector direction,FP len)
        {
            if(CanMove() && (TSVector.Angle(curForward,direction) > FP.EN1 || !m_cMove.isMoving))
            {
                Frame_ReqMoveForward_Data data = new Frame_ReqMoveForward_Data();
                data.unitId = id;
                data.forward = GameInTool.ToProtoVector2(direction);
                data.len = len.ToSourceLong();
                NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_ReqMoveForward, data);
            }
        }

        public void MoveForward(TSVector direction,FP len)
        {
            if(CanMove())
            {
                TSVector nextPoint = direction * len;
                m_cMove.Move(m_sCurPosition, nextPoint, this.moveSpeed);
            }
        }

        public void ReqStopMove()
        {
            if(m_cMove != null && m_cMove.isMoving)
            {
                Frame_ReqStopMove_Data data = new Frame_ReqStopMove_Data();
                data.unitId = id;
                NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_ReqStopMove, data);
            }
        }

        public void StopMove()
        {
            if (m_cMove != null)
            {
                m_cMove.StopMove();
            }
        }

        public bool CanMove()
        {
            return m_cMove != null && !IsForbid(UnitForbidType.ForbidMove);
        }

        public void StopRotate()
        {
            if (m_cRotate != null)
            {
                m_cRotate.StopRotate();
            }
        }

        protected void InitMove()
        {
            m_cMove = new PointMove();
            m_cMove.OnMoveStart += OnStartMove;
            m_cMove.OnMove += OnMove;
            m_cMove.OnMoveStop += OnStopMove;
            m_cMove.OnWillMove += OnWillMove;
            m_cRotate = new ForwardRotate();
            m_cRotate.OnStartRotate += OnStartRotate;
            m_cRotate.OnRotate += OnRotate;
            m_cRotate.OnStopRotate += OnStopRotate;
            m_cLerpMoveView = this.gameObject.AddComponentOnce<LerpMoveView>();
            m_cLerpMoveView.Init();
        }

        protected virtual void RotateToTarget(TSVector targetForward)
        {
            m_cRotate.StartRotateBySpeed(m_sCurForward, targetForward, 360 * 4);
        }

        protected virtual void OnStartRotate(TSVector preForward, TSVector newForward)
        {
        }

        protected void OnRotate(TSVector preForward, TSVector newForward)
        {
            SetForward(newForward, ForwardFromType.UnitMove, true,false);
        }

        protected virtual void OnStopRotate(TSVector preForward, TSVector newForward)
        {
        }

        protected virtual void OnStartMove(TSVector position, TSVector forward,bool stopToMove)
        {
            List<Vector3> lst = GameInTool.TSVectorToLstUnityVector3(m_cMove.lstNextPosition);
            m_cLerpMoveView.StartMove(transform.position, lst, stopToMove);
            SetForward(forward, ForwardFromType.UnitMove, false);
            //RotateToTarget(forward);
            if(null != OnUnitMoveStart)
            {
                OnUnitMoveStart(position, forward);
            }
        }

        protected virtual void OnMove(TSVector position, TSVector forward)
        {
            curPosition = position;
            SetForward(forward, ForwardFromType.UnitMove, false);
            m_cLerpMoveView.Move(position.ToUnityVector3(), m_cMove.moveTimes);
            if(null != OnUnitMove)
            {
                OnUnitMove(position, forward);
            }
            //RotateToTarget(forward);
        }

        protected virtual void OnStopMove(TSVector position, TSVector forward)
        {
            //停止移动时，差值到逻辑位置
            m_cLerpMoveView.StopMove(transform.position, curPosition.ToUnityVector3(), this.moveSpeed.AsFloat());
            //m_cLerpMoveView.StopMove();
            //停止运行时检查逻辑位置与表现位置是否相差很大，（如果表现位置还没到达逻辑位置，表现位置差值到逻辑位置，如果表现位置超过逻辑位置,误差范围内不动，误差范围外移动到逻辑位置（表现会有点奇怪））
            //CLog.LogArgs("offset:",position.ToUnityVector3(),m_cLerpMoveView.transform.position,(position.ToUnityVector3()- m_cLerpMoveView.transform.position).magnitude);
            if (!(m_sCurForward - forward).IsNearlyZero())
            {
                m_cRotate.StartRotate(m_sCurForward, forward,0);
            }
            if(null != OnUnitMoveStop)
            {
                OnUnitMoveStop(position, forward);
            }
        }

        private void OnWillMove(TSVector willPosition, TSVector willforward, PM_CenterPoints willCenterPoints)
        {
            m_cLerpMoveView.WillMove(willPosition.ToUnityVector3(), LPM_CenterPoints.FromPM_CenterPoints(willCenterPoints));
        }

        protected void ResetMove()
        {
            if (m_cMove != null)
            {
                m_cMove.Clear();
            }
            if (m_cRotate != null)
            {
                m_cRotate.Clear();
            }
            if (m_cLerpMoveView != null)
            {
                m_cLerpMoveView.StopMove();
            }
        }

        //private GameObject box;
        protected void UpdateMove(FP deltaTime)
        {
            if (m_cMove != null)
            {
                m_cMove.OnUpdate(deltaTime);
            }
            if(m_cRotate != null)
            {
                m_cRotate.OnUpdate(deltaTime);
            }
            //if (null == box)
            //{
            //    box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //}
            //box.transform.position = m_sCurPosition.ToUnityVector3();
        }

        protected void DieMove(DamageInfo damageInfo)
        {
            if (m_cMove != null)
            {
                m_cMove.StopMove();
            }
            if (m_cRotate != null)
            {
                m_cRotate.StopRotate();
            }
            if (m_cLerpMoveView != null)
            {
                m_cLerpMoveView.StopMove();
            }
        }
    }
}
