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
        private static FP MoveForwardDistance = 1000;
        protected PointMove m_cMove;
        protected ForwardRotate m_cRotate;
        protected LerpMoveView m_cLerpMoveView;
        protected FP m_sMoveSpeed = 10;

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
                m_cMove.Move(m_sCurPosition, movePath, m_sMoveSpeed);
            }
        }

        public void ReqMove(TSVector targetPosition)
        {
            if(CanMove())
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
                m_cMove.Move(m_sCurPosition, targetPosition, m_sMoveSpeed);
            }
        }

        public void ReqMoveForward(TSVector direction)
        {
            if(CanMove() && (TSVector.Angle(curForward,direction) > FP.EN1 || !m_cMove.isMoving))
            {
                Frame_ReqMoveForward_Data data = new Frame_ReqMoveForward_Data();
                data.unitId = id;
                data.forward = GameInTool.ToProtoVector2(direction);
                NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_ReqMoveForward, data);
            }
        }

        public void MoveForward(TSVector direction)
        {
            if(CanMove())
            {
                TSVector nextPoint = direction * MoveForwardDistance;
                m_cMove.Move(m_sCurPosition, nextPoint, m_sMoveSpeed);
            }
        }

        public void ReqStopMove()
        {
            if(m_cMove.isMoving)
            {
                Frame_ReqStopMove_Data data = new Frame_ReqStopMove_Data();
                data.unitId = id;
                NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_ReqStopMove, data);
            }
        }

        public void StopMove()
        {
            m_cMove.StopMove();
        }

        public bool CanMove()
        {
            return true;
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

        private void RotateToTarget(TSVector targetForward)
        {
            m_cRotate.StartRotateBySpeed(m_sCurForward, targetForward, 360 * 8);
        }

        private void OnStartRotate(TSVector preForward, TSVector newForward)
        {
        }

        private void OnRotate(TSVector preForward, TSVector newForward)
        {
            SetForward(newForward, true);
        }

        private void OnStopRotate(TSVector preForward, TSVector newForward)
        {
        }

        private void OnStartMove(TSVector position, TSVector forward)
        {
            List<Vector3> lst = GameInTool.TSVectorToLstUnityVector3(m_cMove.lstNextPosition);
            m_cLerpMoveView.StartMove(transform.position, lst);
            RotateToTarget(forward);
        }

        private void OnMove(TSVector position, TSVector forward)
        {
            curPosition = position;
            RotateToTarget(forward);
        }

        private void OnStopMove(TSVector position, TSVector forward)
        {
            m_cLerpMoveView.StopMove();
            if (!(m_sCurForward - forward).IsNearlyZero())
            {
                m_cRotate.StartRotate(m_sCurForward, forward,0);
            }
        }

        private void OnWillMove(TSVector position, TSVector forward)
        {
            m_cLerpMoveView.Move(position.ToUnityVector3(), m_cMove.lstNextPosition.Count);
        }

        protected void ResetMove()
        {
            m_cMove.Clear();
            m_cRotate.Clear();
            m_cLerpMoveView.StopMove();
        }

        protected void DisposeMove()
        {
            m_cMove.Clear();
            m_cRotate.Clear();
            m_cLerpMoveView.StopMove();
        }

        private GameObject box;
        protected void UpdateMove(FP deltaTime)
        {
            m_cMove.OnUpdate(deltaTime);
            m_cRotate.OnUpdate(deltaTime);
            if(null == box)
            {
                box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            box.transform.position = m_sCurPosition.ToUnityVector3();
        }
    }
}
