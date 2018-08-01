using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class ForwardRotate
    {
        public delegate void ForwardRotateHandler(TSVector preForward, TSVector newForward);
        public event ForwardRotateHandler OnStartRotate;
        public event ForwardRotateHandler OnRotate;
        public event ForwardRotateHandler OnStopRotate;
        private TSVector m_sStartForward;
        private TSVector m_sTargetForward;
        private TSVector m_sCurForward;
        private TSQuaternion m_sTargetRotation;
        private TSQuaternion m_sStartRotation;
        private FP m_sSpeed;
        private bool m_nRotating;
        private FP m_sLerp;

        public bool StartRotate(TSVector startForward,TSVector targetForward,FP time)
        {
            var rotation = TSQuaternion.FromToRotation(m_sStartForward, m_sTargetForward);
            FP angle = TSQuaternion.Angle(TSQuaternion.identity, rotation);
            FP angleSpeed = FP.MaxValue;
            if (time > 0)
            {
                angleSpeed = angle / time;
            }
            return StartRotateBySpeed(startForward, targetForward, angleSpeed);
        }

        public bool StartRotateBySpeed(TSVector startForward,TSVector targetForward,FP angleSpeed)
        {
            if (startForward.IsZero() || targetForward.IsZero())
            {
                return false;
            }
            startForward.Normalize();
            m_sCurForward = m_sStartForward = startForward;
            targetForward.Normalize();
            m_sTargetForward = targetForward;
            if ((startForward - targetForward).IsZero()) return false;
            m_sSpeed = angleSpeed / 360;
            m_sStartRotation = TSQuaternion.identity;
            if ((m_sStartForward - TSVector.Negate(m_sTargetForward)).IsNearlyZero())
            {
                //如果旋转是180度，这边有问题，特殊处理
                m_sTargetRotation = TSQuaternion.Euler(0, 180, 0);
            }
            else
            {
                m_sTargetRotation = TSQuaternion.FromToRotation(m_sStartForward, m_sTargetForward);
            }
            m_nRotating = true;
            m_sLerp = 0;
            if(null != OnStartRotate)
            {
                OnStartRotate(m_sCurForward, m_sCurForward);
            }
            return true;
        }

        public void StopRotate()
        {
            if(null != OnStopRotate)
            {
                OnStopRotate(m_sCurForward,m_sCurForward);
            }
            m_nRotating = false;
        }

        public void Clear()
        {
            StopRotate();
            OnStartRotate = null;
            OnRotate = null;
            OnStopRotate = null;
        }

        public void OnUpdate(FP deltaTime)
        {
            if(m_nRotating)
            {
                m_sLerp += m_sSpeed * deltaTime;
                if(m_sLerp <= FP.One)
                {
                    TSQuaternion rotate = TSQuaternion.Lerp(m_sStartRotation, m_sTargetRotation, m_sLerp);
                    SetForward(rotate* m_sStartForward);
                }
                else
                {
                    SetForward(m_sTargetForward);
                    StopRotate();
                }
            }
        }

        private void SetForward(TSVector forward)
        {
            TSVector preForward = m_sCurForward;
            m_sCurForward = forward;
            m_sCurForward.Normalize();
            if(null != OnRotate)
            {
                OnRotate(preForward, m_sCurForward);
            }
        }
    }
}
