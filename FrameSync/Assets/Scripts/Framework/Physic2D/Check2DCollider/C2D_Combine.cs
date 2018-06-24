using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class C2D_Combine : Check2DCollider
    {
        private List<Check2DCollider> m_lstColliders;
        public List<Check2DCollider> lstColliders { get { return m_lstColliders; } }
        private FP m_sForwardLen;
        public override FP forwardLen
        {
            get
            {
                return m_sForwardLen;
            }
        }

        public C2D_Combine():base()
        {
            m_lstColliders = new List<Check2DCollider>();
            m_sForwardLen = 0;
        }

        public override void Reset()
        {
            base.Reset();
            m_lstColliders.Clear();
        }

        public void AddChild(Check2DCollider collider)
        {
            m_lstColliders.Add(collider);
            Reculate();
        }

        private void Reculate()
        {
            TSVector2 center = TSVector2.zero;
            TSVector2 forward = TSVector2.zero;
            for (int i = 0; i < m_lstColliders.Count; i++)
            {
                var collider = m_lstColliders[i];
                center += collider.center;
                forward += collider.forward * collider.forwardLen;
            }
            this.center = center;
            if (forward == TSVector2.zero)
            {
                forward = TSVector2.up;
            }
            this.m_sForwardLen = forward.magnitude;
            forward.Normalize();
            this.forward = forward;
        }

        public override bool CheckCircle(TSVector2 sCenter, FP nRadius)
        {
            for (int i = m_lstColliders.Count - 1; i > -1; i--)
            {
                if(m_lstColliders[i].CheckCircle(sCenter,nRadius))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CheckCollider(Check2DCollider collider)
        {
            for (int i = m_lstColliders.Count - 1; i > -1; i--)
            {
                if (m_lstColliders[i].CheckCollider(collider))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CheckLine(TSVector2 sOrgPos, TSVector2 sOffset, out TSVector2 sCrossPoint)
        {
            sCrossPoint = sOrgPos;
            for (int i = m_lstColliders.Count - 1; i > -1; i--)
            {
                if (m_lstColliders[i].CheckLine(sOrgPos, sOffset,out sCrossPoint))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CheckPos(TSVector2 sPosition)
        {
            for (int i = m_lstColliders.Count - 1; i > -1; i--)
            {
                if (m_lstColliders[i].CheckPos(sPosition))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CheckRect(TSVector2 sCenter, TSVector2 sDir, FP nHalfWidth, FP nHalfHeight)
        {
            for (int i = m_lstColliders.Count - 1; i > -1; i--)
            {
                if (m_lstColliders[i].CheckRect(sCenter, sDir, nHalfWidth, nHalfHeight))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
