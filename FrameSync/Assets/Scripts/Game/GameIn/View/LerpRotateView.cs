using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class LerpRotateView : MonoBehaviour
    {
        private Vector3 m_sStart;
        private Vector3 m_sEnd;
        private float m_fTime;
        private float m_fCurTime;
        private bool m_bStart;
        public void LerpTo(Vector3 forward,float time)
        {
            m_sStart = transform.forward;
            m_sEnd = forward;
            m_fTime = time;
            m_fCurTime = 0;
            if(time > 0)
            {
                m_bStart = true;
            }
            else
            {
                transform.forward = forward;
            }
        }

        public void Stop()
        {
            m_bStart = false;
        }

        void Update()
        {
            if (m_bStart)
            {
                m_fCurTime += Time.time;
                float t = Mathf.Clamp01(m_fCurTime / m_fTime);
                transform.forward = Vector3.Slerp(m_sStart, m_sEnd, t);
                if (m_fCurTime >= m_fTime) m_bStart = false;
            }
        }
    }
}
