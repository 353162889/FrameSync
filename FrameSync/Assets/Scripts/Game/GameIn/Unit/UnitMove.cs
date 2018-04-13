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
        protected UPointMove m_cMove;
        protected UPointMoveView m_cMoveView;
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
                List<Vector3> lst = GameInTool.TSVectorToLstUnityVector3(movePath);
                m_cMoveView.Move(m_cMoveView.transform.position, lst, m_sMoveSpeed.AsFloat());
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
                m_cMoveView.Move(m_cMoveView.transform.position, targetPosition.ToUnityVector3(), m_sMoveSpeed.AsFloat());
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
                m_cMoveView.Move(m_cMoveView.transform.position, nextPoint.ToUnityVector3(), m_sMoveSpeed.AsFloat());
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
            m_cMoveView.StopMove();
        }

        public bool CanMove()
        {
            return true;
        }

        protected void InitMove()
        {
            m_cMove = new UPointMove();
            m_cMoveView = this.gameObject.AddComponentOnce<UPointMoveView>();
            m_cMoveView.Init(m_cMove);
        }

        protected void ResetMove()
        {
            m_cMove.StopMove();
            m_cMoveView.StopMove();
        }

        protected void DisposeMove()
        {
            m_cMove.StopMove();
            m_cMoveView.Clear();
        }

        protected void UpdateMove(FP deltaTime)
        {
            bool isMoving = m_cMove.isMoving;
            m_cMove.OnUpdate(deltaTime);
            if (isMoving)
            {
                m_sCurPosition = m_cMove.curPosition;
                m_sCurForward = m_cMove.curForward;
            }
        }
    }
}
