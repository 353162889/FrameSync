using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class CameraViewport : MonoBehaviour
    {
        [SerializeField]
        protected long m_lX;
        [SerializeField]
        protected long m_lY;
        [SerializeField]
        protected long m_lWidth;
        [SerializeField]
        protected long m_lHeight;

        protected TSRect m_sRect = TSRect.zero;
        public TSRect mRect {
            get {
                if(m_sRect == TSRect.zero)
                {
                    m_sRect = new TSRect(FP.FromSourceLong(m_lX),FP.FromSourceLong(m_lY),FP.FromSourceLong(m_lWidth),FP.FromSourceLong(m_lHeight));
                }
                return m_sRect;
            }
            set {
                m_sRect = value;
                m_lX = m_sRect.x._serializedValue;
                m_lY = m_sRect.y._serializedValue;
                m_lWidth = m_sRect.width._serializedValue;
                m_lHeight = m_sRect.height._serializedValue;
            }
        }
    }
}
