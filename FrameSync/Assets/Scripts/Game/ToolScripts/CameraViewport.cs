using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    [ExecuteInEditMode]
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

#if UNITY_EDITOR
        private Camera m_cCamera;
        Vector3[] frustumCorners = new Vector3[4];
        void OnDrawGizmos()
        {
            if(m_cCamera == null)
            {
                m_cCamera = gameObject.GetComponent<Camera>();
                if (m_cCamera == null)
                {
                    m_cCamera = gameObject.GetComponentInChildren<Camera>();
                }
                if (m_cCamera == null) return;
            }
            float z = (m_cCamera.transform.position - transform.position).y;
            m_cCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), z, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
            var oldColor = Gizmos.color;
            Gizmos.color = Color.red;
            for (int i = 0; i < frustumCorners.Length; i++)
            {
                frustumCorners[i] = m_cCamera.transform.TransformPoint(frustumCorners[i]);
            }
            for (int i = 0; i < frustumCorners.Length; i++)
            {
                Vector3 nextPos = frustumCorners[(i + 1) % frustumCorners.Length];
                Gizmos.DrawLine(frustumCorners[i], nextPos);
            }
            Gizmos.color = oldColor;

            oldColor = Gizmos.color;
            Gizmos.color = Color.green;
            float y = frustumCorners[0].y;
            Gizmos.DrawLine(new Vector3(mRect.xMin.AsFloat(),y,mRect.yMin.AsFloat()), new Vector3(mRect.xMax.AsFloat(), y, mRect.yMin.AsFloat()));
            Gizmos.DrawLine(new Vector3(mRect.xMax.AsFloat(), y, mRect.yMin.AsFloat()), new Vector3(mRect.xMax.AsFloat(), y, mRect.yMax.AsFloat()));
            Gizmos.DrawLine(new Vector3(mRect.xMax.AsFloat(), y, mRect.yMax.AsFloat()), new Vector3(mRect.xMin.AsFloat(), y, mRect.yMax.AsFloat()));
            Gizmos.DrawLine(new Vector3(mRect.xMin.AsFloat(), y, mRect.yMax.AsFloat()), new Vector3(mRect.xMin.AsFloat(), y, mRect.yMin.AsFloat()));
            Gizmos.color = oldColor;
        }
#endif
    }
}
