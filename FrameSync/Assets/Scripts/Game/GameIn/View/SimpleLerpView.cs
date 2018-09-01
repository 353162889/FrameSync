using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class SimpleLerpView : MonoBehaviour
    {
        private Vector3 m_sStart;
        private Vector3 m_sEnd;
        private float m_fTime;
        private float m_fCurTime;
        private bool m_bStart;
        public void LerpTo(Vector3 position,float time)
        {
            m_sStart = transform.position;
            m_sEnd = position;
            m_fTime = time;
            m_fCurTime = 0;
            m_bStart = time > 0;
        }

        public void Stop()
        {
            m_bStart = false;
        }

        void Update()
        {
            if (m_bStart)
            {
                m_fCurTime += Time.deltaTime;
                float t = Mathf.Clamp01(m_fCurTime /m_fTime);
                transform.position = Vector3.Lerp(m_sStart, m_sEnd, t);
                if (m_fCurTime >= m_fTime) m_bStart = false;
            }
        }
    }
}
