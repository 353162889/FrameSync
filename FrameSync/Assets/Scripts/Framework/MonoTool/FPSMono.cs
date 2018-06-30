using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class FPSMono : SingletonMonoBehaviour<FPSMono>
    {
        private int m_nFPS;
        public int realFPS
        {
            get { return m_nFPS; }
        }
        private int m_frameCount;
        private float m_fLastFpsUpdateTime;

        void Update()
        {
            m_frameCount++;
            if (Time.realtimeSinceStartup - m_fLastFpsUpdateTime > 1.0f)
            {
                m_nFPS = Mathf.CeilToInt(m_frameCount / (Time.realtimeSinceStartup - m_fLastFpsUpdateTime));
                m_frameCount = 0;
                m_fLastFpsUpdateTime = Time.realtimeSinceStartup;
            }
        }
    }
}
